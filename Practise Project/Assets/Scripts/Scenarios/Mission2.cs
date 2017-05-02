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
    public class Mission2 : Scenario
    {
        private float fazeCounter;
        private float delay;
        protected override void Start()
        {
            //File.WriteAllText("mission1.brif", "Kill them all!");
            Global = FindObjectOfType<GlobalController>();
            //Debug.Log("Scenario started");
            Name = Global.Texts["Mision2_name"];
            string path = Application.streamingAssetsPath + "\\local";
            switch (Global.Localisation)
            {
                case Languages.English:
                    {
                        path += "\\eng\\mission2_brief_eng.xml";

                        break;
                    }
                case Languages.Russian:
                    {
                        path += "\\rus\\mission2_brief_rus.xml";
                        break;
                    }
            }
            Brief = File.ReadAllText(path);
            //fazeCounter = 0;
            delay = 1;
        }
        protected override void Update()
        {
            if (delay == 1)
            {
                Move();
                delay = 0;
            }
            //if (fazeCounter <= 0)
            //{
            //    Move();
            //    fazeCounter = 40;
            //}
            //else fazeCounter -= Time.deltaTime;
        }
        public override int CheckVictory()
        {
            int alies = 0;
            int enemy = 0;
            int left = 0;
            foreach (IUnit x in Global.unitList)
            {
                if (x.Team == Global.playerArmy)
                    alies++;
                else
                {
                    enemy++;
                    if (x.ObjectTransform.position.z < 0)
                        left++;
                }
            }
            if (left >= 3)
                return -1;
            else if (enemy == 0)
                return 1;
            else if (alies == 0)
                return -1;
            else return 0;
        }

        private void Move()
        {
            foreach (SpaceShip x in Global.unitList)
            {
                if (x.Team != Global.playerArmy)
                {
                    x.Anchor = x.Anchor + new Vector3(0, 0, -2000);
                }
            }
        }
    }
}
