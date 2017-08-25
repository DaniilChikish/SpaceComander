using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DeusUtility.Random;

namespace SpaceCommander.Weapons
{
    public class MediumCannon : RoundWeapon
    {
        public MediumShellType AmmoType;
        public override void StatUp()
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            range = 355;
            dispersion = 0.05f;
            shildBlinkTime = 0.15f;
            firerate = 100;
            ammoCampacity = 60;
            ammo = ammoCampacity;
            reloadingTime = 15;
            PreAiming = true;
            averageRoundSpeed = 133.33f;
        }
        protected override void Shoot(Transform target)
        {
            ammo -= 1;
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

            shell.transform.localScale = shell.transform.localScale * 2;

            float speed, damage, armorPiersing, mass;
            bool canRicochet = false;
            GameObject explosionPrefab = null;
            switch (AmmoType)
            {
                case MediumShellType.Camorous:
                    {
                        speed = 110f;
                        damage = 35f;
                        armorPiersing = 3f;
                        mass = 5f;
                        explosionPrefab = Global.ShellBlast;
                        break;
                    }
                case MediumShellType.Subcaliber:
                    {
                        speed = 110f;
                        damage = 40f;
                        armorPiersing = 7;
                        mass = 4;
                        break;
                    }
                case MediumShellType.CamorousAP:
                default:
                    {
                        speed = 110f;
                        damage = 35f;
                        armorPiersing = 5;
                        mass = 5.5f;
                        canRicochet = true;
                        explosionPrefab = Global.ShellBlast;
                        break;
                    }
            }

            shell.GetComponent<IShell>().StatUp(speed, damage, armorPiersing, mass, canRicochet, explosionPrefab);
        }
    }
}
