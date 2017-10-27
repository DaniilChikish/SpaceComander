using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SpaceCommander
{
    public abstract class TraningScenario : Scenario
    {
        private float fazeCounter;
        private float delay;
        protected override void Start()
        {
            //File.WriteAllText("mission1.brif", "Kill them all!");
            Global = FindObjectOfType<GlobalController>();
            //Debug.Log("Scenario started");
            Name = Global.Texts("TrainingMission_name");
            string path = Application.streamingAssetsPath + "\\local";
            switch (Global.Settings.Localisation)
            {
                case Languages.Russian:
                    {
                        path += "\\rus\\training_brief_rus.xml";
                        break;
                    }
                case Languages.English:
                default:
                    {
                        path += "\\eng\\training_brief_eng.xml";
                        break;
                    }
            }
            Brief = File.ReadAllText(path);
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
