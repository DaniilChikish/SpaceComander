using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PracticeProject
{
    public class SpruteLauncher : Weapon
    {
        private Unit owner;
        protected override void StatUp()
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            range = 250;
            ammo = 8;
            coolingTime = 20f;
            cooldown = 0;
            dispersion = 3f;
            shildBlinkTime = 0.5f;
            owner = this.transform.GetComponentInParent<Unit>();
        }
        protected override void Shoot(Transform target)
        {
            GameObject torpedo = Instantiate(Global.SpruteTorpedo, gameObject.transform.position, transform.rotation);
            torpedo.GetComponent<Torpedo>().SetTarget(target.position);
            torpedo.GetComponent<Torpedo>().SetTeam(owner.Team);
        }
    }
}
