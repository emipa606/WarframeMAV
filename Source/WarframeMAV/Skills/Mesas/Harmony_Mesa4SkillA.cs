using System;
using System.Linq;
using HarmonyLib;
using Verse;
using Warframe;

namespace WarframeMAV.Skills.Mesas
{
    [HarmonyPatch(typeof(Pawn_HealthTracker), "HealthTick", new Type[]
    {
    })]
    public static class Harmony_Mesa4SkillA
    {
        public static void Prefix(Pawn_HealthTracker __instance)
        {
            var tv = Traverse.Create(__instance);
            var wf = tv.Field("pawn").GetValue<Pawn>();
            if (!wf.IsWarframe())
            {
                return;
            }

            var verbs = wf.equipment.AllEquipmentVerbs.ToList();
            foreach (var vb in verbs)
            {
                if (vb is Verb_Mesa mesa)
                {
                    mesa.resetSpeed();
                }
            }
        }
    }
}