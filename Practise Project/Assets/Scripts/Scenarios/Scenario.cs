using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCommander
{
    public abstract class Scenario : MonoBehaviour
    {
        protected GlobalController Global;
        public string Name;
        public string Brief;
        // Use this for initialization
        protected abstract void Start();

        // Update is called once per frame
        protected abstract void Update();
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
