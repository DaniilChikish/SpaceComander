using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PracticeProject
{
    public class Lazer : Weapon
    {
        protected override void Start()
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            range = 200;
            ammo = Int32.MaxValue;
            coolingTime = 5f;
            cooldown = 0;
            dispersion = 0.001f;//exponential
        }
        protected override void Shoot(Transform target)
        {
            Quaternion direction = transform.rotation;
            double[] random = Randomizer.Exponential(7, 32, 0, 128);
            double[] randomOffset = Randomizer.Uniform(0, 100, 2);
            if (randomOffset[0] > 50)
                direction.x = direction.x + (Convert.ToSingle(random[0]) * dispersion);
            else
                direction.x = direction.x + (Convert.ToSingle(random[0]) * -dispersion);
            if (randomOffset[1] > 50)
                direction.y = direction.y + (Convert.ToSingle(random[1]) * dispersion);
            else
                direction.y = direction.y + (Convert.ToSingle(random[1]) * -dispersion);
            Instantiate(ShellPrefab, gameObject.transform.position, direction);
        }
    }
}
