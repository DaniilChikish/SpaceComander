using SpaceCommander.AI;
using SpaceCommander.Mechanics.Modules;
using UnityEngine;
/**
 * Торпедоносец/бомбардировщик (Bomber)
 * Физические параметры: (по образу B-25)
 *      Масса = 20000кг
 *      Длина ~ 14м
 * **/
namespace SpaceCommander.Mechanics.Units
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

            module = new SpellModule[3];
            module[0] = new MissileTrapLauncher(this);
            module[1] = new NukeTorpedoLauncher(this);
            module[2] = new RechargeAcceleratorPassive(this);
        }

        protected override void Explosion()
        {
            GameObject blast = Instantiate(Global.Prefab.ShipDieBlast, gameObject.transform.position, gameObject.transform.rotation);
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
    }
}
