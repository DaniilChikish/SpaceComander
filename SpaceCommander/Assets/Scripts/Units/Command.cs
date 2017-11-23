using SpaceCommander.AI;
using SpaceCommander.Mechanics.Modules;

namespace SpaceCommander.Mechanics.Units
{
    public class Command : SpaceShip
    {
        protected override void StatsUp()
        {
            base.StatsUp();
            type = UnitClass.Command;
            EnemySortDelegate = FigtherSortEnemys;
            AlliesSortDelegate = ReconSortEnemys;

            module = new SpellModule[4];
            module[0] = new MissileTrapLauncher(this);
            module[1] = new EmergencyShieldRecharging(this);
            module[2] = new ForcedTargetDesignator(this);
            module[3] = new Inspiration(this);
        }
        protected override void DecrementLocalCounters()
        {
        }
        //AI logick
        protected override bool AttackManeuver()
        {
            if (allies.Count > 0)
                UseModule(new SpellFunction[] { SpellFunction.Allies, SpellFunction.Buff });
            if (CurrentTarget != null)
                UseModule(new SpellFunction[] { SpellFunction.Enemy, SpellFunction.Debuff});

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
    }
}
