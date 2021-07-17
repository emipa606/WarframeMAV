using HarmonyLib;
using Verse;
using Warframe;

namespace WarframeMAV.Skills.Mesas
{
    [HarmonyPatch(typeof(Projectile), "Impact", typeof(Thing))]
    public static class Harmony_Mesa3SkillStep2
    {
        public static void Postfix(Projectile __instance, Thing hitThing)
        {
            if (hitThing is not Pawn pawn)
            {
                return;
            }

            if (!pawn.IsWarframe())
            {
                return;
            }

            var tv = Traverse.Create(__instance);
            var launcher = tv.Field("launcher").GetValue<Thing>();
            foreach (var hed in pawn.health.hediffSet.hediffs)
            {
                if (hed.def.defName != "WFMesa3Skill_Mesa")
                {
                    continue;
                }

                var bdef = __instance.def;


                var projectile2 = (Projectile) GenSpawn.Spawn(bdef, pawn.Position, pawn.Map);
                var hitTypes = ProjectileHitFlags.All;
                projectile2.Launch(pawn, pawn.Position.ToVector3(), launcher.Position, launcher,
                    hitTypes);


                break;
            }
        }
    }
}