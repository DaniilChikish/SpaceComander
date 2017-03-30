using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PracticeProject
{
    public class RocketLauncher : Weapon
    {
        protected override void StatUp()
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            range = 150;
            ammo = 12;
            coolingTime = 12f;
            cooldown = 0;
            dispersion = 6f;
        }
        protected override void Shoot(Transform target)
        {
            GameObject missile = Instantiate(FindObjectsOfType<GlobalController>()[0].SelfGuidedMissile, gameObject.transform.position, transform.rotation);
            missile.GetComponent<Missile>().SetTarget(target);
        }
    }
}