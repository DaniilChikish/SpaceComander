using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCommander
{
    public class ForceShield : MonoBehaviour, IShield
    {
        [SerializeField]
        private float maxCampacity;
        [SerializeField]
        private float force;
        [SerializeField]
        private float recharging;
        [SerializeField]
        private bool isOverheat;
        [SerializeField]
        private float cooldownChield;
        [SerializeField]
        private float firstBlinker;
        [SerializeField]
        private float secondBlinker;
        [SerializeField]
        private float shootCount;
        MeshRenderer firstFieldRend;
        Collider firstFieldColl;
        MeshRenderer secondField;
        ParticleSystem shildCollaps;
        private SpaceShip owner;
        private GlobalController Global;

        public float Force { get { return force; } set { force = value; } }
        public float MaxCampacity { get { return maxCampacity; } set { maxCampacity = value; } }
        public float Recharging { get { return recharging; } set { recharging = value; } }
        public bool IsOverheat { get { return isOverheat; } }
        void Start()
        {
            firstFieldRend = this.transform.FindChild("FirstField").GetComponent<MeshRenderer>();
            firstFieldColl = this.transform.FindChild("FirstField").GetComponent<Collider>();
            shildCollaps = this.transform.FindChild("FirstField").GetComponentInChildren<ParticleSystem>();
            secondField = this.transform.FindChild("SecondField").GetComponent<MeshRenderer>();
            owner = transform.GetComponentInParent<SpaceShip>();
            Global = FindObjectOfType<GlobalController>();
        }

        // Update is called once per frame
        void Update()
        {
            if (shootCount > 0)
                shootCount = shootCount * 0.95f;
            if (force < 0 && !isOverheat)
                Owerheat();
            else if (cooldownChield <= 0 && secondBlinker <= 0 && force < owner.ShieldCampacity)
            {
                force += owner.ShieldRecharging * Time.deltaTime;
            }
            if (cooldownChield > 0)
                cooldownChield -= Time.deltaTime;
            if (!isOverheat)
            {
                if (firstBlinker <= 0)
                    firstFieldColl.enabled = true;
                else firstBlinker -= Time.deltaTime;
            }
            else if (force > maxCampacity * 0.1)
                Reload();
            if (secondBlinker <= 0)
                secondField.enabled = false;
            else secondBlinker -= Time.deltaTime;
        }
        //protected void OnCollisionEnter(Collision collision)
        //{
        //    //Debug.Log("Hit shield");
        //    if (!shildOwerheat)
        //    {
        //        switch (collision.gameObject.tag)
        //        {
        //            case "Shell":
        //                {
        //                    this.force -= collision.gameObject.GetComponent<Round>().Damage * 0.3f;
        //                    secondField.enabled = true;
        //                    secondBlinker = 0.5f;
        //                    break;
        //                }
        //            case "Energy":
        //                {
        //                    this.force -= collision.gameObject.GetComponent<Round>().Damage * 1.5f;
        //                    secondField.enabled = true;
        //                    secondBlinker = 0.5f;
        //                    break;
        //                }
        //            case "Missile":
        //                {
        //                    secondField.enabled = true;
        //                    secondBlinker = 0.5f;
        //                    break;
        //                }
        //        }
        //    }
        //}
        protected void OnTriggerEnter(Collider collision)
        {

            if (!isOverheat)
            {
                switch (collision.gameObject.tag)
                {
                    case "Shell":
                        {
                            Rigidbody shell = collision.GetComponent<Rigidbody>();
                            shell.velocity = shell.velocity / 2;
                            shootCount += 1 + (shell.mass * 0.2f);
                            break;
                        }
                    case "Energy":
                        {
                            break;
                        }
                    case "Missile":
                        {
                            SelfguidedMissile armtrigger = collision.GetComponent<SelfguidedMissile>();
                            if (armtrigger) armtrigger.Arm();
                            break;
                        }
                    case "Explosion":
                        {
                            this.force = this.force - collision.gameObject.GetComponent<Explosion>().Damage * 0.01f;
                            break;
                        }
                }
            }
        }
        protected void OnTriggerStay(Collider collision)
        {
            if (!isOverheat)
            {
                switch (collision.gameObject.tag)
                {
                    case "Shell":
                        {
                            Rigidbody shell = collision.GetComponent<Rigidbody>();
                            this.force -= shell.mass * (1 + shootCount / 8);
                            secondField.enabled = true;
                            secondBlinker = 0.5f;
                            //shell.AddForce((collision.transform.position-this.transform.position).normalized * Mathf.Sqrt(shell.velocity.magnitude * maxCampacity), ForceMode.Impulse);//velocity = collision.GetComponent<Rigidbody>().velocity / 2;
                            shell.velocity = shell.velocity / 2 + (collision.transform.position - this.transform.position).normalized * Mathf.Sqrt(shell.velocity.magnitude * maxCampacity);
                            break;
                        }
                    case "Energy":
                        {
                            this.force -= collision.gameObject.GetComponent<IEnergy>().GetEnergy();
                            secondField.enabled = true;
                            secondBlinker = 0.5f;
                            break;
                        }
                    case "Missile":
                        {
                            secondField.enabled = true;
                            Rigidbody shell = collision.GetComponent<Rigidbody>();
                            SelfguidedMissile armtrigger = collision.GetComponent<SelfguidedMissile>();
                                if (armtrigger) armtrigger.Arm();
                            secondBlinker = 1.5f;
                            shell.AddForce((collision.transform.position - this.transform.position).normalized * Mathf.Sqrt(shell.mass * maxCampacity * 10), ForceMode.Force);//velocity = collision.GetComponent<Rigidbody>().velocity / 2;
                            break;
                        }
                    case "Explosion":
                        {
                            this.force = this.force - collision.gameObject.GetComponent<Explosion>().Damage * 0.1f * Time.deltaTime;
                            break;
                        }
                }
            }
        }
        public void Blink(float blink)
        {
            firstBlinker = blink;
            firstFieldColl.enabled = false;
        }
        public void Owerheat()
        {
            isOverheat = true;
            firstFieldRend.enabled = false;
            firstFieldColl.enabled = false;
            shildCollaps.Play();
            force = 0;
            cooldownChield = 2;
        }
        public void Reload()
        {
            isOverheat = false;
            firstFieldRend.enabled = true;
            firstFieldColl.enabled = true;
        }
    }
}
