using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;
using Warframe;

namespace WarframeMAV.Skills.Mesas
{
    public class Hediff_Mesa2Skill : HediffWithComps
    {
        public int level = 1;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.level, "level",0, false);
        }
        public override void Tick()
        {
            base.Tick();

            if (this.ageTicks % 30 == 0)
                DrawHediffExtras();

            if (ShouldBeRemove)
            {
                SoundDef.Named("Mesa_2SkillEnd").PlayOneShot(this.pawn);
                this.pawn.health.RemoveHediff(this);
                return;
            }

            if (!ShouldAffectEnemy) return;


            List<Pawn> plist = getTargets(10f * (1+(level*1f/60f)));
            if (plist.Count < 1) return;

            foreach(Pawn p in plist)
            {
                p.stances.stunner.StunFor(180,p);
                {
                    Mote mote = (Mote)ThingMaker.MakeThing(ThingDef.Named("Mote_2ExFlash"), null);
                    mote.exactPosition = p.Position.ToVector3Shifted();
                    mote.Scale = (float)Mathf.Max(3f, 5f);
                    mote.rotationRate = 1.2f;
                    // mote.Scale = 0.2f;
                    GenSpawn.Spawn(mote, p.Position + new IntVec3(0, 1, 0), p.Map, WipeMode.Vanish);
                }
            }


            
        }


        public bool ShouldBeRemove
        {
            get
            {
                return this.ageTicks / 60 >= (15 * (1+this.pawn.getLevel()*1f/30f));
            }
        }
        public bool ShouldAffectEnemy
        {
            get
            {
                return this.ageTicks % 90 == 0;
            }
        }
        public void DrawHediffExtras()
        {

            Vector3 loc = this.pawn.DrawPos;
            Map map = this.pawn.Map;
            float size = 1f;
            if (!loc.ShouldSpawnMotesAt(map) || map.moteCounter.SaturatedLowPriority)
            {
                return;
            }
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(ThingDefOf.Mote_Smoke, null);
            moteThrown.Scale = Rand.Range(1.5f, 2.5f) * size;
            moteThrown.rotationRate = Rand.Range(-30f, 30f);
            moteThrown.exactPosition = loc;
            moteThrown.SetVelocity((float)Rand.Range(30, 40), Rand.Range(0.5f, 0.7f));
            moteThrown.instanceColor = Color.yellow;
            GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map, WipeMode.Vanish);
        }

        public List<Pawn> getTargets(float range)
        {
            List<Pawn> result = new List<Pawn>();
            IntVec3 pos = this.pawn.Position;
            Map map = this.pawn.Map;
            if (!GenGrid.InBounds(pos, map))
            {
                return result;
            }
            Region region = GridsUtility.GetRegion(pos, map, RegionType.Set_Passable);
            if (region == null)
            {
                return result;
            }
            RegionTraverser.BreadthFirstTraverse(region, (Region from, Region r) => r.door == null, delegate (Region r)
            {
                foreach (IntVec3 item in r.Cells)
                {
                    if (result.Count >= 3) break;
                    if (item.InHorDistOf(pos, range))
                    {
                        foreach(Thing th in map.thingGrid.ThingsAt(item))
                        {
                            if(th is Pawn)
                            {
                                if((th as Pawn)== this.pawn)continue;
                                if (result.Count >= 3) break;
                                result.Add(th as Pawn);
                            }
                        }
                    }
                }
                return false;
            }, 50, RegionType.Set_Passable);

            return result;
        }


    }
}
