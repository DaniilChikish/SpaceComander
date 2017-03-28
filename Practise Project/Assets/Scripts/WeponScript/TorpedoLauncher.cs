using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PracticeProject
{
    public class TorpedoLauncher : Weapon
    {
        protected override void Start()
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            range = 300;
            ammo = 12;
            coolingTime = 20f;
            cooldown = 0;
            dispersion = 3f;
        }
        protected override void Shoot(Transform target)
        {
            GameObject torpedo = Instantiate(ShellPrefab, gameObject.transform.position, transform.rotation);
            torpedo.GetComponent<Torpedo>().SetTarget(target.position);
            torpedo.GetComponent<Torpedo>().allies = target.GetComponent<Unit>().alliesArmy;
        }
    }
}
