using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
using Warframe;

namespace WarframeMAV.Skills.Valkyrs
{
    public class Hediff_Valkyr2Skill: HediffWithComps
    {

        public Pawn self;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look<Pawn>(ref this.self, "self", false);
        }
        private bool shouldHRemove
        {
            get
            {
                return (this.ageTicks ) >= 420 * (1 + self.GetLevel() / 30f);
            }
        }



        public override void Tick()
        {
            base.Tick();
            if (this.ageTicks % 120 == 0)
                DrawHediffExtras();

            if (shouldHRemove)
            {
                this.pawn.health.RemoveHediff(this);
                return;
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
            moteThrown.instanceColor = Color.red;
            GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map, WipeMode.Vanish);
        }





    }
}
