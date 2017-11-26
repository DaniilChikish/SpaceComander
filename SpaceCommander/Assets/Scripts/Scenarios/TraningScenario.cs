using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SpaceCommander.Scenarios
{
    public abstract class TraningScenario : Scenario
    {
        private float fazeCounter;
        private float delay;
        protected override void Start()
        {
            base.Start();
            //fazeCounter = 0;
            delay = 1;
        }
        public override int CheckVictory()
        {
            return 0;
        }
    }
}
