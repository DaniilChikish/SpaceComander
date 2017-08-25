using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpaceCommander.Weapons
{
    public class Railgun : RoundWeapon {
        public override void StatUp()
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            range = 500;
            ammo = 15; //1.5 Min
            firerate = 10;//200 DD, 40 DpS
            reloadingTime = 30;
            dispersion = 0.0f;
            shildBlinkTime = 0.05f;
            averageRoundSpeed = 300;
            PreAiming = true;
        }
        protected override void Shoot(Transform target)
        {
            Quaternion direction = transform.rotation;

            GameObject shell = Instantiate(Global.RailgunShell, gameObject.transform.position, direction);
            shell.GetComponent<RailgunShell>().StatUp();
        }
    }
}
