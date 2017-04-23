using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace PracticeProject
{
    public class Cannon : Weapon
    {
        public ShellType AmmoType;
        public override void StatUp()
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            range = 150;
            ammo = 200; //~2 Min
            coolingTime = 0.7f; //30 DD, 43 DpS
            cooldown = 0;
            dispersion = 0.05f;
            shildBlinkTime = 0.1f;
            switch (AmmoType)
            {
                case ShellType.Solid:
                    {
                        averageRoundSpeed = 133.33f;
                        break;
                    }
                case ShellType.SolidBig:
                    {
                        averageRoundSpeed = 133.33f;
                        coolingTime = coolingTime * 5f;
                        range = range * 2f;
                        break;
                    }
                case ShellType.SolidAP:
                    {
                        this.averageRoundSpeed = 88.89f;
                        break;
                    }
                case ShellType.Subcaliber:
                    {
                        this.averageRoundSpeed = 177.78f;
                        break;
                    }
                case ShellType.SubcaliberBig:
                    {
                        this.averageRoundSpeed = 177.78f;
                        coolingTime = coolingTime * 5f;
                        range = range * 2f;
                        break;
                    }
                case ShellType.Camorous:
                    {
                        this.averageRoundSpeed = 148.15f;
                        break;
                    }
                case ShellType.CamorousBig:
                    {
                        this.averageRoundSpeed = 148.15f;
                        coolingTime = coolingTime * 5f;
                        range = range * 2f;
                        break;
                    }
                case ShellType.CamorousAP:
                    {
                        this.averageRoundSpeed = 98.77f;
                        break;
                    }
                case ShellType.Сumulative:
                    {
                        this.averageRoundSpeed = 111.11f;
                        break;
                    }
                case ShellType.HightExplosive:
                    {
                        this.averageRoundSpeed = 102.56f;
                        break;
                    }
                case ShellType.Uranium:
                    {
                        this.averageRoundSpeed = 40f;
                        break;
                    }
                case ShellType.Railgun:
                    {
                        this.averageRoundSpeed = 300f;
                        break;
                    }
            }
        }
        protected override void Shoot(Transform target)
        {
            ammo -= 1;
            cooldown = CoolingTime;
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
            shell.GetComponent<IShell>().StatUp(AmmoType);
        }
    }
}
