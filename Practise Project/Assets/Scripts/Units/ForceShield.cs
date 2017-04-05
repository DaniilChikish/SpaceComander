using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PracticeProject
{
    public class ForceShield : MonoBehaviour
    {
        public float maxCampacity;
        public float force;
        public float recharging;
        public bool shildOwerheat;
        public float cooldownChield;
        public float firstBlinker;
        public float secondBlinker;
        MeshRenderer firstFieldRend;
        MeshCollider firstFieldColl;
        MeshRenderer secondField;
        // Use this for initialization
        void Start()
        {
            firstFieldRend = this.transform.FindChild("FirstField").GetComponent<MeshRenderer>();
            firstFieldColl = this.transform.FindChild("FirstField").GetComponent<MeshCollider>();
            secondField = this.transform.FindChild("SecondField").GetComponent<MeshRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            if (force < 0 && !shildOwerheat)
                Owerheat();
            else if (cooldownChield <= 0 && secondBlinker < 0 && force < maxCampacity)
            {
                force += recharging * Time.deltaTime;
                shildOwerheat = false;
                firstFieldRend.enabled = true;
                firstFieldColl.enabled = true;
            }
            if (cooldownChield > 0)
                cooldownChield -= Time.deltaTime;
            if (firstBlinker < 0 && !shildOwerheat)
                firstFieldColl.enabled = true;
            else firstBlinker -= Time.deltaTime;
            if (secondBlinker < 0)
                secondField.enabled = false;
            else secondBlinker -= Time.deltaTime;
        }
        protected void OnCollisionEnter(Collision collision)
        {
            //Debug.Log("Hit shield");
            if (!shildOwerheat)
            {
                switch (collision.gameObject.tag)
                {
                    case "Shell":
                        {
                            this.force -= collision.gameObject.GetComponent<Round>().Damage * 0.3f;
                            secondField.enabled = true;
                            secondBlinker = 0.5f;
                            break;
                        }
                    case "Energy":
                        {
                            this.force -= collision.gameObject.GetComponent<Round>().Damage * 1.5f;
                            secondField.enabled = true;
                            secondBlinker = 0.5f;
                            break;
                        }
                    case "Missile":
                        {
                            secondField.enabled = true;
                            secondBlinker = 0.5f;
                            break;
                        }
                }
            }
        }
        protected void OnTriggerStay(Collider collision)
        {
            //Debug.Log("Trigget shield");
            if (!shildOwerheat)
            {
                switch (collision.gameObject.tag)
                {
                    //case "Shell":
                    //    {
                    //        this.force -= collision.gameObject.GetComponent<Round>().Damage * 0.3f;
                    //        secondField.enabled = true;
                    //        secondBlinker = 0.5f;
                    //        collision.GetComponent<Rigidbody>().AddForce(this.transform.position - collision.transform.position, ForceMode.Impulse);
                    //        break;
                    //    }
                    //case "Energy":
                    //    {
                    //        this.force -= collision.gameObject.GetComponent<Round>().Damage * 1.5f;
                    //        secondField.enabled = true;
                    //        secondBlinker = 0.5f;
                    //        break;
                    //    }
                    //case "Missile":
                    //    {
                    //        secondField.enabled = true;
                    //        secondBlinker = 0.5f;
                    //        break;
                    //    }
                    case "Explosion":
                        {
                            this.force = this.force - collision.gameObject.GetComponent<Explosion>().Damage * 0.01f;
                            break;
                        }
                }
            }
        }
        public void Blink(float blink)
        {
            firstFieldColl.enabled = false;
            firstBlinker = blink;
        }
        public void Owerheat()
        {
            shildOwerheat = true;
            firstFieldRend.enabled = false;
            firstFieldColl.enabled = false;
            force = 0;
            cooldownChield = 2;
        }
    }
}
