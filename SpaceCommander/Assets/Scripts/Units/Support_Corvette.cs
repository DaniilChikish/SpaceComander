using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpaceCommander.Units
{
    public class Support_Corvette : SpaceShip
    {

        private bool idleFulag;
        protected override void StatsUp()
        {
            type = UnitClass.Support_Corvette;
            radarRange = 500; //set in child
            radarPover = 0.6f;
            speedThrust = 7; //set in child
            speedRotation = 6;
            speedShift = 6;
            stealthness = 0.1f; //set in child
            radiolink = 1f;
            EnemySortDelegate = SupportCorvetteSortEnemys;
            AlliesSortDelegate = SupportCorvetteSortEnemys;
            module = new SpellModule[2];
            module[0] = new EmergencySelfRapairing(this);
            module[1] = new EmergencyShieldRecharging(this);
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
