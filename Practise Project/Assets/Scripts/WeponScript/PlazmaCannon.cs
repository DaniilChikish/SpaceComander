using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace PracticeProject
{
    public class PlazmaCannon : Weapon
    {
        protected override void Start()
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            range = 70;
            ammo = 500;
            coolingTime = 1f;
            cooldown = 0;
            dispersion = 5f;
        }
        protected override void Shoot(Transform target)
        {
            Quaternion direction = transform.rotation;
            double[] random = Randomizer.Normal(1, 1, 32, 0, 128);
            double[] randomOffset = Randomizer.Uniform(0, 100, 2);
            if (randomOffset[0] > 50)
                direction.x = direction.x + (Convert.ToSingle(random[0] - (random.Min() + random.Max()) / 2) * dispersion * 4);
            else
                direction.x = direction.x + (Convert.ToSingle(random[0] - (random.Min() + random.Max()) / 2) * -dispersion * 4);
            if (randomOffset[1] > 50)
                direction.y = direction.y + (Convert.ToSingle(random[1] - (random.Min() + random.Max()) / 2) * dispersion);
            else
                direction.y = direction.y + (Convert.ToSingle(random[1] - (random.Min() + random.Max()) / 2) * -dispersion);
            GameObject shell = Instantiate(ShellPrefab, gameObject.transform.position, transform.rotation);
            shell.GetComponent<PlazmaSphere>().SetTarget(target);
        }
    }
}
