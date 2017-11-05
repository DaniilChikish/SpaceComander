using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DeusUtility.Random;
    /**
 * Средняя пушка
 * Физические параметры: (по образу Bordkanone 7,5)
 *      Скорострельность = 30 в/м
 *      Начальная скорость ~ 800 м/с
 *      Боезапас = 30
 *      Масса снаряда ~ 6кг
 * **/
namespace SpaceCommander.Weapons
{ 
    public class MediumCannon : MagWeapon
    {
        public MediumShellType AmmoType;
        protected override void StatUp()
        {
            base.StatUp();
            type = WeaponType.Cannon;
        }
        protected override void Shoot(Transform target)
        {
            Quaternion dispersionDelta = RandomDirectionNormal(Dispersion);

            GameObject shell = Instantiate(Global.Prefab.UnitaryShell, gameObject.transform.position, this.transform.rotation * dispersionDelta);

            shell.transform.localScale = shell.transform.localScale * 2;

            float damage, armorPiersing, mass;
            bool canRicochet = false;
            GameObject explosionPrefab = null;
            switch (AmmoType)
            {
                case MediumShellType.Camorous:
                    {
                        damage = 45f;
                        armorPiersing = 3f;
                        mass = 6f;
                        explosionPrefab = Global.Prefab.ShellBlast;
                        break;
                    }
                case MediumShellType.Subcaliber:
                    {
                        damage = 40f;
                        armorPiersing = 7;
                        mass = 5;
                        break;
                    }
                case MediumShellType.CamorousAP:
                default:
                    {
                        damage = 35f;
                        armorPiersing = 5;
                        mass = 6.2f;
                        canRicochet = true;
                        explosionPrefab = Global.Prefab.ShellBlast;
                        break;
                    }
            }

            shell.GetComponent<IShell>().StatUp(owner.Velocity + (RoundSpeed * ( dispersionDelta *this.transform.forward)), damage * (1 + DamageMultiplacator), armorPiersing * (1 + APMultiplacator), mass * (1 + ShellmassMultiplacator), canRicochet, explosionPrefab);
            ownerBody.AddForceAtPosition(-this.transform.forward * mass * RoundSpeed, this.transform.position, ForceMode.Impulse);
        }
    }
}
