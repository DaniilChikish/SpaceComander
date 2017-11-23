using SpaceCommander.AI;
using SpaceCommander.Mechanics.Modules;
using UnityEngine;
namespace SpaceCommander.Mechanics.Units
{
    public class LR_Corvette : SpaceShip
    {

        private bool idleFulag;
        protected override void StatsUp()
        {
            base.StatsUp();
            type = UnitClass.LR_Corvette;
            EnemySortDelegate = LRCorvetteSortEnemys;
            AlliesSortDelegate = SupportCorvetteSortEnemys;

            module = new SpellModule[1];
            module[0] = new EmergencySelfRapairing(this);
        }
        protected override void Explosion()
        {
            GameObject blast = Instantiate(Global.Prefab.ShipDieBlast, gameObject.transform.position, gameObject.transform.rotation);
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
                        return Driver.ExeceteTargetManeuver(TatgetManeuverType.IncreaseDistance, Gunner.Target.transform);
                    }
                case TargetStateType.InSecondaryRange:
                    {
                        return Driver.ExeceteTargetManeuver(TatgetManeuverType.Evasion, CurrentTarget.transform);
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
                return Driver.ExecetePointManeuver(PointManeuverType.PatroolLine, this.transform.position, this.transform.forward * 150);
            else return Driver.ExecetePointManeuver(PointManeuverType.PatroolLine, this.transform.position, this.transform.forward * 50);
        }
    }
}
