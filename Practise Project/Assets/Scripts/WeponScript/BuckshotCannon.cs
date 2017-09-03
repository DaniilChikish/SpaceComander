using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DeusUtility.Random;

namespace SpaceCommander.Weapons
{
    public class BuckshotCannon : RoundWeapon
    {
        private int bucksotRate;
        public override void StatUp()
        {
            type = WeaponType.ShootCannon;
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            range = 150;
            dispersion = 0.7f;
            shildBlinkTime = 0.1f;
            firerate = 70;
            ammoCampacity = 50;
            ammo = ammoCampacity;
            reloadingTime = 15;
            PreAiming = true;
            averageRoundSpeed = 100f;
            bucksotRate = 7;
        }
        protected override void Shoot(Transform target)
        {
            for (int i = 0; i < bucksotRate; i++)
            {
                Quaternion direction = transform.rotation;
                double[] randomOffset = Randomizer.Uniform(10, 90, 2);
                if (randomOffset[0] > 50)
                    direction.x = direction.x + (Convert.ToSingle(Global.RandomNormalPool[Convert.ToInt32(randomOffset[0])] - Convert.ToSingle(Global.RandomNormalAverage)) * dispersion);
                else
                    direction.x = direction.x + (Convert.ToSingle(Global.RandomNormalPool[Convert.ToInt32(randomOffset[0])] - Convert.ToSingle(Global.RandomNormalAverage)) * -dispersion);
                if (randomOffset[1] > 50)
                    direction.y = direction.y + (Convert.ToSingle(Global.RandomNormalPool[Convert.ToInt32(randomOffset[1])] - Convert.ToSingle(Global.RandomNormalAverage)) * dispersion);
                else
                    direction.y = direction.y + (Convert.ToSingle(Global.RandomNormalPool[Convert.ToInt32(randomOffset[1])] - Convert.ToSingle(Global.RandomNormalAverage)) * -dispersion);
                GameObject shell = Instantiate(Global.UnitaryShell, gameObject.transform.position, direction);

                float speed, damage, armorPiersing, mass;
                bool canRicochet = true;
                GameObject explosionPrefab = null;
                speed = 100f;
                damage = 5f;
                armorPiersing = 1;
                mass = 0.6f;

                shell.GetComponent<IShell>().StatUp(speed, damage, armorPiersing, mass, canRicochet, explosionPrefab);
            }
        }
    }
}

