using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.Sound;
using Warframe;
using Warframe.Skills.WFPublic;

namespace WarframeMAV.Skills.Ashs
{
    public class Ash4Thing : ThingWithComps
    {
        public List<Pawn> affected;
        public float damage;
        public int hitTime;
        public int lastATKTick = -1;
        public IntVec3 opos;
        public Pawn self;


        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref self, "self");
            Scribe_Values.Look(ref damage, "damage");
            Scribe_Values.Look(ref lastATKTick, "lastATKTick");
            Scribe_Values.Look(ref hitTime, "hitTime");
            Scribe_Values.Look(ref opos, "opos");
            Scribe_Collections.Look(ref affected, "affected", LookMode.Reference);
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
                self.SetPositionDirect(opos);
                self.pather.StopDead();
                foreach (var hed in self.health.hediffSet.hediffs)
                {
                    if (hed.def.defName == "WFGod")
                    {
                        //self.health.RemoveHediff(hed);
                        ((Hediff_God) hed).del = true;
                    }
                }

                Destroy();
                return;
            }

            if (self == null || self.Dead || !self.Spawned)
            {
                Destroy();
                return;
            }

            var target = affected[0];


            if (lastATKTick < 1 || Find.TickManager.TicksGame - lastATKTick > 60)
            {
                lastATKTick = Find.TickManager.TicksGame;
                var hediff = (Hediff_God) HediffMaker.MakeHediff(HediffDef.Named("WFGod"), self);
                self.health.AddHediff(hediff);
                self.stances.stunner.StunFor(180, self);
                if (!target.Dead)
                {
                    target.stances.stunner.StunFor(180, self);
                    self.SetPositionDirect(target.Position);
                }
                //play sound to do
            }

            //atk

            if ((Find.TickManager.TicksGame - lastATKTick) % 20 == 0 && hitTime < 3 && !target.Dead && target.Spawned)
            {
                var timeMul = hitTime == 2 ? 1 : 0.5f;
                var damags = (int) ((damage + getHandATK()) * timeMul);
                // Log.Warning("FINAL:"+damags +" "+ getHandATK() );
                var dinfo = new DamageInfo(DamageDefOf.Stab, damags, 1, -1, self);
                WarframeStaticMethods.ShowDamageAmount(target, damags.ToString());
                target.TakeDamage(dinfo);
                hitTime++;
                SoundDefOf.Pawn_Melee_Punch_HitPawn.PlayOneShot(target);
            }

            if (Find.TickManager.TicksGame - lastATKTick != 60)
            {
                return;
            }

            affected.RemoveAt(0);
            hitTime = 0;
        }

        private float getHandATK()
        {
            var melee = self.equipment.Primary;
            if (melee == null || melee.def.tools == null)
            {
                return 0;
            }

            var tool = melee.def.tools.RandomElement();

            return tool.power * (1 + (self.GetLevel() / 60f));
        }
    }
}