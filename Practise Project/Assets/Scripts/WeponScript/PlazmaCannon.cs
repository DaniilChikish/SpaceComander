using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace SpaceCommander
{
    public class PlazmaCannon : Weapon
    {
        public override void StatUp()
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            range = 105;
            ammo = 100; //2 Min
            coolingTime = 1.5f; //30 DD, 33 DpS
            cooldown = 0;
            dispersion = 5f;
            shildBlinkTime = 0.5f;
            averageRoundSpeed = 60;
        }
        protected override void Shoot(Transform target)
        {
            ammo -= 1;
            cooldown = CoolingTime;
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
