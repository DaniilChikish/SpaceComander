using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PracticeProject
{
    public class Support_Corvette : SpaceShip
    {
        private float cooldownRepairBot;
        private float cooldownReloadBot;
        private bool idleFulag;
        protected override void StatsUp()
        {
            type = UnitClass.Support_Corvette;
            radarRange = 200; //set in child
            radarPover = 1;
            speed = 4; //set in child
            stealthness = 0.9f; //set in child
            radiolink = 1f;
            EnemySortDelegate = SupportCorvetteSortEnemys;
            AlliesSortDelegate = SupportCorvetteSortEnemys;
        }
        protected override void Explosion()
        {
            GameObject blast = Instantiate(Global.ShipDieBlast, gameObject.transform.position, gameObject.transform.rotation);
            blast.GetComponent<Explosion>().StatUp(BlastType.Corvette);
        }
        protected override void DecrementCounters()
        {
            if (cooldownRepairBot > 0)
                cooldownRepairBot -= Time.deltaTime;
            if (cooldownReloadBot > 0)
                cooldownReloadBot -= Time.deltaTime;
        }
        protected override bool IdleManeuverFunction()
        {
            idleFulag = !idleFulag;
            if (idleFulag)
                return PatroolLinePerpendicularly(150);
            else return PatroolPoint();
        }
        protected override bool RoleFunction()
        {
            RepairBot();
            ReloadBot();
            return true;
        }
        protected override bool SelfDefenceFunction()
        {
            SelfDefenceFunctionBase();
            return true;
        }
        private bool RepairBot()
        {
            if (cooldownRepairBot <= 0)
            {
                if (Health < MaxHealth * 0.4)
                {
                    this.Impacts.Add(new Repairing(this, 10));
                    cooldownRepairBot = 15;
                    return true;
                }
                else
                    foreach (SpaceShip x in allies)
                    {
                        if ((Vector3.Distance(x.transform.position, this.transform.position) < radarRange * 0.2) && (x.Health < x.MaxHealth * 0.7))
                        {
                            x.Impacts.Add(new Repairing(x, 10));
                            cooldownRepairBot = 5;
                            return true;
                        }
                    }
            }
            return false;
        }
        private bool ReloadBot()
        {
            if (cooldownReloadBot <= 0)
            {
                if (this.NeedReloading)
                {
                    this.Impacts.Add(new Reloading(this, 0));
                    cooldownReloadBot = 20;
                    return true;
                }
                foreach (SpaceShip x in allies)
                {
                    if ((Vector3.Distance(x.transform.position, this.transform.position) < radarRange * 0.4) && (x.NeedReloading))
                    {
                        x.Impacts.Add(new Reloading(x, 0));
                        cooldownReloadBot = 20;
                        return true;
                    }
                }
            }
            return false;
        }
    }
    public class Repairing : IImpact
    {
        public string ImpactName { get { return "Repairing"; } }
        private float ttl;
        private float repairSpeed;
        private SpaceShip owner;
        public Repairing(SpaceShip owner, float time)
        {
            this.owner = owner;
            switch (owner.Type)
            {
                case UnitClass.Bomber:
                    {
                        repairSpeed = 10;
                        break;
                    }
                case UnitClass.Command:
                    {
                        repairSpeed = 14;
                        break;
                    }
                case UnitClass.ECM:
                    {
                        repairSpeed = 4;
                        break;
                    }
                case UnitClass.Figther:
                    {
                        repairSpeed = 8;
                        break;
                    }
                case UnitClass.Guard_Corvette:
                    {
                        repairSpeed = 20;
                        break;
                    }
                case UnitClass.LR_Corvette:
                    {
                        repairSpeed = 20;
                        break;
                    }
                case UnitClass.Recon:
                    {
                        repairSpeed = 4;
                        break;
                    }
                case UnitClass.Scout:
                    {
                        repairSpeed = 6;
                        break;
                    }
                case UnitClass.Support_Corvette:
                    {
                        repairSpeed = 20;
                        break;
                    }
            }
            //if (owner.Impacts.Exists(x => x.ImpactName == this.ImpactName))
            //    ttl = 0;
            //else
            //{
                ttl = time;
            //}
        }
        public void ActImpact()
        {
            if (ttl > 0 && owner.Health < (owner.MaxHealth*1.2))
            {
                owner.Health += repairSpeed * Time.deltaTime;
                ttl -= Time.deltaTime;
            }
            else CompleteImpact();
        }
        public void CompleteImpact()
        {
            owner.Impacts.Remove(this);
        }
        public override string ToString()
        {
            return ImpactName;
        }
    }
    public class Reloading : IImpact
    {
        public string ImpactName { get { return "Reloading"; } }
        private float ttl;
        private SpaceShip owner;
        public Reloading(SpaceShip owner, float time)
        {
            this.owner = owner;
            ttl = 0;
        }
        public void ActImpact()
        {
            if (ttl > 0)
            {
                ttl -= Time.deltaTime;
            }
            else CompleteImpact();
        }
        public void CompleteImpact()
        {
            owner.ReloadWeapons();
            owner.Impacts.Remove(this);
        }
        public override string ToString()
        {
            return ImpactName;
        }
    }
}
