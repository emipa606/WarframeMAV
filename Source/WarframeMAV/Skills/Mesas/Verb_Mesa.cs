using RimWorld;
using UnityEngine;
using Verse;
using Warframe;

namespace WarframeMAV.Skills.Mesas
{
    public class Verb_Mesa : Verb_LaunchProjectile
    {
        private int lastShotTick;
        private float newWarmupTime = 99;

        public float NWT => newWarmupTime;


        protected override int ShotsPerBurst => verbProps.burstShotCount;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref newWarmupTime, "newWarmupTime");
            Scribe_Values.Look(ref lastShotTick, "lastShotTick");
        }

        public void resetSpeed()
        {
            if (lastShotTick == 0 || Find.TickManager.TicksGame - lastShotTick < 60)
            {
                return;
            }

            newWarmupTime = verbProps.warmupTime;
            lastShotTick = 0;
            WarframeStaticMethods.ShowColorText(caster, "ShootSpeedReset!", Color.cyan, GameFont.Medium);
        }


        public bool MesaTryStartCastOn(LocalTargetInfo castTarg, bool attack = false,
            bool canHitNonTargetPawns = true)
        {
            if (newWarmupTime > verbProps.warmupTime)
            {
                newWarmupTime = verbProps.warmupTime;
            }

            if (caster == null)
            {
                Log.Error("Verb " + GetUniqueLoadID() + " needs caster to work (possibly lost during saving/loading).");
                return false;
            }

            if (!caster.Spawned)
            {
                return false;
            }

            if (state == VerbState.Bursting || !CanHitTarget(castTarg))
            {
                return false;
            }

            if (CausesTimeSlowdown(castTarg))
            {
                Find.TickManager.slower.SignalForceNormalSpeed();
            }

            surpriseAttack = attack;
            canHitNonTargetPawnsNow = canHitNonTargetPawns;
            currentTarget = castTarg;
            if (CasterIsPawn && verbProps.warmupTime > 0f)
            {
                if (!TryFindShootLineFromTo(caster.Position, castTarg, out var newShootLine))
                {
                    return false;
                }

                CasterPawn.Drawer.Notify_WarmingCastAlongLine(newShootLine, caster.Position);
                var statValue = CasterPawn.GetStatValue(StatDefOf.AimingDelayFactor);
                var ticks = (newWarmupTime * statValue).SecondsToTicks();
                CasterPawn.stances.SetStance(new Stance_Warmup(ticks, castTarg, this));
            }
            else
            {
                WarmupComplete();
            }

            return true;
        }


        private bool CausesTimeSlowdown(LocalTargetInfo castTarg)
        {
            if (!verbProps.CausesTimeSlowdown)
            {
                return false;
            }

            if (!castTarg.HasThing)
            {
                return false;
            }

            var thing = castTarg.Thing;
            return (thing.def.category == ThingCategory.Pawn ||
                    thing.def.building is {IsTurret: true}) &&
                   (thing.Faction == Faction.OfPlayer && caster.HostileTo(Faction.OfPlayer) ||
                    caster.Faction == Faction.OfPlayer && thing.HostileTo(Faction.OfPlayer));
        }


        // Token: 0x06006413 RID: 25619 RVA: 0x001B4CF0 File Offset: 0x001B30F0
        public override void WarmupComplete()
        {
            base.WarmupComplete();
            if (currentTarget.Thing is not Pawn pawn || pawn.Downed || !base.CasterIsPawn ||
                base.CasterPawn.skills == null)
            {
                return;
            }

            var num = !pawn.HostileTo(caster) ? 20f : 170f;
            var num2 = verbProps.AdjustedFullCycleTime(this, base.CasterPawn);
            base.CasterPawn.skills.Learn(SkillDefOf.Shooting, num * num2);
        }

        // Token: 0x06006414 RID: 25620 RVA: 0x001B4D90 File Offset: 0x001B3190
        protected override bool TryCastShot()
        {
            if (!base.TryCastShot() || !base.CasterIsPawn)
            {
                return base.TryCastShot();
            }

            base.CasterPawn.records.Increment(RecordDefOf.ShotsFired);

            if (newWarmupTime > verbProps.warmupTime)
            {
                newWarmupTime = verbProps.warmupTime;
            }

            if (newWarmupTime > 0.001f)
            {
                newWarmupTime *= 0.9f;
                //WarframeStaticMethods.ShowColorText(this.caster, newWarmupTime + "?", UnityEngine.Color.cyan, GameFont.Medium);
            }

            if (newWarmupTime < 0.001f)
            {
                newWarmupTime = 0; //= 0.001f;
            }


            lastShotTick = Find.TickManager.TicksGame;

            return base.TryCastShot();
        }
    }
}