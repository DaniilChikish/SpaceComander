using SpaceCommander.General;
using SpaceCommander.Mechanics;
using UnityEngine;

namespace SpaceCommander.Mechanics.Weapons
{
    public class BuckshotCannon : MagWeapon
    {
        private int bucksotRate;
        protected override void StatUp()
        {
            base.StatUp();
            type = WeaponType.ShootCannon;
            bucksotRate = 20;
        }
        protected override void Shoot(Transform target)
        {
                float damage, armorPiersing, mass;
                bool canRicochet = true;
                GameObject explosionPrefab = null;
                damage = 5f;
                armorPiersing = 1.5f;
                mass = 0.2f;
            for (int i = 0; i < bucksotRate; i++)
            {
                Quaternion dispersionDelta = RandomDirectionNormal(Dispersion);
                GameObject shell = Instantiate(Global.Prefab.Buckshot, gameObject.transform.position, this.transform.rotation * dispersionDelta);
                shell.GetComponent<IShell>().StatUp(owner.Velocity + (RoundSpeed * (1 + RoundspeedMultiplacator) * (dispersionDelta * this.transform.forward)), damage * (1 + DamageMultiplacator), armorPiersing * (1 + APMultiplacator), mass * (1 + ShellmassMultiplacator), canRicochet, explosionPrefab);
            }
            ownerBody.AddForceAtPosition(-this.transform.forward * mass * bucksotRate * RoundSpeed, this.transform.position, ForceMode.Impulse);
        }
    }
}

