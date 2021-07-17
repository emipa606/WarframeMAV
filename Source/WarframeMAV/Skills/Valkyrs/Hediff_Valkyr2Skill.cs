using RimWorld;
using UnityEngine;
using Verse;
using Warframe;

namespace WarframeMAV.Skills.Valkyrs
{
    public class Hediff_Valkyr2Skill : HediffWithComps
    {
        public Pawn self;

        private bool shouldHRemove => ageTicks >= 420 * (1 + (self.GetLevel() / 30f));

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref self, "self");
        }


        public override void Tick()
        {
            base.Tick();
            if (ageTicks % 120 == 0)
            {
                DrawHediffExtras();
            }

            if (shouldHRemove)
            {
                pawn.health.RemoveHediff(this);
            }
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
            dataStatic.instanceColor = Color.red;
            map.flecks.CreateFleck(dataStatic);
        }
    }
}