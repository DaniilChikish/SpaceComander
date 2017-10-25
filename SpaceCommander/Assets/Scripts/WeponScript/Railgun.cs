using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpaceCommander.Weapons
{
    public class Railgun : ShellWeapon {
        protected override void StatUp()
        {
            base.StatUp();
            type = WeaponType.Railgun;
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        protected override void Shoot(Transform target)
        {
            float damage = 400f;
            float armorPiersing = 6f;
            float mass = 400f;

            Quaternion direction = transform.rotation;
            GameObject shell = Instantiate(Global.RailgunShell, gameObject.transform.position, direction);

            shell.GetComponent<IShell>().StatUp(owner.Velocity + (RoundSpeed * (1 + RoundspeedMultiplacator) * this.transform.forward), damage * (1 + DamageMultiplacator), armorPiersing * (1 + APMultiplacator), mass * (1 + ShellmassMultiplacator), true, null);
            ownerBody.AddForceAtPosition(-this.transform.forward * mass * RoundSpeed, this.transform.position, ForceMode.Impulse);
        }
    }
}
