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
        protected override void StatUp()
        {
            base.StatUp();
            type = WeaponType.Railmortar;
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

