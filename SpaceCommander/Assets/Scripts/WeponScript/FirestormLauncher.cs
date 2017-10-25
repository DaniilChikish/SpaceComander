using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpaceCommander.Weapons
{
    public class FirestormLauncher : MagWeapon
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
            Quaternion dispersionDelta = RandomDirectionNormal(Dispersion);
            switch (AmmoType)
            {
                case MissileType.Interceptor:
                    {
                        missile = Instantiate(Global.Missile, gameObject.transform.position, transform.rotation * dispersionDelta);
                        missile.AddComponent<InterceptorMissile>().SetTarget(target);
                        break;
                    }
                case MissileType.Hunter:
                    {
                         missile = Instantiate(Global.Missile, gameObject.transform.position, transform.rotation * dispersionDelta);
                        missile.AddComponent<HunterMissile>().SetTarget(target);
                        break;
                    }
                case MissileType.Metheor:
                    {
                         missile = Instantiate(Global.Missile, gameObject.transform.position, transform.rotation * dispersionDelta);
                        missile.AddComponent<MetheorMissile>().SetTarget(target);
                        break;
                    }
                case MissileType.Bombardier:
                default:
                    {
                        missile = Instantiate(Global.Missile, gameObject.transform.position, transform.rotation * dispersionDelta);
                        missile.AddComponent<BombardierMissile>();
                        break;
                    }
            }
            missile.GetComponent<Rigidbody>().AddForce(ownerBody.velocity, ForceMode.VelocityChange);
        }
    }
}