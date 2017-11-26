using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;
using System.IO;

namespace SpaceCommander.Scenarios
{
    public class Mission4 : Scenario
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
            return base.CheckVictory();
        }
    }
}
