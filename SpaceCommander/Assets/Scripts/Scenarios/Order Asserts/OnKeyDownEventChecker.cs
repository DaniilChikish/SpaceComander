using System;
using UnityEngine;

namespace SpaceCommander.Mechanics
{
    class OnKeyDownEventChecker : EventChecker
    {
        [SerializeField]
        KeyCode key;
        private void Update()
        {
            Check();
        }
        public override bool Check()
        {
            if (previous == null || (previous != null && previous.Occured))
                if (Input.GetKeyDown(key) || (key == KeyCode.None && Input.anyKeyDown))
                {
                    this.Occured = true;
                    return true;
                }
            return false;
        }
    }
}
