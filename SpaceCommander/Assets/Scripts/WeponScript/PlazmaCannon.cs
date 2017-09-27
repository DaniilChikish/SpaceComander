using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DeusUtility.Random;

namespace SpaceCommander.Weapons
{
    public class PlazmaCannon : EnergyWeapon
    {
        protected override void StatUp()
        {
            base.StatUp();
            type = WeaponType.Plazma;
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        protected override void Shoot(Transform target)
        {
            heat += 5;
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
            GameObject sphere = Instantiate(FindObjectsOfType<GlobalController>()[0].PlasmaSphere, gameObject.transform.position, transform.rotation);
            sphere.GetComponent<PlazmaSphere>().SetTarget(target);
            sphere.GetComponent<PlazmaSphere>().StatUp(EnergyType.Plazma);
        }
    }
}
