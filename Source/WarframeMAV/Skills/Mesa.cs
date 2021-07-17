using System;
using RimWorld;
using UnityEngine;
using Verse;
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
            var ck = new Command_CastSkill
            {
                defaultLabel = "MesaSkill1.name".Translate(),
                icon = ContentFinder<Texture2D>.Get("Skills/MesaSkill1"),
                targetingParams = WarframeStaticMethods.OnlyPawn(),
                cooldownTime = 0.2f,
                range = 1f,
                hotKey = KeyBindingDefOf.Misc5
            };
            ck.action = delegate(Pawn self)
            {
                foreach (var hef in self.health.hediffSet.hediffs)
                {
                    if (hef.def.defName == "WFMesa1Skill_Start")
                    {
                        var hediffA = hef as Hediff_Mesa1SkillA;
                        var hediffB =
                            (Hediff_Mesa1SkillB) HediffMaker.MakeHediff(HediffDef.Named("WFMesa1Skill_End"), self);
                        if (hediffA != null)
                        {
                            hediffB.sdamage = hediffA.sdamage;
                        }

                        self.health.RemoveHediff(hef);
                        self.health.AddHediff(hediffB);
                        return;
                    }

                    if (hef.def.defName != "WFMesa1Skill_End")
                    {
                        continue;
                    }

                    Messages.Message("MesaSkillUsing".Translate(), MessageTypeDefOf.RejectInput, false);
                    return;
                }


                SoundDef.Named("Mesa_1Skill").PlayOneShot(self);
                var hediff = (Hediff_Mesa1SkillA) HediffMaker.MakeHediff(HediffDef.Named("WFMesa1Skill_Start"), self);
                hediff.sdamage = 0;
                self.health.AddHediff(hediff);


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
                defaultLabel = "MesaSkill2.name".Translate(),
                icon = ContentFinder<Texture2D>.Get("Skills/MesaSkill2"),
                targetingParams = WarframeStaticMethods.OnlyPawn(),
                cooldownTime = 1f,
                range = 1f,
                hotKey = KeyBindingDefOf.Misc8
            };
            ck.action = delegate(Pawn self)
            {
                foreach (var hef in self.health.hediffSet.hediffs)
                {
                    if (hef.def.defName != "WFMesa2Skill_Mesa")
                    {
                        continue;
                    }

                    Messages.Message("MesaSkillUsing".Translate(), MessageTypeDefOf.RejectInput, false);
                    return;
                }


                SoundDef.Named("Mesa_2Skill").PlayOneShot(self);
                var hediff = (Hediff_Mesa2Skill) HediffMaker.MakeHediff(HediffDef.Named("WFMesa2Skill_Mesa"), self);
                hediff.level = (int) self.GetLevel();
                self.health.AddHediff(hediff);


                WarframeStaticMethods.StartCooldown(self, ck.cooldownTime * (1 + (self.GetLevel() * 1f / 10f)), 2,
                    WarframeStaticMethods.GetArmor(self).TryGetComp<CompWarframeSkill>().Props.mana2);
            };


            return ck;
        }


        //咖喱技能3
        public static Command_CastSkill Skill3()
        {
            var ck = new Command_CastSkill
            {
                defaultLabel = "MesaSkill3.name".Translate(),
                icon = ContentFinder<Texture2D>.Get("Skills/MesaSkill3"),
                targetingParams = WarframeStaticMethods.JumpTP(),
                cooldownTime = 1f,
                range = 1f,
                hotKey = KeyBindingDefOf.Misc4
            };
            ck.action = delegate(Pawn self)
            {
                foreach (var hef in self.health.hediffSet.hediffs)
                {
                    if (hef.def.defName != "WFMesa3Skill_Mesa")
                    {
                        continue;
                    }

                    Messages.Message("MesaSkillUsing".Translate(), MessageTypeDefOf.RejectInput, false);
                    return;
                }

                SoundDef.Named("Mesa_3Skill").PlayOneShot(self);
                var hediff = (Hediff_Mesa3Skill) HediffMaker.MakeHediff(HediffDef.Named("WFMesa3Skill_Mesa"), self);
                hediff.self = self;
                self.health.AddHediff(hediff);

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
                defaultLabel = "MesaSkill4.name".Translate(),
                icon = ContentFinder<Texture2D>.Get("Skills/MesaSkill4"),
                targetingParams = WarframeStaticMethods.OnlyPawn(),
                cooldownTime = 0.2f,
                range = 1f,
                hotKey = KeyBindingDefOf.Misc7
            };
            // WarframeArmor sa = WarframeStaticMethods.GetArmor(ck.self);

            ck.action = delegate(Pawn self)
            {
                var wa = WarframeStaticMethods.GetArmor(self);
                if (wa.tillSkillOpen > 0)
                {
                    EndSkill4(self);

                    return;
                }


                Find.CameraDriver.shaker.DoShake(20000f * 15f /
                                                 (self.Position.ToVector3Shifted() - Find.Camera.transform.position)
                                                 .magnitude);
                SoundDef.Named("Mesa_4Skill").PlayOneShot(self);
                {
                    FleckMaker.ThrowMicroSparks(self.Position.ToVector3(), self.Map);
                }

                //WFModBase.Instance._WFcontrolstorage.saveOldGun(self,self.equipment.Primary);
                if (self.equipment.Primary != null)
                {
                    wa.oldWeapon.Add(self.equipment.Primary);
                }

                wa.tillSkillOpen = 4;
                wa.tillSkillMul = 0.6f;
                self.equipment.Remove(self.equipment.Primary); //Primary.Destroy(DestroyMode.Vanish);
                self.equipment.AddEquipment((ThingWithComps) ThingMaker.MakeThing(ThingDef.Named("Mesa_SkillGun")));
                self.stances.stunner.StunFor(10, self);

                var hediff = (Hediff_NoMove) HediffMaker.MakeHediff(HediffDef.Named("WFMesa4Skill_Nomove"), self);

                self.health.AddHediff(hediff);


                WarframeStaticMethods.StartCooldown(self, ck.cooldownTime, 4,
                    WarframeStaticMethods.GetArmor(self).TryGetComp<CompWarframeSkill>().Props.mana4);
            };


            return ck;
        }

        //咖喱4结束action
        public static void EndSkill4(Pawn self)
        {
            SoundDef.Named("Mesa_4SkillEnd").PlayOneShot(self);
            var wa = WarframeStaticMethods.GetArmor(self);
            self.equipment.Remove(self.equipment.Primary); //.Primary.Destroy(DestroyMode.Vanish);
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
            {
                self.equipment.AddEquipment(gun);
            }


            wa.tillSkillOpen = 0;
            wa.tillSkillMul = 1;

            foreach (var hef in self.health.hediffSet.hediffs)
            {
                if (hef.def.defName != "WFMesa4Skill_Nomove")
                {
                    continue;
                }

                self.health.RemoveHediff(hef);
                break;
            }
        }
    }
}