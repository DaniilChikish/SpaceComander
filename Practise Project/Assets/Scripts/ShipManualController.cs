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
        private GlobalController Global;
        private RTS_Cam.RTS_Camera folowCam;
        public bool tridimensional;
        public float mainThrust;
        public float verticalShiftThrust;
        public float horisontalShiftThrust;
        public float turnSpeed;
        public string thrustAxis = "Thrust";
        public string horizontalShiftAxis = "HorizontalShift";
        public string verticalShiftAxis = "VerticalShift";
        public string primaryWeaponAxis = "PrimaryWeapon";
        public string secondaryWeaponAxis = "SecondaryWeapon";

        private void OnEnable()
        {
            Global = FindObjectOfType<GlobalController>();
            this.gameObject.GetComponent<NavMeshAgent>().enabled = false;
            tridimensional = false;
            owner = this.gameObject.GetComponent<SpaceShip>();
            owner.Gunner.ResetAim();
            body = this.gameObject.GetComponent<Rigidbody>();
            turnSpeed = owner.Speed * 5.5f;
            folowCam = FindObjectOfType<RTS_Cam.RTS_Camera>();
            folowCam.followingSpeed = owner.Speed * 5f;
        }
        private void OnDisable()
        {
            this.gameObject.GetComponent<NavMeshAgent>().enabled = true;
        }
        private void Update()
        {
            Fire();
        }

        private void Fire()
        {
            if (Input.GetAxis(primaryWeaponAxis) != 0)
                owner.Gunner.ShootHim(0);
            if (Input.GetAxis(secondaryWeaponAxis) != 0)
                owner.Gunner.ShootHim(1);
        }
        private void OnGUI()
        {
            Vector3 crd = Camera.main.WorldToScreenPoint(transform.position + transform.forward * 100);
            crd.y = Screen.height - crd.y;
            Vector2 texSize = new Vector2(Global.aimPointTexture.width, Global.aimPointTexture.height);
            float frameX = crd.x - texSize.x / 2f;
            float frameY = crd.y - texSize.y / 2f;
            GUI.DrawTexture(new Rect(new Vector2(frameX, frameY), texSize), Global.aimPointTexture);
        }

        private void FixedUpdate()
        {
            Move();
            Rotate();
            if (!tridimensional)
                Stabilisation();
        }

        private void Move()
        {
            if (tridimensional)
                throw new NotImplementedException();
            else
            {
                float mainThrustLocal = Input.GetAxis(thrustAxis);
                if (mainThrust <= 2.5 && mainThrust >= -1)
                    mainThrust += mainThrustLocal * Time.deltaTime;

                if (mainThrustLocal == 0)
                {
                    if (mainThrust <= 0.3 && mainThrust >= -0.3)
                        mainThrust = mainThrust * 0.7f;
                    else if (mainThrust > 0.3f)
                        mainThrust -= Time.deltaTime * 0.3f;
                    else if (mainThrust < -0.3f)
                        mainThrust += Time.deltaTime * 0.5f;
                }
                if (mainThrust > 2.5) mainThrust = 2.5f;
                if (mainThrust < 0.0001 && mainThrust > -0.0001) mainThrust = 0;
                if (mainThrust < -1) mainThrust = -1;

                float horisontalShiftLocal = Input.GetAxis(horizontalShiftAxis);
                if (horisontalShiftThrust <= 1 && horisontalShiftThrust >= -1)
                    horisontalShiftThrust += horisontalShiftLocal * Time.deltaTime;

                if (horisontalShiftLocal == 0)
                {
                    if (horisontalShiftThrust <= 0.1 && horisontalShiftThrust >= -0.1)
                        horisontalShiftThrust = horisontalShiftThrust * 0.7f;
                    else if (horisontalShiftThrust > 0.1f)
                        horisontalShiftThrust -= Time.deltaTime * 0.7f;
                    else if (horisontalShiftThrust < -0.1f)
                        horisontalShiftThrust += Time.deltaTime * 0.7f;
                }
                if (horisontalShiftThrust > 1) horisontalShiftThrust = 1;
                if (horisontalShiftThrust < 0.0001 && horisontalShiftThrust > -0.0001) horisontalShiftThrust = 0;
                if (horisontalShiftThrust < -1) horisontalShiftThrust = -1;

                Vector3 shiftLocal = (this.transform.right * horisontalShiftThrust + this.transform.forward * mainThrust) * owner.Speed * 25;

                body.AddForce(shiftLocal, ForceMode.Acceleration);
            }
        }
        private void Rotate()
        {
            var rot = new Vector3(0f, 0f, 0f);
            // rotates Left
            if (Input.GetAxis("Mouse X") < 0 || Input.GetKey(KeyCode.LeftArrow))
            {
                rot.y -= 1;
            }
            // rotates Left
            if (Input.GetAxis("Mouse X") > 0 || Input.GetKey(KeyCode.RightArrow))
            {
                rot.y += 1;
            }
            if (tridimensional)
            {
                // rotates Up
                if (Input.GetAxis("Mouse Y") < 0)
                {
                    rot.x -= 1;
                }
                // rotates Down
                if (Input.GetAxis("Mouse Y") > 0)
                {
                    rot.x += 1;
                }
            }

            transform.Rotate(rot, turnSpeed * Time.deltaTime);
        }
        private void Stabilisation()
        {
            Quaternion rotDest = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            if (Quaternion.Angle(this.transform.rotation, rotDest) > 1)
                transform.rotation = Quaternion.RotateTowards(this.transform.rotation, rotDest, Time.deltaTime * turnSpeed * 2.7f);
            else this.transform.rotation = rotDest;

            Vector3 targetPos = this.transform.position;
            targetPos.y = 0;
            this.transform.position = Vector3.MoveTowards(this.transform.position, targetPos, Time.deltaTime * owner.Speed * 20);
        }
    }
}
