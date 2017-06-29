using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCommander
{
    public class GunMachine : SpaceShip
    {
        protected override void StatsUp()
        {
            type = UnitClass.LR_Corvette;
            radarRange = 500; //set in child
            radarPover = 10;
            speed = 100; //set in child
            stealthness = 0f; //set in child
            radiolink = 5f;
            movementAiEnabled = false;
            selfDefenceModuleEnabled = false;
            roleModuleEnabled = false;
        }
        protected override void DecrementCounters()
        {
        }
        protected override bool RoleFunction()
        {
            return false;
        }
        protected override bool SelfDefenceFunction()
        {
            SelfDefenceFunctionBase();
            return true;
        }
    }
}
