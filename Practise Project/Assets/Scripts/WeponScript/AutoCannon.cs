using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DeusUtility.Random;

namespace SpaceCommander.Weapons
{
    public class AutoCannon : RoundWeapon
    {
        public ShellLineType AmmoType;
        public SmallShellType[] ShellLine;
        public int shellPosition;
        public override void StatUp()
        {
            type = WeaponType.Cannon;
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            range = 200;
            ammoCampacity = 250; //~2 Min
            ammo = ammoCampacity;
            firerate = 300;
            reloadingTime = 20;
            dispersion = 0.3f;
            shildBlinkTime = 0.1f;
            averageRoundSpeed = 130;
            PreAiming = true;
            switch (AmmoType)
            {
                case ShellLineType.Solid:
                    {
                        ShellLine = new SmallShellType[5];
                        ShellLine[0] = SmallShellType.SemiShell;
                        ShellLine[1] = SmallShellType.Solid;
                        ShellLine[2] = SmallShellType.Solid;
                        ShellLine[3] = SmallShellType.Solid;
                        ShellLine[4] = SmallShellType.APShell;
                        averageRoundSpeed = 133.33f;
                        break;
                    }
                case ShellLineType.ArmorPenetration:
                    {
                        ShellLine = new SmallShellType[4];
                        ShellLine[0] = SmallShellType.Solid;
                        ShellLine[1] = SmallShellType.APShell;
                        ShellLine[2] = SmallShellType.Incendiary;
                        ShellLine[3] = SmallShellType.APShell;
                        averageRoundSpeed = 113.58f;
                        break;
                    }
                case ShellLineType.ShildOwerheat:
                    {
                        ShellLine = new SmallShellType[6];
                        ShellLine[0] = SmallShellType.SemiShell;
                        ShellLine[1] = SmallShellType.SemiShell;
                        ShellLine[2] = SmallShellType.SemiShell;
                        ShellLine[3] = SmallShellType.Solid;
                        ShellLine[4] = SmallShellType.Incendiary;
                        ShellLine[5] = SmallShellType.Solid;
                        averageRoundSpeed = 112.83f;
                        break;
                    }
                case ShellLineType.Incendiary:
                    {
                        ShellLine = new SmallShellType[4];
                        ShellLine[0] = SmallShellType.Solid;
                        ShellLine[1] = SmallShellType.Incendiary;
                        ShellLine[2] = SmallShellType.APShell;
                        ShellLine[3] = SmallShellType.Incendiary;
                        averageRoundSpeed = 124.4f;
                        break;
                    }
                case ShellLineType.Universal:
                    {
                        ShellLine = new SmallShellType[6];
                        ShellLine[0] = SmallShellType.SemiShell;
                        ShellLine[1] = SmallShellType.Solid;
                        ShellLine[2] = SmallShellType.APShell;
                        ShellLine[3] = SmallShellType.SemiShell;
                        ShellLine[4] = SmallShellType.Incendiary;
                        ShellLine[5] = SmallShellType.APShell;
                        averageRoundSpeed = 118.13f;
                        break;
                    }
            }
            shellPosition = 0;
        }
        protected override void Shoot(Transform target)
        {
            Quaternion direction = transform.rotation;
            double[] randomOffset = Randomizer.Uniform(0, 100, 2);
            if (randomOffset[0] > 50)
                direction.x = direction.x + (Convert.ToSingle(Global.RandomNormalPool[Convert.ToInt32(randomOffset[0])] - Convert.ToSingle(Global.RandomNormalAverage)) * dispersion);
            else
                direction.x = direction.x + (Convert.ToSingle(Global.RandomNormalPool[Convert.ToInt32(randomOffset[0])] - Convert.ToSingle(Global.RandomNormalAverage)) * -dispersion);
            if (randomOffset[1] > 50)
                direction.y = direction.y + (Convert.ToSingle(Global.RandomNormalPool[Convert.ToInt32(randomOffset[1])] - Convert.ToSingle(Global.RandomNormalAverage)) * dispersion);
            else
                direction.y = direction.y + (Convert.ToSingle(Global.RandomNormalPool[Convert.ToInt32(randomOffset[1])] - Convert.ToSingle(Global.RandomNormalAverage)) * -dispersion);
            GameObject shell = Instantiate(Global.UnitaryShell, gameObject.transform.position, direction);
            if (shellPosition >= ShellLine.Length)
                shellPosition = 0;

            float speed, damage, armorPiersing, mass;
            bool canRicochet = false;
            GameObject explosionPrefab = null;
            switch (ShellLine[shellPosition])
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

            shell.GetComponent<IShell>().StatUp(speed, damage, armorPiersing, mass, canRicochet, explosionPrefab);
            shellPosition++;
        }
    }
}

