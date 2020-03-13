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
using WarframeMAV.Skills.Mesas;

namespace Warframe.Skills
{
    public static class Mesa
    {
        //咖喱技能1
        public static Command_CastSkill Skill1()
        {
            Command_CastSkill ck = new Command_CastSkill();
            ck.defaultLabel = "MesaSkill1.name".Translate();
            ck.icon = ContentFinder<Texture2D>.Get("Skills/MesaSkill1");
            ck.targetingParams = WarframeStaticMethods.onlyPawn();
            ck.cooldownTime = 0.2f;
            ck.range = 1f;
            ck.hotKey = KeyBindingDefOf.Misc5;
            ck.action = delegate (Pawn self)
            {
                foreach(Hediff hef in self.health.hediffSet.hediffs)
                {
                    if (hef.def.defName == "WFMesa1Skill_Start")
                    {
                        Hediff_Mesa1SkillA hediffA = hef as Hediff_Mesa1SkillA;
                        Hediff_Mesa1SkillB hediffB = (Hediff_Mesa1SkillB)HediffMaker.MakeHediff(HediffDef.Named("WFMesa1Skill_End"), self, null);
                        hediffB.sdamage = hediffA.sdamage;

                        self.health.RemoveHediff(hef);
                        self.health.AddHediff(hediffB, null, null, null);
                        return;
                    }else if (hef.def.defName == "WFMesa1Skill_End")
                    {
                        Messages.Message("MesaSkillUsing".Translate(),MessageTypeDefOf.RejectInput,false);
                        return;
                    }
                }



                SoundDef.Named("Mesa_1Skill").PlayOneShot(self);
                Hediff_Mesa1SkillA hediff = (Hediff_Mesa1SkillA)HediffMaker.MakeHediff(HediffDef.Named("WFMesa1Skill_Start"), self, null);
                hediff.sdamage = 0;
                self.health.AddHediff(hediff, null, null, null);



                WarframeStaticMethods.startCooldown(self, ck.cooldownTime, 1, WarframeStaticMethods.getArmor(self).TryGetComp<CompWarframeSkill>().Props.mana1);


            };


            return ck;

        }

        //咖喱技能2
        public static Command_CastSkill Skill2()
        {
            Command_CastSkill ck = new Command_CastSkill();
            ck.defaultLabel = "MesaSkill2.name".Translate();
            ck.icon = ContentFinder<Texture2D>.Get("Skills/MesaSkill2");
            ck.targetingParams = WarframeStaticMethods.onlyPawn();
            ck.cooldownTime = 1f;
            ck.range = 1f;
            ck.hotKey = KeyBindingDefOf.Misc8;
            ck.action = delegate (Pawn self)
            {

                foreach (Hediff hef in self.health.hediffSet.hediffs)
                {
                   
                    if (hef.def.defName == "WFMesa2Skill_Mesa")
                    {
                        Messages.Message("MesaSkillUsing".Translate(), MessageTypeDefOf.RejectInput, false);
                        return;
                    }
                }





                SoundDef.Named("Mesa_2Skill").PlayOneShot(self);
                Hediff_Mesa2Skill hediff = (Hediff_Mesa2Skill)HediffMaker.MakeHediff(HediffDef.Named("WFMesa2Skill_Mesa"), self, null);
                hediff.level = (int)self.getLevel();
                self.health.AddHediff(hediff, null, null, null);



                WarframeStaticMethods.startCooldown(self, ck.cooldownTime * (1 + (self.getLevel() * 1f / 10f)), 2, WarframeStaticMethods.getArmor(self).TryGetComp<CompWarframeSkill>().Props.mana2);


            };


            return ck;

        }


        //咖喱技能3
        public static Command_CastSkill Skill3()
        {
            Command_CastSkill ck = new Command_CastSkill();
            ck.defaultLabel = "MesaSkill3.name".Translate();
            ck.icon = ContentFinder<Texture2D>.Get("Skills/MesaSkill3");
            ck.targetingParams = WarframeStaticMethods.jumpTP();
            ck.cooldownTime = 1f;
            ck.range = 1f;
            ck.hotKey = KeyBindingDefOf.Misc4;
            ck.action = delegate (Pawn self)
            {

                foreach (Hediff hef in self.health.hediffSet.hediffs)
                {

                    if (hef.def.defName == "WFMesa3Skill_Mesa")
                    {
                        Messages.Message("MesaSkillUsing".Translate(), MessageTypeDefOf.RejectInput, false);
                        return;
                    }
                }
                SoundDef.Named("Mesa_3Skill").PlayOneShot(self);
                Hediff_Mesa3Skill hediff = (Hediff_Mesa3Skill)HediffMaker.MakeHediff(HediffDef.Named("WFMesa3Skill_Mesa"), self, null);
                hediff.self = self;
                self.health.AddHediff(hediff, null, null, null);

                WarframeStaticMethods.startCooldown(self, ck.cooldownTime, 3, WarframeStaticMethods.getArmor(self).TryGetComp<CompWarframeSkill>().Props.mana3);


            };


            return ck;

        }

        //咖喱技能4
        public static Command_CastSkill Skill4()
        {
            Command_CastSkill ck = new Command_CastSkill();
            ck.defaultLabel = "MesaSkill4.name".Translate();
            ck.icon = ContentFinder<Texture2D>.Get("Skills/MesaSkill4");
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
                SoundDef.Named("Mesa_4Skill").PlayOneShot(self);
                {
                    MoteMaker.ThrowMicroSparks(self.Position.ToVector3(), self.Map);
                }

                //WFModBase.Instance._WFcontrolstorage.saveOldGun(self,self.equipment.Primary);
                if (self.equipment.Primary != null)
                    wa.oldWeapon.Add(self.equipment.Primary);

                wa.tillSkillOpen = 4;
                wa.tillSkillMul = 0.6f;
                self.equipment.Remove(self.equipment.Primary);//Primary.Destroy(DestroyMode.Vanish);
                self.equipment.AddEquipment((ThingWithComps)ThingMaker.MakeThing(ThingDef.Named("Mesa_SkillGun")));
                self.stances.stunner.StunFor(10, self);

                Hediff_NoMove hediff = (Hediff_NoMove)HediffMaker.MakeHediff(HediffDef.Named("WFMesa4Skill_Nomove"), self, null);
               
                self.health.AddHediff(hediff, null, null, null);




                WarframeStaticMethods.startCooldown(self, ck.cooldownTime, 4, WarframeStaticMethods.getArmor(self).TryGetComp<CompWarframeSkill>().Props.mana4);


            };


            return ck;

        }
        //咖喱4结束action
        public static void EndSkill4(Pawn self)
        {
            SoundDef.Named("Mesa_4SkillEnd").PlayOneShot(self);
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
                if(hef.def.defName =="WFMesa4Skill_Nomove")
                {
                    self.health.RemoveHediff(hef);
                    break;
                }
            }
        }

    }
}
