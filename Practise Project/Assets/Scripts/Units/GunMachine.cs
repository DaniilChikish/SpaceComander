using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCommander.Units
{
    public class GunMachine : SpaceShip
    {
        protected override void StatsUp()
        {
            type = UnitClass.LR_Corvette;
            radarRange = 500; //set in child
            radarPover = 10;
            speed = 10; //set in child
            stealthness = 1f; //set in child
            radiolink = 5f;
            movementAiEnabled = false;
            selfDefenceModuleEnabled = false;
        }
        protected override void DecrementLocalCounters()
        {
        }
    }
}
