using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DeusUtility.Random;

namespace SpaceCommander.Weapons
{
    public class BigCannon : RoundWeapon
    {
        public BigShellType AmmoType;
        public override void StatUp()
        {
            type = WeaponType.Cannon;
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            range = 400;
            dispersion = 0.05f;
            shildBlinkTime = 0.2f;
            firerate = 45;
            ammoCampacity = 30;
            ammo = ammoCampacity;
            reloadingTime = 20;
            PreAiming = true;
            averageRoundSpeed = 133.33f;
        }
        protected override void Shoot(Transform target)
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

            shell.transform.localScale = shell.transform.localScale * 3;

            float speed, damage, armorPiersing, mass;
            bool canRicochet = false;
            GameObject explosionPrefab = null;
            switch (AmmoType)
            {
                case BigShellType.HigExplosive:
                    {
                        speed = 100f;
                        damage = 30f;
                        armorPiersing = 3f;
                        mass = 10f;
                        explosionPrefab = Global.ExplosiveBlast;
                        break;
                    }
                case BigShellType.UraniumIngot:
                    {
                        speed = 100f;
                        damage = 70f;
                        armorPiersing = 5f;
                        mass = 15;
                        break;
                    }
                case BigShellType.WolframIngot:
                default:
                    {
                        speed = 100f;
                        damage = 60f;
                        armorPiersing = 7;
                        mass = 11f;
                        canRicochet = true;
                        break;
                    }
            }

            shell.GetComponent<IShell>().StatUp(speed, damage, armorPiersing, mass, canRicochet, explosionPrefab);
        }
    }
}
