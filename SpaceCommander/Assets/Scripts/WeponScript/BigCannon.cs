using UnityEngine;
using SpaceCommander.Mechanics;
using SpaceCommander.General;

namespace SpaceCommander.Mechanics.Weapons
{
    public class BigCannon : MagWeapon
    {
        public BigShellType AmmoType;
        protected override void StatUp()
        {
            base.StatUp();
            type = WeaponType.Cannon;
            audio.minDistance = 30;
            audio.maxDistance = 3000;
        }
        protected override void Shoot(Transform target)
        {
            Quaternion dispersionDelta = RandomDirectionNormal(Dispersion);

            GameObject shell = Instantiate(Global.Prefab.UnitaryShell, gameObject.transform.position, this.transform.rotation * dispersionDelta);

            shell.transform.localScale = shell.transform.localScale * 3;

            float damage, armorPiersing, mass;
            bool canRicochet = false;
            GameObject explosionPrefab = null;
            switch (AmmoType)
            {
                case BigShellType.HigExplosive:
                    {
                        damage = 30f;
                        armorPiersing = 3f;
                        mass = 20f;
                        explosionPrefab = Global.Prefab.ExplosiveBlast;
                        break;
                    }
                case BigShellType.UraniumIngot:
                    {
                        damage = 70f;
                        armorPiersing = 5f;
                        mass = 30;
                        break;
                    }
                case BigShellType.WolframIngot:
                default:
                    {
                        damage = 60f;
                        armorPiersing = 7;
                        mass = 25f;
                        canRicochet = true;
                        break;
                    }
            }

            shell.GetComponent<IShell>().StatUp(owner.Velocity + (RoundSpeed * (dispersionDelta * this.transform.forward)), damage * (1 + DamageMultiplacator), armorPiersing * (1 + APMultiplacator), mass * (1 + ShellmassMultiplacator), canRicochet, explosionPrefab, 10f);
            ownerBody.AddForceAtPosition(-this.transform.forward * mass * RoundSpeed, this.transform.position, ForceMode.Impulse);
        }
    }
}
