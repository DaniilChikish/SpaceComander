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

        protected override void StatUp()
        {
            base.StatUp();
            type = WeaponType.Laser;
            //gameObject.GetComponent<MeshRenderer>().enabled = true;
            beam = gameObject.transform.GetChild(0).gameObject;
            beam.GetComponent<LaserImpulse>().StatUp(AmmoType);
        }
        protected override void UpdateLocal()
        {
            if (impulseBackount > 0)
            {
                this.GetComponentInChildren<ParticleSystem>().Play();
                heat += Time.deltaTime * 25 * ((heat + 20) / maxHeat);
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
