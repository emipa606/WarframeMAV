using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Warframe;

namespace WarframeMAV.Skills.Valkyrs
{


    //技能2减伤
    [HarmonyPatch(typeof(Thing), "TakeDamage", new Type[]
{
    typeof(DamageInfo)
})]
    public static class Harmony_Valkyr2SkillAlly
    {
        public static void Prefix(Thing __instance, ref DamageInfo dinfo)
        {

            if (__instance is Pawn)
            {
                Pawn wf = __instance as Pawn;
                if (wf != null && wf.isWarframe())
                {
                    foreach (Hediff hed in wf.health.hediffSet.hediffs)
                    {
                        if (hed.def.defName == "WFValkyr2Skill_Ally")
                        {
                            float finaldmg = dinfo.Amount * (1 - (0.25f * (1 + (wf.getLevel() * 1f / 30f))));
                            DamageInfo dinfonew = new DamageInfo(dinfo.Def, finaldmg, dinfo.ArmorPenetrationInt, dinfo.Angle, dinfo.Instigator, null, dinfo.Weapon, DamageInfo.SourceCategory.ThingOrUnknown, dinfo.IntendedTarget);
                            dinfo = dinfonew;
                            WarframeStaticMethods.showColorText(wf,"final damage:"+finaldmg,Color.yellow,GameFont.Small);
                            break;
                        }
                    }
                }
            }
           

        }
    }


    //技能4回血&&增加伤害
    [HarmonyPatch(typeof(Thing), "TakeDamage", new Type[]
{
    typeof(DamageInfo)
})]
    public static class Harmony_Valkyr4Skill
    {
        public static void Prefix(Thing __instance,ref DamageInfo dinfo)
        {

            if (__instance is Pawn)
            {
                Pawn target = __instance as Pawn;
                Pawn wf = dinfo.Instigator as Pawn;
                if (wf != null && target!=null&&wf.isWarframe())
                {
                    if (wf.kindDef.defName != "Warframe_Valkyr") return;

                    foreach (Hediff hed in wf.health.hediffSet.hediffs)
                    {
                        if (hed.def.defName == "WFGod")
                        { 
                            float finaldmg = dinfo.Amount * ((1 + (wf.getLevel() * 1f / 20f))) *criMul();
                            float heal = finaldmg * 0.01f * (1 + wf.getLevel() / 6f);




                            DamageInfo dinfonew = new DamageInfo(dinfo.Def, finaldmg, dinfo.ArmorPenetrationInt, dinfo.Angle, dinfo.Instigator, null, dinfo.Weapon, DamageInfo.SourceCategory.ThingOrUnknown, dinfo.IntendedTarget);
                            dinfo = dinfonew;


                            Hediff_Injury hediff_Injury;
                            if ((from x in wf.health.hediffSet.GetHediffs<Hediff_Injury>()
                                 where x.CanHealNaturally() || x.CanHealFromTending()
                                 select x).TryRandomElement(out hediff_Injury))
                            {
                                hediff_Injury.Heal(10f);
                                WarframeStaticMethods.showColorText(wf, "HP+"+heal, new Color(0.2f, 1, 0.1f), GameFont.Small);
                            }



                          //  WarframeStaticMethods.showColorText(wf, "final damage:" + finaldmg, Color.yellow, GameFont.Small);
                            break;
                        }
                    }
                }
            }


        }

        private static float criMul() {
            float result = 1f;
            System.Random r = new System.Random(Find.TickManager.TicksGame);
            int ri = r.Next(100);
            if (ri <= 50)
            {
                result = 3f;
            }


            return result;
        }
    }

}
