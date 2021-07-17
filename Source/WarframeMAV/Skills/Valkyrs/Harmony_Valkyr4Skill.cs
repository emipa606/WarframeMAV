using System.Linq;
using HarmonyLib;
using UnityEngine;
using Verse;
using Warframe;
using Random = System.Random;

namespace WarframeMAV.Skills.Valkyrs
{
    [HarmonyPatch(typeof(Thing), "TakeDamage", typeof(DamageInfo))]
    public static class Harmony_Valkyr4Skill
    {
        public static void Prefix(Thing __instance, ref DamageInfo dinfo)
        {
            if (__instance is not Pawn)
            {
                return;
            }

            if (dinfo.Instigator is not Pawn wf || !wf.IsWarframe())
            {
                return;
            }

            if (wf.kindDef.defName != "Warframe_Valkyr")
            {
                return;
            }

            foreach (var hed in wf.health.hediffSet.hediffs)
            {
                if (hed.def.defName != "WFGod")
                {
                    continue;
                }

                var finaldmg = dinfo.Amount * (1 + (wf.GetLevel() * 1f / 20f)) * criMul();
                var heal = finaldmg * 0.01f * (1 + (wf.GetLevel() / 6f));


                var dinfonew = new DamageInfo(dinfo.Def, finaldmg, dinfo.ArmorPenetrationInt, dinfo.Angle,
                    dinfo.Instigator, null, dinfo.Weapon, DamageInfo.SourceCategory.ThingOrUnknown,
                    dinfo.IntendedTarget);
                dinfo = dinfonew;


                if ((from x in wf.health.hediffSet.GetHediffs<Hediff_Injury>()
                    where x.CanHealNaturally() || x.CanHealFromTending()
                    select x).TryRandomElement(out var hediff_Injury))
                {
                    hediff_Injury.Heal(10f);
                    WarframeStaticMethods.ShowColorText(wf, "HP+" + heal, new Color(0.2f, 1, 0.1f),
                        GameFont.Small);
                }


                //  WarframeStaticMethods.ShowColorText(wf, "final damage:" + finaldmg, Color.yellow, GameFont.Small);
                break;
            }
        }

        private static float criMul()
        {
            var result = 1f;
            var r = new Random(Find.TickManager.TicksGame);
            var ri = r.Next(100);
            if (ri <= 50)
            {
                result = 3f;
            }


            return result;
        }
    }
}