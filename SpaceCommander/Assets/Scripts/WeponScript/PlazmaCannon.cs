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
            Quaternion dispersionDelta = RandomDirectionNormal(Dispersion);
            GameObject sphere = Instantiate(FindObjectOfType<GlobalController>().PlasmaSphere, gameObject.transform.position, transform.rotation * dispersionDelta);
            sphere.GetComponent<PlazmaSphere>().SetTarget(target);
            sphere.GetComponent<PlazmaSphere>().StatUp(EnergyType.Plazma);
        }
    }
}
