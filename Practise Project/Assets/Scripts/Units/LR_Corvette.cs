using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PracticeProject
{
    public class LR_Corvette : SpaceShip
    {
        private float cooldownReloadBot;
        private float cooldownRepairBot;
        protected override void StatsUp()
        {
            type = UnitClass.LR_Corvette;
            radarRange = 350; //set in child
            radarPover = 1;
            speed = 5; //set in child
            stealthness = 0.8f; //set in child
            radiolink = 2.5f;
            EnemySortDelegate = LRCorvetteSortEnemys;
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
        protected override bool AttackManeuver()
        {
            switch (targetStatus)
            {
                case TargetStateType.Captured:
                    {
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
                        return ToSecondaryDistance();
                    }
                default:
                    return false;
            }
        }
        protected override bool RoleFunction()
        {
            return (SelfReloadBot()||SelfRepairBot());
        }
        protected override bool SelfDefenceFunction()
        {
            SelfDefenceFunctionBase();
            return true;
        }
        private bool SelfReloadBot()
        {
            if ((cooldownReloadBot <= 0) && (this.NeedReloading))
            {
                this.Impacts.Add(new Reloading(this, 0));
                cooldownReloadBot = 10;
                return true;
            }
            else return false;
        }
        private bool SelfRepairBot()
        {
            if ((cooldownRepairBot <= 0) && (Health<MaxHealth*0.5))
            {
                this.Impacts.Add(new Repairing(this, 10));
                cooldownRepairBot = 20;
                return true;
            }
            else return false;
        }
    }
}
