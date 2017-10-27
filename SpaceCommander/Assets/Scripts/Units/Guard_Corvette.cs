using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCommander.Units
{
    public class Guard_Corvette : SpaceShip
    {

        private bool idleFulag;
        protected override void StatsUp()
        {
            base.StatsUp();
            type = UnitClass.Guard_Corvette;
            EnemySortDelegate = GuardCorvetteSortEnemys;
            AlliesSortDelegate = SupportCorvetteSortEnemys;

            module = new SpellModule[3];
            module[0] = new EmergencySelfRapairing(this);
            module[1] = new MissileEliminator(this);
            module[2] = new ProtectionMatrix(this);
        }
        protected override void Explosion()
        {
            GameObject blast = Instantiate(Global.ShipDieBlast, gameObject.transform.position, gameObject.transform.rotation);
            blast.GetComponent<Explosion>().StatUp(BlastType.Corvette);
        }
        protected override void DecrementLocalCounters()
        {

        }
        protected override bool IdleManeuverFunction()
        {
            idleFulag = !idleFulag;
            if (idleFulag)
                return Driver.ExecetePointManeuver(PointManeuverType.PatroolLine, this.transform.position, this.transform.right * 150);
            else return Driver.ExecetePointManeuver(PointManeuverType.PatroolDiamond, this.transform.position, this.transform.forward * 50);

        }
    }
}
    
