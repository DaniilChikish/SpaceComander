using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace PracticeProject
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
            ammo = 2000;
            coolingTime = 0.35f;
            cooldown = 0;
            dispersion = 0.08f;
            shildBlinkTime = 0.1f;
            averageRoundSpeed = 130;

            switch (AmmoType)
            {
                case ShellLineType.ArmorPenetration:
                    {
                        ShellLine = new ShellType[3];
                        ShellLine[0] = ShellType.Subcaliber;
                        ShellLine[1] = ShellType.CamorousAP;
                        ShellLine[2] = ShellType.SolidAP;
                        averageRoundSpeed = 121.81f;
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
                        ShellLine = new ShellType[4];
                        ShellLine[0] = ShellType.Camorous;
                        ShellLine[1] = ShellType.Solid;
                        ShellLine[2] = ShellType.Camorous;
                        ShellLine[3] = ShellType.Subcaliber;
                        averageRoundSpeed = 151.85f;
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
                        ShellLine[1] = ShellType.CamorousAP;
                        ShellLine[2] = ShellType.SolidAP;
                        ShellLine[3] = ShellType.Camorous;
                        ShellLine[4] = ShellType.Subcaliber;
                        ShellLine[5] = ShellType.HightExplosive;
                        averageRoundSpeed = 124.91f;
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

