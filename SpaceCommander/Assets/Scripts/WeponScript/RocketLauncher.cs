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
            audio.minDistance = 5;
            audio.maxDistance = 1000;
        }
        protected override void Shoot(Transform target)
        {
            GameObject missile;
            Transform targetTr = null;
            if (Target != null) targetTr = Target.transform;
            switch (AmmoType)
            {
                case MissileType.Interceptor:
                    {
                        missile = Instantiate(Global.Prefab.Missile, gameObject.transform.position, transform.rotation);
                        missile.AddComponent<InterceptorMissile>().SetTarget(targetTr);
                        break;
                    }
                case MissileType.Hunter:
                    {
                         missile = Instantiate(Global.Prefab.Missile, gameObject.transform.position, transform.rotation);
                        missile.AddComponent<HunterMissile>().SetTarget(targetTr);
                        break;
                    }
                case MissileType.Metheor:
                    {
                         missile = Instantiate(Global.Prefab.Missile, gameObject.transform.position, transform.rotation);
                        missile.AddComponent<MetheorMissile>().SetTarget(targetTr);
                        break;
                    }
                case MissileType.Bombardier:
                default:
                    {
                         missile = Instantiate(Global.Prefab.Missile, gameObject.transform.position, transform.rotation);
                            missile.AddComponent<BombardierMissile>();
                        break;
                    }
            }
            missile.GetComponent<Missile>().SetTeam(owner.Team);
            missile.GetComponent<Rigidbody>().AddForce(ownerBody.velocity, ForceMode.VelocityChange);
        }
    }
}