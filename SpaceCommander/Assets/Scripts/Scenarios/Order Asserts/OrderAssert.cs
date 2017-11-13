using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpaceCommander.Scenarios
{
    public abstract class OrderAssert: MonoBehaviour, IComparable
    {
        [SerializeField]
        private OrderAccertState state;
        public OrderAccertState State { get { return state; } protected set { state = value; } }
        [SerializeField]
        private int orderNumber;
        public int OrderNumber { get { return orderNumber; } protected set { orderNumber = value; } }
        //[SerializeField]
        //private int missionID;
        //public int MissionID { get { return missionID; } protected set { missionID = value; } }
        [SerializeField]
        private bool isNecessary;
        public bool IsNecessary { get { return isNecessary; } protected set { isNecessary = value; } }

        int IComparable.CompareTo(object obj)
        {
            return orderNumber.CompareTo(obj);
        }
        public override string ToString()
        {
            return /*MissionID.ToString() + "." + */OrderNumber.ToString() + "(" + State + ")";
        }
    }
}
