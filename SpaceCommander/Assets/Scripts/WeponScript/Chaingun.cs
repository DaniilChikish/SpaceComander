using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DeusUtility.Random;

namespace SpaceCommander.Weapons
{
    public class Chaingun : MagWeapon
    {
        public ShellLineType AmmoType;
        public SmallShellType[] ShellLine;
        public int shellPosition;
        private float rotSpeed;
        public override void StatUp()
        {
            type = WeaponType.MachineCannon;
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            range = 250;
            ammoCampacity = 1000; //~2 Min
            ammo = AmmoCampacity;
            firerate = 100;
            reloadingTime = 20;
            dispersion = 0.06f;
            shildBlinkTime = 0.1f;
            averageRoundSpeed = 250;
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
                        break;
                    }
                case ShellLineType.ArmorPenetration:
                    {
                        ShellLine = new SmallShellType[4];
                        ShellLine[0] = SmallShellType.Solid;
                        ShellLine[1] = SmallShellType.APShell;
                        ShellLine[2] = SmallShellType.Incendiary;
                        ShellLine[3] = SmallShellType.APShell;
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
                        break;
                    }
                case ShellLineType.Incendiary:
                    {
                        ShellLine = new SmallShellType[4];
                        ShellLine[0] = SmallShellType.Solid;
                        ShellLine[1] = SmallShellType.Incendiary;
                        ShellLine[2] = SmallShellType.APShell;
                        ShellLine[3] = SmallShellType.Incendiary;
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
                        break;
                    }
            }
            shellPosition = 0;
        }
        protected override void UpdateLocal()
        {
            if (rotSpeed > 2f)
            {
                rotSpeed -= Time.deltaTime * 2;
                firerate = (100 + 12000 * (rotSpeed / 5)) * (1 + FirerateMultiplacator);
            }
            else if (rotSpeed > 0)
            {
                rotSpeed -= Time.deltaTime;
                firerate = (100 + 12000 * (rotSpeed / 5)) * (1 + FirerateMultiplacator);
            }
            else
            {
                firerate = 100;
                rotSpeed = 0;
            }
        }
        protected override void Shoot(Transform target)
        {
            this.rotSpeed += 0.75f - (0.75f * rotSpeed / 5) + Time.deltaTime;
            float localDisp = Dispersion + (Dispersion * this.rotSpeed / 2);
            Quaternion direction = transform.rotation;
            double[] randomOffset = Randomizer.Uniform(0, 100, 2);
            if (randomOffset[0] > 50)
                direction.x = direction.x + (Convert.ToSingle(Global.RandomNormalPool[Convert.ToInt32(randomOffset[0])] - Convert.ToSingle(Global.RandomNormalAverage)) * localDisp);
            else
                direction.x = direction.x + (Convert.ToSingle(Global.RandomNormalPool[Convert.ToInt32(randomOffset[0])] - Convert.ToSingle(Global.RandomNormalAverage)) * -localDisp);
            if (randomOffset[1] > 50)
                direction.y = direction.y + (Convert.ToSingle(Global.RandomNormalPool[Convert.ToInt32(randomOffset[1])] - Convert.ToSingle(Global.RandomNormalAverage)) * localDisp);
            else
                direction.y = direction.y + (Convert.ToSingle(Global.RandomNormalPool[Convert.ToInt32(randomOffset[1])] - Convert.ToSingle(Global.RandomNormalAverage)) * -localDisp);
            GameObject shell = Instantiate(Global.UnitaryShell, gameObject.transform.position, direction);
            if (shellPosition >= ShellLine.Length)
                shellPosition = 0;

            float speed, damage, armorPiersing, mass;
            bool canRicochet = false;
            GameObject explosionPrefab = null;
            speed = 250f;
            switch (ShellLine[shellPosition])
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
                        mass = 1.05f;
                        canRicochet = true;
                        break;
                    }
                case SmallShellType.Incendiary:
                    {
                        damage = 5f;
                        armorPiersing = 1;
                        mass = 1;
                        explosionPrefab = Global.ShellBlast;
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
                        mass = 1;
                        canRicochet = true;
                        break;
                    }
            }
            armorPiersing = armorPiersing * 1.5f;
            shell.GetComponent<IShell>().StatUp(speed * (1 + RoundspeedMultiplacator), damage * (1 + DamageMultiplacator), armorPiersing * (1 + APMultiplacator), mass * (1 + ShellmassMultiplacator), canRicochet, explosionPrefab);
            shellPosition++;
        }
    }
}

