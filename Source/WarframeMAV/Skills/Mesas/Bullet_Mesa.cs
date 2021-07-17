using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;
using Warframe;
using Random = System.Random;

namespace WarframeMAV.Skills.Mesas
{
    public class Bullet_Mesa : Projectile
    {
        private bool isCR;

        // Token: 0x060026E7 RID: 9959 RVA: 0x00127F60 File Offset: 0x00126360
        protected override void Impact(Thing hitThing)
        {
            var map = Map;
            base.Impact(hitThing);
            var battleLogEntry_RangedImpact = new BattleLogEntry_RangedImpact(launcher, hitThing,
                intendedTarget.Thing, equipmentDef, def, targetCoverDef);
            Find.BattleLog.Add(battleLogEntry_RangedImpact);
            if (hitThing != null)
            {
                var damageDef = def.projectile.damageDef;
                float amount = DamageAmount;
                var armorPenetration = ArmorPenetration;
                var y = ExactRotation.eulerAngles.y;
                var instigator = launcher;
                var thingDef = equipmentDef;
                var finaldmg = changeDamage(amount);
                var dinfo = new DamageInfo(damageDef, finaldmg, armorPenetration, y, instigator, null, thingDef,
                    DamageInfo.SourceCategory.ThingOrUnknown, intendedTarget.Thing);
                if (isCR)
                {
                    WarframeStaticMethods.ShowColorText(hitThing, "X3 -" + finaldmg, Color.magenta, GameFont.Medium);
                }
                else
                {
                    WarframeStaticMethods.ShowDamageAmount(hitThing, finaldmg + "");
                }

                hitThing.TakeDamage(dinfo).AssociateWithLog(battleLogEntry_RangedImpact);
                if (hitThing is Pawn {stances: { }} pawn && pawn.BodySize <= def.projectile.StoppingPower + 0.001f)
                {
                    pawn.stances.StaggerFor(95);
                }
            }
            else
            {
                SoundDefOf.BulletImpact_Ground.PlayOneShot(new TargetInfo(Position, map));
                FleckMaker.Static(ExactPosition, map, FleckDefOf.ShotHit_Dirt);
                if (Position.GetTerrain(map).takeSplashes)
                {
                    FleckMaker.WaterSplash(ExactPosition, map, Mathf.Sqrt(DamageAmount) * 1f, 4f);
                }
            }
        }


        private float changeDamage(float damage)
        {
            var result = damage;
            var levelmul = 1 + (1 * (launcher as Pawn).GetLevel() * 1f / 60f);

            result = result + (result * levelmul);

            var r = new Random(Find.TickManager.TicksGame);
            var fins = r.Next(100);
            if (fins > 25)
            {
                return result;
            }

            result *= 3f;
            isCR = true;

            return result;
        }
    }
}