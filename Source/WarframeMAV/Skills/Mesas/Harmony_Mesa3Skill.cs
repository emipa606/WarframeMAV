using HarmonyLib;
using UnityEngine;
using Verse;
using Warframe;

namespace WarframeMAV.Skills.Mesas
{
    [HarmonyPatch(typeof(Thing), "TakeDamage", typeof(DamageInfo))]
    public static class Harmony_Mesa3Skill
    {
        public static void Prefix(Thing __instance, ref DamageInfo dinfo)
        {
            if (__instance is not Pawn pawn)
            {
                return;
            }

            if (!pawn.IsWarframe())
            {
                return;
            }

            foreach (var hed in pawn.health.hediffSet.hediffs)
            {
                if (hed.def.defName != "WFMesa3Skill_Mesa")
                {
                    continue;
                }

                var finaldmg = dinfo.Amount * (1 - (0.5f * (1 + (pawn.GetLevel() * 1f / 50f))));
                var dinfonew = new DamageInfo(dinfo.Def, finaldmg, dinfo.ArmorPenetrationInt, dinfo.Angle,
                    dinfo.Instigator, null, dinfo.Weapon, DamageInfo.SourceCategory.ThingOrUnknown,
                    dinfo.IntendedTarget);
                dinfo = dinfonew;
                WarframeStaticMethods.ShowColorText(pawn, "final damage:" + finaldmg, Color.yellow,
                    GameFont.Small);
                break;
            }
        }
    }
}