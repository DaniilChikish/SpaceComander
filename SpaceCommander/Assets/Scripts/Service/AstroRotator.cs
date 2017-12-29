using UnityEngine;

namespace SpaceCommander.Service
{
    class AstroRotator : MonoBehaviour
    {
        [SerializeField]
        private Vector3 aroundPoint;
        [SerializeField]
        private Vector3 axiss;
        [SerializeField]
        private float speedAroundAxiss;
        [SerializeField]
        private float speedAroundSelf;
        private void Update()
        {
            this.transform.RotateAround(aroundPoint, axiss, speedAroundAxiss * Time.deltaTime);
            this.transform.Rotate(Vector3.up, speedAroundSelf * Time.deltaTime);
        }
    }
}
