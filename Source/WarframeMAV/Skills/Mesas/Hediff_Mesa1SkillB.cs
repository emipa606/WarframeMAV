using Verse;

namespace WarframeMAV.Skills.Mesas
{
    public class Hediff_Mesa1SkillB : HediffWithComps
    {
        public int sdamage;


        public override string LabelInBrackets => base.LabelInBrackets + "Damage:" + sdamage;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref sdamage, "sdamage");
        }
    }
}