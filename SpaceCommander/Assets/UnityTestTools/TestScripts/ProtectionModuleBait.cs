using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCommander.Test
{
    public class ProtectionModuleBait : AI.SpaceShip
    {
        protected override void StatsUp()
        {
            type = Mechanics.UnitClass.Scout;
            radarRange = 3000; //set in child
            radarPover = 10;
            speedThrust = 12; //set in child
            speedRotation = 14;
            speedShift = 12;
            stealthness = 0.1f; //set in child
            radiolink = 2f;
            movementAiEnabled = false;
            combatAIEnabled = false;
            selfDefenceModuleEnabled = true;
            module = new Mechanics.SpellModule[1];
            module[0] = new Mechanics.Modules.ProtectionMatrix(this);
        }

        protected override void DecrementLocalCounters()
        {
        }
    }
}
