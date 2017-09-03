using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCommander.Units
{
    public class Bait : SpaceShip
    {
        protected override void StatsUp()
        {
            type = UnitClass.LR_Corvette;
            radarRange = 300; //set in child
            radarPover = 10;
            speedThrust = 10; //set in child
            stealthness = 1f; //set in child
            radiolink = 2f;
            movementAiEnabled = false;
            combatAIEnabled = false;
            selfDefenceModuleEnabled = true;
        }

        protected override void DecrementLocalCounters()
        {
        }
    }
}
