using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;
using Warframe;

namespace WarframeMAV.Skills.Ashs
{
    public class Hediff_AshVanish : HediffWithComps
    {

        //public int level;
        public override void Tick()
        {
            this.ageTicks++;
            if (this.ageTicks > getMaxTick)
            {
                this.TimeOut();
            }

            if (this.ageTicks % 10 == 0)
                DrawHediffExtras();


        }

        // Token: 0x06004BF8 RID: 19448 RVA: 0x00232324 File Offset: 0x00230724
        private void TimeOut()
        {
            SoundDef.Named("Ash_2SkillEnd").PlayOneShot(this.pawn);
            this.pawn.health.RemoveHediff(this);
        }


        private float getMaxTick
        {
            get
            {
                return (200 * (1 + (this.pawn.getLevel() * 1f / 10f)));
            }
        }


        public void DrawHediffExtras()
        {

            // MoteMaker.ThrowExplosionInteriorMote(new Vector3(this.pawn.TrueCenter().x, 0, this.pawn.TrueCenter().z), this.pawn.Map, ThingDef.Named("Mote_ElectricalSpark"));
           // MoteMaker.ThrowSmoke(this.pawn.DrawPos,this.pawn.Map,2f);
            Vector3 loc = this.pawn.DrawPos;
            Map map = this.pawn.Map;
            float size = 2f;
            if (!loc.ShouldSpawnMotesAt(map) || map.moteCounter.SaturatedLowPriority)
            {
                return;
            }
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(ThingDefOf.Mote_Smoke, null);
            moteThrown.Scale = Rand.Range(1.5f, 2.5f) * size;
            moteThrown.rotationRate = Rand.Range(-30f, 30f);
            moteThrown.exactPosition = loc;
            moteThrown.SetVelocity((float)Rand.Range(30, 40), Rand.Range(0.5f, 0.7f));
            moteThrown.instanceColor = Color.blue;
            GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map, WipeMode.Vanish);
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
