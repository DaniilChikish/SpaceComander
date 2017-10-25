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
        public float damage;
        public float armorPiersing;
        public float ArmorPiersing { get { return armorPiersing * (1 + APMultiplacator); } }
        public float Damage { get { return damage * (1 + DamageMultiplacator); } }


        protected override void StatUp()
        {
            base.StatUp();
            type = WeaponType.Laser;
            damage = 10;
            armorPiersing = 4;
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
                float dist = Range;
                RaycastHit[] hits = Physics.RaycastAll(this.transform.position, this.transform.forward);
                foreach (RaycastHit x in hits)
                {
                    if (!x.collider.isTrigger)
                    {
                        dist = Vector3.Distance(gameObject.transform.position, x.point) + 0.5f;
                            Unit enemy = x.collider.gameObject.GetComponentInParent<Unit>();

                        if (enemy!=null)
                        {
                            float difference = this.ArmorPiersing - enemy.EnergyResist;
                            float multiplicator;
                            if (difference > 0.5)
                                multiplicator = 1f;
                            else if (difference > -3)
                                multiplicator = (Mathf.Sin((difference / 1.1f) + 1f) + 1f) * 0.5f;
                            else
                                multiplicator = 0.0f;
                            enemy.MakeDamage(Damage * multiplicator * Time.deltaTime);
                        }
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
        protected override void Shoot(Transform target)
        {
            //GlobalController Global = FindObjectsOfType<GlobalController>()[0];
            impulseBackount = 0.1f;
        }

        public override void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
