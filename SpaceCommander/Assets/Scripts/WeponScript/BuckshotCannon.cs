using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DeusUtility.Random;

namespace SpaceCommander.Weapons
{
    public class BuckshotCannon : MagWeapon
    {
        private int bucksotRate;
        protected override void StatUp()
        {
            base.StatUp();
            type = WeaponType.ShootCannon;
            bucksotRate = 7;
        }
        protected override void Shoot(Transform target)
        {
                float damage, armorPiersing, mass;
                bool canRicochet = true;
                GameObject explosionPrefab = null;
                damage = 5f;
                armorPiersing = 1;
                mass = 0.2f;
            for (int i = 0; i < bucksotRate; i++)
            {
                Quaternion dispersionDelta = RandomDirectionNormal(Dispersion);
                GameObject shell = Instantiate(Global.UnitaryShell, gameObject.transform.position, this.transform.rotation * dispersionDelta);
                shell.GetComponent<IShell>().StatUp(owner.Velocity + (RoundSpeed * (1 + RoundspeedMultiplacator) * (dispersionDelta * this.transform.forward)), damage * (1 + DamageMultiplacator), armorPiersing * (1 + APMultiplacator), mass * (1 + ShellmassMultiplacator), canRicochet, explosionPrefab);
            }
            ownerBody.AddForceAtPosition(-this.transform.forward * mass * bucksotRate * RoundSpeed, this.transform.position, ForceMode.Impulse);
        }
    }
}

