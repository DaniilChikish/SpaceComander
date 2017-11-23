using UnityEngine;
using SpaceCommander.AI;
using SpaceCommander.Mechanics.Modules;
/**
* Перехватчик-диверсант (Recon)
* Физические параметры: (по образу самолета малой авиации)
*      Масса = 2000кг
*      Длина ~ 6м
* **/
namespace SpaceCommander.Mechanics.Units
{
    public class Recon : SpaceShip
    {
        private bool transpond;
        protected override void StatsUp()
        {
            base.StatsUp();
            type = UnitClass.Recon;
            EnemySortDelegate = ReconSortEnemys;
            AlliesSortDelegate = EMCSortEnemys;
            module = new SpellModule[3];
            module[0] = new MissileTrapLauncher(this);
            module[1] = new Transponder(this);
            module[2] = new ShieldStunner(this);
        }
        protected override void Explosion()
        {
            GameObject blast = Instantiate(Global.Prefab.ShipDieBlast, gameObject.transform.position, gameObject.transform.rotation);
            blast.GetComponent<Explosion>().StatUp(BlastType.SmallShip);
        }
        protected override void DecrementLocalCounters()
        {
            if (module != null && module.Length > 0)
            {
                foreach (SpellModule m in module)
                {
                    if (m.GetType() == typeof(Transponder) && m.State == SpellModuleState.Active)
                    {
                        transpond = true;
                        break;
                    }
                    else transpond = false;
                }
            }
        }
        //AI logick
        protected override bool AttackManeuver()
        {
            switch (targetStatus)
            {
                case TargetStateType.Captured:
                    {
                        UseModule(new SpellFunction[] { SpellFunction.Attack });
                        return ToSecondaryDistance();
                    }
                case TargetStateType.InPrimaryRange:
                    {
                        UseModule(new SpellFunction[] { SpellFunction.Defence, SpellFunction.Buff });
                        return Driver.ExeceteTargetManeuver(TatgetManeuverType.IncreaseDistance, Gunner.Target.transform);
                    }
                case TargetStateType.InSecondaryRange:
                    {
                        UseModule(new SpellFunction[] { SpellFunction.Attack, SpellFunction.Buff });
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
        public override Army Team
        {
            get
            {
                if (transpond)
                    return Army.Civil;
                else
                    return base.Team;
            }
        }
    }
}
