using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PracticeProject
{
    public class RocketLauncher : Weapon
    {
        protected override void Start()
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            range = 200;
            ammo = 12;
            coolingTime = 7f;
            cooldown = 0;
            dispersion = 6f;
        }
        protected override void Shoot(Transform target)
        {
            GameObject missile = Instantiate(ShellPrefab, gameObject.transform.position, transform.rotation);
            missile.GetComponent<Missile>().SetTarget(target);
        }
    }
}