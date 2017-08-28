using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using UnityEngine.AI;
using DeusUtility.Random;
using DeusUtility.UI;


namespace SpaceCommander
{
    class ShipManualController : MonoBehaviour
    {
        private SpaceShip owner;
        private Rigidbody body;
        public bool tridimensional;
        public float trust;
        public string thrustAxis = "Thrust";
        public string horizontalShiftAxis = "HorizontalShift";
        public string verticalShiftAxis = "VerticalShift";
        public string primaryWeaponAxis = "PrimaryWeapon";
        public string secondaryWeaponAxis = "SecondaryWeapon";

        private void OnEnable()
        {
            this.gameObject.GetComponent<NavMeshAgent>().enabled = false;
            tridimensional = false;
            owner = this.gameObject.GetComponent<SpaceShip>();
            body = this.gameObject.GetComponent<Rigidbody>();
        }
        private void OnDisable()
        {
            this.gameObject.GetComponent<NavMeshAgent>().enabled = true;
        }
        private void Update()
        {
        }
        private void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            if (tridimensional)
                throw new NotImplementedException();
            else
            {
                float thrustLocal = Input.GetAxis(thrustAxis);
                if (trust <= 2.5 && trust >= -1)
                    trust += thrustLocal * Time.deltaTime;

                if (thrustLocal == 0)
                {
                    if (trust < 0.5 && trust > -0.5)
                        trust = trust * 0.7f;
                    else if (trust > 0.5f)
                        trust -= Time.deltaTime * 0.3f;
                    else if (trust < -0.5f)
                        trust += Time.deltaTime * 0.5f;
                }
                if (trust > 2.5) trust = 2.5f;
                if (trust < 0.0001 && trust > -0.0001) trust = 0;
                if (trust < -1) trust = -1;

                Vector3 shiftLocal = (this.transform.right * Input.GetAxis(horizontalShiftAxis) + this.transform.forward * trust) * owner.Speed * 25;

                body.AddForce(shiftLocal, ForceMode.Acceleration);
            }
        }
    }
}
