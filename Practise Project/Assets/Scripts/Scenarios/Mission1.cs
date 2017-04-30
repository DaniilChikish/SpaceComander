using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace PracticeProject
{
    public class Mission1 : Scenario
    {
        public GameObject WarpGate1;
        public GameObject WarpGate2;
        public GameObject WarpGate3;
        protected override void Start()
        {
            //File.WriteAllText("mission1.brif", "Kill them all!");
            Global = FindObjectOfType<GlobalController>();
            //Debug.Log("Scenario started");
            Name = "Mission 1: \"Find and destroy\"";
            Brief = File.ReadAllText("mission1.brif");
        }
        protected override void Update()
        {

        }
        public override int CheckVictory()
        {
            int alies = 0;
            int enemy = 0;
            foreach (IUnit x in Global.unitList)
            {
                if (x.Team == Global.playerArmy)
                    alies++;
                else enemy++;
            }
            if (enemy == 0)
                return 1;
            else if (alies == 0)
            {
                GetHelp();
                return 0;
            }
            else return 0;
        }

        private void GetHelp()
        {
            WarpGate1.GetComponent<WarpArrive>().Arrive();
            WarpGate2.GetComponent<WarpArrive>().Arrive();
            WarpGate3.GetComponent<WarpArrive>().Arrive();
        }
    }
}
