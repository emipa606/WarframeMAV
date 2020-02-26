using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
using Warframe;

namespace WarframeMAV.Skills.Mesas
{
    public class Hediff_Mesa3Skill : HediffWithComps
    {


        public override void Tick()
        {
            this.ageTicks++;
            if (this.ageTicks >= getMaxTick)
            {
                TimeOut();
                return;
            }


            DrawHediffExtras();
        }

        // Token: 0x06004BF8 RID: 19448 RVA: 0x00232324 File Offset: 0x00230724
        private void TimeOut()
        {
            this.pawn.health.RemoveHediff(this);
        }

       



        private float getMaxTick
        {
            get
            {
                return (600 * (1 + ((WarframeStaticMethods.getWFLevel(self) * 1.0f) / 20f)));
            }
        }
        public void DrawHediffExtras()
        {

            float num = 1.8f;//Mathf.Lerp(1.8f, 1.2f, (this.ageTicks)*1.0f/getMaxTick);
            Vector3 vector = this.pawn.Drawer.DrawPos;
            vector.y = AltitudeLayer.MoteOverhead.AltitudeFor();

            float angle = (float)Rand.Range(0, 360);
            Vector3 s = new Vector3(num, 1f, num);
            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
            Graphics.DrawMesh(MeshPool.plane10, matrix, MaterialPool.MatFrom("Other/ShieldBubble", ShaderDatabase.Transparent, Color.yellow), 0);

        }


        // Token: 0x06004BFA RID: 19450 RVA: 0x002324EB File Offset: 0x002308EB
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look<Pawn>(ref this.self, "self", false);
        }



        // Token: 0x040033BA RID: 13242
        public Pawn self;



    }
}
