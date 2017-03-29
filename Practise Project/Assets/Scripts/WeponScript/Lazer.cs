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
            range = 300;
            ammo = Int32.MaxValue;
            coolingTime = 5f;
            cooldown = 0;
            dispersion = 0.001f;//exponential
            shildBlinkTime = 0.01f;
        }
        protected override void Shoot(Transform target)
        {
					Global = FindObjectsOfType<GlobalController>()[0];
            Quaternion direction = transform.rotation;
            double[] randomOffset = Randomizer.Uniform(0, 100, 2);
            if (randomOffset[0] > 50)
                direction.x = direction.x + (Convert.ToSingle(RandomExponentPool[randomOffset[0]]) * dispersion);
            else
                direction.x = direction.x + (Convert.ToSingle(RandomExponentPool[randomOffset[0]]) * -dispersion);
            if (randomOffset[1] > 50)
                direction.y = direction.y + (Convert.ToSingle(RandomExponentPool[randomOffset[1]]) * dispersion);
            else
                direction.y = direction.y + (Convert.ToSingle(RandomExponentPool[randomOffset[1]]) * -dispersion);
            Instantiate(Global.LaserBeam, gameObject.transform.position, direction);
        }
    }
}
