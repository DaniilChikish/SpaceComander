using DeusUtility.Random;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpaceCommander.Weapons
{
    public class MagnetohydrodynamicCannon : EnergyWeapon
    {
        protected override void StatUp()
        {
            base.StatUp();
            type = WeaponType.MagnetohydrodynamicGun;
        }
        protected override void UpdateLocal()
        {
            Firerate = (200 + 800 * (heat / maxHeat));
        }
        protected override void Shoot(Transform target)
        {
            float damage = 10f;
            float armorPiersing = 9f;
            float mass = 20f;
            heat += 3;
            float localDisp = (Dispersion * heat / 25);
            Quaternion dispersionDelta = RandomDirectionNormal(localDisp);

            GameObject shell = Instantiate(Global.MagnetoShell, gameObject.transform.position, this.transform.rotation * dispersionDelta);
            shell.GetComponent<IShell>().StatUp(owner.Velocity + (RoundSpeed * (dispersionDelta * this.transform.forward)), damage * (1 + DamageMultiplacator), armorPiersing * (1 + APMultiplacator), mass, false, null);
            ownerBody.AddForceAtPosition(-this.transform.forward * mass * RoundSpeed, this.transform.position, ForceMode.Impulse);
        }
    }
}


