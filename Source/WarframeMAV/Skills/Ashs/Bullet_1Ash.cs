using UnityEngine;
using Verse;
using Warframe;

namespace WarframeMAV.Skills.Ashs
{
    public class Bullet_1Ash : Projectile
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

            destination = target.Position.ToVector3();
            Draw();
        }

        public override void Draw()
        {
            var rotat = Find.TickManager.TicksGame * 25f % 360;
            var mesh = MeshPool.GridPlane(def.graphicData.drawSize);
            Graphics.DrawMesh(mesh, DrawPos, rotat.ToQuat(), def.DrawMatSingle, 0);
            Comps_PostDraw();
        }

        // Token: 0x060026E7 RID: 9959 RVA: 0x00127F60 File Offset: 0x00126360
        protected override void Impact(Thing hitThingf)
        {
            Thing hitThing = target;
            var unused = Map;
            base.Impact(hitThing);
            var battleLogEntry_RangedImpact = new BattleLogEntry_RangedImpact(launcher, hitThing,
                intendedTarget.Thing, equipmentDef, def, targetCoverDef);
            Find.BattleLog.Add(battleLogEntry_RangedImpact);
            if (hitThing == null)
            {
                return;
            }

            var damageDef = def.projectile.damageDef;
            float Baseamount = DamageAmount;
            var armorPenetration = ArmorPenetration;
            var y = ExactRotation.eulerAngles.y;
            var instigator = launcher;
            var thingDef = equipmentDef;
            var amount = Baseamount * (1 + ((instigator as Pawn).GetLevel() / 6f));
            var dinfo = new DamageInfo(damageDef, amount, armorPenetration, y, instigator, null, thingDef,
                DamageInfo.SourceCategory.ThingOrUnknown, intendedTarget.Thing);
            WarframeStaticMethods.ShowDamageAmount(target, amount.ToString());
            hitThing.TakeDamage(dinfo).AssociateWithLog(battleLogEntry_RangedImpact);
            if (hitThing is Pawn {stances: { }} pawn && pawn.BodySize <= def.projectile.StoppingPower + 0.001f)
            {
                pawn.stances.StaggerFor(95);
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