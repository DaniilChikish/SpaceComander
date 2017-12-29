using UnityEngine;

namespace SpaceCommander.Service
{
    class InfinityFarObject : MonoBehaviour
    {
        public GameObject cam;
        private void Start()
        {
            cam = Camera.main.gameObject;
        }
        private void Update()
        {
            if (cam)
            {
                this.transform.position = cam.transform.position;
                //this.transform.right = -cam.transform.right;
            }
        }
    }
}
