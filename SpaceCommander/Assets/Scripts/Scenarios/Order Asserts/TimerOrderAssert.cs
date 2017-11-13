using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpaceCommander.Scenarios
{
    class TimerOrderAssert : OrderAssert
    {
        public enum Method
        {
            Pass,  //не провалить дольше указанного срока
            Fail   //выполнить быстрее указанного срока
        }
        [SerializeField]
        private Method methodToCall;

        public OrderAssert dependOn;

        [SerializeField]
        private float counter;
        public float Counter { get { return counter; } }

        private void TryToCallOrder()
        {
            if (counter <= 0)
            {
                if (dependOn != null)
                {
                    if (methodToCall == Method.Fail)
                    {
                        if (dependOn.State != OrderAccertState.Complete)
                            State = OrderAccertState.Fail;
                        else State = OrderAccertState.Complete;
                    }
                    else
                    {
                        if (dependOn.State != OrderAccertState.Fail)
                            State = OrderAccertState.Complete;
                    }
                }
                else
                {
                    if (methodToCall == Method.Pass)
                        State = OrderAccertState.Complete;
                    else
                        State = OrderAccertState.Fail;
                }
            }
            else
            {
                counter -= Time.deltaTime;
                if (dependOn != null)
                {
                    if (methodToCall == Method.Fail)
                        State = dependOn.State;
                    else if (dependOn.State == OrderAccertState.Fail)
                        State = OrderAccertState.Fail;
                }
            }
        }

        public void Start()
        {
        }
        public void LateUpdate()
        {
            TryToCallOrder();
        }

    }
}
