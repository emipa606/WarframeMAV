using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;
using WarframeMAV.Skills.Ashs;

namespace Warframe.Skills
{
    public static class Ash
    {
        //咖喱技能1
        public static Command_CastSkillTargeting Skill1()
        {
            var ck = new Command_CastSkillTargeting
            {
                defaultLabel = "AshSkill1.name".Translate(),
                icon = ContentFinder<Texture2D>.Get("Skills/AshSkill1"),
                targetingParams = WarframeStaticMethods.OnlyPawn(),
                cooldownTime = 0.2f,
                range = 35f
            };
            ck.finishAction = delegate
            {
                //GenDraw.DrawFieldEdges(WarframeStaticMethods.GetCellsAround(ck.self.Position, ck.self.Map, ck.range));
                GenDraw.DrawRadiusRing(ck.self.Position, ck.range);
            };
            ck.hotKey = KeyBindingDefOf.Misc5;
            ck.action = delegate(Pawn self, Thing target)
            {
                // GenExplosion.DoExplosion(self.Position, self.Map, 3.5f, DamageDefOf.Bomb, self, -1, -1, null, null, null, null, null, 0, 1, false, null, 0, 1, 0, false);
                if (!target.Position.InHorDistOf(self.Position,
                    ck.range)) //(!WarframeStaticMethods.GetCellsAround(self.Position, self.Map, ck.range).Contains(target.Position))
                {
                    SoundDefOf.ClickReject.PlayOneShotOnCamera();
                    return;
                }

                var linec = WarframeStaticMethods.GetLineCell(self, target);
                if (linec == null)
                {
                    Messages.Message("BeBlockedByBuilding".Translate(), MessageTypeDefOf.RejectInput, false);
                    return;
                }


                SoundDef.Named("Ash_1Skill").PlayOneShot(self);
                var time = 1;
                if (self.GetLevel() > 15)
                {
                    time = 2;
                }

                for (var i = 0; i < time; i++)
                {
                    var projectile2 = (Bullet_1Ash) GenSpawn.Spawn(ThingDef.Named("Bullet_Ash1Bullet"),
                        self.Position + new IntVec3(i, 0, 0), self.Map);
                    var hitTypes = ProjectileHitFlags.All;
                    Thing gun = null;
                    projectile2.target = target as Pawn;
                    if (self.equipment is {Primary: { }})
                    {
                        gun = self.equipment.Primary;
                    }

                    projectile2.Launch(self, target.Position, target, hitTypes, false, gun);
                }

                WarframeStaticMethods.StartCooldown(self, ck.cooldownTime, 1,
                    WarframeStaticMethods.GetArmor(self).TryGetComp<CompWarframeSkill>().Props.mana1);
            };


            return ck;
        }

        //咖喱技能2
        public static Command_CastSkill Skill2()
        {
            var ck = new Command_CastSkill
            {
                defaultLabel = "AshSkill2.name".Translate(),
                icon = ContentFinder<Texture2D>.Get("Skills/AshSkill2"),
                targetingParams = WarframeStaticMethods.OnlyPawn(),
                cooldownTime = 2f,
                range = 1f,
                hotKey = KeyBindingDefOf.Misc8
            };
            ck.action = delegate(Pawn self)
            {
                SoundDef.Named("Ash_2Skill").PlayOneShot(self);


                var hediff = (Hediff_AshVanish) HediffMaker.MakeHediff(HediffDef.Named("WFVanish_Ash"), self);

                self.health.AddHediff(hediff);

                WarframeStaticMethods.StartCooldown(self, ck.cooldownTime * (1 + (self.GetLevel() * 1f / 10f)), 2,
                    WarframeStaticMethods.GetArmor(self).TryGetComp<CompWarframeSkill>().Props.mana2);
            };


            return ck;
        }


