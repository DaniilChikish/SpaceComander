using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PracticeProject
{
    public abstract class Scenario : MonoBehaviour
    {
        private GlobalController Global;
        public string Name;
        public string Brief;
        // Use this for initialization
        protected virtual void Start()
        {
            Global = FindObjectOfType<GlobalController>();
            //Debug.Log("Scenario started");
        }

        // Update is called once per frame
        void Update()
        {

        }
        public virtual int CheckVictory()
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
                return -1;
            else return 0;
        }
    }
}
