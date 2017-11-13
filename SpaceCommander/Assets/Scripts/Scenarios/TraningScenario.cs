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
        protected override void Update()
        {
            //if (fazeCounter <= 0)
            //{
            //    Move();
            //    fazeCounter = 40;
            //}
            //else fazeCounter -= Time.deltaTime;
        }
        public override int CheckVictory()
        {
            return 0;
        }
    }
}
