using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpaceCommander.Mechanics
{
    class OnDestroyAllOfEventChecker : OnDestroyEventChecker
    {
        protected override bool CheckExists()
        {
            for (int i = 0; i < accertionObjects.Length; i++)
                if (accertionObjects[i] != null)
                    return true;
            return false;
        }
    }

}
