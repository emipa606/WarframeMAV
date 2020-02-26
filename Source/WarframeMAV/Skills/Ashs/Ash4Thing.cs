using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using Verse;
using Verse.Sound;
using Warframe;
using Warframe.Skills.WFPublic;

namespace WarframeMAV.Skills.Ashs
{
    public class Ash4Thing : ThingWithComps
    {
        public Pawn self;
        public List<Pawn> affected;
        public float damage;
        public int lastATKTick=-1;
        public IntVec3 opos;
        public int hitTime = 0;
        



        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look<Pawn>(ref this.self, "self", false);
            Scribe_Values.Look<float>(ref this.damage, "damage", 0, false);
            Scribe_Values.Look<int>(ref this.lastATKTick, "lastATKTick", 0, false);
            Scribe_Values.Look<int>(ref this.hitTime, "hitTime", 0, false);
            Scribe_Values.Look<IntVec3>(ref this.opos, "opos", default(IntVec3), false);
            Scribe_Collections.Look<Pawn>(ref this.affected, "affected", LookMode.Reference, new object[0]);


        }
        public override void Draw()
        {
            //base.Draw();
        }

        public override void Tick()
        {
            base.Tick();
           
            if (affected.Count < 1)
            {
                self.SetPositionDirect(this.opos);
                self.pather.StopDead();
                foreach (Hediff hed in self.health.hediffSet.hediffs)
                {
                    if (hed.def.defName == "WFGod")
                    {
                        //self.health.RemoveHediff(hed);
                        ((Hediff_God)hed).del = true;
                    }
                }
                
                this.Destroy(DestroyMode.Vanish);
                return;
            }
            if(self==null|| self.Dead || !self.Spawned)
            {
                this.Destroy(DestroyMode.Vanish);
                return;
            }

            Pawn target = affected[0];




            if(this.lastATKTick<1 || Find.TickManager.TicksGame - this.lastATKTick > 60)
            {
                this.lastATKTick = Find.TickManager.TicksGame;
                Hediff_God hediff = (Hediff_God)HediffMaker.MakeHediff(HediffDef.Named("WFGod"), self, null);
                self.health.AddHediff(hediff, null, null, null);
                self.stances.stunner.StunFor(180, self);
                if (!target.Dead)
                {
                    target.stances.stunner.StunFor(180, self);
                    self.SetPositionDirect(target.Position);
                }
                //play sound to do
            }
      
            //atk
             
            if ((Find.TickManager.TicksGame - this.lastATKTick)%20==0&&hitTime<3 && !target.Dead &&target.Spawned)
            {
                float timeMul = (hitTime==2)?1:0.5f;
                int damags = (int)((damage+getHandATK()) * timeMul);
               // Log.Warning("FINAL:"+damags +" "+ getHandATK() );
                DamageInfo dinfo = new DamageInfo(DamageDefOf.Stab, damags, 1, -1, self, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null);
                WarframeStaticMethods.showDamageAmount(target, damags.ToString());
                target.TakeDamage(dinfo);
                hitTime++;
                SoundDefOf.Pawn_Melee_Punch_HitPawn.PlayOneShot(target);
            }
            if (Find.TickManager.TicksGame - this.lastATKTick == 60)
            {

                    affected.RemoveAt(0);
                hitTime = 0;
  
            }


            
            
        }

        private float getHandATK() {
            ThingWithComps melee  = self.equipment.Primary;
            if(melee==null || melee.def.tools == null)
            {
                return 0;
            }

            Tool tool =  melee.def.tools.RandomElement();
         
            return tool.power*(1 + self.getLevel() / 60f); 
        }
    }
}
