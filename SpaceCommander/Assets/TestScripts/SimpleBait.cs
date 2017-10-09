using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCommander.Units
{
    public class SimpleBait : SpaceShip
    {
        protected override void StatsUp()
        {
            type = UnitClass.Scout;
            radarRange = 3000; //set in child
            radarPover = 10;
            speedThrust = 12; //set in child
            speedRotation = 14;
            speedShift = 12;
            stealthness = 0.1f; //set in child
            radiolink = 2f;
            armor.maxHitpoints = 100;
            armor.hitpoints = armor.maxHitpoints;
            shield.maxCampacity = 100;
            shield.force = shield.maxCampacity;
            movementAiEnabled = false;
            combatAIEnabled = false;
            selfDefenceModuleEnabled = false;
            module = new SpellModule[1];
            module[0] = new MissileTrapLauncher(this);
        }

        protected override void DecrementLocalCounters()
        {
        }
    }
}
