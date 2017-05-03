using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PracticeProject
{
    public class Railgun : Weapon {
        public override void StatUp()
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            range = 500;
            ammo = 15; //1.5 Min
            coolingTime = 6f;//200 DD, 40 DpS
            cooldown = 0;
            dispersion = 0.000001f;
            shildBlinkTime = 0.05f;
            averageRoundSpeed = 300;
            PreAiming = true;
        }
        protected override void Shoot(Transform target)
        {
            Quaternion direction = transform.rotation;
            //double[] randomOffset = Randomizer.Uniform(0, 100, 2);
            //if (randomOffset[0] > 50)
            //    direction.x = direction.x + (Convert.ToSingle(Global.RandomNormalPool[Convert.ToInt32(randomOffset[0])] - Convert.ToSingle(Global.RandomNormalMin)) * dispersion);
            //else
            //    direction.x = direction.x + (Convert.ToSingle(Global.RandomNormalPool[Convert.ToInt32(randomOffset[0])] - Convert.ToSingle(Global.RandomNormalMin)) * -dispersion);
            //if (randomOffset[1] > 50)
            //    direction.y = direction.y + (Convert.ToSingle(Global.RandomNormalPool[Convert.ToInt32(randomOffset[1])] - Convert.ToSingle(Global.RandomNormalMin)) * dispersion);
            //else
            //    direction.y = direction.y + (Convert.ToSingle(Global.RandomNormalPool[Convert.ToInt32(randomOffset[1])] - Convert.ToSingle(Global.RandomNormalMin)) * -dispersion);
            GameObject shell = Instantiate(Global.RailgunShell, gameObject.transform.position, direction);
            shell.GetComponent<IShell>().StatUp(ShellType.Railgun);
        }
    }
}
