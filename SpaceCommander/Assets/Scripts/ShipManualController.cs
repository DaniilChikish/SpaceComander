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
    /**
     * v 0.6
     * **/
    //class ShipManualController : MonoBehaviour
    //{
    //    public float guiTexOffsetY;
    //    public SpaceShip owner;
    //    private Rigidbody body;
    //    private GlobalController Global;
    //    private HUDBase hud;
    //    public enum AimStateType { Default, Locking, Locked}
    //    private const float lockdownDuration = 1.5f;
    //    private float maxThrust = 2.5f;
    //    private const float tgrustDeah = 0.3f;
    //    private float maxShift = 1f;
    //    private float lockdownCount;
    //    private Unit targetBuffer;
    //    public Unit TargetBuffer
    //    {
    //        set
    //        {
    //            targetBuffer = value;
    //            if (value != null)
    //            {
    //                lockdownCount = lockdownDuration;
    //                aimState = AimStateType.Locking;
    //                value.gameObject.GetComponentInChildren<Camera>().enabled = true;
    //            }
    //            else
    //            {
    //                aimState = AimStateType.Default;
    //            }
    //        }
    //        get { return targetBuffer; }
    //    }
    //    private int targetQinuePosition;
    //    public Texture DefaultMainAim;
    //    public Texture TargetFrame;
    //    public Texture LockingAimFirst;
    //    public Texture LockingAimSecond;
    //    private bool firstLookingTexture;
    //    private float lockingTextureSwichCoutn;
    //    public Texture LockedAim;
    //    public Texture TargetDott;
    //    public Texture FireAim;
    //    //private RTS_Cam.RTS_Camera folowCam;
    //    public AimStateType aimState;

    //    public bool tridimensional;
    //    private float mainThrust;
    //    private float verticalShiftThrust;
    //    private float horisontalShiftThrust;
    //    //controlAxis;
    //    private string thrustAxis = "Thrust";
    //    private string horizontalShiftAxis = "HorizontalShift";
    //    private string verticalShiftAxis = "VerticalShift";
    //    private string pitchAxis = "Pitch";
    //    private string yawAxis = "Yaw";
    //    private string pitchAxisMouse = "PitchMouse";
    //    private string yawAxisMouse = "YawMouse";
    //    private string rollAxis = "Roll";
    //    private string primaryWeaponAxis = "PrimaryWeapon";
    //    private string secondaryWeaponAxis = "SecondaryWeapon";
    //    private string lockTargetAxis = "LockTarget";
    //    private string swithTargetAxis = "SwitchTarget";
    //    private KeyCode freeCursor = KeyCode.LeftAlt;

    //    private void OnEnable()
    //    {
    //        Global = FindObjectOfType<GlobalController>();
    //        hud = FindObjectOfType<HUDBase>();
    //        ///owner.gameObject.GetComponent<NavMeshAgent>().enabled = false;
    //        owner.SendTo(owner.transform.position);
    //        //tridimensional = true;
    //        owner.Gunner.ResetAim();
    //        SetThrust();
    //        body = owner.gameObject.GetComponent<Rigidbody>();
    //        //folowCam = FindObjectOfType<RTS_Cam.RTS_Camera>();
    //        //folowCam.followingSpeed = owner.Speed * 5f;
    //    }
    //    private void OnDisable()
    //    {
    //        //owner.gameObject.GetComponent<NavMeshAgent>().enabled = true;
    //        TargetBuffer = null;
    //    }
    //    private void SetThrust()
    //    {
    //        switch (owner.Type)
    //        {
    //            case UnitClass.Scout:
    //            case UnitClass.Recon:
    //            case UnitClass.ECM:
    //                {
    //                    maxThrust = 2.5f;
    //                    maxShift = 1f;
    //                    break;
    //                }
    //            case UnitClass.Figther:
    //            case UnitClass.Bomber:
    //            case UnitClass.Command:
    //                {
    //                    maxThrust = 3.5f;
    //                    maxShift = 1.1f;
    //                    break;
    //                }
    //            case UnitClass.LR_Corvette:
    //            case UnitClass.Support_Corvette:
    //            case UnitClass.Guard_Corvette:
    //                {
    //                    maxThrust = 8f;
    //                    maxShift = 3f;
    //                    break;
    //                }
    //        }
    //    }
    //    private void Update()
    //    {
    //        Fire();
    //        ActivateModule();
    //        TagetLockdown();
    //        ScaleJetream();
    //    }


    //    private void Fire()
    //    {
    //        if (Input.GetAxis(primaryWeaponAxis) != 0)
    //            owner.Gunner.ShootHim(0);
    //        if (Input.GetAxis(secondaryWeaponAxis) != 0)
    //            owner.Gunner.ShootHim(1);
    //    }

    //    private void TagetLockdown()
    //    {
    //        if (Input.GetAxis(lockTargetAxis) > 0 && (lockdownCount < lockdownDuration * 0.8))//lock target
    //        {
    //            //if (aimState == AimStateType.Default) 
    //            //{
    //            Vector3 enemyPos;
    //            Vector3 aim = Camera.main.WorldToScreenPoint(owner.transform.position + owner.transform.forward * (owner.Gunner.Weapon[0][0].Range + owner.Gunner.Weapon[1][0].Range) / 2);
    //            aim.z = 0;
    //            float minDistance = Screen.height / 2f;
    //            int enemyIndex = -1;
    //            Unit[] enemys = owner.GetEnemys();
    //            float dist;
    //            for (int i = 0; i < enemys.Length; i++)
    //            {
    //                enemyPos = Camera.main.WorldToScreenPoint(enemys[i].transform.position);
    //                enemyPos.z = 0;
    //                dist = Vector3.Distance(aim, enemyPos);
    //                if (dist < minDistance)
    //                {
    //                    minDistance = dist;
    //                    enemyIndex = i;
    //                }
    //            }
    //            if (enemyIndex != -1)
    //                TargetBuffer = enemys[enemyIndex];
    //            else //reset target
    //            {
    //                aimState = AimStateType.Default;
    //                owner.Gunner.ResetAim();
    //            }
    //        }
    //        else if (Input.GetAxis(swithTargetAxis) > 0 && (lockdownCount < lockdownDuration * 0.8))
    //        {
    //            Unit[] enemys = owner.GetEnemys();
    //            if (enemys.Length > 0)
    //            {
    //                targetQinuePosition++;
    //                if (targetQinuePosition >= enemys.Length)
    //                    targetQinuePosition = 0;
    //                TargetBuffer = enemys[targetQinuePosition];
    //            }
    //        }

    //        switch (aimState)
    //        {
    //            case AimStateType.Default:
    //                {
    //                    break;
    //                }
    //            case AimStateType.Locked:
    //                {
    //                    if (owner.Gunner.Target != TargetBuffer)
    //                        owner.Gunner.SetAim(TargetBuffer, true, 0);
    //                    else if (!TargetInSight())
    //                    {
    //                        TargetBuffer = null;
    //                        owner.Gunner.ResetAim();
    //                    }
    //                    break;
    //                }
    //            case AimStateType.Locking:
    //                {
    //                    if (lockdownCount <= 0)
    //                    {
    //                        aimState = AimStateType.Locked;
    //                    }
    //                    else lockdownCount -= Time.deltaTime;

    //                    if (lockingTextureSwichCoutn <= 0)
    //                    {
    //                        firstLookingTexture = !firstLookingTexture;
    //                        lockingTextureSwichCoutn = (lockdownCount / lockdownDuration) * 0.5f;
    //                    }
    //                    else lockingTextureSwichCoutn -= Time.deltaTime;
    //                    break;
    //                }
    //        }

    //    }
    //    private bool TargetInSight()
    //    {
    //        if (TargetBuffer == null)
    //            return false;
    //        float distance = Vector3.Distance(owner.transform.position, TargetBuffer.transform.position);
    //        if (distance < owner.RadarRange)
    //            return true;
    //        else
    //        {
    //            if (!owner.TargetScouting())
    //                return false;//переходим в ожидение
    //            else return true;
    //        }
    //    }

    //    private void ActivateModule()
    //    {
    //        if (Input.GetKeyDown(KeyCode.Alpha1) && owner.Module[0] != null && owner.Module[0].State == SpellModuleState.Ready)
    //            owner.Module[0].EnableIfReady();
    //        if (Input.GetKeyDown(KeyCode.Alpha2) && owner.Module[1] != null && owner.Module[1].State == SpellModuleState.Ready)
    //            owner.Module[1].EnableIfReady();
    //        if (Input.GetKeyDown(KeyCode.Alpha3) && owner.Module[2] != null && owner.Module[2].State == SpellModuleState.Ready)
    //            owner.Module[2].EnableIfReady();
    //        if (Input.GetKeyDown(KeyCode.Alpha4) && owner.Module[3] != null && owner.Module[3].State == SpellModuleState.Ready)
    //            owner.Module[3].EnableIfReady();
    //        if (Input.GetKeyDown(KeyCode.Alpha5) && owner.Module[4] != null && owner.Module[4].State == SpellModuleState.Ready)
    //            owner.Module[4].EnableIfReady();
    //    }
    //    private void OnGUI()
    //    {
    //        //GUI.skin = hud.Skin;
    //        //if (Global.StaticProportion && hud.scale != 1)
    //        //    GUI.matrix = Matrix4x4.Scale(Vector3.one * hud.scale);
    //        float scaleLocal = (hud.scale / 1.5f) * Global.Settings.IconsScale;

    //        //mainAimPoint
    //        Vector3 aim = owner.transform.position + owner.transform.forward * (owner.Gunner.Weapon[0][0].Range + owner.Gunner.Weapon[1][0].Range) / 2f;

    //        Vector3 crd = Camera.main.WorldToScreenPoint(aim);
    //        crd.y = Screen.height - crd.y;

    //        Texture aimTexture;
    //        switch (aimState)
    //        {
    //            case AimStateType.Locked:
    //                {
    //                    aimTexture = LockedAim;
    //                    break;
    //                }
    //            case AimStateType.Locking:
    //                {
    //                    if (firstLookingTexture)
    //                        aimTexture = LockingAimFirst;
    //                    else
    //                        aimTexture = LockingAimSecond;
    //                    break;
    //                }
    //            case AimStateType.Default:
    //            default:
    //                {
    //                    aimTexture = DefaultMainAim;
    //                    break;
    //                }
    //        }
    //        Vector2 texSize = new Vector2(aimTexture.width, aimTexture.height) * scaleLocal;
    //        float texXPos = crd.x - texSize.x / 2f;
    //        float texYPos = crd.y - texSize.y / 2f + guiTexOffsetY;

    //        GUI.DrawTexture(new Rect(new Vector2(texXPos, texYPos), texSize), aimTexture);

    //        float border = 40;
    //        bool outOfBorder = false;

    //        //current target point
    //        if (TargetBuffer != null)
    //        {

    //            crd = UIUtil.WorldToScreenCircle(TargetBuffer.transform.position, border, out outOfBorder);
    //            //texSize = new Vector2(TargetDott.width, TargetDott.height) * scaleLocal;
    //            texXPos = crd.x - texSize.x / 2f;
    //            texYPos = crd.y - texSize.y / 2f + guiTexOffsetY;
    //            if (aimState == AimStateType.Locked)
    //                GUI.DrawTexture(new Rect(new Vector2(texXPos, texYPos), texSize), FireAim);
    //            else
    //                GUI.DrawTexture(new Rect(new Vector2(texXPos, texYPos), texSize), TargetDott);
    //        }

    //        //locked Aim
    //        //texSize = new Vector2(TargetFrame.width, TargetFrame.height);

    //        foreach (Unit x in owner.GetEnemys())
    //        {
    //            crd = UIUtil.WorldToScreenCircle(x.transform.position, border, out outOfBorder);
    //            texXPos = crd.x - texSize.x / 2f;
    //            texYPos = crd.y - texSize.y / 2f + guiTexOffsetY;
    //            GUI.DrawTexture(new Rect(new Vector2(texXPos, texYPos), texSize), TargetFrame);
    //        }
    //    }

    //    private void FixedUpdate()
    //    {
    //        Move();
    //        Rotate();
    //        if (!Input.GetKey(freeCursor))
    //        {
    //            Cursor.visible = false;
    //            Cursor.lockState = CursorLockMode.Locked;
    //            RotateByMouse();
    //        }
    //        else
    //        {
    //            Cursor.visible = true;
    //            Cursor.lockState = CursorLockMode.None;
    //        }
    //        if (!tridimensional)
    //            Stabilisation();
    //    }

    //    private void Move()
    //    {
    //        float mainThrustLocal = Input.GetAxis(thrustAxis);
    //        if (mainThrust <= maxThrust && mainThrust >= -maxShift)
    //            mainThrust += mainThrustLocal * Time.deltaTime;

    //        if (mainThrustLocal == 0)
    //        {
    //            if (mainThrust <= tgrustDeah && mainThrust >= -tgrustDeah)
    //                mainThrust = mainThrust * 0.7f;
    //            else if (mainThrust > tgrustDeah)
    //                mainThrust -= Time.deltaTime * tgrustDeah;
    //            else if (mainThrust < -tgrustDeah)
    //                mainThrust += Time.deltaTime * tgrustDeah;
    //        }
    //        if (mainThrust > maxThrust) mainThrust = maxThrust;
    //        if (mainThrust < 0.0001 && mainThrust > -0.0001) mainThrust = 0;
    //        if (mainThrust < -maxShift) mainThrust = -maxShift;

    //        float horisontalShiftLocal = Input.GetAxis(horizontalShiftAxis);
    //        if (horisontalShiftThrust <= maxShift && horisontalShiftThrust >= -maxShift)
    //            horisontalShiftThrust += horisontalShiftLocal * Time.deltaTime;

    //        if (horisontalShiftLocal == 0)
    //        {
    //            if (horisontalShiftThrust <= 0.1 && horisontalShiftThrust >= -0.1)
    //                horisontalShiftThrust = horisontalShiftThrust * 0.7f;
    //            else if (horisontalShiftThrust > 0.1f)
    //                horisontalShiftThrust -= Time.deltaTime * 0.7f;
    //            else if (horisontalShiftThrust < -0.1f)
    //                horisontalShiftThrust += Time.deltaTime * 0.7f;
    //        }
    //        if (horisontalShiftThrust > maxShift) horisontalShiftThrust = maxShift;
    //        if (horisontalShiftThrust < 0.0001 && horisontalShiftThrust > -0.0001) horisontalShiftThrust = 0;
    //        if (horisontalShiftThrust < -maxShift) horisontalShiftThrust = -maxShift;

    //        if (tridimensional)
    //        {
    //            float vertikalShiftLocal = Input.GetAxis(verticalShiftAxis);
    //            if (verticalShiftThrust <= maxShift && verticalShiftThrust >= -maxShift)
    //                verticalShiftThrust += vertikalShiftLocal * Time.deltaTime;

    //            if (vertikalShiftLocal == 0)
    //            {
    //                if (verticalShiftThrust <= 0.1 && verticalShiftThrust >= -0.1)
    //                    verticalShiftThrust = verticalShiftThrust * 0.7f;
    //                else if (verticalShiftThrust > 0.1f)
    //                    verticalShiftThrust -= Time.deltaTime * 0.7f;
    //                else if (verticalShiftThrust < -0.1f)
    //                    verticalShiftThrust += Time.deltaTime * 0.7f;
    //            }
    //            if (verticalShiftThrust > maxShift) verticalShiftThrust = maxShift;
    //            if (verticalShiftThrust < 0.0001 && verticalShiftThrust > -0.0001) verticalShiftThrust = 0;
    //            if (verticalShiftThrust < -maxShift) verticalShiftThrust = -maxShift;
    //        }
    //        Vector3 shiftLocal = (owner.transform.right * horisontalShiftThrust * owner.ShiftSpeed + owner.transform.up * verticalShiftThrust * owner.ShiftSpeed + owner.transform.forward * mainThrust * owner.Speed);

    //        body.AddForce(shiftLocal, ForceMode.Acceleration);
    //    }
    //    //private void Rotate()
    //    //{
    //    //    var rot = new Vector3(0f, 0f, 0f);
    //    //    // rotates Left
    //    //    if (Input.GetAxis(yawAxis) < 0)
    //    //    {
    //    //        rot.y -= 1;
    //    //    }
    //    //    // rotates Left
    //    //    if (Input.GetAxis(yawAxis) > 0)
    //    //    {
    //    //        rot.y += 1;
    //    //    }
    //    //    if (tridimensional)
    //    //    {
    //    //        // rotates Up
    //    //        if (Input.GetAxis(pitchAxis) < 0)
    //    //        {
    //    //            rot.x -= 1;
    //    //        }
    //    //        // rotates Down
    //    //        if (Input.GetAxis(pitchAxis) > 0)
    //    //        {
    //    //            rot.x += 1;
    //    //        }
    //    //        // roll left
    //    //        if (Input.GetAxis(rollAxis) < 0)
    //    //        {
    //    //            rot.z -= 1;
    //    //        }
    //    //        // roll right
    //    //        if (Input.GetAxis(rollAxis) > 0)
    //    //        {
    //    //            rot.z += 1;
    //    //        }
    //    //    }

    //    //    transform.Rotate(rot, owner.RotationSpeed * 5.5f * Time.deltaTime);
    //    //}

    //    private void Rotate()
    //    {
    //        var rot = new Vector3(0f, 0f, 0f);
    //        rot.y += Input.GetAxis(yawAxis);

    //        if (tridimensional)
    //        {
    //            rot.x -= Input.GetAxis(pitchAxis);
    //            rot.z -= Input.GetAxis(rollAxis);
    //        }

    //        owner.transform.Rotate(rot, owner.RotationSpeed * Time.deltaTime);
    //    }
    //    private void RotateByMouse()
    //    {
    //        var rot = new Vector3(0f, 0f, 0f);
    //        rot.y += Input.GetAxis(yawAxisMouse);

    //        if (tridimensional)
    //        {
    //            rot.x -= Input.GetAxis(pitchAxisMouse);
    //        }
    //        owner.transform.Rotate(rot, owner.RotationSpeed * Time.deltaTime);
    //    }
    //    private void Stabilisation()
    //    {
    //        Quaternion rotDest = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    //        if (Quaternion.Angle(owner.transform.rotation, rotDest) > 1)
    //            owner.transform.rotation = Quaternion.RotateTowards(owner.transform.rotation, rotDest, Time.deltaTime * owner.RotationSpeed * 6f);
    //        else owner.transform.rotation = rotDest;

    //        Vector3 targetPos = owner.transform.position;
    //        targetPos.y = 0;
    //        owner.transform.position = Vector3.MoveTowards(owner.transform.position, targetPos, Time.deltaTime * owner.Speed * 20);
    //    }
    //    private void ScaleJetream()
    //    {
    //        owner.ScaleJetream = new Vector3(mainThrust, 1, 1);
    //    }
    //}
    /**
 * v 0.7
 * **/
    class ShipManualController : MonoBehaviour
    {
        public float guiTexOffsetY;
        public SpaceShip owner;
        private Rigidbody body;
        private GlobalController Global;
        private HUDBase hud;
        public enum AimStateType { Default, Locking, Locked }
        private const float lockdownDuration = 1.5f;
        private const float thrustAccel = 1f;
        private const float thrustDeah = 0.3f;
        private float lockdownCount;
        private Unit targetBuffer;
        public bool accelCompensator;
        public Unit TargetBuffer
        {
            set
            {
                targetBuffer = value;
                if (value != null)
                {
                    lockdownCount = lockdownDuration;
                    aimState = AimStateType.Locking;
                    value.gameObject.GetComponentInChildren<Camera>().enabled = true;
                }
                else
                {
                    aimState = AimStateType.Default;
                }
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
        public float mainThrust;
        private bool fixedMainThrust;
        public float verticalShiftThrust;
        private bool fixedVerticalThrust;
        public float horisontalShiftThrust;
        private bool fixedHorisontalThrust;

        //controlAxis;
        private string thrustAxis = "Thrust";
        private string horizontalShiftAxis = "HorizontalShift";
        private string verticalShiftAxis = "VerticalShift";
        private string pitchAxis = "Pitch";
        private string yawAxis = "Yaw";
        private string pitchAxisMouse = "PitchMouse";
        private string yawAxisMouse = "YawMouse";
        private string rollAxis = "Roll";
        private string primaryWeaponAxis = "PrimaryWeapon";
        private string secondaryWeaponAxis = "SecondaryWeapon";
        private string lockTargetAxis = "LockTarget";
        private string swithTargetAxis = "SwitchTarget";
        private KeyCode freeCursor = KeyCode.LeftAlt;
        private KeyCode switchCompensator = KeyCode.Space;
        private void OnEnable()
        {
            Global = FindObjectOfType<GlobalController>();
            hud = FindObjectOfType<HUDBase>();
            ///owner.gameObject.GetComponent<NavMeshAgent>().enabled = false;
            owner.SendTo(owner.transform.position);
            //tridimensional = true;
            owner.Gunner.ResetAim();
            body = owner.gameObject.GetComponent<Rigidbody>();
            //folowCam = FindObjectOfType<RTS_Cam.RTS_Camera>();
            //folowCam.followingSpeed = owner.Speed * 5f;
            accelCompensator = true;
            fixedMainThrust = true;
            mainThrust = 0;
            fixedVerticalThrust = true;
            verticalShiftThrust = 0;
            fixedHorisontalThrust = true;
            horisontalShiftThrust = 0;
        }
        private void OnDisable()
        {
            //owner.gameObject.GetComponent<NavMeshAgent>().enabled = true;
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
                float minDistance = Screen.height / 2f;
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
            //GUI.skin = hud.Skin;
            //if (Global.StaticProportion && hud.scale != 1)
            //    GUI.matrix = Matrix4x4.Scale(Vector3.one * hud.scale);
            float scaleLocal = (hud.scale / 1.5f) * Global.Settings.IconsScale;

            //mainAimPoint
            Vector3 aim = owner.transform.position + owner.transform.forward * (owner.Gunner.Weapon[0][0].Range + owner.Gunner.Weapon[1][0].Range) / 2f;

            Vector3 crd = Camera.main.WorldToScreenPoint(aim);
            crd.y = Screen.height - crd.y;

            Texture aimTexture;
            switch (aimState)
            {
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
                case AimStateType.Default:
                default:
                    {
                        aimTexture = DefaultMainAim;
                        break;
                    }
            }
            Vector2 texSize = new Vector2(aimTexture.width, aimTexture.height) * scaleLocal;
            float texXPos = crd.x - texSize.x / 2f;
            float texYPos = crd.y - texSize.y / 2f + guiTexOffsetY;

            GUI.DrawTexture(new Rect(new Vector2(texXPos, texYPos), texSize), aimTexture);

            float border = 40;
            bool outOfBorder = false;

            //current target point
            if (TargetBuffer != null)
            {

                crd = UIUtil.WorldToScreenCircle(TargetBuffer.transform.position, border, out outOfBorder);
                //texSize = new Vector2(TargetDott.width, TargetDott.height) * scaleLocal;
                texXPos = crd.x - texSize.x / 2f;
                texYPos = crd.y - texSize.y / 2f + guiTexOffsetY;
                if (aimState == AimStateType.Locked)
                    GUI.DrawTexture(new Rect(new Vector2(texXPos, texYPos), texSize), FireAim);
                else
                    GUI.DrawTexture(new Rect(new Vector2(texXPos, texYPos), texSize), TargetDott);
            }

            //locked Aim
            //texSize = new Vector2(TargetFrame.width, TargetFrame.height);

            foreach (Unit x in owner.GetEnemys())
            {
                crd = UIUtil.WorldToScreenCircle(x.transform.position, border, out outOfBorder);
                texXPos = crd.x - texSize.x / 2f;
                texYPos = crd.y - texSize.y / 2f + guiTexOffsetY;
                GUI.DrawTexture(new Rect(new Vector2(texXPos, texYPos), texSize), TargetFrame);
            }
        }

        private void FixedUpdate()
        {
            Move();
            Rotate();
            if (!Input.GetKey(freeCursor))
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                RotateByMouse();
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            if (!tridimensional)
                Stabilisation();
            if (Input.GetKeyDown(switchCompensator))
                SwitchCompensator();
        }

        private void Move()
        {
            Vector3 shift = Vector3.zero; //normal thrust imput
            shift.z = Mathf.Clamp(Input.GetAxis(thrustAxis), -thrustAccel, thrustAccel);
            if (!fixedMainThrust)
            {
                mainThrust += shift.z * owner.Speed * Time.deltaTime;
                mainThrust = Mathf.Clamp(mainThrust, -owner.ShiftSpeed, owner.Speed);
            }

            shift.x = Mathf.Clamp(Input.GetAxis(horizontalShiftAxis), -thrustAccel, thrustAccel);
            if (!fixedHorisontalThrust)
            {
                horisontalShiftThrust += shift.x * owner.ShiftSpeed * Time.deltaTime;
                horisontalShiftThrust = Mathf.Clamp(horisontalShiftThrust, -owner.ShiftSpeed, owner.ShiftSpeed);
            }
            if (tridimensional)
            {
                shift.y = Mathf.Clamp(Input.GetAxis(verticalShiftAxis), -thrustAccel, thrustAccel);
                if (!fixedVerticalThrust)
                {
                    verticalShiftThrust += shift.y * owner.ShiftSpeed * Time.deltaTime;
                    verticalShiftThrust = Mathf.Clamp(horisontalShiftThrust, -owner.ShiftSpeed, owner.ShiftSpeed);
                }
            }

            if (shift.magnitude < thrustDeah)
                shift = Vector3.zero;

            if (accelCompensator)
            {
                Vector3 velocity = body.velocity;
                float sign;
                float mainSpeed = Vector3.Project(body.velocity, body.transform.forward).magnitude;
                if (Mathf.Abs(shift.z) < thrustDeah || mainSpeed > owner.Speed)
                {
                    if (Vector3.Angle(body.velocity, body.transform.forward) < 90)
                        sign = 1;
                    else sign = -1;
                    shift.z = Mathf.Clamp((mainThrust) - (mainSpeed * sign), -1, 1);
                }
                float horisontalSpeed = Vector3.Project(body.velocity, body.transform.right).magnitude;
                if (Mathf.Abs(shift.x) < thrustDeah || horisontalSpeed > owner.ShiftSpeed)
                {
                    if (Vector3.Angle(body.velocity, body.transform.right) < 90)
                        sign = 1;
                    else sign = -1;
                    shift.x = Mathf.Clamp((horisontalShiftThrust) - (horisontalSpeed * sign), -1, 1);
                }
                float verticalSpeed = Vector3.Project(body.velocity, body.transform.up).magnitude;
                if (Mathf.Abs(shift.y) < thrustDeah || verticalSpeed > owner.ShiftSpeed)
                {
                    if (Vector3.Angle(body.velocity, body.transform.up) < 90)
                        sign = 1;
                    else sign = -1;
                    shift.y = Mathf.Clamp((verticalShiftThrust) - (verticalSpeed * sign), -1, 1);
                }
            }
            ScaleJetream(shift.z);
            body.AddRelativeForce(shift * owner.Acceleration, ForceMode.Acceleration);
        }

        private void Rotate()
        {
            var rot = new Vector3(0f, 0f, 0f);
            rot.y += Input.GetAxis(yawAxis);

            if (tridimensional)
            {
                rot.x -= Input.GetAxis(pitchAxis);
                rot.z -= Input.GetAxis(rollAxis);
            }

            owner.transform.Rotate(rot, owner.RotationSpeed * Time.deltaTime);
        }
        private void RotateByMouse()
        {
            var rot = new Vector3(0f, 0f, 0f);
            rot.y += Input.GetAxis(yawAxisMouse);

            if (tridimensional)
            {
                rot.x -= Input.GetAxis(pitchAxisMouse);
            }
            owner.transform.Rotate(rot, owner.RotationSpeed * Time.deltaTime);
        }
        private void SwitchCompensator()
        {
            if (accelCompensator)
            {
                accelCompensator = false;
                fixedMainThrust = false;
                mainThrust = 0;
                fixedVerticalThrust = false;
                verticalShiftThrust = 0;
                fixedHorisontalThrust = false;
                horisontalShiftThrust = 0;
            }
            else
            {
                accelCompensator = true;
                fixedMainThrust = true;
                mainThrust = 0;
                fixedVerticalThrust = true;
                verticalShiftThrust = 0;
                fixedHorisontalThrust = true;
                horisontalShiftThrust = 0;
            }
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
        private void ScaleJetream(float scale)
        {
            owner.ScaleJetream = new Vector3(Mathf.Clamp01(scale), 1, 1);
        }
    }
}
