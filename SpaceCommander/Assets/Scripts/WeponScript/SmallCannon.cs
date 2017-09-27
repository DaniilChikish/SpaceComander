using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DeusUtility.Random;

namespace SpaceCommander.Weapons
{
    public class SmallCannon : MagWeapon
    {
        public SmallShellType AmmoType;
        protected override void StatUp()
        {
            base.StatUp();
            type = WeaponType.Cannon;
        }
        protected override void Shoot(Transform target)
        {
            Quaternion direction = transform.rotation;
            double[] randomOffset = Randomizer.Uniform(10, 90, 2);
            if (randomOffset[0] > 50)
                direction.x = direction.x + (Convert.ToSingle(Global.RandomNormalPool[Convert.ToInt32(randomOffset[0])] - Convert.ToSingle(Global.RandomNormalAverage)) * Dispersion);
            else
                direction.x = direction.x + (Convert.ToSingle(Global.RandomNormalPool[Convert.ToInt32(randomOffset[0])] - Convert.ToSingle(Global.RandomNormalAverage)) * -Dispersion);
            if (randomOffset[1] > 50)
                direction.y = direction.y + (Convert.ToSingle(Global.RandomNormalPool[Convert.ToInt32(randomOffset[1])] - Convert.ToSingle(Global.RandomNormalAverage)) * Dispersion);
            else
                direction.y = direction.y + (Convert.ToSingle(Global.RandomNormalPool[Convert.ToInt32(randomOffset[1])] - Convert.ToSingle(Global.RandomNormalAverage)) * -Dispersion);
            GameObject shell = Instantiate(Global.UnitaryShell, gameObject.transform.position, direction);

            float speed, damage, armorPiersing, mass;
            bool canRicochet = false;
            GameObject explosionPrefab = null;
            switch (AmmoType)
            {

                case SmallShellType.SemiShell:
                    {
                        speed = 130f;
                        damage = 25f;
                        armorPiersing = 1.5f;
                        mass = 0.75f;
                        break;
                    }
                case SmallShellType.APShell:
                    {
                        speed = 130f;
                        damage = 15f;
                        armorPiersing = 4;
                        mass = 1.05f;
                        canRicochet = true;
                        break;
                    }
                case SmallShellType.Incendiary:
                    {
                        speed = 130f;
                        damage = 5f;
                        armorPiersing = 1;
                        mass = 1;
                        explosionPrefab = Global.ShellBlast;
                        break;
                    }
                case SmallShellType.BuckShot:
                    {
                        speed = 100f;
                        damage = 30f;
                        armorPiersing = 1;
                        mass = 0.6f;
                        canRicochet = true;
                        break;
                    }
                case SmallShellType.Solid:
                default:
                    {
                        speed = 130f;
                        damage = 20f;
                        armorPiersing = 2;
                        mass = 1;
                        canRicochet = true;
                        break;
                    }
            }

            shell.GetComponent<IShell>().StatUp(speed * (1 + RoundspeedMultiplacator), damage * (1 + DamageMultiplacator), armorPiersing * (1 + APMultiplacator), mass * (1 + ShellmassMultiplacator), canRicochet, explosionPrefab);
        }
    }
}
