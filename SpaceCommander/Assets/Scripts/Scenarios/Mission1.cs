using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;
using System.IO;

namespace SpaceCommander.Scenarios
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
            Name = Global.Texts("Mision1_name");
            string path = Application.streamingAssetsPath + "\\local";
            switch (Global.Settings.Localisation)
            {
                case Languages.Russian:
                    {
                        path += "\\rus\\mission1_brief_rus.xml";
                        break;
                    }
                case Languages.English:
                default:
                    {
                        path += "\\eng\\mission1_brief_eng.xml";

                        break;
                    }
            }
            Brief = File.ReadAllText(path);
        }
        protected override void Update()
        {

        }
        public override int CheckVictory()
        {
            int alies = 0;
            int enemy = 0;
            foreach (Unit x in Global.unitList)
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
