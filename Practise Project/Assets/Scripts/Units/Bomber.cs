using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PracticeProject
{
    public class Bomber : SpaceShip
    {
        public TorpedoType StrategicLoad;
        private float cooldovnRocketVolley;
        protected override void StatsUp()
        {
            type = UnitClass.Bomber;
            radarRange = 300; //set in child
            radarPover = 1;
            speed = 6.5f; //set in child
            stealthness = 0.5f; //set in child
            radiolink = 2.5f;
            StrategicLoad = TorpedoType.Nuke;
            EnemySortDelegate = BomberSortEnemys;
            AlliesSortDelegate = SupportCorvetteSortEnemys;
        }
        protected override void Explosion()
        {
            GameObject blast = Instantiate(Global.ShipDieBlast, gameObject.transform.position, gameObject.transform.rotation);
            blast.GetComponent<Explosion>().StatUp(BlastType.MediumShip);
        }
        protected override void DecrementCounters()
        {
            if (cooldovnRocketVolley > 0)
                cooldovnRocketVolley -= Time.deltaTime;
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
            return RocketVolley();
        }
        protected override bool SelfDefenceFunction()
        {
            SelfDefenceFunctionBase();
            return true;
        }
        protected bool RocketVolley()
        {
            if (cooldovnRocketVolley <= 0)
            {
                if (CurrentTarget != null && (CurrentTarget.Type == UnitClass.LR_Corvette || CurrentTarget.Type == UnitClass.Guard_Corvette || CurrentTarget.Type == UnitClass.Support_Corvette))
                {
                    Gunner.Volley(new SpaceShip[] { CurrentTarget }, 1);
                    cooldovnRocketVolley = 30;
                    return true;
                }
                else
                {
                    Gunner.Volley(enemys.ToArray(), 0);
                    cooldovnRocketVolley = 40;
                    return true;
                }
            }
            else return false;
        }
    }
}
