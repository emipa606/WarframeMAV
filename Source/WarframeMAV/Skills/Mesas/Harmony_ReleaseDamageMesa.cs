using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Warframe;

namespace WarframeMAV.Skills.Mesas
{
    //子弹击中释放伤害&储存伤害
    [HarmonyPatch(typeof(Projectile), "Impact", typeof(Thing))]
    public static class Harmony_ReleaseDamageMesa
    {
        public static void Postfix(Projectile __instance, Thing hitThing)
        {
            if (hitThing == null)
            {
                return;
            }

            var tv = Traverse.Create(__instance);
            var launcher = tv.Field("launcher").GetValue<Thing>();
            var intendedTarget = tv.Field("intendedTarget").GetValue<LocalTargetInfo>();
            var equipmentDef = tv.Field("equipmentDef").GetValue<ThingDef>();
            if (launcher is not Pawn pawn || !pawn.IsWarframe())
            {
                return;
            }

            foreach (var hef in pawn.health.hediffSet.hediffs)
            {
                if (hef.def.defName == "WFMesa1Skill_End")
                {
                    if (hef is Hediff_Mesa1SkillB hf)
                    {
                        var damage = hf.sdamage;
                        var dinfo = new DamageInfo(DamageDefOf.Bullet, damage, 1, -1, pawn);
                        WarframeStaticMethods.ShowDamageAmount(hitThing, damage.ToString());
                        if (hitThing is Pawn {Dead: true})
                        {
                            return;
                        }

                        hitThing.TakeDamage(dinfo);
                    }

                    {
                        var mote = (Mote) ThingMaker.MakeThing(ThingDef.Named("Mote_2ExFlash"));
                        mote.exactPosition = hitThing.Position.ToVector3Shifted();
                        mote.Scale = Mathf.Max(5f, 8f);
                        mote.rotationRate = 1.2f;
                        // mote.Scale = 0.2f;
                        GenSpawn.Spawn(mote, hitThing.Position + new IntVec3(0, 1, 0), hitThing.Map);
                    }
                    pawn.health.RemoveHediff(hef);
                }
                else if (hef.def.defName == "WFMesa1Skill_Start")
                {
                    if (hitThing is not Pawn)
                    {
                        continue;
                    }

                    var bfb = 0.4f * (1 + (pawn.GetLevel() * 1f / 30f));


                    var amount = __instance.DamageAmount * bfb;
                    var maxsdmg = 2 * (1 + (pawn.GetLevel() * 1f / 5f));
                    if (amount > maxsdmg)
                    {
                        amount = maxsdmg;
                    }

                    var hfa = hef as Hediff_Mesa1SkillA;
                    if (hfa is {isMax: false})
                    {
                        WarframeStaticMethods.ShowColorText(hitThing, "Add " + (int) amount + " Damage",
                            Color.green, GameFont.Small);
                    }

                    if (hfa != null)
                    {
                        hfa.add((int) amount);
                    }
                }
                else if (hef.def.defName == "WFMesa1Skil2_Mesa")
                {
                    var damageDef = __instance.def.projectile.damageDef;
                    var amount = __instance.DamageAmount * 0.1f * (1 + (pawn.GetLevel() * 1f / 20f));
                    var armorPenetration = __instance.ArmorPenetration;
                    var y = __instance.ExactRotation.eulerAngles.y;


                    var dinfo = new DamageInfo(damageDef, amount, armorPenetration, y, pawn, null,
                        equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, intendedTarget.Thing);
                    WarframeStaticMethods.ShowColorText(hitThing, "Extra " + (int) amount + " Damage",
                        Color.red, GameFont.Small);
                    if (hitThing is Pawn {Dead: true})
                    {
                        return;
                    }

                    hitThing.TakeDamage(dinfo);
                }
            }
        }
    }


    //Step1   减伤


    //Step2   反射


    //监察者 重置速度


    //监察者TryStartAttack
}