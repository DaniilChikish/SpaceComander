using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeusUtility.UI;


namespace SpaceCommander
{
    class WarpArrive : MonoBehaviour
    {
        public GameObject ArriveUnit;
        private GlobalController Global;
        public bool isArrive;
        public Vector3 ancor;
        public float Speed;// скорость ракеты
        private void Start()
        {
            Global = GlobalController.GetInstance();
            ancor = this.gameObject.transform.position;
            Sleep();
        }
        public void Update()
        {
            if (isArrive)
            {
                if (Vector3.Distance(this.transform.position, ancor) < (Speed * 1.1f))
                    Instant();
                //полет по прямой
                gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * Speed);
            }
        }
        private void OnGUI()
        {
            if (isArrive)
            {
                FindObjectOfType<HUDBase>().ShowExclamation(Global.Texts("Reinforcements arrive!"));
            }
        }

        private void Instant()
        {
            Instantiate(ArriveUnit, this.transform.position, this.transform.rotation);
            Sleep();
        }

        private void Sleep()
        {
            this.GetComponent<MeshRenderer>().enabled = false;
            isArrive = false;
            this.transform.GetComponentInChildren<ParticleSystem>().Stop();
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            this.gameObject.transform.position = ancor;
        }

        public void Arrive()
        {
            if (!isArrive)
            {
                isArrive = true;
                this.transform.position = this.transform.position + this.transform.forward.normalized * (-1000);
                this.GetComponent<MeshRenderer>().enabled = true;
                this.transform.GetComponentInChildren<ParticleSystem>().Play();
            }
        }
    }
}
