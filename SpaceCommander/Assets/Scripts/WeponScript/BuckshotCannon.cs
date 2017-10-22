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
            for (int i = 0; i < bucksotRate; i++)
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
                bool canRicochet = true;
                GameObject explosionPrefab = null;
                speed = roundSpeed;
                damage = 5f;
                armorPiersing = 1;
                mass = 0.2f;

                shell.GetComponent<IShell>().StatUp(owner.Velocity + (speed * (1 + RoundspeedMultiplacator) * this.transform.forward), damage * (1 + DamageMultiplacator), armorPiersing * (1 + APMultiplacator), mass * (1 + ShellmassMultiplacator), canRicochet, explosionPrefab);
            }
        }
    }
}

