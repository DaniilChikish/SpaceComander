using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpaceCommander.Weapons
{
    public class Railmortar : MagWeapon
    {
        private SpaceShip owner;
        public float DropImpulse;
        public override void StatUp()
        {
            type = WeaponType.Railmortar;
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            range = 150;
            ammoCampacity = 10;
            ammo = AmmoCampacity;
            firerate = 60;
            reloadingTime = 30;
            dispersion = 3f;
            shildBlinkTime = 1f;
            PreAiming = false;
            owner = this.transform.GetComponentInParent<SpaceShip>();
        }
        protected override void Shoot(Transform target)
        {
            Quaternion direction = transform.rotation;
            GameObject mine = Instantiate(Global.MagMine, gameObject.transform.position, direction);
            mine.GetComponent<MagnetoMine>().team = owner.team;
            mine.GetComponent<Rigidbody>().AddForce(transform.forward * DropImpulse * (1 + RangeMultiplacator), ForceMode.Impulse);
        }
    }
}

