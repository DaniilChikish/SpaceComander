using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpaceCommander.Service
{
    class LookOnCam : MonoBehaviour
    {
        public GameObject cam;
        private void Start()
        {
            cam = GameObject.Find("MapCam");
        }
        private void Update()
        {
            if (cam)
            {
                this.transform.forward = -cam.transform.forward;
                //this.transform.right = -cam.transform.right;
            }
        }
    }
}