        //咖喱技能3
        public static Command_CastSkill Skill3()
        {
            var ck = new Command_CastSkillTargetingFloor
            {
                defaultLabel = "AshSkill3.name".Translate(),
                icon = ContentFinder<Texture2D>.Get("Skills/AshSkill3"),
                targetingParams = WarframeStaticMethods.JumpTP(),
                cooldownTime = 0.2f,
                range = 35f,
                hotKey = KeyBindingDefOf.Misc4
            };
            ck.finishAction = delegate
            {
                //GenDraw.DrawFieldEdges(WarframeStaticMethods.GetCellsAround(ck.self.Position, ck.self.Map, ck.range));
                GenDraw.DrawRadiusRing(ck.self.Position, ck.range);
            };
            ck.action = delegate(Pawn self, LocalTargetInfo target)
            {
                if (!target.Cell.InHorDistOf(self.Position,
                    ck.range)) //!WarframeStaticMethods.GetCellsAround(self.Position, self.Map, ck.range).Contains(target.Cell))
                {
                    SoundDefOf.ClickReject.PlayOneShotOnCamera();
                    return;
                }

                if (!target.Cell.Walkable(self.Map))
                {
                    SoundDefOf.ClickReject.PlayOneShotOnCamera();
                    return;
                }

                SoundDef.Named("Ash_3Skill").PlayOneShot(self);
                var tlist = self.Map.thingGrid.ThingsAt(target.Cell).ToList();


                foreach (var t in tlist)
                {
                    if (t is not Pawn pawn)
                    {
                        continue;
                    }

                    if (pawn.Faction.HostileTo(self.Faction))
                    {
                        pawn.stances.stunner.StunFor(180, self);
                    }
                }

                var map = self.Map;
                ThingSelectionUtility.SelectNextColonist();
                self.DeSpawn();
                GenSpawn.Spawn(self, target.Cell, map);
                self.drafter.Drafted = true;
                ThingSelectionUtility.SelectPreviousColonist();
                self.stances.stunner.StunFor(20, self);
                FleckMaker.ThrowSmoke(target.Cell.ToVector3(), map, 4f);


                WarframeStaticMethods.StartCooldown(self, ck.cooldownTime, 3,
                    WarframeStaticMethods.GetArmor(self).TryGetComp<CompWarframeSkill>().Props.mana3);
            };


            return ck;
        }

        //咖喱技能4
        public static Command_CastSkill Skill4()
        {
            var ck = new Command_CastSkill
            {
                defaultLabel = "AshSkill4.name".Translate(),
                icon = ContentFinder<Texture2D>.Get("Skills/AshSkill4"),
                targetingParams = WarframeStaticMethods.OnlyPawn(),
                cooldownTime = 0.2f,
                range = 30f,
                hotKey = KeyBindingDefOf.Misc7
            };
            ck.action = delegate(Pawn self)
            {
                Find.CameraDriver.shaker.DoShake(20000f * 15f /
                                                 (self.Position.ToVector3Shifted() - Find.Camera.transform.position)
                                                 .magnitude);
                SoundDef.Named("Ash_4Skill").PlayOneShot(self);
                /*
                {
                    Mote mote = (Mote)ThingMaker.MakeThing(ThingDef.Named("Mote_2ExFlash"), null);
                    mote.exactPosition = self.Position.ToVector3Shifted();
                    mote.Scale = (float)Mathf.Max(10f, 15f);
                    mote.rotationRate = 1.2f;
                    GenSpawn.Spawn(mote, self.Position + new IntVec3(0, 1, 0), self.Map, WipeMode.Vanish);
                }
                */
                var plist = new List<Pawn>();
                foreach (var ic in WarframeStaticMethods.GetCellsAround(self.Position, self.Map, ck.range))
                {
                    foreach (var p in self.Map.thingGrid.ThingsAt(ic))
                    {
                        if (plist.Count >= 18)
                        {
                            break;
                        }

                        if (p is not Pawn pawn)
                        {
                            continue;
                        }

                        if (pawn.Faction != self.Faction && pawn.AnimalOrWildMan() || pawn.HostileTo(self))
                        {
                            plist.Add(pawn);
                        }
                    }
                }


                var ash4t = (Ash4Thing) ThingMaker.MakeThing(ThingDef.Named("AshSkill4Item"));
                ash4t.self = self;
                ash4t.damage = 70 * (1 + (self.GetLevel() / 60f));
                ash4t.affected = plist;
                ash4t.opos = self.Position;
                GenSpawn.Spawn(ash4t, self.Position, self.Map);


                WarframeStaticMethods.StartCooldown(self, ck.cooldownTime, 4,
                    WarframeStaticMethods.GetArmor(self).TryGetComp<CompWarframeSkill>().Props.mana4);
            };


            return ck;
        }
    }
}