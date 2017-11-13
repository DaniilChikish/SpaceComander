using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpaceCommander.Scenarios
{
    class OnDestroyOrderAssert : OrderAssert
    {
        public enum Method
        {
            Pass,
            Fail
        }
        public GameObject accertionObject;
        [SerializeField]
        private Method methodToCall;

        private void TryToCallOrder()
        {
            if (methodToCall == Method.Pass)
                State = OrderAccertState.Complete;
            else
                State = OrderAccertState.Fail;
        }
        private void LateUpdate()
        {
            if (!CheckExists())
                TryToCallOrder();
        }
        private bool CheckExists()
        {
            return (accertionObject != null);
        }
    }


}
