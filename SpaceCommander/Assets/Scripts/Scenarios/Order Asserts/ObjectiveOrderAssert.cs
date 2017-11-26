using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpaceCommander.Scenarios
{
    class ObjectiveOrderAssert : OrderAssert
    {
        public enum Functions
        {
            OnEnable,
            OnDisable,
            OnBecameInvisible,
            OnBecameVisible,
        }
        [SerializeField]
        private Functions callOnMethod;
        private void TryToCallOrder(Functions invokingMethod)
        {
            if (invokingMethod == callOnMethod)
            {
                if (methodToCall == Method.Pass)
                    State = OrderAccertState.Complete;
                else
                    State = OrderAccertState.Fail;
            }
        }

        public void OnDisable()
        {
            TryToCallOrder(Functions.OnDisable);
        }
        public void OnEnable()
        {
            TryToCallOrder(Functions.OnEnable);
        }
        public void OnBecameInvisible()
        {
            TryToCallOrder(Functions.OnBecameInvisible);
        }
        public void OnBecameVisible()
        {
            TryToCallOrder(Functions.OnBecameVisible);
        }
    }
}
