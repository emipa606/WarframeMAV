using System;
using System.Collections.Generic;
using System.Text;
using Verse;
using Warframe;

namespace WarframeMAV.Skills.Mesas
{
    public class Hediff_Mesa1SkillB: HediffWithComps
    {

        public int sdamage = 0;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.sdamage, "sdamage", 0, false);
        }


        public override string LabelInBrackets
        {
            get
            {
                return base.LabelInBrackets + "Damage:" + this.sdamage;
            }
        }



    }
}
