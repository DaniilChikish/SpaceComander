using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpaceCommander.Weapons
{
    public class Railmortar : MagWeapon
    {
        public float DropImpulse;
        protected override void StatUp()
        {
            base.StatUp();
            type = WeaponType.Railmortar;
        }
        protected override void Shoot(Transform target)
        {
            Quaternion direction = transform.rotation;
            GameObject mine = Instantiate(Global.MagMine, gameObject.transform.position, direction);
            mine.GetComponent<MagnetoMine>().team = owner.Team;
            mine.GetComponent<Rigidbody>().AddForce(owner.Velocity + (DropImpulse * (1 + RoundspeedMultiplacator) * this.transform.forward), ForceMode.VelocityChange);
        }
    }
}

