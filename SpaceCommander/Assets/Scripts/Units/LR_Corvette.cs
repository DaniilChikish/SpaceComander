using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpaceCommander.Units
{
    public class LR_Corvette : SpaceShip
    {

        private bool idleFulag;
        protected override void StatsUp()
        {
            type = UnitClass.LR_Corvette;
            radarRange = 650; //set in child
            radarPover = 0.7f;
            speedThrust = 6; //set in child
            speedRotation = 40;
            speedShift = 6;
            stealthness = 0.3f; //set in child
            radiolink = 2.5f;
            EnemySortDelegate = LRCorvetteSortEnemys;
            AlliesSortDelegate = SupportCorvetteSortEnemys;

            module = new SpellModule[1];
            module[0] = new EmergencySelfRapairing(this);
        }
        protected override void Explosion()
        {
            GameObject blast = Instantiate(Global.ShipDieBlast, gameObject.transform.position, gameObject.transform.rotation);
            blast.GetComponent<Explosion>().StatUp(BlastType.Corvette);
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
        protected override bool IdleManeuverFunction()
        {
            idleFulag = !idleFulag;
            if (idleFulag)
                return PatroolLineParallel(150);
            else return PatroolLineParallel(50);
        }
    }
}
