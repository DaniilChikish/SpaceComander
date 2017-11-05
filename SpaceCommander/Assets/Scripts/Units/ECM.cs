using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeusUtility.Random;
/**
  * Перехватчик-РЭБ (EMC)
  * Физические параметры: (по образу самолета малой авиации)
  *      Масса = 2000кг
  *      Длина ~ 6м
  * **/
namespace SpaceCommander.Units
{
    public class ECM : SpaceShip
    {
        public bool jamming;//Make private after debug;
        public override Vector3 Velocity
        {
            get
            {
                if (jamming)
                    return Vector3.zero;
                else
                    return base.Velocity;
            }
        }


        protected override void StatsUp()
        {
            base.StatsUp();
            type = UnitClass.ECM;
            jamming = false;
            EnemySortDelegate = EMCSortEnemys;
            AlliesSortDelegate = ScoutSortEnemys;

            module = new SpellModule[4];
            module[0] = new MissileTrapLauncher(this);
            module[1] = new Jammer(this);
            module[2] = new TrusterStunner(this);
            module[3] = new ShieldsBraker(this);
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
                    if (m.GetType() == typeof(Jammer) && m.State == SpellModuleState.Active)
                    {
                        jamming = true;
                        break;
                    }
                    else jamming = false;
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
                        UseModule(new SpellFunction[] { SpellFunction.Attack, SpellFunction.Debuff });
                        return Driver.ExeceteTargetManeuver(TatgetManeuverType.DecreaseDistance, Gunner.Target.transform);
                    }
                case TargetStateType.InPrimaryRange:
                    {
                        UseModule(new SpellFunction[] { SpellFunction.Attack });
                        UseModule(new SpellFunction[] { SpellFunction.Self, SpellFunction.Buff });
                        return Driver.ExeceteTargetManeuver(TatgetManeuverType.IncreaseDistance, Gunner.Target.transform);
                    }
                case TargetStateType.InSecondaryRange:
                    {
                        UseModule(new SpellFunction[] { SpellFunction.Attack, SpellFunction.Debuff });
                        return Driver.ExeceteTargetManeuver(TatgetManeuverType.DecreaseDistance, Gunner.Target.transform);
                    }
                case TargetStateType.BehindABarrier:
                    {
                        return Rush();
                    }
                default:
                    return false;
            }
        }
        protected override int RadarWarningResiever()
        {
            capByTarget.Clear();
            foreach (Unit x in enemys)
            {
                UseModule(new SpellFunction[] { SpellFunction.Enemy, SpellFunction.Debuff });
                SpaceShip enemy = x.GetComponent<SpaceShip>();
                if (enemy.CurrentTarget != null && enemy.CurrentTarget.transform == this.transform)
                    capByTarget.Add(x);
                if (jamming)
                {
                    enemy.ResetTarget();
                }
            }
            capByTarget.Sort(delegate (Unit x, Unit y) { return EnemySortDelegate(x, y); });
            return capByTarget.Count;
        }

    }
}
