using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PracticeProject
{
    public class RocketLauncher : Weapon
    {
        public MissileType AmmoType;
        public override void StatUp()
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            range = 150;
            ammo = 12;
            coolingTime = 10f;
            cooldown = 0;
            dispersion = 6f;
            shildBlinkTime = 0.8f;
        }
        protected override void Shoot(Transform target)
        {
            switch (AmmoType)
            {
                case MissileType.Selfguided:
                    {
                        GameObject missile = Instantiate(FindObjectsOfType<GlobalController>()[0].SelfGuidedMissile, gameObject.transform.position, transform.rotation);
                        missile.GetComponent<SelfguidedMissile>().SetTarget(target);
                        break;
                    }
                case MissileType.Unguided:
                    {
                        GameObject torpedo = Instantiate(FindObjectsOfType<GlobalController>()[0].UnguidedMissile, gameObject.transform.position, transform.rotation);
                        torpedo.GetComponent<Torpedo>().SetTarget(target.position);
                        break;
                    }
            }
        }
    }
}