using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpaceCommander
{
    public class RocketLauncher : Weapon
    {
        public MissileType AmmoType;
        public override void StatUp()
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            range = 250;
            ammo = 40;
            coolingTime = 5f;
            cooldown = 0;
            dispersion = 6f;
            shildBlinkTime = 0.8f;
        }
        protected override void Shoot(Transform target)
        {
            switch (AmmoType)
            {
                case MissileType.Interceptor:
                    {
                        ammo -= 3;
                        cooldown = CoolingTime * 3;
                        GameObject missile = Instantiate(FindObjectsOfType<GlobalController>()[0].Missile, gameObject.transform.position, transform.rotation);
                        missile.AddComponent<InterceptorMissile>().SetTarget(target);
                        break;
                    }
                case MissileType.Hunter:
                    {
                        ammo -= 2;
                        cooldown = CoolingTime * 2;
                        GameObject missile = Instantiate(FindObjectsOfType<GlobalController>()[0].Missile, gameObject.transform.position, transform.rotation);
                        missile.AddComponent<HunterMissile>().SetTarget(target);
                        break;
                    }
                case MissileType.Metheor:
                    {
                        ammo -= 2;
                        cooldown = CoolingTime * 2;
                        GameObject missile = Instantiate(FindObjectsOfType<GlobalController>()[0].Missile, gameObject.transform.position, transform.rotation);
                        missile.AddComponent<MetheorMissile>().SetTarget(target);
                        break;
                    }
                case MissileType.Bombardier:
                    {
                        ammo -= 1;
                        cooldown = CoolingTime;
                        GameObject torpedo = Instantiate(FindObjectsOfType<GlobalController>()[0].Missile, gameObject.transform.position, transform.rotation);
                        torpedo.AddComponent<BombardierMissile>().SetTarget(target.position);                       
                        break;
                    }
            }
        }
    }
}