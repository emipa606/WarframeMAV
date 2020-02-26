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
    public class Bullet_1Ash:Projectile
    {
        public Pawn target;
        public override void Tick()
        {
            base.Tick();
            /*
            {
                int num = Mathf.RoundToInt((this.ExactPosition - this.destination).magnitude / (this.def.projectile.speed / 100f));
                if (num < 1)
                {
                    num = 1;
                }
                this.ticksToImpact = num;


            }
            */
          //  this.origin = this.ExactPosition;
  
            this.destination = target.Position.ToVector3();
            Draw();
            
        }

        public override void Draw()
        {
            float rotat = (Find.TickManager.TicksGame*25f) % 360;
            Mesh mesh = MeshPool.GridPlane(this.def.graphicData.drawSize);
            Graphics.DrawMesh(mesh, this.DrawPos, rotat.ToQuat(), this.def.DrawMatSingle, 0);
            base.Comps_PostDraw();
        }
        // Token: 0x060026E7 RID: 9959 RVA: 0x00127F60 File Offset: 0x00126360
        protected override void Impact(Thing hitThingf)
        {
            Thing hitThing = this.target;
            Map map = base.Map;
            base.Impact(hitThing);
            BattleLogEntry_RangedImpact battleLogEntry_RangedImpact = new BattleLogEntry_RangedImpact(this.launcher, hitThing, this.intendedTarget.Thing, this.equipmentDef, this.def, this.targetCoverDef);
            Find.BattleLog.Add(battleLogEntry_RangedImpact);
            if (hitThing != null)
            {
                DamageDef damageDef = this.def.projectile.damageDef;
                float Baseamount = (float)base.DamageAmount;
                float armorPenetration = base.ArmorPenetration;
                float y = this.ExactRotation.eulerAngles.y;
                Thing launcher = this.launcher;
                ThingDef equipmentDef = this.equipmentDef;
                float amount = Baseamount * (1+ (launcher as Pawn).getLevel()/6f);
                DamageInfo dinfo = new DamageInfo(damageDef, amount, armorPenetration, y, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, this.intendedTarget.Thing);
                WarframeStaticMethods.showDamageAmount(target, amount.ToString());
                hitThing.TakeDamage(dinfo).AssociateWithLog(battleLogEntry_RangedImpact);
                Pawn pawn = hitThing as Pawn;
                if (pawn != null && pawn.stances != null && pawn.BodySize <= this.def.projectile.StoppingPower + 0.001f)
                {
                    pawn.stances.StaggerFor(95);
                }
            }
            /*
            else
            {
                SoundDefOf.BulletImpact_Ground.PlayOneShot(new TargetInfo(base.Position, map, false));
                MoteMaker.MakeStaticMote(this.ExactPosition, map, ThingDefOf.Mote_ShotHit_Dirt, 1f);
                if (base.Position.GetTerrain(map).takeSplashes)
                {
                    MoteMaker.MakeWaterSplash(this.ExactPosition, map, Mathf.Sqrt((float)base.DamageAmount) * 1f, 4f);
                }
            }
            */
        }
    }
}
