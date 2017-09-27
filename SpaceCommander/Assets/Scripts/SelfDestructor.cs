using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpaceCommander.Service
{
    class SelfDestructor : MonoBehaviour
    {
        public float ttl;
        void Update()
        {
            if (ttl > 0)
                ttl -= Time.deltaTime;
            else Destroy(this.gameObject);
        }
    }
}
