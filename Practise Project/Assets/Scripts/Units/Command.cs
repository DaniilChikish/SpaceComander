using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeusUtility.Random;

namespace SpaceCommander.Units
{
    public class Command : SpaceShip
    {
        public bool jamming;//Make private after debug;
        public new float Stealthness { get { if (jamming) return stealthness * 1.8f; else return stealthness; } }

        protected override void StatsUp()
        {
            type = UnitClass.Command;
            radarRange = 500; //set in child
            radarPover = 0.8f;
            speed = 12; //set in child
            combatAIEnabled = true; //set in child
            selfDefenceModuleEnabled = true; //set in child
            jamming = false;
            stealthness = 0.45f; //set in child
        }
        protected override void DecrementLocalCounters()
        {
        }
        //AI logick
        //protected override bool ManeuverFunction()
        //{
        //    return false;
        //}
    }
}
