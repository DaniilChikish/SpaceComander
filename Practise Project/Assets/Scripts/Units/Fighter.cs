using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DeusUtility.Random;


namespace SpaceCommander.Units
{
    public class Fighter : SpaceShip
    {

        private bool idleFulag;


        protected override void StatsUp()
        {
            type = UnitClass.Figther;
            radarRange = 350; //set in child
            radarPover = 0.8f;
            speed = 9; //set in child
            stealthness = 0.5f; //set in child
            radiolink = 1.5f;
            module = new SpellModule[2];
            module[0] = new MissileTrapLauncher(this);
            module[1] = new Jammer(this);
            EnemySortDelegate = FigtherSortEnemys;
            AlliesSortDelegate = ReconSortEnemys;
        }

        protected override void Explosion()
        {
            GameObject blast = Instantiate(Global.ShipDieBlast, gameObject.transform.position, gameObject.transform.rotation);
            blast.GetComponent<Explosion>().StatUp(BlastType.SmallShip);
        }
        protected override void DecrementLocalCounters()
        {
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
                        UseModule(new SpellFunction[] { SpellFunction.Self, SpellFunction.Buff });
                        UseModule(new SpellFunction[] { SpellFunction.Attack, SpellFunction.Buff });
                        return Evasion(CurrentTarget.transform.right);
                    }
                case TargetStateType.InSecondaryRange:
                    {
                        UseModule(new SpellFunction[] { SpellFunction.Self, SpellFunction.Defence });
                        return ToPrimaryDistance();
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
            if (owner.HaveImpact(this.Name))
                ttl = 0;
            else
            {
                if (owner.HaveImpact("TrusterInhibitorImpact"))
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
            owner.RemoveImpact(this);
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
            ownerShieldMaxPowerPrew = owner.ShieldCampacity;
            ownerShieldRechargingPrew = owner.ShieldRecharging;
            if (owner.HaveImpact(this.Name))
                ttl = 0;
            else
            {
                if (owner.HaveImpact("ShieldInhibitorImpact"))
                    ttl = 0;
                else
                {
                    ttl = time;
                    owner.ShieldCampacity = owner.ShieldCampacity * 4;
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
            owner.ShieldCampacity = ownerShieldMaxPowerPrew;
            owner.ShieldRecharging = ownerShieldRechargingPrew;
            owner.RemoveImpact(this);
        }
    }
}