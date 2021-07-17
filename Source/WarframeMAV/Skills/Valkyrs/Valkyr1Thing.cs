using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;
using Warframe;

namespace WarframeMAV.Skills.Valkyrs
{
    public class Valkyr1Thing : ThingWithComps
    {
        public Pawn self;
        public int startTick;
        public Pawn target;

        public override void Tick()
        {
            base.Tick();
            if (Find.TickManager.TicksGame - startTick < 15)
            {
                return;
            }

            GenDraw.DrawLineBetween(self.Position.ToVector3Shifted(), target.DrawPos, SimpleColor.Red);
            if (Find.TickManager.TicksGame - startTick == 25)
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
                }
                else if (target.AnimalOrWildMan() || target.HostileTo(self))
                {
                    var pos = getPos(target);
                    target.pather.StartPath(pos, PathEndMode.Touch);
                    target.Position = pos;
                    target.pather.StopDead();
                    if (target.jobs.curJob != null)
                    {
                        target.jobs.curDriver.Notify_PatherArrived();
                    }

                    target.stances.stunner.StunFor(120, self);
                    var amount = getDMG();
                    WarframeStaticMethods.ShowDamageAmount(target, amount + "");
                    var dinfo = new DamageInfo(DamageDefOf.Cut, amount, 0, -1, self, null, null,
                        DamageInfo.SourceCategory.ThingOrUnknown, target);

                    target.TakeDamage(dinfo);
                }
            }

            if (Find.TickManager.TicksGame - startTick >= 40)
            {
                Destroy();
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref startTick, "startTick");
            Scribe_References.Look(ref self, "self");
            Scribe_References.Look(ref target, "target");
        }

        private float getDMG()
        {
            var result = 30f;
            var levelmul = 1 + (self.GetLevel() / 30f);
            result *= levelmul;
            foreach (var hef in self.health.hediffSet.hediffs)
            {
                if (hef.def.defName != "WFMesa1Skill_Mul")
                {
                    continue;
                }

                if (hef is Hediff_Valkyr1Skill hev)
                {
                    result *= hev.mul * 1f;
                }
            }


            return result;
        }

        private IntVec3 getPos(Pawn tg)
        {
            var eway = self.CellsAdjacent8WayAndInside().ToList();
            var finalpoc = self.Position;
            if (tg.Position.x == self.Position.x)
            {
                finalpoc = tg.Position.z > self.Position.z ? eway[5] : eway[3];
            }
            else if (tg.Position.z == self.Position.z)
            {
                finalpoc = tg.Position.x > self.Position.x ? eway[7] : eway[1];
            }
            else if (tg.Position.x > self.Position.x)
            {
                finalpoc = tg.Position.z > self.Position.z ? eway[8] : eway[6];
            }
            else if (tg.Position.x < self.Position.x)
            {
                finalpoc = tg.Position.z > self.Position.z ? eway[2] : eway[0];
            }

            return finalpoc;
        }
    }
}