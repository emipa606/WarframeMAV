using Verse;
using Warframe;

namespace WarframeMAV.Skills.Mesas
{
    public class Hediff_Mesa1SkillA : HediffWithComps
    {
        public int sdamage;

        public int maxdamage => 40 * (int) (1 + (pawn.GetLevel() * 1f / 7.5f));

        public bool isMax => sdamage >= maxdamage;

        public override string LabelInBrackets =>
            base.LabelInBrackets + "NowDamage:" + sdamage + (sdamage >= maxdamage ? "(Max)" : "");

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref sdamage, "sdamage");
        }

        public void add(int damage)
        {
            sdamage += damage;
            if (isMax)
            {
                sdamage = maxdamage;
            }
        }
    }
}