using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace PracticeProject
{
    public class Cannon : Weapon
    {
        protected override void StatUp()
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            range = 90;
            ammo = 2000;
            coolingTime = 0.15f;
            cooldown = 0;
            dispersion = 0.5f;
            shildBlinkTime = 0.01f;
            avarageRounSpeed = Global.CannonUnitaryShell.GetComponent<Round>().Speed;
        }
        protected override void Shoot(Transform target)
        {
            Quaternion direction = transform.rotation;
            double[] randomOffset = Randomizer.Uniform(0, 100, 2);
            if (randomOffset[0] > 50)
                direction.x = direction.x + (Convert.ToSingle(Global.RandomNormalPool[Convert.ToInt32(randomOffset[0])] - Convert.ToSingle(Global.RandomNormalMin)) * dispersion);
            else
                direction.x = direction.x + (Convert.ToSingle(Global.RandomNormalPool[Convert.ToInt32(randomOffset[0])] - Convert.ToSingle(Global.RandomNormalMin)) * -dispersion);
            if (randomOffset[1] > 50)
                direction.y = direction.y + (Convert.ToSingle(Global.RandomNormalPool[Convert.ToInt32(randomOffset[1])] - Convert.ToSingle(Global.RandomNormalMin)) * dispersion);
            else
                direction.y = direction.y + (Convert.ToSingle(Global.RandomNormalPool[Convert.ToInt32(randomOffset[1])] - Convert.ToSingle(Global.RandomNormalMin)) * -dispersion);
            Instantiate(Global.CannonUnitaryShell, gameObject.transform.position, direction);
        }
    }
}
