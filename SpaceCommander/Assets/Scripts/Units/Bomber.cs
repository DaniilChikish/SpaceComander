using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCommander.Units
{
    public class Bomber : SpaceShip
    {
        public TorpedoType StrategicLoad;
        protected override void StatsUp()
        {
            base.StatsUp();
            type = UnitClass.Bomber;
            StrategicLoad = TorpedoType.Nuke;
            EnemySortDelegate = BomberSortEnemys;
            AlliesSortDelegate = SupportCorvetteSortEnemys;

            module = new SpellModule[2];
            module[0] = new MissileTrapLauncher(this);
            module[1] = new RechargeAcceleratorPassive(this);
        }

        protected override void Explosion()
        {
            GameObject blast = Instantiate(Global.ShipDieBlast, gameObject.transform.position, gameObject.transform.rotation);
            blast.GetComponent<Explosion>().StatUp(BlastType.MediumShip);
        }
        protected override void DecrementLocalCounters()
        {

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
    }
}
