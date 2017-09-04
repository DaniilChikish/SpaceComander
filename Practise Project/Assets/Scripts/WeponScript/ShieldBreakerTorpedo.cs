using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCommander.Weapons
{
    class ShieldBreakerTorpedo : Torpedo
    {
        protected override void Start()
        {
            Speed = 25f;// скорость ракеты      
            TurnSpeed = 3f;// скорость поворота ракеты            
            DropImpulse = 4000f;//импульс сброса                  
            explosionRange = 25f; //расстояние детонации
            gameObject.GetComponent<Rigidbody>().AddForce(-transform.up * DropImpulse, ForceMode.Impulse);
            Global = FindObjectOfType<GlobalController>();
            lt = 0;
        }
        public override void Explode()
        {
            foreach (Unit x in Global.unitList)
            {
                if (Vector3.Distance(this.transform.position, x.transform.position) < 50)
                    x.ShieldForce -= (x.ShieldCampacity+1);
            }
            Instantiate(Global.EMIExplosionPrefab, this.transform.position, this.transform.rotation);
            GameObject blast = Instantiate(FindObjectOfType<GlobalController>().ExplosiveBlast, this.transform.position, this.transform.rotation);
            blast.GetComponent<Explosion>().StatUp(BlastType.Missile);
            Destroy(gameObject);
        }
    }
}
