using DeusUtility.Random;
using SpaceCommander.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCommander.Test
{
    public class ScoutTDBait : SpaceShip
    {
        private bool idleFulag;

        protected override void StatsUp()
        {
            type = UnitClass.Scout;

            speedThrust = Convert.ToSingle(Global.SpecINI.GetValue(typeof(Scout).ToString(), "speedThrust"));
            speedRotation = Convert.ToSingle(Global.SpecINI.GetValue(typeof(Scout).ToString(), "speedRotation"));
            speedShift = Convert.ToSingle(Global.SpecINI.GetValue(typeof(Scout).ToString(), "speedShift"));
            radarRange = Convert.ToSingle(Global.SpecINI.GetValue(typeof(Scout).ToString(), "radarRange"));
            radarPover = Convert.ToSingle(Global.SpecINI.GetValue(typeof(Scout).ToString(), "radarPover"));
            stealthness = Convert.ToSingle(Global.SpecINI.GetValue(typeof(Scout).ToString(), "stealthness"));
            radiolink = Convert.ToSingle(Global.SpecINI.GetValue(typeof(Scout).ToString(), "radiolink"));

            //armor.maxHitpoints = Convert.ToSingle(Global.SpecINI.ReadINI(typeof(Scout).ToString(), "maxHitpoints"));
            //armor.hitpoints = armor.maxHitpoints;
            //armor.shellResist = Convert.ToSingle(Global.SpecINI.ReadINI(typeof(Scout).ToString(), "shellResist"));
            //armor.energyResist = Convert.ToSingle(Global.SpecINI.ReadINI(typeof(Scout).ToString(), "energyResist"));
            //armor.blastResist = Convert.ToSingle(Global.SpecINI.ReadINI(typeof(Scout).ToString(), "blastResist"));

            //shield.maxCampacity = Convert.ToSingle(Global.SpecINI.ReadINI(typeof(Scout).ToString(), "maxCampacity"));
            //shield.force = shield.maxCampacity;
            //shield.recharging = Convert.ToSingle(Global.SpecINI.ReadINI(typeof(Scout).ToString(), "recharging"));

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
        protected override bool IdleManeuverFunction()
        {
            //Debug.Log("new loop");
            idleFulag = !idleFulag;
            if (idleFulag)
                return Driver.ExecetePointManeuver(PointManeuverType.PatroolSpiral, this.transform.position, this.transform.forward * 50);
            else return Driver.ExecetePointManeuver(PointManeuverType.PatroolDiamond, this.transform.position, this.transform.forward * 50);
        }
        private bool PatroolTriangle()
        {
            //waitingBackCount = 40f;
            float dirflag;
            if (Randomizer.Uniform(-10, 10, 1)[0] > 0)
                dirflag = -1;
            else dirflag = 1;
            Vector3 point;
            float n = Convert.ToSingle(Randomizer.Uniform(10, 25, 1)[0]);
            float j = 1;
            for (float i = 1; i < 8; i++)
            {
                point = (transform.forward * n * i) + (transform.right * j * dirflag * n) + new Vector3(0, 0.5f, 0) + this.transform.position;
                dirflag = dirflag * -1;
                Driver.MoveToQueue(point);
                j++;
            }
            Driver.MoveToQueue(this.transform.position);
            return true;
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

