using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;
using Warframe.Skills.WFPublic;
using WarframeMAV.Skills.Valkyrs;

namespace Warframe.Skills
{
    public static class Valkyr
    {
        //咖喱技能1
        public static Command_CastSkillTargeting Skill1()
        {
            Command_CastSkillTargeting ck = new Command_CastSkillTargeting();
            ck.defaultLabel = "ValkyrSkill1.name".Translate();
            ck.icon = ContentFinder<Texture2D>.Get("Skills/ValkyrSkill1");
            ck.targetingParams = WarframeStaticMethods.onlyPawn();
            ck.cooldownTime = 0.7f;
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





                SoundDef.Named("Valkyr_1Skill").PlayOneShot(self);
                Valkyr1Thing ash4t = (Valkyr1Thing)ThingMaker.MakeThing(ThingDef.Named("ValkyrSkill1Item"));
                ash4t.self = self;
                ash4t.target = target as Pawn;
                ash4t.startTick = Find.TickManager.TicksGame;
                GenSpawn.Spawn(ash4t, self.Position, self.Map);

                Hediff_Valkyr1Skill hediff = (Hediff_Valkyr1Skill)HediffMaker.MakeHediff(HediffDef.Named("WFValkyr1Skill_Mul"), self, null);
                WarframeStaticMethods.startCooldown(self, ck.cooldownTime, 1, WarframeStaticMethods.getArmor(self).TryGetComp<CompWarframeSkill>().Props.mana1);

                foreach (Hediff hef in self.health.hediffSet.hediffs)
                {
                    if (hef.def.defName == "WFValkyr1Skill_Mul")
                    {
                        Hediff_Valkyr1Skill hev = hef as Hediff_Valkyr1Skill;
                        hev.mul += 2;
                        return;
                    }
                }
                

                
                self.health.AddHediff(hediff, null, null, null);
            };


            return ck;

        }

        //咖喱技能2
        public static Command_CastSkill Skill2()
        {
            Command_CastSkill ck = new Command_CastSkill();
            ck.defaultLabel = "ValkyrSkill2.name".Translate();
            ck.icon = ContentFinder<Texture2D>.Get("Skills/ValkyrSkill2");
            ck.targetingParams = WarframeStaticMethods.onlyPawn();
            ck.cooldownTime = 2f;
            ck.range = 16f;
            ck.hotKey = KeyBindingDefOf.Misc8;
            ck.action = delegate (Pawn self)
            {
                SoundDef.Named("Valkyr_2Skill").PlayOneShot(self);
                self.stances.stunner.StunFor(40,self);

                foreach (IntVec3 ic in WarframeStaticMethods.getCellsAround(self.Position,self.Map,ck.range))
                {
                    foreach(Thing th in self.Map.thingGrid.ThingsAt(ic))
                    {
                        if(th is Pawn)
                        {
                            if((th as Pawn).HostileTo(self.Faction))
                            {
                                Hediff_Valkyr2Skill hediff = (Hediff_Valkyr2Skill)HediffMaker.MakeHediff(HediffDef.Named("WFValkyr2Skill_Enemy"), self, null);
                                hediff.self = self;
                                (th as Pawn).health.AddHediff(hediff, null, null, null);
                            }
                            else
                            {
                                Hediff_Valkyr2Skill hediff = (Hediff_Valkyr2Skill)HediffMaker.MakeHediff(HediffDef.Named("WFValkyr2Skill_Ally"), self, null);
                                hediff.self = self;
                                (th as Pawn).health.AddHediff(hediff, null, null, null);
                            }
                            
                        }
                    }
                }

                
                

                WarframeStaticMethods.startCooldown(self, ck.cooldownTime * (1 + (self.getLevel() * 1f / 10f)), 2, WarframeStaticMethods.getArmor(self).TryGetComp<CompWarframeSkill>().Props.mana2);


            };


            return ck;

        }


