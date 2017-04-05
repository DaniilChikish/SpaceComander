using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PracticeProject
{
    public class LR_Corvette : Unit
    {
        protected override void StatsUp()
        {
            type = UnitClass.LR_Corvette;
            radarRange = 200; //set in child
            radarPover = 1;
            speed = 5; //set in child
            stealthness = 0.8f; //set in child
            radiolink = 2.5f;
            movementAiEnabled = false;
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
