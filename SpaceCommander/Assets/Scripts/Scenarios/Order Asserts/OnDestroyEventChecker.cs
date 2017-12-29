using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpaceCommander.Mechanics
{
    public abstract class OnDestroyEventChecker : EventChecker
    {
        public GameObject[] accertionObjects;
        public override bool Check()
        {
            if (previous == null || (previous != null && previous.Occured))
                if (!CheckExists())
                {
                    this.Occured = true;
                    return true;
                }
            return false;
        }
        protected void LateUpdate()
        {
            Check();
        }
        protected abstract bool CheckExists();
    }


}
