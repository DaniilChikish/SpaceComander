using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpaceCommander.Mechanics
{
    class TimerEventChecker : EventChecker
    {
        [SerializeField]
        private float counter;
        public float Counter { get { return counter; } }

        //private void TryToCallOrder()
        //{
        //    if (counter <= 0)
        //    {
        //        if (dependOn != null)
        //        {
        //            if (methodToCall == Method.Fail)
        //            {
        //                if (dependOn.State != OrderAccertState.Complete)
        //                    State = OrderAccertState.Fail;
        //                else State = OrderAccertState.Complete;
        //            }
        //            else
        //            {
        //                if (dependOn.State != OrderAccertState.Fail)
        //                    State = OrderAccertState.Complete;
        //            }
        //        }
        //        else
        //        {
        //            if (methodToCall == Method.Pass)
        //                State = OrderAccertState.Complete;
        //            else
        //                State = OrderAccertState.Fail;
        //        }
        //    }
        //    else
        //    {
        //        counter -= Time.deltaTime;
        //        if (dependOn != null)
        //        {
        //            if (methodToCall == Method.Fail)
        //                State = dependOn.State;
        //            else if (dependOn.State == OrderAccertState.Fail)
        //                State = OrderAccertState.Fail;
        //        }
        //    }
        //}
        public override bool Check()
        {
            if (previous==null||(previous != null && previous.Occured))
                if (counter <= 0)
                {
                    this.Occured = true;
                    return true;
                }
                else
                    counter -= Time.deltaTime;
            return false;
        }
        public void Start()
        {
        }
        public void LateUpdate()
        {
            Check();
        }

    }
}
