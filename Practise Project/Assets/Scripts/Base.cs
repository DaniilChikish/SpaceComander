using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PracticeProject
{
    class Base : MonoBehaviour
    {
        public Army team;
        public Vector3 GetDock()
        {
            return this.transform.position;
        }
    }
}
