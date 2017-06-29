using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCommander
{
    public class Recon : SpaceShip
    {
        private bool jamming;
        private bool transpond;
        public override float Stealthness { get { if (jamming) return stealthness * 0.2f; else return stealthness; } }
        //private float cooldownInhibitor;
        private float cooldownJammer;
        private float cooldownTransponder;
        public float cooldownMissileInhibitor;//Make private after debug;
        private float cooldownShieldStunner;

        protected override void StatsUp()
        {
            type = UnitClass.Recon;
            radarRange = 250; //set in child
            radarPover = 1.1f;
            speed = 8.5f; //set in child
            stealthness = 0.2f; //set in child
            radiolink = 1.5f;
            EnemySortDelegate = ReconSortEnemys;
            AlliesSortDelegate = EMCSortEnemys;
        }
        protected override void Explosion()
        {
            GameObject blast = Instantiate(Global.ShipDieBlast, gameObject.transform.position, gameObject.transform.rotation);
            blast.GetComponent<Explosion>().StatUp(BlastType.SmallShip);
        }
        protected override void DecrementCounters()
        {
            cooldownJammer -= Time.deltaTime;
            if (jamming && cooldownJammer <= 0)
            {
                jamming = false;
                cooldownJammer = 5;
            }
            cooldownTransponder -= Time.deltaTime;
            if (transpond && cooldownTransponder <= 0)
            {
                transpond = false;
                //Debug.Log("TransponderOff");
                cooldownTransponder = 30;
            }
            if (cooldownMissileInhibitor > 0)
                cooldownMissileInhibitor -= Time.deltaTime;
            if (cooldownShieldStunner > 0)
                cooldownShieldStunner -= Time.deltaTime;
        }
        //AI logick
        protected override bool AttackManeuver()
        {
            switch (targetStatus)
            {
                case TargetStateType.Captured:
                    {
                        if (RadarTransponder())
                            return ToPrimaryDistance();
                        else
                        return ToSecondaryDistance();
                    }
                case TargetStateType.InPrimaryRange:
                    {
                        return IncreaseDistance();
                    }
                case TargetStateType.InSecondaryRange:
                    {
                        return Evasion(CurrentTarget.transform.right);
                    }
                case TargetStateType.BehindABarrier:
                    {
                        if (RadarTransponder())
                            return Rush();
                        else return ToSecondaryDistance();
                    }
                default:
                    return false;
            }
        }
        protected override bool RoleFunction()
        {
            return (RadarTransponder() || ShieldStunner(CurrentTarget));
        }
        protected override bool SelfDefenceFunction()
        {
            SelfDefenceFunctionBase();
            if (RadarWarningResiever() > 5)
                Jammer();
            MissileGuidanceInhibitor();
            return true;
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
                                cooldownMissileInhibitor = 10;
                                return true;
                            }
                        }
                    }
            }
            return false;
        }
        private bool RadarTransponder()
        {
            if (!transpond && cooldownTransponder <= 0)
            {
                //Debug.Log("TransponderOn");
                transpond = true;
                cooldownTransponder = 10f;
                return true;
            }
            else return false;
        }
        private bool ShieldStunner(SpaceShip target)
        {
            if (cooldownShieldStunner <= 0f && target != null)
            {
                target.MakeImpact(new ShildStunImpact(target, 5f));
                cooldownShieldStunner = 15f;
                return true;
            }
            return false;
        }

        public override bool Allies(Army army)
        {
            if (!transpond)
            {
                if (army == Global.playerArmy)
                {
                    cooldownDetected = 1;
                    this.gameObject.transform.FindChild("MinimapPict").FindChild("EnemyMinimapPict").GetComponent<Renderer>().enabled = true;
                }
                return (team == army);
            }
            else
            {
                return true;
            }
        }
    }
    public class ShildStunImpact : IImpact
    {
        public bool Act = false;
        public string Name { get { return "ShildStunImpact"; } }
        private float ttl;
        private SpaceShip owner;
        private float ownerShildRechargingPrev;
        public ShildStunImpact(SpaceShip owner, float time)
        {

            this.owner = owner;
            ownerShildRechargingPrev = owner.ShieldRecharging;
            if (owner.HaveImpact(this.Name))
                ttl = 0;
            else
            {
                if (owner.HaveImpact("ShildBoosterImpact"))
                    ttl = 0;
                else
                {
                    Act = true;
                    ttl = time;
                    owner.ShieldForce -= 25;
                    owner.ShieldRecharging = -10;
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
            if (Act) owner.ShieldRecharging = ownerShildRechargingPrev;
            owner.RemoveImpact(this);
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
