using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpaceCommander
{
    public enum OrbitalCamMode { Free, Folloving, ThirthPerson }
    class OrbitalCamera : MonoBehaviour
    {
        [SerializeField] private float MoveSpeed; // How fast the rig will move to keep up with target's position
        [SerializeField] private float TurnSpeed; // How fast the rig will turn to keep up with target's rotation
        [SerializeField] private float RollSpeed; // How fast the rig will roll (around Z axis) to match target's roll.
        [SerializeField] private float borderRadius;
        [SerializeField] private float minZoom = 3f;
        [SerializeField] private float maxZoom = 150f;
        [SerializeField] private float zoomSens = 10f;
        [SerializeField] private bool FollowVelocity = false;// Whether the rig will rotate in the direction of the target's velocity.
        [SerializeField] private bool FollowTilt = true; // Whether the rig will tilt (around X axis) with the target.
        [SerializeField] private float SpinTurnLimit = 90;// The threshold beyond which the camera stops following the target's rotation. (used in situations where a car spins out, for example)
        [SerializeField] private float TargetVelocityLowerLimit = 4f;// the minimum velocity above which the camera turns towards the object's velocity. Below this we use the object's forward direction.
        [SerializeField] private float SmoothTurnTime = 0.2f; // the smoothing for the camera's rotation

        private float LastFlatAngle; // The relative angle of the target and the rig from the previous frame.
        private float CurrentTurnAmount; // How much to turn the camera
        private float TurnSpeedVelocityChange; // The change in the turn speed velocity
        private Vector3 RollUp = Vector3.up;// The roll of the camera around the z axis ( generally this will always just be up )

        public Vector3 targetOffset;
        private float zoomPos = 1;
        private GameObject owner;
        private float holding;
        Transform mainCam;
        public OrbitalCamMode mode;

        private Transform target;
        private Rigidbody targetRigidbody;

        public Transform TargetFollow//target to follow
        {
            set
            {
                target = value;
                if (value != null)
                    targetRigidbody = value.GetComponent<Rigidbody>();
                else targetRigidbody = null;
            }
            get
            {
                return target;
            }
        }

        private string thrustAxis = "Thrust";
        private string horizontalShiftAxis = "HorizontalShift";
        private string verticalShiftAxis = "VerticalShift";
        private string pitchAxis = "Pitch";
        private string yawAxis = "Yaw";
        private string pitchAxisMouse = "PitchMouse";
        private string yawAxisMouse = "YawMouse";
        private string rollAxis = "Roll";
        private string zoomWheel = "Mouse ScrollWheel";
        private KeyCode freeCursor = KeyCode.LeftAlt;

        void Start()
        {
            owner = this.gameObject;
            mainCam = this.transform.FindChild("Main Camera");
        }

        private void FixedUpdate()
        {
            if (mode == OrbitalCamMode.Free)
                Move(Time.deltaTime);
            else
                FollowTarget(Time.deltaTime);
            if (mode != OrbitalCamMode.ThirthPerson)
            {
                Rotate();
                if (Input.GetKey(freeCursor))
                {
                    Cursor.visible = false;
                    RotateByMouse();
                }
                else Cursor.visible = true;
            }
            Zoom(Time.deltaTime);
        }
        private void Move(float deltaTime)
        {
            float mainThrustLocal = Input.GetAxis(thrustAxis);
            float horisontalShiftLocal = Input.GetAxis(horizontalShiftAxis);
                float vertikalShiftLocal = Input.GetAxis(verticalShiftAxis);

            if (mainThrustLocal == 0 && horisontalShiftLocal == 0 && vertikalShiftLocal == 0)
            {
                if (holding > 0.00001f) holding = holding * 0.7f;
                else holding = 0;
            }
            else if (holding < 50) holding += deltaTime * 10;

            Vector3 shiftLocal = (owner.transform.forward * mainThrustLocal + owner.transform.up * vertikalShiftLocal + owner.transform.right * horisontalShiftLocal) * (1 + holding) * MoveSpeed * deltaTime;
            if ((owner.transform.position + shiftLocal).magnitude < borderRadius)
                owner.transform.position += shiftLocal;
        }
        private void Rotate()
        {
            Vector3 aroundPoint = Vector3.zero;
            if (mode == OrbitalCamMode.Folloving)
                aroundPoint = TargetFollow.transform.position;
            else
                aroundPoint = mainCam.position;

            transform.RotateAround(aroundPoint, owner.transform.right, -Input.GetAxis(pitchAxis) * Time.deltaTime * TurnSpeed * 10);
            transform.RotateAround(aroundPoint, owner.transform.up, Input.GetAxis(yawAxis) * Time.deltaTime * TurnSpeed * 10);
            transform.RotateAround(aroundPoint, owner.transform.forward, Input.GetAxis(rollAxis) * Time.deltaTime * TurnSpeed * 10);

            //var rot = new Vector3(0f, 0f, 0f);
            //rot.y += Input.GetAxis(yawAxis);
            //rot.x -= Input.GetAxis(pitchAxis);
            //rot.z -= Input.GetAxis(rollAxis);

            //if (TargetFollow)
            //    owner.transform.RotateAround(TargetFollow.position, rot, TurnSpeed * 10 * Time.deltaTime);
            //else
            //    owner.transform.RotateAround(mainCam.transform.position, rot, TurnSpeed * 10 * Time.deltaTime);
        }
        private void RotateByMouse()
        {
            Vector3 aroundPoint = Vector3.zero;
            if (mode == OrbitalCamMode.Folloving)
                aroundPoint = TargetFollow.transform.position;
            else
                aroundPoint = mainCam.position;

            transform.RotateAround(aroundPoint, owner.transform.right, -Input.GetAxis(pitchAxisMouse) * Time.deltaTime * TurnSpeed * 10);
            transform.RotateAround(aroundPoint, owner.transform.up, Input.GetAxis(yawAxisMouse) * Time.deltaTime * TurnSpeed * 10);
        }
        private void Zoom(float deltaTime)
        {
            zoomPos += Input.GetAxis(zoomWheel) * zoomSens * deltaTime;

            zoomPos = Mathf.Clamp01(zoomPos);

            //if (mode != OrbitalCamMode.ThirthPerson)
            //    mainCam.localPosition = Vector3.MoveTowards(mainCam.localPosition, (-mainCam.forward * (minZoom + ((maxZoom - minZoom) * zoomPos))), MoveSpeed * 10 * deltaTime);
            //else
            //    mainCam.localPosition = Vector3.MoveTowards(mainCam.localPosition, (-mainCam.forward * minZoom), MoveSpeed * 10 * deltaTime);
            if (mode != OrbitalCamMode.ThirthPerson)
                mainCam.localPosition = (-Vector3.forward * (minZoom + ((maxZoom - minZoom) * zoomPos)));
            else
                mainCam.localPosition = (-Vector3.forward * minZoom);
        }
        private void FollowTarget(float deltaTime)
        {
            // if no target, or no time passed then we quit early, as there is nothing to do
            if (!(deltaTime > 0) || target == null)
            {
                return;
            }
            Vector3 offsetLocal = Vector3.zero;
            // initialise some vars, we'll be modifying these in a moment
            Vector3 targetForward = target.forward;
            Vector3 targetUp = target.up;

            if (mode == OrbitalCamMode.ThirthPerson)
            {
                if (targetRigidbody != null && FollowVelocity && Application.isPlaying)
                {
                    FollowTargetVelocity(ref targetForward, ref targetUp);
                }
                else
                {
                    FollowTargetRotation(deltaTime, targetForward);
                }

                if (!FollowTilt)
                {
                    targetForward.y = 0;
                    if (targetForward.sqrMagnitude < float.Epsilon)
                    {
                        targetForward = transform.forward;
                    }
                }
                var rollRotation = Quaternion.LookRotation(targetForward, RollUp);

                // and aligning with the target object's up direction (i.e. its 'roll')
                RollUp = RollSpeed > 0 ? Vector3.Slerp(RollUp, targetUp, RollSpeed * deltaTime) : Vector3.up;
                transform.rotation = Quaternion.Lerp(transform.rotation, rollRotation, TurnSpeed * CurrentTurnAmount * deltaTime);

                // camera position moves towards target position:
                Vector3 offsetX = targetOffset.x * target.right;
                Vector3 offsetY = targetOffset.y * target.up;
                Vector3 offsetZ = targetOffset.z * transform.forward;
                offsetLocal = offsetX + offsetY + offsetZ;
            }
            transform.position = Vector3.Lerp(transform.position, target.position + offsetLocal, deltaTime * MoveSpeed);

            // camera's rotation is split into two parts, which can have independend speed settings:
            // rotating towards the target's forward direction (which encompasses its 'yaw' and 'pitch')


            //Transform mapcam = this.transform.FindChild("MapCam");
            //if (mapcam != null)
            //{
            //    //mapPos.z = 0;
            //    mapcam.position = Vector3.Lerp(mapcam.position, target.position, deltaTime * m_MoveSpeed);
            //}
        }

        private void FollowTargetRotation(float deltaTime, Vector3 targetForward)
        {
            // we're in 'follow rotation' mode, where the camera rig's rotation follows the object's rotation.

            // This section allows the camera to stop following the target's rotation when the target is spinning too fast.
            // eg when a car has been knocked into a spin. The camera will resume following the rotation
            // of the target when the target's angular velocity slows below the threshold.
            var currentFlatAngle = Mathf.Atan2(targetForward.x, targetForward.z) * Mathf.Rad2Deg;
            if (SpinTurnLimit > 0)
            {
                var targetSpinSpeed = Mathf.Abs(Mathf.DeltaAngle(LastFlatAngle, currentFlatAngle)) / deltaTime;
                var desiredTurnAmount = Mathf.InverseLerp(SpinTurnLimit, SpinTurnLimit * 0.75f, targetSpinSpeed);
                var turnReactSpeed = (CurrentTurnAmount > desiredTurnAmount ? .1f : 1f);
                if (Application.isPlaying)
                {
                    CurrentTurnAmount = Mathf.SmoothDamp(CurrentTurnAmount, desiredTurnAmount,
                                                         ref TurnSpeedVelocityChange, turnReactSpeed);
                }
                else
                {
                    // for editor mode, smoothdamp won't work because it uses deltaTime internally
                    CurrentTurnAmount = desiredTurnAmount;
                }
            }
            else
            {
                CurrentTurnAmount = 1;
            }
            LastFlatAngle = currentFlatAngle;
        }

        private void FollowTargetVelocity(ref Vector3 targetForward, ref Vector3 targetUp)
        {
            // in follow velocity mode, the camera's rotation is aligned towards the object's velocity direction
            // but only if the object is traveling faster than a given threshold.

            if (targetRigidbody.velocity.magnitude > TargetVelocityLowerLimit)
            {
                // velocity is high enough, so we'll use the target's velocty
                targetForward = targetRigidbody.velocity.normalized;
            }
            targetUp = Vector3.up;
            CurrentTurnAmount = Mathf.SmoothDamp(CurrentTurnAmount, 1, ref TurnSpeedVelocityChange, SmoothTurnTime);
        }
    }
}
