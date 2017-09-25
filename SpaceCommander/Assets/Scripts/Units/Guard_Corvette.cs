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
            type = UnitClass.Guard_Corvette;
            radarRange = 550; //set in child
            radarPover = 0.8f;
            speedThrust = 8; //set in child
            speedRotation = 50;
            speedShift = 9;
            stealthness = 0.2f; //set in child
            radiolink = 1f;
            EnemySortDelegate = SupportCorvetteSortEnemys;
            AlliesSortDelegate = SupportCorvetteSortEnemys;

            module = new SpellModule[1];
            module[0] = new EmergencySelfRapairing(this);
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
                return PatroolLinePerpendicularly(150);
            else return PatroolPoint();
        }
    }
}
    
