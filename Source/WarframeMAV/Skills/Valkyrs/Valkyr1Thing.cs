using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Warframe;

namespace WarframeMAV.Skills.Valkyrs
{
    public class Valkyr1Thing : ThingWithComps
    {
        public int startTick;
        public Pawn self;
        public Pawn target;
        public override void Tick()
        {
            base.Tick();
            if (Find.TickManager.TicksGame - startTick >= 15)
            {
                GenDraw.DrawLineBetween(self.Position.ToVector3Shifted(),target.DrawPos,SimpleColor.Red);
                if(Find.TickManager.TicksGame - startTick == 25)
                {
                    if (!target.AnimalOrWildMan() && (target.Faction == self.Faction || !target.HostileTo(self)))
                    {
                        self.pather.StartPath(target.Position, PathEndMode.Touch);
                        self.Position = target.Position;
                        self.pather.StopDead();
                        if (self.jobs.curJob != null)
                        {
                            self.jobs.curDriver.Notify_PatherArrived();
                        }
                        
                    }else if (target.AnimalOrWildMan() ||target.HostileTo(self))
                    {
                        IntVec3 pos = getPos(target);
                        target.pather.StartPath(pos, PathEndMode.Touch);
                        target.Position = pos;
                        target.pather.StopDead();
                        if (target.jobs.curJob != null)
                        {
                            target.jobs.curDriver.Notify_PatherArrived();
                        }
                        target.stances.stunner.StunFor(120, self);
                        float amount = getDMG();
                        WarframeStaticMethods.showDamageAmount(target, amount + "");
                        DamageInfo dinfo = new DamageInfo(DamageDefOf.Cut, amount,0, -1, self, null, null, DamageInfo.SourceCategory.ThingOrUnknown, target);

                        target.TakeDamage(dinfo);
                        


                    }
                }

                if(Find.TickManager.TicksGame - startTick >= 40)
                {
                    this.Destroy(DestroyMode.Vanish);
                    return;
                }
                
            }
             
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.startTick, "startTick", 0, false);
            Scribe_References.Look<Pawn>(ref this.self, "self", false);
            Scribe_References.Look<Pawn>(ref this.target, "target", false);
        }

        private float getDMG() {
            float result = 30f;
            float levelmul = 1 + self.getLevel() / 30f;
            result *= levelmul;
            foreach (Hediff hef in self.health.hediffSet.hediffs)
            {
                if(hef.def.defName== "WFMesa1Skill_Mul")
                {
                    Hediff_Valkyr1Skill hev = hef as Hediff_Valkyr1Skill;
                    result *= (hev.mul * 1f);
                }
            }


            return result;
        }

        private IntVec3 getPos(Pawn tg) {
            List<IntVec3> eway = self.CellsAdjacent8WayAndInside().ToList();
            IntVec3 finalpoc = self.Position;
            if (tg.Position.x == self.Position.x)
            {
                if (tg.Position.z > self.Position.z)
                    finalpoc = eway[5];
                else
                    finalpoc = eway[3];
            }
            else
            if (tg.Position.z == self.Position.z)
            {
                if (tg.Position.x > self.Position.x)
                    finalpoc = eway[7];
                else
                    finalpoc = eway[1];
            }
            else
            if (tg.Position.x > self.Position.x)
            {
                if (tg.Position.z > self.Position.z)
                    finalpoc = eway[8];
                else
                    finalpoc = eway[6];
            }
            else
            if (tg.Position.x < self.Position.x)
            {
                if (tg.Position.z > self.Position.z)
                    finalpoc = eway[2];
                else
                    finalpoc = eway[0];
            }

            return finalpoc;
        }












    }
}
