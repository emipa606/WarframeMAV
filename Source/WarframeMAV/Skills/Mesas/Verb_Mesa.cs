using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;
using Warframe;

namespace WarframeMAV.Skills.Mesas
{
    public class Verb_Mesa : Verb_LaunchProjectile
    {
        private float newWarmupTime=99;
        private int lastShotTick;
        public float NWT
        {
            get
            {
                return this.newWarmupTime;
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<float>(ref this.newWarmupTime,"newWarmupTime",0,false);
            Scribe_Values.Look<int>(ref this.lastShotTick, "lastShotTick", 0, false);
        }

        public void resetSpeed() {
            if (lastShotTick!=0 && Find.TickManager.TicksGame - lastShotTick >=60) {
                this.newWarmupTime = this.verbProps.warmupTime;
                this.lastShotTick = 0;
                WarframeStaticMethods.showColorText(this.caster, "ShootSpeedReset!", UnityEngine.Color.cyan, GameFont.Medium);
            }
        }


        protected override int ShotsPerBurst
        {
            get
            {
                return this.verbProps.burstShotCount;
            }
        }


        public bool MesaTryStartCastOn(LocalTargetInfo castTarg, bool surpriseAttack = false, bool canHitNonTargetPawns = true)
        {
            if (this.newWarmupTime > this.verbProps.warmupTime)
            {
                this.newWarmupTime = this.verbProps.warmupTime;
            }
            if (this.caster == null)
            {
                Log.Error("Verb " + this.GetUniqueLoadID() + " needs caster to work (possibly lost during saving/loading).", false);
                return false;
            }
            if (!this.caster.Spawned)
            {
                return false;
            }
            if (this.state == VerbState.Bursting || !this.CanHitTarget(castTarg))
            {
                return false;
            }
            if (this.CausesTimeSlowdown(castTarg))
            {
                Find.TickManager.slower.SignalForceNormalSpeed();
            }
            this.surpriseAttack = surpriseAttack;
            this.canHitNonTargetPawnsNow = canHitNonTargetPawns;
            this.currentTarget = castTarg;
            if (this.CasterIsPawn && this.verbProps.warmupTime > 0f)
            {
                ShootLine newShootLine;
                if (!this.TryFindShootLineFromTo(this.caster.Position, castTarg, out newShootLine))
                {
                    return false;
                }
                this.CasterPawn.Drawer.Notify_WarmingCastAlongLine(newShootLine, this.caster.Position);
                float statValue = this.CasterPawn.GetStatValue(StatDefOf.AimingDelayFactor, true);
                int ticks = (this.newWarmupTime * statValue).SecondsToTicks();
                this.CasterPawn.stances.SetStance(new Stance_Warmup(ticks, castTarg, this));
            }
            else
            {
                this.WarmupComplete();
            }
            return true;
        }



        private bool CausesTimeSlowdown(LocalTargetInfo castTarg)
        {
            if (!this.verbProps.CausesTimeSlowdown)
            {
                return false;
            }
            if (!castTarg.HasThing)
            {
                return false;
            }
            Thing thing = castTarg.Thing;
            return (thing.def.category == ThingCategory.Pawn || (thing.def.building != null && thing.def.building.IsTurret)) && ((thing.Faction == Faction.OfPlayer && this.caster.HostileTo(Faction.OfPlayer)) || (this.caster.Faction == Faction.OfPlayer && thing.HostileTo(Faction.OfPlayer)));
        }





        // Token: 0x06006413 RID: 25619 RVA: 0x001B4CF0 File Offset: 0x001B30F0
        public override void WarmupComplete()
        {
            base.WarmupComplete();
            Pawn pawn = this.currentTarget.Thing as Pawn;
            if (pawn != null && !pawn.Downed && base.CasterIsPawn && base.CasterPawn.skills != null)
            {
                float num = (!pawn.HostileTo(this.caster)) ? 20f : 170f;
                float num2 = this.verbProps.AdjustedFullCycleTime(this, base.CasterPawn);
                base.CasterPawn.skills.Learn(SkillDefOf.Shooting, num * num2, false);
            }
        }

        // Token: 0x06006414 RID: 25620 RVA: 0x001B4D90 File Offset: 0x001B3190
        protected override bool TryCastShot()
        {
            bool flag = base.TryCastShot();


      
            if (flag && base.CasterIsPawn)
            {
                base.CasterPawn.records.Increment(RecordDefOf.ShotsFired);

                if (this.newWarmupTime > this.verbProps.warmupTime)
                {
                    this.newWarmupTime = this.verbProps.warmupTime;
                }
                if (this.newWarmupTime > 0.001f)
                {
                    this.newWarmupTime *= 0.9f;
                    //WarframeStaticMethods.showColorText(this.caster, newWarmupTime + "?", UnityEngine.Color.cyan, GameFont.Medium);
                }

                if (this.newWarmupTime < 0.001f) this.newWarmupTime = 0;//= 0.001f;

               

                this.lastShotTick = Find.TickManager.TicksGame;



            }
          
            return flag;
        }

    }
}
