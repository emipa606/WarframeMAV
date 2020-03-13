using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;
using WarframeMAV.Skills.Ashs;

namespace Warframe.Skills
{
    public static class Ash
    {
        //咖喱技能1
        public static Command_CastSkillTargeting Skill1()
        {
            Command_CastSkillTargeting ck = new Command_CastSkillTargeting();
            ck.defaultLabel = "AshSkill1.name".Translate();
            ck.icon = ContentFinder<Texture2D>.Get("Skills/AshSkill1");
            ck.targetingParams = WarframeStaticMethods.onlyPawn();
            ck.cooldownTime = 0.2f;
            ck.range = 35f;
            ck.finishAction = delegate {
                //GenDraw.DrawFieldEdges(WarframeStaticMethods.getCellsAround(ck.self.Position, ck.self.Map, ck.range));
                GenDraw.DrawRadiusRing(ck.self.Position,ck.range);
            };
            ck.hotKey = KeyBindingDefOf.Misc5;
            ck.action = delegate (Pawn self, Thing target)
            {

                // GenExplosion.DoExplosion(self.Position, self.Map, 3.5f, DamageDefOf.Bomb, self, -1, -1, null, null, null, null, null, 0, 1, false, null, 0, 1, 0, false);
                if (!target.Position.InHorDistOf(self.Position, ck.range))//(!WarframeStaticMethods.getCellsAround(self.Position, self.Map, ck.range).Contains(target.Position))
                {
                    SoundDefOf.ClickReject.PlayOneShotOnCamera();
                    return;
                }
                List<Pawn> linec = WarframeStaticMethods.getLineCell(self, target);
                if (linec == null)
                {
                    Messages.Message("BeBlockedByBuilding".Translate(), MessageTypeDefOf.RejectInput, false);
                    return;
                }


                SoundDef.Named("Ash_1Skill").PlayOneShot(self);
                int time = 1;
                if (self.getLevel() > 15) time = 2;

                for (int i = 0; i < time; i++)
                {
                    Bullet_1Ash projectile2 = (Bullet_1Ash)GenSpawn.Spawn(ThingDef.Named("Bullet_Ash1Bullet"), self.Position+new IntVec3(i,0,0), self.Map, WipeMode.Vanish);
                    ProjectileHitFlags projectileHitFlags = ProjectileHitFlags.All;
                    Thing gun = null;
                    projectile2.target = target as Pawn;
                    if (self.equipment != null && self.equipment.Primary != null)
                        gun = self.equipment.Primary;

                    projectile2.Launch(self, target.Position, target, projectileHitFlags, gun);
                }
               
                WarframeStaticMethods.startCooldown(self, ck.cooldownTime, 1, WarframeStaticMethods.getArmor(self).TryGetComp<CompWarframeSkill>().Props.mana1);


            };


            return ck;

        }

        //咖喱技能2
        public static Command_CastSkill Skill2()
        {
            Command_CastSkill ck = new Command_CastSkill();
            ck.defaultLabel = "AshSkill2.name".Translate();
            ck.icon = ContentFinder<Texture2D>.Get("Skills/AshSkill2");
            ck.targetingParams = WarframeStaticMethods.onlyPawn();
            ck.cooldownTime = 2f;
            ck.range = 1f;
            ck.hotKey = KeyBindingDefOf.Misc8;
            ck.action = delegate (Pawn self)
            {
                SoundDef.Named("Ash_2Skill").PlayOneShot(self);
               

                Hediff_AshVanish hediff = (Hediff_AshVanish)HediffMaker.MakeHediff(HediffDef.Named("WFVanish_Ash"), self, null);
                
                self.health.AddHediff(hediff,null,null,null);

                WarframeStaticMethods.startCooldown(self, ck.cooldownTime * (1 + (self.getLevel() * 1f / 10f)), 2, WarframeStaticMethods.getArmor(self).TryGetComp<CompWarframeSkill>().Props.mana2);


            };


            return ck;

        }


