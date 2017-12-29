using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpaceCommander.Mechanics
{
    class ObjectiveEventChecker : EventChecker
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
        private void TryToCall(Functions invokingMethod)
        {
            if (invokingMethod == callOnMethod)
                Check();
        }
        public override bool Check()
        {
            if (previous == null || (previous != null && previous.Occured))
            {
                this.Occured = true;
                return true;
            }
            return false;
        }
        public void OnDisable()
        {
            TryToCall(Functions.OnDisable);
        }
        public void OnEnable()
        {
            TryToCall(Functions.OnEnable);
        }
        public void OnBecameInvisible()
        {
            TryToCall(Functions.OnBecameInvisible);
        }
        public void OnBecameVisible()
        {
            TryToCall(Functions.OnBecameVisible);
        }
    }
}
