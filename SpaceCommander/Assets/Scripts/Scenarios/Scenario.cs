using SpaceCommander.General;
using SpaceCommander.Mechanics;
using SpaceCommander.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SpaceCommander.Scenarios
{
    public enum OrderAccertState { InProgress, Complete, Fail }
    public abstract class Scenario : MonoBehaviour
    {
        [SerializeField]
        private string missionID;
        public string MissionID { get { return missionID; } protected set { missionID = value; } }
        public string Name { get; protected set; }
        private string missionBrief;
        private string[] orderBrief;
        public string Brief {
            get
            {
                StringBuilder outp = new StringBuilder();
                GlobalController Global = GlobalController.Instance;
                outp.Append(missionBrief);
                outp.Append("\n\r" + Global.Texts("Orders") + ":");
                for (int i = 0; i < orders.Length; i++)
                {
                    outp.Append("\n\r" + (i+1) + ". " + orderBrief[i]);
                    if (orders[i].GetType() == typeof(TimerOrderAssert) && orders[i].State == OrderAccertState.InProgress)
                        outp.Append("(" + Math.Round(((TimerOrderAssert)orders[i]).Counter / 60, 2) + "min. left)");
                    else
                        outp.Append("(" + Global.Texts(orders[i].State.ToString()) + ")");
                    if (!orders[i].IsNecessary)
                        outp.Append("(" + Global.Texts("Secondary") + ")");
                }
                return outp.ToString();
            }
        }
        public bool useDefault;
        public bool defaultHavePriority;
        public OrderAssert[] orders;
        // Use this for initialization
        protected virtual void Start()
        {
            MissionID = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            orders = FindObjectsOfType<OrderAssert>();
            Array.Sort(orders);
            {
                GlobalController Global = GlobalController.Instance;
                //Debug.Log("Scenario started");
                Name = Global.Texts(MissionID);
                string path = Application.streamingAssetsPath + "/missions/" + MissionID + ".dat";
                INIHandler reader = new INIHandler(path);
                missionBrief = reader.ReadINI("Text." + Global.Settings.Localisation.ToString(), "missionBrief", 1024);
                missionBrief = missionBrief.Replace("\\r\\n", ((char)13).ToString() + ((char)10).ToString());
                orderBrief = new string[orders.Length];
                for (int i = 0; i < orders.Length; i++)
                    orderBrief[i] = reader.ReadINI("Text." + Global.Settings.Localisation.ToString(), ("order_" + orders[i].OrderNumber), 1024);
            }
        }

        // Update is called once per frame
        protected abstract void Update();
        public virtual int CheckVictory()
        {
            if (useDefault && defaultHavePriority || (orders.Length == 0))
                return DefaultOrder();
            else
            {
                int completeCount = 0;
                int necessaryCount = 0;
                for (int i = 0; i < orders.Length; i++)
                {
                    if (orders[i].IsNecessary)
                    {
                        necessaryCount++;
                        if (orders[i].State == OrderAccertState.Fail)
                            return -1;
                        else if (orders[i].State == OrderAccertState.Complete)
                            completeCount++;
                    }
                }
                if (completeCount == necessaryCount)
                    return 1;
                else if (useDefault && DefaultOrder() == -1)
                    return -1;
                else return 0;
            }
        }
        public static int DefaultOrder()
        {
            int alies = 0;
            int enemy = 0;
            foreach (Unit x in GlobalController.Instance.unitList)
            {
                if (x.Team == GlobalController.Instance.playerArmy)
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