        //咖喱技能3
        public static Command_CastSkill Skill3()
        {
            Command_CastSkillTargetingFloor ck = new Command_CastSkillTargetingFloor();
            ck.defaultLabel = "AshSkill3.name".Translate();
            ck.icon = ContentFinder<Texture2D>.Get("Skills/AshSkill3");
            ck.targetingParams = WarframeStaticMethods.jumpTP();
            ck.cooldownTime = 0.2f;
            ck.range = 35f;
            ck.hotKey = KeyBindingDefOf.Misc4;
            ck.finishAction = delegate {
                //GenDraw.DrawFieldEdges(WarframeStaticMethods.getCellsAround(ck.self.Position, ck.self.Map, ck.range));
                GenDraw.DrawRadiusRing(ck.self.Position, ck.range);
            };
            ck.action = delegate (Pawn self,LocalTargetInfo target)
            {
                if (!target.Cell.InHorDistOf(self.Position,ck.range))//!WarframeStaticMethods.getCellsAround(self.Position, self.Map, ck.range).Contains(target.Cell))
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
                List<Thing> tlist = self.Map.thingGrid.ThingsAt(target.Cell).ToList();



                foreach (Thing t in tlist)
                {
                    if (t is Pawn)
                        if ((t as Pawn).Faction.HostileTo(self.Faction))
                        {
                            (t as Pawn).stances.stunner.StunFor(180, self);

                        }

                }

                Pawn casterPawn = self;
                Map map = self.Map;
                ThingSelectionUtility.SelectNextColonist();
                self.DeSpawn(DestroyMode.Vanish);
                GenSpawn.Spawn(casterPawn, target.Cell, map, WipeMode.Vanish);
                casterPawn.drafter.Drafted = true;
                ThingSelectionUtility.SelectPreviousColonist();
                casterPawn.stances.stunner.StunFor(20, self);
                MoteMaker.ThrowSmoke(target.Cell.ToVector3(),map,4f);



                WarframeStaticMethods.startCooldown(self, ck.cooldownTime, 3, WarframeStaticMethods.getArmor(self).TryGetComp<CompWarframeSkill>().Props.mana3);


            };


            return ck;

        }

        //咖喱技能4
        public static Command_CastSkill Skill4()
        {
            Command_CastSkill ck = new Command_CastSkill();
            ck.defaultLabel = "AshSkill4.name".Translate();
            ck.icon = ContentFinder<Texture2D>.Get("Skills/AshSkill4");
            ck.targetingParams = WarframeStaticMethods.onlyPawn();
            ck.cooldownTime = 0.2f;
            ck.range = 30f;
            ck.hotKey = KeyBindingDefOf.Misc7;
            ck.action = delegate (Pawn self)
            {
             
                


                Find.CameraDriver.shaker.DoShake(20000f * 15f / (self.Position.ToVector3Shifted() - Find.Camera.transform.position).magnitude);
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
                List<Pawn> plist = new List<Pawn>();
                foreach(IntVec3 ic in WarframeStaticMethods.getCellsAround(self.Position, self.Map, ck.range))
                {
                    foreach(Thing p in self.Map.thingGrid.ThingsAt(ic))
                    {
                        if (plist.Count >= 18) break;

                        if(p is Pawn)
                        {
                            Pawn tar = p as Pawn;
                            if ((tar.Faction!=self.Faction&& (tar.AnimalOrWildMan())|| tar.HostileTo(self)))
                            {
                                plist.Add(tar);
                            }
                        }
                    }
                }



                Ash4Thing ash4t = (Ash4Thing)ThingMaker.MakeThing(ThingDef.Named("AshSkill4Item"));
                ash4t.self = self;
                ash4t.damage = 70 * (1 + self.getLevel() / 60f);
                ash4t.affected = plist;
                ash4t.opos = self.Position;
                GenSpawn.Spawn(ash4t, self.Position, self.Map);




                WarframeStaticMethods.startCooldown(self, ck.cooldownTime, 4, WarframeStaticMethods.getArmor(self).TryGetComp<CompWarframeSkill>().Props.mana4);


            };


            return ck;

        }

    }
}