        //咖喱技能3
        public static Command_CastSkill Skill3()
        {
            Command_CastSkill ck = new Command_CastSkill();
            ck.defaultLabel = "ValkyrSkill3.name".Translate();
            ck.icon = ContentFinder<Texture2D>.Get("Skills/ValkyrSkill3");
            ck.targetingParams = WarframeStaticMethods.jumpTP();
            ck.cooldownTime = 0.2f;
            ck.range = 10f;
            ck.hotKey = KeyBindingDefOf.Misc4;
            ck.action = delegate (Pawn self)
            {

               
                SoundDef.Named("Valkyr_3Skill").PlayOneShot(self);
                WarframeBelt wb = WarframeStaticMethods.getBelt(self);
                float damage = wb.Energy * 5;
                float levelmul = 2 + (1 * self.getLevel() / 30f);
                damage *= levelmul;




                foreach (IntVec3 ic in WarframeStaticMethods.getCellsAround(self.Position, self.Map, ck.range))
                {
                    foreach (Thing th in self.Map.thingGrid.ThingsAt(ic))
                    {
                        if (th is Pawn)
                        {
                            if ((th as Pawn).HostileTo(self.Faction))
                            {
                                (th as Pawn).stances.stunner.StunFor(150,self);
                                DamageInfo dinfo = new DamageInfo(DamageDefOf.Blunt, damage,0, -1, self, null, null, DamageInfo.SourceCategory.ThingOrUnknown, th);
                                WarframeStaticMethods.showDamageAmount(th,damage+"");
                                th.TakeDamage(dinfo);
                                MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(ThingDefOf.Mote_Smoke, null);
                                moteThrown.Scale = Rand.Range(1.5f, 2.5f);
                                moteThrown.rotationRate = Rand.Range(-30f, 30f);
                                moteThrown.exactPosition = th.Position.ToVector3();
                                moteThrown.SetVelocity((float)Rand.Range(30, 40), Rand.Range(0.5f, 0.7f));
                                moteThrown.instanceColor = Color.white;
                                GenSpawn.Spawn(moteThrown, th.Position, self.Map, WipeMode.Vanish);
                            }


                        }
                    }
                }




                float adde = wb.Energy * -0.33f;
                wb.addEnergy(adde*100);
                DamageInfo wfdinfo = new DamageInfo(DamageDefOf.Stun, 1, 0, -1, self, null, null, DamageInfo.SourceCategory.ThingOrUnknown, self);
                self.TakeDamage(wfdinfo);

                WarframeStaticMethods.startCooldown(self, ck.cooldownTime, 3, WarframeStaticMethods.getArmor(self).TryGetComp<CompWarframeSkill>().Props.mana3);


            };


            return ck;

        }

        //咖喱技能4
        public static Command_CastSkill Skill4()
        {
            Command_CastSkill ck = new Command_CastSkill();
            ck.defaultLabel = "ValkyrSkill4.name".Translate();
            ck.icon = ContentFinder<Texture2D>.Get("Skills/ValkyrSkill4");
            ck.targetingParams = WarframeStaticMethods.onlyPawn();
            ck.cooldownTime = 0.2f;
            ck.range = 1f;
            ck.hotKey = KeyBindingDefOf.Misc7;
            // WarframeArmor sa = WarframeStaticMethods.getArmor(ck.self);

            ck.action = delegate (Pawn self)
            {
                WarframeArmor wa = WarframeStaticMethods.getArmor(self);
                if (wa.tillSkillOpen > 0)
                {
                    EndSkill4(self);

                    return;
                }


                Find.CameraDriver.shaker.DoShake(20000f * 15f / (self.Position.ToVector3Shifted() - Find.Camera.transform.position).magnitude);
                SoundDef.Named("Valkyr_4Skill").PlayOneShot(self);
                {
                    Mote mote = (Mote)ThingMaker.MakeThing(ThingDef.Named("Mote_2ExFlash"), null);
                    mote.exactPosition = self.Position.ToVector3Shifted();
                    mote.Scale = (float)Mathf.Max(10f, 15f);
                    mote.rotationRate = 1.2f;
                    GenSpawn.Spawn(mote, self.Position + new IntVec3(0, 1, 0), self.Map, WipeMode.Vanish);
                }

                //WFModBase.Instance._WFcontrolstorage.saveOldGun(self,self.equipment.Primary);
                if (self.equipment.Primary != null)
                    wa.oldWeapon.Add(self.equipment.Primary);

                wa.tillSkillOpen = 4;
                wa.tillSkillMul = 0.3f;
                self.equipment.Remove(self.equipment.Primary);//Primary.Destroy(DestroyMode.Vanish);
                self.equipment.AddEquipment((ThingWithComps)ThingMaker.MakeThing(ThingDef.Named("Valkyr_SkillBlade")));
                self.stances.stunner.StunFor(60, self);

                Hediff_God hediff = (Hediff_God)HediffMaker.MakeHediff(HediffDef.Named("WFGod"), self, null);

                self.health.AddHediff(hediff, null, null, null);




                WarframeStaticMethods.startCooldown(self, ck.cooldownTime, 4, WarframeStaticMethods.getArmor(self).TryGetComp<CompWarframeSkill>().Props.mana4);


            };


            return ck;

        }
        //咖喱4结束action
        public static void EndSkill4(Pawn self)
        {
            SoundDef.Named("Valkyr_4SkillEnd").PlayOneShot(self);
            WarframeArmor wa = WarframeStaticMethods.getArmor(self);
            self.equipment.Remove(self.equipment.Primary);//.Primary.Destroy(DestroyMode.Vanish);
            ThingWithComps gun = null;
            try
            {
                // gun = WFModBase.Instance._WFcontrolstorage.getOldGun(self);
                gun = wa.oldWeapon[0];
            }
            catch (Exception)
            {
                // Log.Warning("gun is null");
            }
            // WFModBase.Instance._WFcontrolstorage.clearWFandOG(self);
            wa.oldWeapon.Clear();

            if (gun != null)
                self.equipment.AddEquipment(gun);





            wa.tillSkillOpen = 0;
            wa.tillSkillMul = 1;

            foreach (Hediff hef in self.health.hediffSet.hediffs)
            {
                if (hef.def.defName == "WFGod")
                {
                    self.health.RemoveHediff(hef);
                    break;
                }
            }
        }

    }
}
