using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;
using Warframe;

namespace WarframeMAV.Skills.Mesas
{
    public class Hediff_Mesa2Skill : HediffWithComps
    {
        public int level = 1;


        public bool ShouldBeRemove => ageTicks / (float) 60 >= 15 * (1 + (pawn.GetLevel() * 1f / 30f));

        public bool ShouldAffectEnemy => ageTicks % 90 == 0;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref level, "level");
        }

        public override void Tick()
        {
            base.Tick();

            if (ageTicks % 30 == 0)
            {
                DrawHediffExtras();
            }

            if (ShouldBeRemove)
            {
                SoundDef.Named("Mesa_2SkillEnd").PlayOneShot(pawn);
                pawn.health.RemoveHediff(this);
                return;
            }

            if (!ShouldAffectEnemy)
            {
                return;
            }


            var plist = getTargets(10f * (1 + (level * 1f / 60f)));
            if (plist.Count < 1)
            {
                return;
            }

            foreach (var p in plist)
            {
                p.stances.stunner.StunFor(180, p);
                {
                    var mote = (Mote) ThingMaker.MakeThing(ThingDef.Named("Mote_2ExFlash"));
                    mote.exactPosition = p.Position.ToVector3Shifted();
                    mote.Scale = Mathf.Max(3f, 5f);
                    mote.rotationRate = 1.2f;
                    // mote.Scale = 0.2f;
                    GenSpawn.Spawn(mote, p.Position + new IntVec3(0, 1, 0), p.Map);
                }
            }
        }

        public void DrawHediffExtras()
        {
            var loc = pawn.DrawPos;
            var map = pawn.Map;
            var size = 1f;
            if (!loc.ShouldSpawnMotesAt(map) || map.moteCounter.SaturatedLowPriority)
            {
                return;
            }

            var dataStatic = FleckMaker.GetDataStatic(loc, map, FleckDefOf.Smoke, Rand.Range(1.5f, 2.5f) * size);
            dataStatic.rotationRate = Rand.Range(-30f, 30f);
            dataStatic.velocityAngle = Rand.Range(30, 40);
            dataStatic.velocitySpeed = Rand.Range(0.5f, 0.7f);
            dataStatic.instanceColor = Color.yellow;
            map.flecks.CreateFleck(dataStatic);
        }

        public List<Pawn> getTargets(float range)
        {
            var result = new List<Pawn>();
            var pos = pawn.Position;
            var map = pawn.Map;
            if (!pos.InBounds(map))
            {
                return result;
            }

            var region = pos.GetRegion(map);
            if (region == null)
            {
                return result;
            }

            RegionTraverser.BreadthFirstTraverse(region, (_, r) => r.door == null, delegate(Region r)
            {
                foreach (var item in r.Cells)
                {
                    if (result.Count >= 3)
                    {
                        break;
                    }

                    if (!item.InHorDistOf(pos, range))
                    {
                        continue;
                    }

                    foreach (var th in map.thingGrid.ThingsAt(item))
                    {
                        if (th is not Pawn pawn1)
                        {
                            continue;
                        }

                        if (pawn1 == pawn)
                        {
                            continue;
                        }

                        if (result.Count >= 3)
                        {
                            break;
                        }

                        result.Add(pawn1);
                    }
                }

                return false;
            }, 50);

            return result;
        }
    }
}