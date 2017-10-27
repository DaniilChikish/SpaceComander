using SpaceCommander;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCommander.Test
{
    public class GunMachine : SpaceShip
    {
        protected override void StatsUp()
        {
            type = UnitClass.LR_Corvette;
            radarRange = 2500; //set in child
            radarPover = 10;
            speedThrust = 10; //set in child
            speedShift = 10;
            speedRotation = 30;
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
