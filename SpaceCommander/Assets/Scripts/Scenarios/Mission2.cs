using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;
using System.IO;

namespace SpaceCommander.Scenarios
{
    public class Mission2 : Scenario
    {
        private float fazeCounter;
        private float delay;
        protected override void Start()
        {
            base.Start();
            //fazeCounter = 0;
            delay = 1;
        }
        protected override void Update()
        {

        }
        public override int CheckVictory()
        {
            return base.CheckVictory();
        }
    }
}
