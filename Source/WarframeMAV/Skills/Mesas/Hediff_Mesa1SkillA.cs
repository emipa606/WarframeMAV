using System;
using System.Collections.Generic;
using System.Text;
using Verse;
using Warframe;

namespace WarframeMAV.Skills.Mesas
{
    public class Hediff_Mesa1SkillA: HediffWithComps
    {

        public int sdamage = 0;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.sdamage, "sdamage", 0, false);
        }
        public void add(int damage)
        {
            this.sdamage += damage;
            if (isMax)
            {
                this.sdamage = this.maxdamage;
            }
        }

        public int maxdamage
        {
            get
            {
                return 40 * (int)(1 + (this.pawn.getLevel() * 1f / 7.5f));
            }
        }
        public bool isMax
        {
            get
            {
                return this.sdamage >= this.maxdamage;
            }
        }

        public override string LabelInBrackets
        {
            get
            {

                return base.LabelInBrackets + "NowDamage:" + this.sdamage+(sdamage>=maxdamage?"(Max)":"");
            }
        }



    }
}
