using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeusUtility.Random;

namespace SpaceCommander.Units
{
    public class Recon : SpaceShip
    {
        private bool transpond;
        protected override void StatsUp()
        {
            type = UnitClass.Recon;
            radarRange = 700; //set in child
            radarPover = 0.8f;
            speedThrust = 10f; //set in child
            speedRotation = 70;
            speedShift = 12;
            stealthness = 0.7f; //set in child
            radiolink = 1.5f;
            EnemySortDelegate = ReconSortEnemys;
            AlliesSortDelegate = EMCSortEnemys;
            module = new SpellModule[3];
            module[0] = new MissileTrapLauncher(this);
            module[1] = new Transponder(this);
            module[2] = new ShieldStunner(this);
        }
        protected override void Explosion()
        {
            GameObject blast = Instantiate(Global.ShipDieBlast, gameObject.transform.position, gameObject.transform.rotation);
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

        public override bool Allies(Army army)
        {
            if (!transpond)
            {
                if (army == Global.playerArmy)
                {
                    cooldownDetected = 1;
                    this.gameObject.transform.FindChild("MinimapPict").FindChild("EnemyMinimapPict").GetComponent<Renderer>().enabled = true;
                }
                return (team == army);
            }
            else
            {
                return true;
            }
        }
    }
}
