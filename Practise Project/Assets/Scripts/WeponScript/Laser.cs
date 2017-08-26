using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpaceCommander.Weapons
{
    public class Laser : EnergyWeapon
    {
        public EnergyType AmmoType;
        [SerializeField]
        private LayerMask ignoreMask; // фильтр по слоям
        private float impulseBackount;
        private GameObject beam;

        public override void StatUp()
        {
            type = WeaponType.Laser;
            //gameObject.GetComponent<MeshRenderer>().enabled = false;
            range = 350;
            dispersion = 0.00001f;//exponential
            shildBlinkTime = 0.05f;
            averageRoundSpeed = 1000000;
            maxHeat = 20;
            firerate = 6000;
            PreAiming = true;
            beam = gameObject.transform.GetChild(0).gameObject;
            beam.GetComponent<LaserImpulse>().StatUp(AmmoType);
        }
        protected override void UpdateLocal()
        {
            if (impulseBackount > 0 && target !=null)
            {
                this.GetComponentInChildren<ParticleSystem>().Play();
                heat += Time.deltaTime * 0.7f;
                float dist = range;
                RaycastHit[] hits = Physics.RaycastAll(this.transform.position, this.transform.forward);
                foreach (RaycastHit x in hits)
                {
                    if (!x.collider.isTrigger)
                    {
                        dist = Vector3.Distance(gameObject.transform.position, x.point) + 0.5f;
                        //break;
                    }
                }

                beam.transform.localScale = new Vector3(1, 1, dist);
                beam.transform.position = this.transform.position + this.transform.forward.normalized * dist/2;

                beam.GetComponent<IEnergy>().StatUp(AmmoType);
                impulseBackount -= Time.deltaTime;

                beam.SetActive(true);
            }
            else beam.SetActive(false);
        }
        public override bool IsReady { get { return (!overheat && backCount <= 0 &&!beam.activeInHierarchy); } }

        protected override void Shoot(Transform target)
        {
            //GlobalController Global = FindObjectsOfType<GlobalController>()[0];
            impulseBackount = 1;
        }

        public override void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
