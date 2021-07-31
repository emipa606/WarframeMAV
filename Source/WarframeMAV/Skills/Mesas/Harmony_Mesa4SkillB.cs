using HarmonyLib;
using RimWorld;
using Verse;
using Warframe;

namespace WarframeMAV.Skills.Mesas
{
    [HarmonyPatch(typeof(Pawn), "TryStartAttack", typeof(LocalTargetInfo))]
    public static class Harmony_Mesa4SkillB
    {
        public static bool Prefix(Pawn __instance, LocalTargetInfo targ, ref bool __result)
        {
            if (!__instance.IsWarframe())
            {
                return true;
            }

            var tv = Traverse.Create(__instance);
            var stances = tv.Field("stances").GetValue<Pawn_StanceTracker>();
            var story = tv.Field("story").GetValue<Pawn_StoryTracker>();

            if (stances.FullBodyBusy)
            {
                return false;
            }

            if (story != null && story.DisabledWorkTagsBackstoryAndTraits.OverlapsWithOnAnyWorkType(WorkTags.Violent))
            {
                return false;
            }

            var allowManualCastWeapons = !__instance.IsColonist;
            var verb = __instance.TryGetAttackVerb(targ.Thing, allowManualCastWeapons);
            if (verb == null || verb is not Verb_Mesa)
            {
                return true;
            }

            // Log.Warning("IS MESA VERB");
            __result = verb is Verb_Mesa vba && vba.MesaTryStartCastOn(targ);
            return false;
        }
    }
}