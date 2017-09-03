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
        public float guiTexOffsetY;
        public SpaceShip owner;
        private Rigidbody body;
        private GlobalController Global;
        public enum AimStateType { Default, Locking, Locked}
        private const float lockdownDuration = 1.5f;
        private float lockdownCount;
        private Unit targetBuffer;
        public Unit TargetBuffer
        {
            set
            {
                targetBuffer = value;
                if (value != null)
                {
                    lockdownCount = lockdownDuration;
                    aimState = AimStateType.Locking;
                }
                else aimState = AimStateType.Default;
            }
            get { return targetBuffer; }
        }
        private int targetQinuePosition;
        public Texture DefaultMainAim;
        public Texture TargetFrame;
        public Texture LockingAimFirst;
        public Texture LockingAimSecond;
        private bool firstLookingTexture;
        private float lockingTextureSwichCoutn;
        public Texture LockedAim;
        public Texture TargetDott;
        public Texture FireAim;
        //private RTS_Cam.RTS_Camera folowCam;
        public AimStateType aimState;

        public bool tridimensional;
        private float mainThrust;
        private float verticalShiftThrust;
        private float horisontalShiftThrust;
        //controlAxis;
        private string thrustAxis = "Thrust";
        private string horizontalShiftAxis = "HorizontalShift";
        private string verticalShiftAxis = "VerticalShift";
        private string pitchAxis = "Pitch";
        private string yawAxis = "Yaw";
        private string rollAxis = "Roll";
        private string primaryWeaponAxis = "PrimaryWeapon";
        private string secondaryWeaponAxis = "SecondaryWeapon";
        private string lockTargetAxis = "LockTarget";
        private string swithTargetAxis = "SwitchTarget";

        private void OnEnable()
        {
            Global = FindObjectOfType<GlobalController>();
            owner.gameObject.GetComponent<NavMeshAgent>().enabled = false;
            tridimensional = false;
            owner.Gunner.ResetAim();
            body = owner.gameObject.GetComponent<Rigidbody>();
            //folowCam = FindObjectOfType<RTS_Cam.RTS_Camera>();
            //folowCam.followingSpeed = owner.Speed * 5f;
        }
        private void OnDisable()
        {
            owner.gameObject.GetComponent<NavMeshAgent>().enabled = true;
            TargetBuffer = null;
        }
        private void Update()
        {
            Fire();
            ActivateModule();
            TagetLockdown();
        }


        private void Fire()
        {
            if (Input.GetAxis(primaryWeaponAxis) != 0)
                owner.Gunner.ShootHim(0);
            if (Input.GetAxis(secondaryWeaponAxis) != 0)
                owner.Gunner.ShootHim(1);
        }

        private void TagetLockdown()
        {
            if (Input.GetAxis(lockTargetAxis) > 0 && (lockdownCount < lockdownDuration * 0.8))//lock target
            {
                //if (aimState == AimStateType.Default) 
                //{
                Vector3 enemyPos;
                Vector3 aim = Camera.main.WorldToScreenPoint(owner.transform.position + owner.transform.forward * (owner.Gunner.Weapon[0][0].Range + owner.Gunner.Weapon[1][0].Range) / 2);
                aim.z = 0;
                float minDistance = Screen.width / 4;
                int enemyIndex = -1;
                Unit[] enemys = owner.GetEnemys();
                float dist;
                for (int i = 0; i < enemys.Length; i++)
                {
                    enemyPos = Camera.main.WorldToScreenPoint(enemys[i].transform.position);
                    enemyPos.z = 0;
                    dist = Vector3.Distance(aim, enemyPos);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        enemyIndex = i;
                    }
                }
                if (enemyIndex != -1)
                    TargetBuffer = enemys[enemyIndex];
                else //reset target
                {
                    aimState = AimStateType.Default;
                    owner.Gunner.ResetAim();
                }
            }
            else if (Input.GetAxis(swithTargetAxis) > 0 && (lockdownCount < lockdownDuration * 0.8))
            {
                Unit[] enemys = owner.GetEnemys();
                if (enemys.Length > 0)
                {
                    targetQinuePosition++;
                    if (targetQinuePosition >= enemys.Length)
                        targetQinuePosition = 0;
                    TargetBuffer = enemys[targetQinuePosition];
                }
            }

            switch (aimState)
            {
                case AimStateType.Default:
                    {
                        break;
                    }
                case AimStateType.Locked:
                    {
                        if (owner.Gunner.Target != TargetBuffer)
                            owner.Gunner.SetAim(TargetBuffer, true, 0);
                        else if (!TargetInSight())
                        {
                            TargetBuffer = null;
                            owner.Gunner.ResetAim();
                        }
                        break;
                    }
                case AimStateType.Locking:
                    {
                        if (lockdownCount <= 0)
                        {
                            aimState = AimStateType.Locked;
                        }
                        else lockdownCount -= Time.deltaTime;

                        if (lockingTextureSwichCoutn <= 0)
                        {
                            firstLookingTexture = !firstLookingTexture;
                            lockingTextureSwichCoutn = (lockdownCount / lockdownDuration) * 0.5f;
                        }
                        else lockingTextureSwichCoutn -= Time.deltaTime;
                        break;
                    }
            }

        }
        private bool TargetInSight()
        {
            if (TargetBuffer == null)
                return false;
            float distance = Vector3.Distance(owner.transform.position, TargetBuffer.transform.position);
            if (distance < owner.RadarRange)
                return true;
            else
            {
                if (!owner.TargetScouting())
                    return false;//переходим в ожидение
                else return true;
            }
        }

        private void ActivateModule()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && owner.Module[0] != null && owner.Module[0].State == SpellModuleState.Ready)
                owner.Module[0].EnableIfReady();
            if (Input.GetKeyDown(KeyCode.Alpha2) && owner.Module[1] != null && owner.Module[1].State == SpellModuleState.Ready)
                owner.Module[1].EnableIfReady();
            if (Input.GetKeyDown(KeyCode.Alpha3) && owner.Module[2] != null && owner.Module[2].State == SpellModuleState.Ready)
                owner.Module[2].EnableIfReady();
            if (Input.GetKeyDown(KeyCode.Alpha4) && owner.Module[3] != null && owner.Module[3].State == SpellModuleState.Ready)
                owner.Module[3].EnableIfReady();
            if (Input.GetKeyDown(KeyCode.Alpha5) && owner.Module[4] != null && owner.Module[4].State == SpellModuleState.Ready)
                owner.Module[4].EnableIfReady();
        }
        private void OnGUI()
        {
            //mainAimPoint
            Vector3 crd = Camera.main.WorldToScreenPoint(owner.transform.position + owner.transform.forward * (owner.Gunner.Weapon[0][0].Range + owner.Gunner.Weapon[1][0].Range) / 2);
            crd.y = Screen.height - crd.y;
            Vector2 texSize = new Vector2(DefaultMainAim.width, DefaultMainAim.height);
            float texXPos = crd.x - texSize.x / 2f;
            float texYPos = crd.y - texSize.y / 2f + guiTexOffsetY;
            Texture aimTexture = DefaultMainAim;

            switch (aimState)
            {
                case AimStateType.Default:
                    {
                        aimTexture = DefaultMainAim;
                        break;
                    }
                case AimStateType.Locked:
                    {
                        aimTexture = LockedAim;
                        break;
                    }
                case AimStateType.Locking:
                    {
                        if (firstLookingTexture)
                            aimTexture = LockingAimFirst;
                        else
                            aimTexture = LockingAimSecond;
                        break;
                    }
            }
            GUI.DrawTexture(new Rect(new Vector2(texXPos, texYPos), texSize), aimTexture);

            //current target point
            if (TargetBuffer != null)
            {
                crd = Camera.main.WorldToScreenPoint(TargetBuffer.transform.position);
                crd.y = Screen.height - crd.y;
                texSize = new Vector2(TargetDott.width, TargetDott.height);
                texXPos = crd.x - texSize.x / 2f;
                texYPos = crd.y - texSize.y / 2f + guiTexOffsetY;
                if (aimState == AimStateType.Locked)
                    GUI.DrawTexture(new Rect(new Vector2(texXPos, texYPos), texSize), FireAim);
                else
                    GUI.DrawTexture(new Rect(new Vector2(texXPos, texYPos), texSize), TargetDott);

            }

            //locked Aim
            texSize = new Vector2(TargetFrame.width, TargetFrame.height);

            foreach (Unit x in owner.GetEnemys())
            {
                crd = Camera.main.WorldToScreenPoint(x.transform.position);
                crd.y = Screen.height - crd.y;
                texXPos = crd.x - texSize.x / 2f;
                texYPos = crd.y - texSize.y / 2f + guiTexOffsetY;
                GUI.DrawTexture(new Rect(new Vector2(texXPos, texYPos), texSize), TargetFrame);
            }
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

                Vector3 shiftLocal = (this.transform.right * horisontalShiftThrust * owner.ShiftSpeed + this.transform.forward * mainThrust * owner.Speed) * 25;

                body.AddForce(shiftLocal, ForceMode.Acceleration);
            }
        }
        private void Rotate()
        {
            var rot = new Vector3(0f, 0f, 0f);
            // rotates Left
            if (Input.GetAxis(yawAxis) < 0)
            {
                rot.y -= 1;
            }
            // rotates Left
            if (Input.GetAxis(yawAxis) > 0 || Input.GetKey(KeyCode.RightArrow))
            {
                rot.y += 1;
            }
            if (tridimensional)
            {
                // rotates Up
                if (Input.GetAxis(pitchAxis) < 0)
                {
                    rot.x -= 1;
                }
                // rotates Down
                if (Input.GetAxis(pitchAxis) > 0)
                {
                    rot.x += 1;
                }
                // roll left
                if (Input.GetAxis(rollAxis) < 0)
                {
                    rot.z -= 1;
                }
                // roll right
                if (Input.GetAxis(rollAxis) > 0)
                {
                    rot.z += 1;
                }
            }

            transform.Rotate(rot, owner.RotationSpeed * 5.5f * Time.deltaTime);
        }
        private void Stabilisation()
        {
            Quaternion rotDest = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            if (Quaternion.Angle(owner.transform.rotation, rotDest) > 1)
                owner.transform.rotation = Quaternion.RotateTowards(owner.transform.rotation, rotDest, Time.deltaTime * owner.RotationSpeed * 6f);
            else owner.transform.rotation = rotDest;

            Vector3 targetPos = owner.transform.position;
            targetPos.y = 0;
            owner.transform.position = Vector3.MoveTowards(owner.transform.position, targetPos, Time.deltaTime * owner.Speed * 20);
        }
    }
}
