using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpaceCommander.Scenarios
{
    class ColliderOrderAssert : OrderAssert
    {
        public enum Functions
        {
            OnControllerColliderHit,
            OnParticleCollision,
            OnJointBreak,
            OnTriggerEnter,
            OnTriggerExit,
            OnTriggerStay,
            OnCollisionEnter,
            OnCollisionExit,
            OnCollisionStay,
            OnTriggerEnter2D,
            OnTriggerExit2D,
            OnTriggerStay2D,
            OnCollisionEnter2D,
            OnCollisionExit2D,
            OnCollisionStay2D,
        }
        public enum Method
        {
            Pass,
            Fail
        }

        [SerializeField]
        private Functions callOnMethod;
        [SerializeField]
        private Method methodToCall;

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

        public void OnControllerColliderHit()
        {
            TryToCallOrder(Functions.OnControllerColliderHit);
        }
        public void OnParticleCollision()
        {
            TryToCallOrder(Functions.OnParticleCollision);
        }
        public void OnJointBreak()
        {
            TryToCallOrder(Functions.OnJointBreak);
        }
        public void OnTriggerEnter()
        {
            TryToCallOrder(Functions.OnTriggerEnter);
        }
        public void OnTriggerExit()
        {
            TryToCallOrder(Functions.OnTriggerExit);
        }
        public void OnTriggerStay()
        {
            TryToCallOrder(Functions.OnTriggerStay);
        }
        public void OnCollisionEnter()
        {
            TryToCallOrder(Functions.OnCollisionEnter);
        }
        public void OnCollisionExit()
        {
            TryToCallOrder(Functions.OnCollisionExit);
        }
        public void OnCollisionStay()
        {
            TryToCallOrder(Functions.OnCollisionStay);
        }
        public void OnTriggerEnter2D()
        {
            TryToCallOrder(Functions.OnTriggerEnter2D);
        }
        public void OnTriggerExit2D()
        {
            TryToCallOrder(Functions.OnTriggerExit2D);
        }
        public void OnTriggerStay2D()
        {
            TryToCallOrder(Functions.OnTriggerStay2D);
        }
        public void OnCollisionEnter2D()
        {
            TryToCallOrder(Functions.OnCollisionEnter2D);
        }
        public void OnCollisionExit2D()
        {
            TryToCallOrder(Functions.OnCollisionExit2D);
        }
        public void OnCollisionStay2D()
        {
            TryToCallOrder(Functions.OnCollisionStay2D);
        }
    }
}
