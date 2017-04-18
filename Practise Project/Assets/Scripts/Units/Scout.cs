using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace PracticeProject
{
    public class Scout : SpaceShip
    {
        public bool jamming;//Make private after debug;
        public new float Stealthness { get { if (jamming) return stealthness * 0.6f; else return stealthness; } }
        //private float cooldownInhibitor;
        private float cooldownJammer; //Make private after debug;
        private float cooldownWarp;//Make private after debug;
        private float cooldownMissileInhibitor;//Make private after debug;
        private float cooldownRadarBooster;
        private bool idleFulag;
        protected override void StatsUp()
        {
            type = UnitClass.Scout;
            radarRange = 300; //set in child
            radarPover = 5;
            speed = 10; //set in child
            jamming = false;
            stealthness = 0.3f; //set in child
            radiolink = 2.5f;
            EnemySortDelegate = ScoutSortEnemys;
            AlliesSortDelegate = ReconSortEnemys;
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
            if (cooldownWarp > 0)
                cooldownWarp -= Time.deltaTime;
            if (cooldownMissileInhibitor > 0)
                cooldownMissileInhibitor -= Time.deltaTime;
        }
        protected override bool RoleFunction()
        {

            return Warp();
        }
        protected override bool SelfDefenceFunction()
        {
            SelfDefenceFunctionBase();
            if (RadarWarningResiever() > 5)
                Jammer();
            MissileGuidanceInhibitor();
            return true;
        }
        //AI logick
        protected override bool AttackManeuver()
        {
            switch (targetStatus)
            {
                case TargetStateType.Captured:
                    {
                        if (Warp())
                            return Rush();
                        else return ToSecondaryDistance();
                    }
                case TargetStateType.InPrimaryRange:
                    {
                        return Evasion(CurrentTarget.transform.right);
                    }
                case TargetStateType.InSecondaryRange:
                    {
                        if (Warp())
                            return Rush();
                        else return ToPrimaryDistance();
                    }
                case TargetStateType.BehindABarrier:
                    {
                        return ToSecondaryDistance();
                    }
                default:
                    return false;
            }
        }
        protected override bool IdleManeuverFunction()
        {
            //Debug.Log("new loop");
            idleFulag = !idleFulag;
            if (idleFulag)
                return PatroolTriangle();
            else return PatroolPoint();
        }
        private bool PatroolTriangle()
        {
            //waitingBackCount = 40f;
            float dirflag;
            if (Randomizer.Uniform(-10, 10, 1)[0] > 0)
                dirflag = -1;
            else dirflag = 1;
            Vector3 point;
            float n = Convert.ToSingle(Randomizer.Uniform(10, 25, 1)[0]);
            float j = 1;
            for (float i = 1; i < 8; i++)
            {
                point = (transform.forward * n * i) + (transform.right * j * dirflag * n) + new Vector3(0, 0.5f, 0) + this.transform.position;
                dirflag = dirflag * -1;
                Driver.MoveToQueue(point);
                j++;
            }
            Driver.MoveToQueue(this.transform.position);
            return true;
        }

        public override void SendTo(Vector3 destination)
        {
            Warp();
            orderBackCount = Vector3.Distance(this.transform.position, destination) / (this.GetComponent<NavMeshAgent>().speed * 0.9f);
            aiStatus = UnitStateType.UnderControl;
            Driver.MoveTo(destination);
        }
        public override void SendToQueue(Vector3 destination)
        {
            Warp();
            orderBackCount += Vector3.Distance(this.transform.position, destination) / (this.GetComponent<NavMeshAgent>().speed * 0.9f);
            aiStatus = UnitStateType.UnderControl;
            Driver.MoveToQueue(destination);
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
                                cooldownMissileInhibitor = 8;
                                return true;
                            }
                        }
                    }
            }
            return false;
        }
        private bool RadarBooster()
        {
            if (cooldownRadarBooster < 0)
            {
                this.Impacts.Add(new RadarBoosterImpact(this, 4f));
                cooldownRadarBooster = 8;
                return true;
            }
            else if (cooldownRadarBooster < 4)
            {
                foreach (SpaceShip x in allies)
                    x.Impacts.Add(new RadarBoosterImpact(x, 0.5f));
                return false;
            }
            return false;
        }
        private bool Warp()
        {
            if (cooldownWarp <= 0)
            {
                cooldownWarp = 14f;
                this.Impacts.Add(new WarpImpact(this));
                return true;
            }
            else return false;
        }
    }
    public class WarpImpact : IImpact
    {
        public string ImpactName { get { return "WarpImpact"; } }
        float ttl;
        SpaceShip owner;
        private float ownerSpeedPrev;
        private float ownerMassPrev;
        public WarpImpact(SpaceShip owner)
        {
            this.owner = owner;
            ownerSpeedPrev = owner.Speed;
            ownerMassPrev = owner.GetComponent<Rigidbody>().mass;
            if (owner.Impacts.Exists(x => x.ImpactName == this.ImpactName))
                ttl = 0;
            else
            {
                if (owner.Impacts.Exists(x => x.ImpactName == "TrusterInhibitorImpact"))
                    ttl = 0;
                else
                {
                    ttl = 4;
                    owner.Speed = owner.Speed * 6;
                    owner.GetComponent<Rigidbody>().mass = owner.GetComponent<Rigidbody>().mass / 10;
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
            owner.GetComponent<Rigidbody>().mass = ownerMassPrev;
            owner.Impacts.Remove(this);
        }
    }
    public class RadarBoosterImpact : IImpact
    {
        public string ImpactName { get { return "RadarBoosterImpact"; } }
        float ttl;
        SpaceShip owner;
        private float ownerRadarRangePrev;
        public RadarBoosterImpact(SpaceShip owner, float time)
        {
            this.owner = owner;
            ownerRadarRangePrev = owner.RadarRange;
            if (owner.Impacts.Exists(x => x.ImpactName == this.ImpactName))
                ttl = 0;
            else
            {
                if (owner.Impacts.Exists(x => x.ImpactName == "RadarInhibitorImpact"))
                    ttl = 0;
                else
                {
                    ttl = time;
                    owner.RadarRange = owner.RadarRange * 2;
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
