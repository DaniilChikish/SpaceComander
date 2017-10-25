using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DeusUtility.Random;
/**
  * Перехватчик-разведчик (Scout)
  * Физические параметры: (по образу самолета малой авиации)
  *      Масса = 2000кг
  *      Длина ~ 6м
  * **/
namespace SpaceCommander.Units
{
    public class Scout : SpaceShip
    {
        private bool idleFulag;

        protected override void StatsUp()
        {
            base.StatsUp();
            type = UnitClass.Scout;
            EnemySortDelegate = ScoutSortEnemys;
            AlliesSortDelegate = ReconSortEnemys;

            module = new SpellModule[4];
            module[0] = new MissileTrapLauncher(this);
            module[1] = new Jammer(this);
            module[2] = new RadarBooster(this);
            module[3] = new Warp(this);
        }
        protected override void Explosion()
        {
            GameObject blast = Instantiate(Global.ShipDieBlast, gameObject.transform.position, gameObject.transform.rotation);
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
        protected override bool IdleManeuverFunction()
        {
            //Debug.Log("new loop");
            idleFulag = !idleFulag;
            if (idleFulag)
                return Driver.ExecetePointManeuver(PointManeuverType.PatroolSpiral, this.transform.position, this.transform.forward * 50);
            else return Driver.ExecetePointManeuver(PointManeuverType.PatroolDiamond, this.transform.position, this.transform.forward * 50);
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
