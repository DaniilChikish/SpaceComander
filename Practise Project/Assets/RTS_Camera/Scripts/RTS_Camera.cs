using UnityEngine;
using System.Collections;
using SpaceCommander;

namespace RTS_Cam
{
    public enum CamMode { Free, Folloving, ThirthPerson}
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("RTS Camera")]
    public class RTS_Camera : MonoBehaviour
    {

        #region Foldouts

#if UNITY_EDITOR

        public int lastTab = 0;

        public bool movementSettingsFoldout;
        public bool zoomingSettingsFoldout;
        public bool rotationSettingsFoldout;
        public bool heightSettingsFoldout;
        public bool mapLimitSettingsFoldout;
        public bool targetingSettingsFoldout;
        public bool inputSettingsFoldout;

#endif

        #endregion
        public CamMode mode;
        private Transform m_Transform; //camera tranform
        public bool useFixedUpdate = true; //use FixedUpdate() or Update()

        #region Movement

        public float keyboardMovementSpeed = 5f; //speed with keyboard movement
        public float screenEdgeMovementSpeed = 3f; //spee with screen edge movement
        public float followingSpeed = 5f; //speed when following a target
        public float rotationSped = 3f;
        public float panningSpeed = 10f;
        public float mouseRotationSpeed = 10f;
        private float holding;
        Transform mapcam;
        private float freeCorner;
        #endregion

        #region Height

        public bool autoHeight = true;
        public LayerMask groundMask = -1; //layermask of ground or other objects that affect height

        public float maxHeight = 10f; //maximal height
        public float minHeight = 15f; //minimnal height
        public float heightDampening = 5f; 
        public float keyboardZoomingSensitivity = 2f;
        public float scrollWheelZoomingSensitivity = 25f;

        private float zoomPos = 0; //value in range (0, 1) used as t in Matf.Lerp

        #endregion

        #region MapLimits

        public bool limitMap = true;
        public float limitX = 50f; //x limit of map
        public float limitY = 50f; //z limit of map

        #endregion

        #region Targeting
        [SerializeField]
        private float m_MoveSpeed = 3; // How fast the rig will move to keep up with target's position
        [SerializeField]
        private float m_TurnSpeed = 1; // How fast the rig will turn to keep up with target's rotation
        [SerializeField]
        private float m_RollSpeed = 0.2f;// How fast the rig will roll (around Z axis) to match target's roll.
        [SerializeField]
        private bool m_FollowVelocity = false;// Whether the rig will rotate in the direction of the target's velocity.
        [SerializeField]
        private bool m_FollowTilt = true; // Whether the rig will tilt (around X axis) with the target.
        [SerializeField]
        private float m_SpinTurnLimit = 90;// The threshold beyond which the camera stops following the target's rotation. (used in situations where a car spins out, for example)
        [SerializeField]
        private float m_TargetVelocityLowerLimit = 4f;// the minimum velocity above which the camera turns towards the object's velocity. Below this we use the object's forward direction.
        [SerializeField]
        private float m_SmoothTurnTime = 0.2f; // the smoothing for the camera's rotation

        private float m_LastFlatAngle; // The relative angle of the target and the rig from the previous frame.
        private float m_CurrentTurnAmount; // How much to turn the camera
        private float m_TurnSpeedVelocityChange; // The change in the turn speed velocity
        private Vector3 m_RollUp = Vector3.up;// The roll of the camera around the z axis ( generally this will always just be up )

        private Unit target;
        public Unit TargetFollow//target to follow
        {
            set
            {
                target = value;
            }
            get
            {
                return target;
            }
        }
        [SerializeField]
        public Vector3 targetOffset;

        /// <summary>
        /// are we following target
        /// </summary>
        public bool FollowingTarget
        {
            get
            {
                return target != null;
            }
        }

        #endregion

        #region Input

        public bool useScreenEdgeInput = true;
        public float screenEdgeBorder = 25f;

        public bool useKeyboardInput = true;
        public string horizontalAxis = "Horizontal";
        public string verticalAxis = "Vertical";

        public bool usePanning = true;
        public KeyCode panningKey = KeyCode.Mouse2;

        public bool useKeyboardZooming = true;
        public KeyCode zoomInKey = KeyCode.E;
        public KeyCode zoomOutKey = KeyCode.Q;

        public bool useScrollwheelZooming = true;
        public string zoomingAxis = "Mouse ScrollWheel";

        public bool useKeyboardRotation = true;
        public KeyCode rotateRightKey = KeyCode.X;
        public KeyCode rotateLeftKey = KeyCode.Z;

        public bool useMouseRotation = true;
        public KeyCode mouseRotationKey = KeyCode.Mouse1;

        private Vector2 KeyboardInput
        {
            get { return useKeyboardInput ? new Vector2(Input.GetAxis(horizontalAxis), Input.GetAxis(verticalAxis)) : Vector2.zero; }
        }

        private Vector2 MouseInput
        {
            get { return Input.mousePosition; }
        }

        private float ScrollWheel
        {
            get { return Input.GetAxis(zoomingAxis); }
        }

        private Vector2 MouseAxis
        {
            get { return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); }
        }

        private int ZoomDirection
        {
            get
            {
                bool zoomIn = Input.GetKey(zoomInKey);
                bool zoomOut = Input.GetKey(zoomOutKey);
                if (zoomIn && zoomOut)
                    return 0;
                else if (!zoomIn && zoomOut)
                    return 1;
                else if (zoomIn && !zoomOut)
                    return -1;
                else 
                    return 0;
            }
        }

        private int RotationDirection
        {
            get
            {
                bool rotateRight = Input.GetKey(rotateRightKey);
                bool rotateLeft = Input.GetKey(rotateLeftKey);
                if(rotateLeft && rotateRight)
                    return 0;
                else if(rotateLeft && !rotateRight)
                    return -1;
                else if(!rotateLeft && rotateRight)
                    return 1;
                else 
                    return 0;
            }
        }

        #endregion

        #region Unity_Methods

        private void Start()
        {
            m_Transform = transform;
            mapcam = this.transform.FindChild("MapCam");
            freeCorner = this.transform.localRotation.eulerAngles.x;
        }

        private void Update()
        {
            if (!useFixedUpdate)
                CameraUpdate();
        }

        private void FixedUpdate()
        {
            if (useFixedUpdate)
                CameraUpdate();
        }

        #endregion

        #region RTSCamera_Methods

        /// <summary>
        /// update camera movement and rotation
        /// </summary>
        private void CameraUpdate()
        {
            switch (mode)
            {
                case CamMode.ThirthPerson:
                    {
                        FollowTarget(Time.deltaTime);
                        break;
                    }
                case CamMode.Folloving:
                    {
                        FollowTarget();
                        HeightCalculation();
                        Rotation();
                        break;
                    }
                case CamMode.Free:
                    {
                        Move();
                        HeightCalculation();
                        Rotation();
                        LimitPosition();
                        break;
                    }
            }
        }

        /// <summary>
        /// move camera with keyboard or with screen edge
        /// </summary>
        private void Move()
        {
            if (useKeyboardInput)
            {
                Vector3 desiredMove = new Vector3(KeyboardInput.x, 0, KeyboardInput.y);
                if (desiredMove.magnitude > 0)
                {
                    if (holding < 5)
                        holding += Time.deltaTime;
                }
                else if (holding > 0.00001f) holding = holding * 0.7f;
                else holding = 0;

                desiredMove *= keyboardMovementSpeed;
                desiredMove *= Time.deltaTime + holding / 4;
                desiredMove = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f)) * desiredMove;
                desiredMove = m_Transform.InverseTransformDirection(desiredMove);

                m_Transform.Translate(desiredMove, Space.Self);
            }

            if (useScreenEdgeInput)
            {

                Vector3 desiredMove = new Vector3();

                Rect leftRect = new Rect(0, 0, screenEdgeBorder, Screen.height);
                Rect rightRect = new Rect(Screen.width - screenEdgeBorder, 0, screenEdgeBorder, Screen.height);
                Rect upRect = new Rect(0, Screen.height - screenEdgeBorder, Screen.width, screenEdgeBorder);
                Rect downRect = new Rect(0, 0, Screen.width, screenEdgeBorder);

                desiredMove.x = leftRect.Contains(MouseInput) ? -1 : rightRect.Contains(MouseInput) ? 1 : 0;
                desiredMove.z = upRect.Contains(MouseInput) ? 1 : downRect.Contains(MouseInput) ? -1 : 0;

                desiredMove *= screenEdgeMovementSpeed;
                desiredMove *= Time.deltaTime + holding * 0.6f;
                desiredMove = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f)) * desiredMove;
                desiredMove = m_Transform.InverseTransformDirection(desiredMove);

                m_Transform.Translate(desiredMove, Space.Self);
            }       
        
            if(usePanning && Input.GetKey(panningKey) && MouseAxis != Vector2.zero)
            {
                Vector3 desiredMove = new Vector3(-MouseAxis.x, 0, -MouseAxis.y);

                desiredMove *= panningSpeed;
                desiredMove *= Time.deltaTime;
                desiredMove = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f)) * desiredMove;
                desiredMove = m_Transform.InverseTransformDirection(desiredMove);

                m_Transform.Translate(desiredMove, Space.Self);
            }
        }

        /// <summary>
        /// calcualte height
        /// </summary>
        private void HeightCalculation()
        {
            float distanceToGround = DistanceToGround();
            if(useScrollwheelZooming)
                zoomPos += ScrollWheel * Time.deltaTime * scrollWheelZoomingSensitivity;
            if (useKeyboardZooming)
                zoomPos += ZoomDirection * Time.deltaTime * keyboardZoomingSensitivity;

            zoomPos = Mathf.Clamp01(zoomPos);

            float targetHeight = Mathf.Lerp(minHeight, maxHeight, zoomPos);
            float difference = 0; 

            if(distanceToGround != targetHeight)
                difference = targetHeight - distanceToGround;

            m_Transform.position = Vector3.Lerp(m_Transform.position, 
                new Vector3(m_Transform.position.x, targetHeight + difference, m_Transform.position.z), Time.deltaTime * heightDampening);
            if (mapcam != null)
            {
                Vector3 mapPos = this.transform.position + mapcam.forward * distanceToGround * 1.3f;
                //mapPos.z = 0;
                mapcam.position = Vector3.MoveTowards(mapcam.position, mapPos, Time.deltaTime * followingSpeed * 25f);
            }
        }

        /// <summary>
        /// rotate camera
        /// </summary>
        
        private void Rotation()
        {
            if (mode != CamMode.ThirthPerson)
            {
                Vector3 aroundPoint = Vector3.zero;
                if (mode == CamMode.Folloving)
                    aroundPoint = TargetFollow.transform.position;
                else
                    aroundPoint = transform.position + transform.forward * DistanceToGround() * 1.3f;
                Quaternion rotDest = Quaternion.Euler(freeCorner, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
                if (Quaternion.Angle(this.transform.rotation, rotDest) > 1)
                    transform.rotation = Quaternion.RotateTowards(this.transform.rotation, rotDest, Time.deltaTime * rotationSped * 0.7f);
                else this.transform.rotation = rotDest;

                if (useKeyboardRotation)
                    transform.RotateAround(aroundPoint, Vector3.up, RotationDirection * Time.deltaTime * rotationSped);//, Space.World);

                if (useMouseRotation && Input.GetKey(mouseRotationKey))
                    m_Transform.RotateAround(aroundPoint, Vector3.up, -MouseAxis.x * Time.deltaTime * mouseRotationSpeed);//, Space.World);
            }
        }

        /// <summary>
        /// follow targetif target != null
        /// </summary>
        private void FollowTarget()
        {
            float heightDiff = this.transform.position.y - TargetFollow.transform.position.y;
            float dethDictance = (2 + heightDiff) * 0.4f;
            Vector3 offsetX = targetOffset.x * TargetFollow.transform.right;
            Vector3 offsetY = targetOffset.y * TargetFollow.transform.up;
            Vector3 offsetZ = (targetOffset.z + heightDiff * -1.43f) * transform.forward;
            Vector3 offsetLocal = offsetX + offsetY + offsetZ;
            offsetLocal -= target.Velocity * (1 - zoomPos * 0.8f) * (1.5f / dethDictance);
            offsetLocal.y = 0;
            Vector3 targetPos = new Vector3(TargetFollow.transform.position.x, m_Transform.position.y, TargetFollow.transform.position.z) + offsetLocal;
            float distance = Vector3.Distance(m_Transform.position, targetPos);
            if (distance < dethDictance)
                m_Transform.position = Vector3.MoveTowards(m_Transform.position, targetPos, Time.deltaTime * followingSpeed);
            else
                m_Transform.position = Vector3.MoveTowards(m_Transform.position, targetPos, Time.deltaTime * followingSpeed * distance / dethDictance);
        }
        private void FollowTarget(float deltaTime)
        {
            // if no target, or no time passed then we quit early, as there is nothing to do
            if (!(deltaTime > 0) || target == null)
            {
                return;
            }

            // initialise some vars, we'll be modifying these in a moment
            var targetForward = target.transform.forward;
            var targetUp = target.transform.up;

            if (m_FollowVelocity && Application.isPlaying)
            {
                // in follow velocity mode, the camera's rotation is aligned towards the object's velocity direction
                // but only if the object is traveling faster than a given threshold.

                if (target.Velocity.magnitude > m_TargetVelocityLowerLimit)
                {
                    // velocity is high enough, so we'll use the target's velocty
                    targetForward = target.Velocity.normalized;
                    targetUp = Vector3.up;
                }
                else
                {
                    targetUp = Vector3.up;
                }
                m_CurrentTurnAmount = Mathf.SmoothDamp(m_CurrentTurnAmount, 1, ref m_TurnSpeedVelocityChange, m_SmoothTurnTime);
            }
            else
            {
                // we're in 'follow rotation' mode, where the camera rig's rotation follows the object's rotation.

                // This section allows the camera to stop following the target's rotation when the target is spinning too fast.
                // eg when a car has been knocked into a spin. The camera will resume following the rotation
                // of the target when the target's angular velocity slows below the threshold.
                var currentFlatAngle = Mathf.Atan2(targetForward.x, targetForward.z) * Mathf.Rad2Deg;
                if (m_SpinTurnLimit > 0)
                {
                    var targetSpinSpeed = Mathf.Abs(Mathf.DeltaAngle(m_LastFlatAngle, currentFlatAngle)) / deltaTime;
                    var desiredTurnAmount = Mathf.InverseLerp(m_SpinTurnLimit, m_SpinTurnLimit * 0.75f, targetSpinSpeed);
                    var turnReactSpeed = (m_CurrentTurnAmount > desiredTurnAmount ? .1f : 1f);
                    if (Application.isPlaying)
                    {
                        m_CurrentTurnAmount = Mathf.SmoothDamp(m_CurrentTurnAmount, desiredTurnAmount,
                                                             ref m_TurnSpeedVelocityChange, turnReactSpeed);
                    }
                    else
                    {
                        // for editor mode, smoothdamp won't work because it uses deltaTime internally
                        m_CurrentTurnAmount = desiredTurnAmount;
                    }
                }
                else
                {
                    m_CurrentTurnAmount = 1;
                }
                m_LastFlatAngle = currentFlatAngle;
            }

            // camera position moves towards target position:
            float heightDiff = this.transform.position.y - target.transform.position.y;
            Vector3 offsetX = targetOffset.x * target.transform.right;
            Vector3 offsetY = targetOffset.y * target.transform.up;
            Vector3 offsetZ = (targetOffset.z + heightDiff * -1.43f) * transform.forward;
            Vector3 offsetLocal = offsetX + offsetY + offsetZ;
            transform.position = Vector3.Lerp(transform.position, target.transform.position + offsetLocal, deltaTime * m_MoveSpeed);

            // camera's rotation is split into two parts, which can have independend speed settings:
            // rotating towards the target's forward direction (which encompasses its 'yaw' and 'pitch')
            if (!m_FollowTilt)
            {
                targetForward.y = 0;
                if (targetForward.sqrMagnitude < float.Epsilon)
                {
                    targetForward = transform.forward;
                }
            }
            var rollRotation = Quaternion.LookRotation(targetForward, m_RollUp);

            // and aligning with the target object's up direction (i.e. its 'roll')
            m_RollUp = m_RollSpeed > 0 ? Vector3.Slerp(m_RollUp, targetUp, m_RollSpeed * deltaTime) : Vector3.up;
            transform.rotation = Quaternion.Lerp(transform.rotation, rollRotation, m_TurnSpeed * m_CurrentTurnAmount * deltaTime);
        }

        /// <summary>
        /// limit camera position
        /// </summary>
        private void LimitPosition()
        {
            if (!limitMap)
                return;
                
            m_Transform.position = new Vector3(Mathf.Clamp(m_Transform.position.x, -limitX, limitX),
                m_Transform.position.y,
                Mathf.Clamp(m_Transform.position.z, -limitY, limitY));
        }

        /// <summary>
        /// set the target
        /// </summary>
        /// <param name="target"></param>
        public void SetTarget(Unit target)
        {
            TargetFollow = target;
        }

        /// <summary>
        /// reset the target (target is set to null)
        /// </summary>
        public void ResetTarget()
        {
            TargetFollow = null;
        }

        /// <summary>
        /// calculate distance to ground
        /// </summary>
        /// <returns></returns>
        private float DistanceToGround()
        {
            Ray ray = new Ray(m_Transform.position, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, groundMask.value))
                return (hit.point - m_Transform.position).magnitude;

            return 0f;
        }

        #endregion
    }
}