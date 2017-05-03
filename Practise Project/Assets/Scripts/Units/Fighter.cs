using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace PracticeProject
{
    public class Fighter : SpaceShip
    {
        //private float cooldownInhibitor;
        private float cooldownMissileInhibitor;//Make private after debug;
        private float cooldownForsage;
        private float cooldownShieldBooster;
        private bool idleFulag;
        private float cooldovnCannonVolley;

        protected override void StatsUp()
        {
            type = UnitClass.Figther;
            radarRange = 300; //set in child
            radarPover = 1;
            speed = 9; //set in child
            stealthness = 0.5f; //set in child
            radiolink = 1.5f;
            EnemySortDelegate = FigtherSortEnemys;
            AlliesSortDelegate = ReconSortEnemys;
        }
        protected override void Explosion()
        {
            GameObject blast = Instantiate(Global.ShipDieBlast, gameObject.transform.position, gameObject.transform.rotation);
            blast.GetComponent<Explosion>().StatUp(BlastType.SmallShip);
        }
        protected override void DecrementCounters()
        {
            if (cooldownMissileInhibitor > 0)
                cooldownMissileInhibitor -= Time.deltaTime;
            if (cooldownForsage > 0)
                cooldownForsage -= Time.deltaTime;
            if (cooldownShieldBooster > 0)
                cooldownShieldBooster -= Time.deltaTime;
            if (cooldovnCannonVolley > 0)
                cooldovnCannonVolley -= Time.deltaTime;
        }
        protected override bool RoleFunction()
        {
            return (ShieldBooster()||Forsage()||CannonVolley());
        }
        protected override bool SelfDefenceFunction()
        {
            SelfDefenceFunctionBase();
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
                        return ToPrimaryDistance();
                    }
                case TargetStateType.InPrimaryRange:
                    {
                        return Evasion(CurrentTarget.transform.right);
                    }
                case TargetStateType.InSecondaryRange:
                    {
                        if (ShieldBooster())
                            return Rush();
                        else return ToPrimaryDistance();
                    }
                case TargetStateType.BehindABarrier:
                    {
                        return ToPrimaryDistance();
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
            orderBackCount = Vector3.Distance(this.transform.position, destination) / (this.GetComponent<NavMeshAgent>().speed * 0.9f);
            aiStatus = UnitStateType.UnderControl;
            Driver.MoveTo(destination);
        }
        public override void SendToQueue(Vector3 destination)
        {
            orderBackCount += Vector3.Distance(this.transform.position, destination) / (this.GetComponent<NavMeshAgent>().speed * 0.9f);
            aiStatus = UnitStateType.UnderControl;
            Driver.MoveToQueue(destination);
        }
        private bool Forsage()
        {
            if (cooldownForsage <= 0)
            {
                cooldownForsage = 20f;
                this.Impacts.Add(new ForsageImpact(this));
                return true;
            }
            else return false;
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
        private bool ShieldBooster()
        {
            if (cooldownShieldBooster <= 0 && !shield.isOwerheat && targetStatus == TargetStateType.InPrimaryRange)
            {
                this.Impacts.Add(new ShieldBoosterImpact(this, 5f));
                cooldownShieldBooster = 15;
                return true;
            }
            else return false;
        }
        protected bool CannonVolley()
        {
            if (cooldovnCannonVolley <= 0)
            {
                if (CurrentTarget != null)
                {
                    Gunner.Volley(new SpaceShip[] { CurrentTarget }, 0);
                    cooldovnCannonVolley = 20;
                    return true;
                }
            }
            return false;
        }
    }
    public class ForsageImpact : IImpact
    {
        public string Name { get { return "WarpImpact"; } }
        float ttl;
        SpaceShip owner;
        private float ownerSpeedPrev;
        public ForsageImpact(SpaceShip owner)
        {
            this.owner = owner;
            ownerSpeedPrev = owner.Speed;
            if (owner.Impacts.Exists(x => x.Name == this.Name))
                ttl = 0;
            else
            {
                if (owner.Impacts.Exists(x => x.Name == "TrusterInhibitorImpact"))
                    ttl = 0;
                else
                {
                    ttl = 15;
                    owner.Speed = owner.Speed * 2;
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
    }
    public class ShieldBoosterImpact : IImpact
    {
        public string Name { get { return "ShieldBoosterImpact"; } }
        float ttl;
        SpaceShip owner;
        private float ownerShieldMaxPowerPrew;
        private float ownerShieldRechargingPrew;
        public ShieldBoosterImpact(SpaceShip owner, float time)
        {
            this.owner = owner;
            ownerShieldMaxPowerPrew = owner.ShieldMaxCampacity;
            ownerShieldRechargingPrew = owner.ShieldRecharging;
            if (owner.Impacts.Exists(x => x.Name == this.Name))
                ttl = 0;
            else
            {
                if (owner.Impacts.Exists(x => x.Name == "ShieldInhibitorImpact"))
                    ttl = 0;
                else
                {
                    ttl = time;
                    owner.ShieldMaxCampacity = owner.ShieldMaxCampacity * 4;
                    owner.ShieldRecharging = owner.ShieldRecharging * 2;
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
            owner.ShieldMaxCampacity = ownerShieldMaxPowerPrew;
            owner.ShieldRecharging = ownerShieldRechargingPrew;
            owner.Impacts.Remove(this);
        }
    }
}