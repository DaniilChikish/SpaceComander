using UnityEngine;

namespace SpaceCommander.Service
{
    class FarFarObject : MonoBehaviour
    {
        public GameObject cam;
        public float distance;
        [SerializeField]
        private Vector3 anchorThis;
        [SerializeField]
        private Vector3 anchorCam;
        private void Start()
        {
            cam = Camera.main.gameObject;
            anchorThis = this.transform.position;
            anchorCam = cam.transform.position;
        }
        private void Update()
        {
            if (cam)
            {
                this.transform.position = anchorThis + ((cam.transform.position - anchorCam) * ((distance - anchorThis.magnitude) / distance));
                //this.transform.right = -cam.transform.right;
            }
        }
    }
}
