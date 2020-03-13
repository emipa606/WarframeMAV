﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;
using Warframe;

namespace WarframeMAV.Skills.Mesas
{
    public class Bullet_Mesa : Projectile
    {
        private bool isCR = false;
        // Token: 0x060026E7 RID: 9959 RVA: 0x00127F60 File Offset: 0x00126360
        protected override void Impact(Thing hitThing)
        {
            Map map = base.Map;
            base.Impact(hitThing);
            BattleLogEntry_RangedImpact battleLogEntry_RangedImpact = new BattleLogEntry_RangedImpact(this.launcher, hitThing, this.intendedTarget.Thing, this.equipmentDef, this.def, this.targetCoverDef);
            Find.BattleLog.Add(battleLogEntry_RangedImpact);
            if (hitThing != null)
            {
                DamageDef damageDef = this.def.projectile.damageDef;
                float amount = (float)base.DamageAmount;
                float armorPenetration = base.ArmorPenetration;
                float y = this.ExactRotation.eulerAngles.y;
                Thing launcher = this.launcher;
                ThingDef equipmentDef = this.equipmentDef;
                float finaldmg = changeDamage(amount);
                DamageInfo dinfo = new DamageInfo(damageDef, finaldmg, armorPenetration, y, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, this.intendedTarget.Thing);
                if (isCR) {
                    WarframeStaticMethods.showColorText(hitThing,"X3 -"+finaldmg,Color.magenta,GameFont.Medium);
                }else
                {
                    WarframeStaticMethods.showDamageAmount(hitThing,finaldmg+"");
                }
                hitThing.TakeDamage(dinfo).AssociateWithLog(battleLogEntry_RangedImpact);
                Pawn pawn = hitThing as Pawn;
                if (pawn != null && pawn.stances != null && pawn.BodySize <= this.def.projectile.StoppingPower + 0.001f)
                {
                    pawn.stances.StaggerFor(95);
                }
            }
            else
            {
                SoundDefOf.BulletImpact_Ground.PlayOneShot(new TargetInfo(base.Position, map, false));
                MoteMaker.MakeStaticMote(this.ExactPosition, map, ThingDefOf.Mote_ShotHit_Dirt, 1f);
                if (base.Position.GetTerrain(map).takeSplashes)
                {
                    MoteMaker.MakeWaterSplash(this.ExactPosition, map, Mathf.Sqrt((float)base.DamageAmount) * 1f, 4f);
                }
            }
        }





        private float changeDamage(float damage) {
            float result = damage;
            float levelmul = 1+(1 * (this.launcher as Pawn).getLevel()*1f/60f);

            result = result + result * levelmul;

            System.Random r = new System.Random(Find.TickManager.TicksGame);
            int fins = r.Next(100);
            if (fins <= 25)
            {
                result *= 3f;
                this.isCR = true;
            }
            
            return result;
        }



    }
}
