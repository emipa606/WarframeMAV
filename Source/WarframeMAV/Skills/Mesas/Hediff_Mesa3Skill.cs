using UnityEngine;
using Verse;
using Warframe;

namespace WarframeMAV.Skills.Mesas
{
    public class Hediff_Mesa3Skill : HediffWithComps
    {
        // Token: 0x040033BA RID: 13242
        public Pawn self;


        private float getMaxTick => 600 * (1 + (WarframeStaticMethods.GetWFLevel(self) * 1.0f / 20f));


        public override void Tick()
        {
            ageTicks++;
            if (ageTicks >= getMaxTick)
            {
                TimeOut();
                return;
            }


            DrawHediffExtras();
        }

        // Token: 0x06004BF8 RID: 19448 RVA: 0x00232324 File Offset: 0x00230724
        private void TimeOut()
        {
            pawn.health.RemoveHediff(this);
        }

        public void DrawHediffExtras()
        {
            var num = 1.8f; //Mathf.Lerp(1.8f, 1.2f, (this.ageTicks)*1.0f/getMaxTick);
            var vector = pawn.Drawer.DrawPos;
            vector.y = AltitudeLayer.MoteOverhead.AltitudeFor();

            float angle = Rand.Range(0, 360);
            var s = new Vector3(num, 1f, num);
            var matrix = default(Matrix4x4);
            matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
            Graphics.DrawMesh(MeshPool.plane10, matrix,
                MaterialPool.MatFrom("Other/ShieldBubble", ShaderDatabase.Transparent, Color.yellow), 0);
        }


        // Token: 0x06004BFA RID: 19450 RVA: 0x002324EB File Offset: 0x002308EB
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref self, "self");
        }
    }
}