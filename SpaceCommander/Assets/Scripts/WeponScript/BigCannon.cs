using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DeusUtility.Random;

namespace SpaceCommander.Weapons
{
    public class BigCannon : MagWeapon
    {
        public BigShellType AmmoType;
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

            shell.transform.localScale = shell.transform.localScale * 3;

            float speed = roundSpeed, damage, armorPiersing, mass;
            bool canRicochet = false;
            GameObject explosionPrefab = null;
            switch (AmmoType)
            {
                case BigShellType.HigExplosive:
                    {
                        damage = 30f;
                        armorPiersing = 3f;
                        mass = 10f;
                        explosionPrefab = Global.ExplosiveBlast;
                        break;
                    }
                case BigShellType.UraniumIngot:
                    {
                        damage = 70f;
                        armorPiersing = 5f;
                        mass = 15;
                        break;
                    }
                case BigShellType.WolframIngot:
                default:
                    {
                        damage = 60f;
                        armorPiersing = 7;
                        mass = 11f;
                        canRicochet = true;
                        break;
                    }
            }

            shell.GetComponent<IShell>().StatUp(speed * (1 + RoundspeedMultiplacator), damage * (1 + DamageMultiplacator), armorPiersing * (1 + APMultiplacator), mass * (1 + ShellmassMultiplacator), canRicochet, explosionPrefab);
        }
    }
}
