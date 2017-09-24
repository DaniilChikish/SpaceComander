using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeusUtility.Random;

namespace SpaceCommander.Units
{
    public class Command : SpaceShip
    {
        protected override void StatsUp()
        {
            type = UnitClass.Command;
            radarRange = 1000; //set in child
            radarPover = 0.8f;
            speedThrust = 10; //set in child
            speedRotation = 61;
            speedShift = 10;
            combatAIEnabled = true; //set in child
            selfDefenceModuleEnabled = true; //set in child
            stealthness = 0.45f; //set in child
            radiolink = 2.5f;
            EnemySortDelegate = FigtherSortEnemys;
            AlliesSortDelegate = ReconSortEnemys;

            module = new SpellModule[4];
            module[0] = new MissileTrapLauncher(this);
            module[1] = new EmergencyShieldRecharging(this);
            module[2] = new ForcedTargetDesignator(this);
            module[3] = new TeamSpirit(this);
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
                        return IncreaseDistance();
                    }
                case TargetStateType.InSecondaryRange:
                    {
                        UseModule(new SpellFunction[] { SpellFunction.Attack, SpellFunction.Buff });
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
