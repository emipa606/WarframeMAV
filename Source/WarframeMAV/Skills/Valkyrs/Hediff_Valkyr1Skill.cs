using Verse;

namespace WarframeMAV.Skills.Valkyrs
{
    public class Hediff_Valkyr1Skill : HediffWithComps
    {
        public int mul;
        public int tick;

        public int maxdamage => 4;

        public bool isMax => mul >= maxdamage;

        public override string LabelInBrackets =>
            base.LabelInBrackets + "Mul:" + (mul * 100) + (mul >= maxdamage ? "(Max)" : "" + "%");

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref mul, "mul");
            Scribe_Values.Look(ref tick, "tick");
        }

        public override void Tick()
        {
            base.Tick();
            if (Find.TickManager.TicksGame - tick >= 30 * mul)
            {
                pawn.health.RemoveHediff(this);
            }
        }

        public void add(int damage)
        {
            mul += damage;
            if (isMax)
            {
                mul = maxdamage;
            }
        }
    }
}