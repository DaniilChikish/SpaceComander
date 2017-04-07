using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PracticeProject
{
    public class Lazer : Weapon
    {
        public EnergyType AmmoType;
        public override void StatUp()
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            range = 350;
            ammo = Int32.MaxValue;
            coolingTime = 1.5f;
            cooldown = 0;
            dispersion = 0.001f;//exponential
            shildBlinkTime = 0.05f;
            averageRoundSpeed = 500;
        }
        protected override void Shoot(Transform target)
        {
		    //GlobalController Global = FindObjectsOfType<GlobalController>()[0];
            Quaternion direction = transform.rotation;
            double[] randomOffset = Randomizer.Uniform(0, 100, 2);
            if (randomOffset[0] > 50)
                direction.x = direction.x + (Convert.ToSingle(Global.RandomExponentPool[Convert.ToInt32(randomOffset[0])]) * dispersion);
            else
                direction.x = direction.x + (Convert.ToSingle(Global.RandomExponentPool[Convert.ToInt32(randomOffset[0])]) * -dispersion);
            if (randomOffset[1] > 50)
                direction.y = direction.y + (Convert.ToSingle(Global.RandomExponentPool[Convert.ToInt32(randomOffset[1])]) * dispersion);
            else
                direction.y = direction.y + (Convert.ToSingle(Global.RandomExponentPool[Convert.ToInt32(randomOffset[1])]) * -dispersion);
            GameObject beam = Instantiate(Global.LaserBeam, gameObject.transform.position, direction);
            beam.GetComponent<IEnergy>().StatUp(AmmoType);
        }
    }
}
