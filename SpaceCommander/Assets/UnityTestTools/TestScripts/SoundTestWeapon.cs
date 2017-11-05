using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpaceCommander.Test
{
    class SoundTestWeapon : MagWeapon
    {
        public SmallShellType AmmoType;
        public AudioSource sound;
        protected override void StatUp()
        {
            base.StatUp();
            type = WeaponType.Cannon;
            sound = GetComponent<AudioSource>();
        }
        protected override void Shoot(Transform target)
        {
            Quaternion dispersionDelta = RandomDirectionNormal(Dispersion);

            GameObject shell = Instantiate(Global.Prefab.UnitaryShell, gameObject.transform.position, this.transform.rotation * dispersionDelta);
            sound.Play();
            float damage, armorPiersing, mass;
            bool canRicochet = false;
            GameObject explosionPrefab = null;
            switch (AmmoType)
            {
                case SmallShellType.SemiShell:
                    {
                        damage = 25f;
                        armorPiersing = 1.5f;
                        mass = 0.75f;
                        break;
                    }
                case SmallShellType.APShell:
                    {
                        damage = 15f;
                        armorPiersing = 4;
                        mass = 1f;
                        canRicochet = true;
                        break;
                    }
                case SmallShellType.Incendiary:
                    {
                        damage = 5f;
                        armorPiersing = 1;
                        mass = 0.9f;
                        explosionPrefab = Global.Prefab.ShellBlast;
                        break;
                    }
                case SmallShellType.BuckShot:
                    {
                        damage = 30f;
                        armorPiersing = 1;
                        mass = 0.6f;
                        canRicochet = true;
                        break;
                    }
                case SmallShellType.Solid:
                default:
                    {
                        damage = 20f;
                        armorPiersing = 2;
                        mass = 0.8f;
                        canRicochet = true;
                        break;
                    }
            }

            shell.GetComponent<IShell>().StatUp(owner.Velocity + (RoundSpeed * (dispersionDelta * this.transform.forward)), damage * (1 + DamageMultiplacator), armorPiersing * (1 + APMultiplacator), mass * (1 + ShellmassMultiplacator), canRicochet, explosionPrefab);
            ownerBody.AddForceAtPosition(-this.transform.forward * mass * RoundSpeed, this.transform.position, ForceMode.Impulse);
        }
    }
}

