using SpaceCommander.AI;
using SpaceCommander.Mechanics.Modules;
using UnityEngine;
/**
  * Перехватчик-разведчик (Scout)
  * Физические параметры: (по образу самолета малой авиации)
  *      Масса = 2000кг
  *      Длина ~ 6м
  * **/
namespace SpaceCommander.Mechanics.Units
{
    public class TraningIntersceptor : SpaceShip
    {
        protected override void StatsUp()
        {
            base.StatsUp();
            type = UnitClass.Scout;
            EnemySortDelegate = SortEnemysBase;
            AlliesSortDelegate = SortEnemysBase;

            movementAiEnabled = false;
            combatAIEnabled = false;

            module = new SpellModule[3];
            module[0] = new MissileTrapLauncher(this);
            module[1] = new EmergencySelfRapairing(this);
            module[2] = new EmergencyShieldRecharging(this);
        }
        protected override void Explosion()
        {
            GameObject blast = Instantiate(Global.Prefab.ShipDieBlast, gameObject.transform.position, gameObject.transform.rotation);
            blast.GetComponent<Explosion>().StatUp(BlastType.SmallShip);
        }
        protected override void DecrementLocalCounters()
        {
        }
        //AI logick
        protected override bool AttackManeuver()
        {
            switch (targetStatus)
            {
                case TargetStateType.Captured:
                    {
                        UseModule(new SpellFunction[] { SpellFunction.Attack });
                        return Rush();
                    }
                case TargetStateType.InPrimaryRange:
                    {
                        UseModule(new SpellFunction[] { SpellFunction.Self, SpellFunction.Buff });
                        return Driver.ExeceteTargetManeuver(TatgetManeuverType.Evasion, CurrentTarget.transform);
                    }
                case TargetStateType.InSecondaryRange:
                    {
                        UseModule(new SpellFunction[] { SpellFunction.Attack, SpellFunction.Buff });
                        return Rush();
                    }
                case TargetStateType.BehindABarrier:
                    {
                        return ToPrimaryDistance();
                    }
                default:
                    return false;
            }
        }
        protected override bool DefenseManeuver()
        {
            switch (targetStatus)
            {
                case TargetStateType.InPrimaryRange:
                    {
                        return Driver.ExeceteTargetManeuver(TatgetManeuverType.Evasion, CurrentTarget.transform);
                    }
                case TargetStateType.InSecondaryRange:
                    {
                        return Rush();
                    }
                default:
                    return base.DefenseManeuver();
            }
        }
        protected override bool RetreatManeuver()
        {
            switch (targetStatus)
            {
                case TargetStateType.InPrimaryRange:
                    {
                        return Driver.ExeceteTargetManeuver(TatgetManeuverType.Evasion, CurrentTarget.transform);
                    }
                case TargetStateType.InSecondaryRange:
                    {
                        return Rush();
                    }
                default:
                    return base.RetreatManeuver();
            }
        }
        protected override bool IdleManeuverFunction()
        {
            UseModule(new SpellFunction[] { SpellFunction.Support, SpellFunction.Buff });
            return Driver.ExecetePointManeuver(PointManeuverType.PatroolDiamond, this.transform.position, this.transform.forward * 50);
        }
        public override void SendTo(Vector3 destination)
        {
            UseModule(new SpellFunction[] { SpellFunction.Attack, SpellFunction.Buff });
            aiStatus = UnitStateType.UnderControl;
            if (Driver.MoveTo(destination) && Team == Global.playerArmy)
                Driver.BuildPathArrows();
        }
        public override void SendToQueue(Vector3 destination)
        {
            UseModule(new SpellFunction[] { SpellFunction.Attack, SpellFunction.Buff });
            aiStatus = UnitStateType.UnderControl;
            if (Driver.MoveToQueue(destination) && Team == Global.playerArmy)
                Driver.BuildPathArrows();
        }
    }
}
