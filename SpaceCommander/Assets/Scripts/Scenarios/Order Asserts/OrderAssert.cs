using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpaceCommander.Scenarios
{
    public enum Method { Pass, Fail }

    public abstract class OrderAssert: MonoBehaviour
    {
        [SerializeField]
        private OrderAccertState state;
        public OrderAccertState State { get { return state; } protected set { state = value; } }
        [SerializeField]
        private int priority;
        public int Priority { get { return priority; } protected set { priority = value; } }
        [SerializeField]
        protected Method methodToCall;
        //[SerializeField]
        //private int missionID;
        //public int MissionID { get { return missionID; } protected set { missionID = value; } }
        [SerializeField]
        private bool isNecessary;
        public bool IsNecessary { get { return isNecessary; } protected set { isNecessary = value; } }
        public override string ToString()
        {
            return /*MissionID.ToString() + "." + */Priority.ToString() + "(" + State + ")";
        }
    }
}
