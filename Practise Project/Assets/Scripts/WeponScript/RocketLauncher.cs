using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpaceCommander.Weapons
{
    public class RocketLauncher : RoundWeapon
    {
        public MissileType AmmoType;
        public override void StatUp()
        {
            type = WeaponType.Missile;
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            range = 250;
            ammo = 20;
            ammoCampacity = 20;
            firerate = 15;
            reloadingTime = 45;
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
                        GameObject missile = Instantiate(FindObjectsOfType<GlobalController>()[0].Missile, gameObject.transform.position, transform.rotation);
                        missile.AddComponent<InterceptorMissile>().SetTarget(target);
                        break;
                    }
                case MissileType.Hunter:
                    {
                        ammo -= 1;
                        GameObject missile = Instantiate(FindObjectsOfType<GlobalController>()[0].Missile, gameObject.transform.position, transform.rotation);
                        missile.AddComponent<HunterMissile>().SetTarget(target);
                        break;
                    }
                case MissileType.Metheor:
                    {
                        ammo -= 1;
                        GameObject missile = Instantiate(FindObjectsOfType<GlobalController>()[0].Missile, gameObject.transform.position, transform.rotation);
                        missile.AddComponent<MetheorMissile>().SetTarget(target);
                        break;
                    }
                case MissileType.Bombardier:
                    {
                        GameObject torpedo = Instantiate(FindObjectsOfType<GlobalController>()[0].Missile, gameObject.transform.position, transform.rotation);
                        if (target != null)
                            torpedo.AddComponent<BombardierMissile>().SetTarget(target.position);
                        else
                            torpedo.AddComponent<BombardierMissile>().SetTarget(this.transform.GetComponentInParent<Unit>().transform.position + this.transform.GetComponentInParent<Unit>().transform.forward * Range);
                        torpedo.GetComponent<Torpedo>().SetMidpoint(this.transform.GetComponentInParent<Unit>().transform.position + this.transform.GetComponentInParent<Unit>().transform.forward * 100);
                        torpedo.tag = "Torpedo";
                        break;
                    }
            }
        }
    }
}