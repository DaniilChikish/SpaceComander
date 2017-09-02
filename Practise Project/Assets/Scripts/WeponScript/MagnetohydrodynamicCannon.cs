using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpaceCommander.Weapons
{
    public class MagnetohydrodynamicCannon : EnergyWeapon
    {
        public override void StatUp()
        {
            type = WeaponType.MagnetohydrodynamicGun;
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            range = 350;
            firerate = 200;
            maxHeat = 100;
            dispersion = 0.0f;
            shildBlinkTime = 0.05f;
            averageRoundSpeed = 600;
            PreAiming = true;
        }
        protected override void Shoot(Transform target)
        {
            heat += 30 * ((heat + 20) / maxHeat);
            Quaternion direction = transform.rotation;
            GameObject shell = Instantiate(Global.MagnetoShell, gameObject.transform.position, direction);
            shell.GetComponent<MagnetoShell>().StatUp();
        }
    }
}


