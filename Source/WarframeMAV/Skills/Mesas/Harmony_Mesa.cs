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

namespace WarframeMAV.Skills.Mesas
{
    //子弹击中释放伤害&储存伤害
    [HarmonyPatch(typeof(Projectile), "Impact", new Type[] {
            typeof(Thing)
        })]
    public static class Harmony_ReleaseDamageMesa
    {
        public static void Postfix(Projectile __instance,Thing hitThing)
        {
            if (hitThing != null)
            {
                Traverse tv = Traverse.Create(__instance);
                Thing launcher = tv.Field("launcher").GetValue<Thing>();
                LocalTargetInfo intendedTarget = tv.Field("intendedTarget").GetValue<LocalTargetInfo>();
                ThingDef equipmentDef = tv.Field("equipmentDef").GetValue<ThingDef>();
                if (launcher is Pawn && (launcher as Pawn).isWarframe())
                {
                    Pawn wf = launcher as Pawn;
                    foreach(Hediff hef in wf.health.hediffSet.hediffs)
                    {
                        if(hef.def.defName== "WFMesa1Skill_End")
                        {
                            Hediff_Mesa1SkillB hf = hef as Hediff_Mesa1SkillB;
                            int damage = hf.sdamage;
                            DamageInfo dinfo = new DamageInfo(DamageDefOf.Bullet, damage, 1, -1, wf, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null);
                            WarframeStaticMethods.showDamageAmount(hitThing, damage.ToString());
                            if (hitThing is Pawn && (hitThing as Pawn).Dead) return;
                            hitThing.TakeDamage(dinfo);
                            {
                                Mote mote = (Mote)ThingMaker.MakeThing(ThingDef.Named("Mote_2ExFlash"), null);
                                mote.exactPosition = hitThing.Position.ToVector3Shifted();
                                mote.Scale = (float)Mathf.Max(5f, 8f);
                                mote.rotationRate = 1.2f;
                                // mote.Scale = 0.2f;
                                GenSpawn.Spawn(mote, hitThing.Position + new IntVec3(0, 1, 0), hitThing.Map, WipeMode.Vanish);
                            }
                            wf.health.RemoveHediff(hef);
                            
                        }else if(hef.def.defName == "WFMesa1Skill_Start")
                        {
                            if(hitThing is Pawn)
                            {
                                float bfb = 0.4f * (1 + (wf.getLevel() * 1f / 30f));


                                float amount = __instance.DamageAmount*bfb;
                                float maxsdmg = 2 * (1 + (wf.getLevel() * 1f / 5f));
                                if (amount > maxsdmg) amount = maxsdmg;
                                Hediff_Mesa1SkillA hfa = hef as Hediff_Mesa1SkillA;
                                if (!hfa.isMax)
                                {
                                    WarframeStaticMethods.showColorText(hitThing, "Add " + (int)amount + " Damage", Color.green, GameFont.Small);
                                }
                                hfa.add((int)amount);

                               
                            }
                        }
                        else if (hef.def.defName == "WFMesa1Skil2_Mesa")
                        {
                            if (hitThing != null)
                            {
                                DamageDef damageDef = __instance.def.projectile.damageDef;
                                float amount = (float)__instance.DamageAmount * 0.1f * (1+ (wf.getLevel()*1f/20f));
                                float armorPenetration = __instance.ArmorPenetration;
                                float y = __instance.ExactRotation.eulerAngles.y;
                               
                                
                                DamageInfo dinfo = new DamageInfo(damageDef, amount, armorPenetration, y, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown,intendedTarget.Thing);
                                WarframeStaticMethods.showColorText(hitThing, "Extra " + (int)amount + " Damage", Color.red, GameFont.Small);
                                if (hitThing is Pawn && (hitThing as Pawn).Dead) return;
                                hitThing.TakeDamage(dinfo);
                              
                            }

                        }
                    }
                }




            }
        }
    }



    //Step1   减伤
    [HarmonyPatch(typeof(Thing), "TakeDamage", new Type[]
{
    typeof(DamageInfo)
})]
    public static class Harmony_Mesa3Skill
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
                        if (hed.def.defName == "WFMesa3Skill_Mesa")
                        {
                            float finaldmg = dinfo.Amount * (1- (0.5f * (1 + (wf.getLevel() * 1f / 50f))));
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


    //Step2   反射
    [HarmonyPatch(typeof(Projectile), "Impact", new Type[]
{
    typeof(Thing)
})]
    public static class Harmony_Mesa3SkillStep2
    {
        public static void Postfix(Projectile __instance, Thing hitThing)
        {

            if (hitThing is Pawn)
            {
                Pawn wf = hitThing as Pawn;
                if (wf != null && wf.isWarframe())
                {
                    Traverse tv = Traverse.Create(__instance);
                    Thing launcher = tv.Field("launcher").GetValue<Thing>();
                    foreach (Hediff hed in wf.health.hediffSet.hediffs)
                    {
                        if (hed.def.defName == "WFMesa3Skill_Mesa")
                        {
                            ThingDef bdef = __instance.def;
                            
                          
                            Projectile projectile2 = (Projectile)GenSpawn.Spawn(bdef,wf.Position,wf.Map);
                            ProjectileHitFlags projectileHitFlags = ProjectileHitFlags.All;
                            projectile2.Launch(wf, wf.Position.ToVector3(),launcher.Position, launcher, projectileHitFlags, null, null);


                            break;
                        }
                    }
                }
            }


        }
    }



    //监察者 重置速度
    [HarmonyPatch(typeof(Pawn_HealthTracker), "HealthTick", new Type[]
{
 
})]
    public static class Harmony_Mesa4SkillA
    {
        public static void Prefix(Pawn_HealthTracker __instance)
        {

            Traverse tv = Traverse.Create(__instance);
            Pawn wf = tv.Field("pawn").GetValue<Pawn>();
            if (wf.isWarframe())
            {
                List<Verb> verbs = wf.equipment.AllEquipmentVerbs.ToList();
                foreach (Verb vb in verbs) {
                    if(vb is Verb_Mesa)
                    {
                        (vb as Verb_Mesa).resetSpeed();
                    }
                }
            }


        }
    }


    //监察者TryStartAttack
    [HarmonyPatch(typeof(Pawn), "TryStartAttack", new Type[]
{
    typeof(LocalTargetInfo)
})]
    public static class Harmony_Mesa4SkillB
    {
        public static bool Prefix(Pawn __instance, LocalTargetInfo targ,bool __result)
        {
            
            Traverse tv = Traverse.Create(__instance);
            Pawn_StanceTracker stances = tv.Field("stances").GetValue<Pawn_StanceTracker>();
            Pawn_StoryTracker story = tv.Field("story").GetValue<Pawn_StoryTracker>();

            if (stances.FullBodyBusy)
            {
                return false;
            }
            if (story != null && story.DisabledWorkTagsBackstoryAndTraits.OverlapsWithOnAnyWorkType(WorkTags.Violent))
            {
                return false;
            }
            bool allowManualCastWeapons = !__instance.IsColonist;
            Verb verb = __instance.TryGetAttackVerb(targ.Thing, allowManualCastWeapons);
            if(verb!=null&& verb is Verb_Mesa)
            {
                Verb_Mesa vba = verb as Verb_Mesa;
               // Log.Warning("IS MESA VERB");
                __result= vba != null && vba.MesaTryStartCastOn(targ, false, true);
                return false;
            }else
            {
                return true;
            }


           


        }
    }


}
