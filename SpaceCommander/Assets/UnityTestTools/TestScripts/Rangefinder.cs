using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpaceCommander.Test
{
    class Rangefinder : MonoBehaviour
    {
        SpaceShip owner;
        public float distance;
        private void Start()
        {
            owner = GetComponent<SpaceShip>();
        }
        private void Update()
        {
            if (owner.CurrentTarget != null)
                distance = Vector3.Distance(this.transform.position, owner.CurrentTarget.transform.position);
            else distance = float.NaN;
        }
    }
}
