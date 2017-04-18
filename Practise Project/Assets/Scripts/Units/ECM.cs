using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PracticeProject
{
    public class ECM : SpaceShip
    {
        public bool jamming;//Make private after debug;
        public new float Stealthness { get { if (jamming) return stealthness * 0.4f; else return stealthness; } }
        public float cooldownJammer; //Make private after debug;
        public float cooldownWeaponInhibitor;//Make private after debug;
        public float cooldownMissileInhibitor;//Make private after debug;
        public float cooldownRadarInhibitor;//Make private after debug;
        protected override void StatsUp()
        {
            type = UnitClass.ECM;
            radarRange = 150; //set in child
            radarPover = 2f;
            speed = 9f; //set in child
            jamming = false;
            stealthness = 0.4f; //set in child
            radiolink = 1.1f;
            EnemySortDelegate = EMCSortEnemys;
            AlliesSortDelegate = ScoutSortEnemys;
        }
        protected override void Explosion()
        {
            GameObject blast = Instantiate(Global.ShipDieBlast, gameObject.transform.position, gameObject.transform.rotation);
            blast.GetComponent<Explosion>().StatUp(BlastType.SmallShip);
        }
        protected override void DecrementCounters()
        {
            if (cooldownJammer > 0)
                cooldownJammer -= Time.deltaTime;
            if (jamming && cooldownJammer <= 0)
            {
                jamming = false;
                cooldownJammer = 5;
            }
            if (cooldownWeaponInhibitor > 0)
                cooldownWeaponInhibitor -= Time.deltaTime;
            if (cooldownRadarInhibitor > 0)
                cooldownRadarInhibitor -= Time.deltaTime;
            if (cooldownMissileInhibitor > 0)
                cooldownMissileInhibitor -= Time.deltaTime;
        }
        //AI logick
        protected override bool AttackManeuver()
        {
            switch (targetStatus)
            {
                case TargetStateType.Captured:
                    {
                        if (RadarInhibitor(CurrentTarget))
                            return Rush();
                        else
                            return ShortenDistance();
                    }
                case TargetStateType.InPrimaryRange:
                    {
                        return IncreaseDistance();
                    }
                case TargetStateType.InSecondaryRange:
                    {

                        if (WeaponInhibitor(CurrentTarget))
                            return Rush();
                        else
                            return ShortenDistance();
                    }
                case TargetStateType.BehindABarrier:
                    {
                        return Rush();
                    }
                default:
                    return false;
            }
        }
        protected override bool RoleFunction()
        {
            if (CurrentTarget != null)
            {
                RadarInhibitor(CurrentTarget);
                WeaponInhibitor(CurrentTarget);
                return true;
            }
            else return false;
        }
        protected override bool SelfDefenceFunction()
        {
            SelfDefenceFunctionBase();
            if (RadarWarningResiever() > 5)
                Jammer();
            MissileGuidanceInhibitor();
            return true;
        }
        protected override int RadarWarningResiever()
        {
            capByTarget.Clear();
            foreach (SpaceShip x in enemys)
            {
                SpaceShip enemy = x.GetComponent<SpaceShip>();
                if (enemy.CurrentTarget == this)
                    capByTarget.Add(x);
                if (jamming)
                {
                    enemy.CurrentTarget = null;
                    if (cooldownWeaponInhibitor < 0)
                        enemy.Impacts.Add(new TrusterInhibitorImpact(enemy, 0.2f));
                    if (cooldownRadarInhibitor < 0)
                        enemy.Impacts.Add(new TrusterInhibitorImpact(enemy, 2f));
                }
            }
            capByTarget.Sort(delegate (SpaceShip x, SpaceShip y) { return EnemySortDelegate(x.GetComponent<IUnit>(), y.GetComponent<IUnit>()); });
            return capByTarget.Count;
        }
        private void Jammer()
        {
            if (!jamming && cooldownJammer <= 0)
            {
                jamming = true;
                cooldownJammer = 3f;
            }
        }
        private bool MissileGuidanceInhibitor()
        {
            if (cooldownMissileInhibitor <= 0)
            {
                GameObject[] missiles = GameObject.FindGameObjectsWithTag("Missile");
                if (missiles.Length > 0)
                    foreach (GameObject x in missiles)
                    {
                        if (x.GetComponent<SelfguidedMissile>().target == gameObject.transform)
                        {
                            float distance = Vector3.Distance(x.transform.position, this.transform.position);
                            float multiplicator = Mathf.Pow(((-distance + (RadarRange * 0.5f)) * 0.02f), (1 / 3));
                            if (Randomizer.Uniform(0, 100, 1)[0] < 70 * multiplicator)
                            {
                                x.GetComponent<SelfguidedMissile>().target = null;
                                cooldownMissileInhibitor = 3.5f;
                                return true;
                            }
                        }
                    }
            }
            return false;
        }
        private bool WeaponInhibitor(SpaceShip target)
        {
            if (cooldownWeaponInhibitor <= 0)
            {
                target.Impacts.Add(new TrusterInhibitorImpact(target, 4f));
                cooldownWeaponInhibitor = 10f;
                return true;
            }
            return false;
        }
        private bool RadarInhibitor(SpaceShip target)
        {
            if (cooldownRadarInhibitor <= 0)
            {
                target.Impacts.Add(new RadarInhibitorImpact(target, 8f));
                cooldownRadarInhibitor = 8f;
                return true;
            }
            return false;
        }
    }
    public class TrusterInhibitorImpact : IImpact
    {
        public string ImpactName { get { return "TrusterInhibitorImpact"; } }
        private float ttl;
        private SpaceShip owner;
        private float ownerSpeedPrev;
        public TrusterInhibitorImpact(SpaceShip owner, float time)
        {

            this.owner = owner;
            ownerSpeedPrev = owner.Speed;
            if (owner.Impacts.Exists(x => x.ImpactName == this.ImpactName))
                ttl = 0;
            else
            {
                if (owner.Impacts.Exists(x => x.ImpactName == "WarpImpact"))
                    ttl = 0;
                else
                {
                    ttl = time;
                    owner.Speed = 0;
                }
            }
        }
        public void ActImpact()
        {
            if (ttl > 0)
                ttl -= Time.deltaTime;
            else CompleteImpact();
        }
        public void CompleteImpact()
        {
            owner.Speed = ownerSpeedPrev;
            owner.Impacts.Remove(this);
        }
        public override string ToString()
        {
            return ImpactName;
        }
    }
    public class RadarInhibitorImpact : IImpact
    {
        public string ImpactName { get { return "RadarInhibitorImpact"; } }
        float ttl;
        SpaceShip owner;
        private float ownerRadarRangePrev;
        public RadarInhibitorImpact(SpaceShip owner, float time)
        {
            this.owner = owner;
            ownerRadarRangePrev = owner.RadarRange;
            if (owner.Impacts.Exists(x => x.ImpactName == this.ImpactName))
                ttl = 0;
            else
            {
                if (owner.Impacts.Exists(x => x.ImpactName == "RadarBoosterImpact"))
                    ttl = 0;
                else
                {
                    ttl = time;
                    owner.RadarRange = owner.RadarRange / 2;
                }
            }
        }
        public void ActImpact()
        {
            if (ttl > 0)
                ttl -= Time.deltaTime;
            else CompleteImpact();
        }

        public void CompleteImpact()
        {
            owner.RadarRange = ownerRadarRangePrev;
            owner.Impacts.Remove(this);
        }
    }
}
