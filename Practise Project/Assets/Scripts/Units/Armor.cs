using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PracticeProject
{
    public class Armor : MonoBehaviour
    {
        public float maxHitpoints;
        public float hitpoints;
        public float shellResist;
        public float energyResist;
        public float blastResist;
        private Unit owner;
        // Use this for initialization
        void Start()
        {
            owner = transform.GetComponentInParent<Unit>();
        }

        // Update is called once per frame
        void Update()
        {
            if (hitpoints < 0)
                owner.Die();
        }
        protected void OnCollisionEnter(Collision collision)
        {
            //Debug.Log("Hit armor");
            if (owner.GetShieldRef.force < 0)
            {
                float multiplicator;
                switch (collision.gameObject.tag)
                {
                    case "Shell":
                        {
                            multiplicator = 1 - shellResist;
                            this.hitpoints -= collision.gameObject.GetComponent<Round>().Damage * multiplicator;
                            break;
                        }
                    case "Energy":
                        {
                            multiplicator = 1 - energyResist;
                            this.hitpoints -= collision.gameObject.GetComponent<Round>().Damage * multiplicator;
                            break;
                        }
                }
            }
        }

        protected void OnTriggerStay(Collider trigger)
        {
            float multiplicator;
            switch (trigger.gameObject.tag)
            {
                case "Explosion":
                    {
                        multiplicator = (1 - blastResist) * Mathf.Pow(((-Vector3.Distance(this.gameObject.transform.position, trigger.gameObject.transform.position) + trigger.gameObject.GetComponent<Explosion>().MaxRadius) * 0.01f), (1 / 3));
                        this.hitpoints -= trigger.gameObject.GetComponent<Explosion>().Damage * multiplicator;
                        break;
                    }
            }
        }
    }
}
