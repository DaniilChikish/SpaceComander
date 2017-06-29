using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace SpaceCommander
{
    public class AutoCannon : Weapon
    {
        public ShellLineType AmmoType;
        public ShellType[] ShellLine;
        public int shellPosition;
        public override void StatUp()
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            range = 110;
            ammo = 350; //~2 Min
            coolingTime = 0.35f; //30 DD, 85 DpS
            cooldown = 0;
            dispersion = 0.08f;
            shildBlinkTime = 0.1f;
            averageRoundSpeed = 130;
            PreAiming = true;
            switch (AmmoType)
            {
                case ShellLineType.Solid:
                    {
                        ShellLine = new ShellType[5];
                        ShellLine[0] = ShellType.Solid;
                        ShellLine[1] = ShellType.Solid;
                        ShellLine[2] = ShellType.Solid;
                        ShellLine[3] = ShellType.Solid;
                        ShellLine[4] = ShellType.SolidAP;
                        averageRoundSpeed = 133.33f;
                        break;
                    }
                case ShellLineType.Camorus:
                    {
                        ShellLine = new ShellType[5];
                        ShellLine[0] = ShellType.Camorous;
                        ShellLine[1] = ShellType.Camorous;
                        ShellLine[2] = ShellType.Camorous;
                        ShellLine[3] = ShellType.Camorous;
                        ShellLine[4] = ShellType.CamorousAP;
                        averageRoundSpeed = 148.15f;
                        break;
                    }
                case ShellLineType.ArmorPenetration:
                    {
                        ShellLine = new ShellType[4];
                        ShellLine[0] = ShellType.SolidAP;
                        ShellLine[1] = ShellType.Subcaliber;
                        ShellLine[2] = ShellType.CamorousAP;
                        ShellLine[3] = ShellType.SolidAP;
                        averageRoundSpeed = 113.58f;
                        break;
                    }
                case ShellLineType.ShildOwerheat:
                    {
                        ShellLine = new ShellType[6];
                        ShellLine[0] = ShellType.Solid;
                        ShellLine[1] = ShellType.Uranium;
                        ShellLine[2] = ShellType.Solid;
                        ShellLine[3] = ShellType.SolidAP;
                        ShellLine[4] = ShellType.Solid;
                        ShellLine[5] = ShellType.Camorous;
                        averageRoundSpeed = 112.83f;
                        break;
                    }
                case ShellLineType.QuickShell:
                    {
                        ShellLine = new ShellType[5];
                        ShellLine[0] = ShellType.Solid;
                        ShellLine[1] = ShellType.Camorous;
                        ShellLine[2] = ShellType.Solid;
                        ShellLine[3] = ShellType.Camorous;
                        ShellLine[4] = ShellType.Subcaliber;
                        averageRoundSpeed = 148.14f;
                        break;
                    }
                case ShellLineType.Explosive:
                    {
                        ShellLine = new ShellType[4];
                        ShellLine[0] = ShellType.Camorous;
                        ShellLine[1] = ShellType.CamorousAP;
                        ShellLine[2] = ShellType.Camorous;
                        ShellLine[3] = ShellType.HightExplosive;
                        averageRoundSpeed = 124.4f;
                        break;
                    }
                case ShellLineType.Universal:
                    {
                        ShellLine = new ShellType[6];
                        ShellLine[0] = ShellType.Solid;
                        ShellLine[1] = ShellType.Camorous;
                        ShellLine[2] = ShellType.Solid;
                        ShellLine[3] = ShellType.Camorous;
                        ShellLine[4] = ShellType.SolidAP;
                        ShellLine[5] = ShellType.CamorousAP;
                        averageRoundSpeed = 118.13f;
                        break;
                    }
            }
            shellPosition = 0;
        }
        protected override void Shoot(Transform target)
        {
            ammo -= 1;
            cooldown = CoolingTime;
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
            shell.GetComponent<IShell>().StatUp(ShellLine[shellPosition]);
            shellPosition++;
        }
    }
}

