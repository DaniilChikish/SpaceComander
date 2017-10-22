using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpaceCommander.Service
{
    class Speedometer : MonoBehaviour
    {
        public Vector3 velocity;
        public float mainSpeed;
        public float verticalSpeed;
        public float horisontalSpeed;
        private Rigidbody body;
        private void Start()
        {
            body = this.transform.GetComponent<Rigidbody>();
        }
        void Update()
        {
            velocity = body.velocity;
            float sign;
            if (Vector3.Angle(body.velocity, body.transform.forward) < 90)
                sign = 1;
            else sign = -1;
            mainSpeed = Vector3.Project(body.velocity, body.transform.forward).magnitude * sign;

            if (Vector3.Angle(body.velocity, body.transform.up) < 90)
                sign = 1;
            else sign = -1;
            verticalSpeed = Vector3.Project(body.velocity, body.transform.up).magnitude * sign;

            if (Vector3.Angle(body.velocity, body.transform.right) < 90)
                sign = 1;
            else sign = -1;
            horisontalSpeed = Vector3.Project(body.velocity, body.transform.right).magnitude * sign;
        }
    }
}
