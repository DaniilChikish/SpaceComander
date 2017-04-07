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
        private SpaceShip owner;
        // Use this for initialization
        void Start()
        {
            owner = transform.GetComponentInParent<SpaceShip>();
        }

        // Update is called once per frame
        void Update()
        {
            if (hitpoints < 0)
                owner.Die();
        }
        protected void OnCollisionStay(Collision collision)
        {
            //Debug.Log("Hit armor");
            //if (owner.GetShieldRef.force < 0)
            //{
            float multiplicator;
            switch (collision.gameObject.tag)
            {
                case "Shell":
                    {
                        float difference = collision.gameObject.GetComponent<IShell>().ArmorPiersing - shellResist;
                        if (difference > 1.5)
                            multiplicator = 1.2f;
                        else if (difference > -3)
                            multiplicator = (Mathf.Sin((difference / 1.4f) + 0.5f) + 1f) * 0.6f;
                        else
                            multiplicator = 0.0f;
                        this.hitpoints -= collision.gameObject.GetComponent<IShell>().Damage * multiplicator;
                        break;
                    }
                case "Energy":
                    {
                        float difference = collision.gameObject.GetComponent<IEnergy>().ArmorPiersing - energyResist;
                        if (difference > 0.5)
                            multiplicator = 1f;
                        else if (difference > -3)
                            multiplicator = (Mathf.Sin((difference / 1.1f) + 1f) + 1f) * 0.5f;
                        else
                            multiplicator = 0.0f;
                        this.hitpoints -= collision.gameObject.GetComponent<IEnergy>().Damage * multiplicator;
                        break;
                    }
            }
            //}
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
