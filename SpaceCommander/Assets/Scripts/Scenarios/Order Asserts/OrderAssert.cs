using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpaceCommander.Mechanics;
namespace SpaceCommander.Scenarios
{
    public enum Method { Pass, Fail }

    public class OrderAssert: MonoBehaviour
    {
        [SerializeField]
        private OrderAccertState state;
        public OrderAccertState State { get { return state; } protected set { state = value; } }
        [SerializeField]
        private int priority;
        public int Priority { get { return priority; } protected set { priority = value; } }
        //[SerializeField]
        //protected Method methodToCall;
        //[SerializeField]
        //private int missionID;
        //public int MissionID { get { return missionID; } protected set { missionID = value; } }
        [SerializeField]
        private bool isNecessary;
        public bool IsNecessary { get { return isNecessary; } protected set { isNecessary = value; } }

        [SerializeField]
        public EventChecker passIf;
        [SerializeField]
        public EventChecker failIf;
        [SerializeField]
        private int checkRate = 10; //per second
        private float counter;
        private void LateUpdate()
        {
            if (counter <= 0)
            {
                counter = checkRate;
                if (passIf != null && passIf.Occured) State = OrderAccertState.Complete;
                else if (failIf != null && failIf.Occured) State = OrderAccertState.Fail;
            }
            else counter -= Time.deltaTime;
        }
        public override string ToString()
        {
            return /*MissionID.ToString() + "." + */Priority.ToString() + "(" + State + ")";
        }
    }
}
