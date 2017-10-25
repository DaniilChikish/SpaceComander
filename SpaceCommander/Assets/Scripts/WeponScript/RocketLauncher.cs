using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpaceCommander.Weapons
{
    public class RocketLauncher : ShellWeapon
    {
        public MissileType AmmoType;
        protected override void StatUp()
        {
            base.StatUp();
            type = WeaponType.Missile;
        }
        protected override void Shoot(Transform target)
        {
            GameObject missile;
            switch (AmmoType)
            {
                case MissileType.Interceptor:
                    {
                        missile = Instantiate(Global.Missile, gameObject.transform.position, transform.rotation);
                        missile.AddComponent<InterceptorMissile>().SetTarget(target);
                        break;
                    }
                case MissileType.Hunter:
                    {
                         missile = Instantiate(Global.Missile, gameObject.transform.position, transform.rotation);
                        missile.AddComponent<HunterMissile>().SetTarget(target);
                        break;
                    }
                case MissileType.Metheor:
                    {
                         missile = Instantiate(Global.Missile, gameObject.transform.position, transform.rotation);
                        missile.AddComponent<MetheorMissile>().SetTarget(target);
                        break;
                    }
                case MissileType.Bombardier:
                default:
                    {
                         missile = Instantiate(Global.Missile, gameObject.transform.position, transform.rotation);
                            missile.AddComponent<BombardierMissile>();
                        break;
                    }
            }
            missile.GetComponent<Rigidbody>().AddForce(ownerBody.velocity, ForceMode.VelocityChange);
        }
    }
}