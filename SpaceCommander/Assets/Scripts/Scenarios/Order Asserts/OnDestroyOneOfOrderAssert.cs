using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpaceCommander.Scenarios
{
    class OnDestroyOneOfOrderAssert : OrderAssert
    {
        public GameObject[] accertionObjects;

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
            for (int i = 0; i < accertionObjects.Length; i++)
                if (accertionObjects[i] == null)
                    return false;
            return true;
        }
    }

}
