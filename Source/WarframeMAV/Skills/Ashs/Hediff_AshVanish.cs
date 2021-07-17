using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;
using Warframe;

namespace WarframeMAV.Skills.Ashs
{
    public class Hediff_AshVanish : HediffWithComps
    {
        private float getMaxTick => 200 * (1 + (pawn.GetLevel() * 1f / 10f));

        //public int level;
        public override void Tick()
        {
            ageTicks++;
            if (ageTicks > getMaxTick)
            {
                TimeOut();
            }

            if (ageTicks % 10 == 0)
            {
                DrawHediffExtras();
            }
        }

        // Token: 0x06004BF8 RID: 19448 RVA: 0x00232324 File Offset: 0x00230724
        private void TimeOut()
        {
            SoundDef.Named("Ash_2SkillEnd").PlayOneShot(pawn);
            pawn.health.RemoveHediff(this);
        }


        public void DrawHediffExtras()
        {
            // MoteMaker.ThrowExplosionInteriorMote(new Vector3(this.pawn.TrueCenter().x, 0, this.pawn.TrueCenter().z), this.pawn.Map, ThingDef.Named("Mote_ElectricalSpark"));
            // MoteMaker.ThrowSmoke(this.pawn.DrawPos,this.pawn.Map,2f);
            var loc = pawn.DrawPos;
            var map = pawn.Map;
            var size = 2f;
            if (!loc.ShouldSpawnMotesAt(map) || map.moteCounter.SaturatedLowPriority)
            {
                return;
            }

            var dataStatic = FleckMaker.GetDataStatic(loc, map, FleckDefOf.Smoke, Rand.Range(1.5f, 2.5f) * size);
            dataStatic.rotationRate = Rand.Range(-30f, 30f);
            dataStatic.velocityAngle = Rand.Range(30, 40);
            dataStatic.velocitySpeed = Rand.Range(0.5f, 0.7f);
            dataStatic.instanceColor = Color.blue;
            map.flecks.CreateFleck(dataStatic);
        }


        /*
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.level, "level", 0, false);
        }

    */
    }
}