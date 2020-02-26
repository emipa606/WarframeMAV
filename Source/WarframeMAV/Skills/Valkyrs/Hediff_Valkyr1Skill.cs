using System;
using System.Collections.Generic;
using System.Text;
using Verse;
using Warframe;

namespace WarframeMAV.Skills.Valkyrs
{
    public class Hediff_Valkyr1Skill: HediffWithComps
    {

        public int mul = 0;
        public int tick;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.mul, "mul", 0, false);
            Scribe_Values.Look<int>(ref this.tick, "tick", 0, false);
        }
        public override void Tick()
        {
            base.Tick();
            if (Find.TickManager.TicksGame - tick >= 30*mul)
            {
               
                    this.pawn.health.RemoveHediff(this);
                    return;
                
            }

        }
        public void add(int damage)
        {
            this.mul += damage;
            if (isMax)
            {
                this.mul = this.maxdamage;
            }
        }

        public int maxdamage
        {
            get
            {
                return 4;
            }
        }
        public bool isMax
        {
            get
            {
                return this.mul >= this.maxdamage;
            }
        }

        public override string LabelInBrackets
        {
            get
            {

                return base.LabelInBrackets + "Mul:" + this.mul*100+(mul>=maxdamage?"(Max)":""+"%");
            }
        }



    }
}
