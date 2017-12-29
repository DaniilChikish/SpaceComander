using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;
using System.IO;
using SpaceCommander.Mechanics;
using SpaceCommander.AI;

namespace SpaceCommander.Scenarios
{
    public class Mission2 : Scenario
    {
        [SerializeField]
        private Mechanics.Units.Cargoship cargo;
        [SerializeField]
        private Vector3 ancor;
        [SerializeField]
        private EventChecker enemyArriveTrigger;
        [SerializeField]
        private WarpArrive[] EnemyPack;
        public bool enemyArrived;
        [SerializeField]
        private EventChecker alliesArriveTrigger;
        [SerializeField]
        private WarpArrive[] AlliesPack;
        public bool alliesArrived;
        protected override void Start()
        {
            base.Start();
            cargo.Anchor = ancor;
        }
        private void Update()
        {
            if (!enemyArrived&&enemyArriveTrigger.Occured)
            {
                enemyArrived = true;
                Arrive(EnemyPack);
            }
            if (!alliesArrived && alliesArriveTrigger.Occured)
            {
                alliesArrived = true;
                Arrive(AlliesPack);
            }
        }
        public void Arrive(WarpArrive[] warp)
        {
            foreach (WarpArrive x in warp)
                x.Arrive();
        }
    }
}
