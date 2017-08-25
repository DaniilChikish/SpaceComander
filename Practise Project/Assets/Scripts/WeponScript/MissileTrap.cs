using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpaceCommander.Weapons
{
    class MissileTrap : MonoBehaviour
    {
        private float ttl;
        public void Start()
        {
            ttl = 5;
            gameObject.GetComponent<Rigidbody>().AddForce(-transform.up * 30, ForceMode.Impulse);
        }
        private void Update()
        {
            if (ttl > 0)
                ttl -= Time.deltaTime;
            else
                Destroy(this.gameObject);
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Missile")
                Destroy(this.gameObject);
        }
    }
}
