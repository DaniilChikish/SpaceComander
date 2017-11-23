using SpaceCommander.Mechanics;
using UnityEngine;
namespace SpaceCommander.Mechanics.Weapons
{
    public class TorpedoLauncher : ShellWeapon
    {
        public TorpedoType AmmoType;
        protected override void StatUp()
        {
            base.StatUp();
            type = WeaponType.Torpedo;
            audio.minDistance = 10;
            audio.maxDistance = 2000;
        }
        protected override void Shoot(Transform target)
        {
            GameObject missile;
            Transform targetTr = null;
            if (Target != null) targetTr = Target.transform;
            switch (AmmoType)
            {
                case TorpedoType.Nuke:
                    {
                        missile = Instantiate(Global.Prefab.Torpedo, gameObject.transform.position, transform.rotation);
                        missile.AddComponent<NukeTorpedo>().SetTarget(targetTr);
                        break;
                    }
                case TorpedoType.Sprute:
                    {
                        missile = Instantiate(Global.Prefab.Torpedo, gameObject.transform.position, transform.rotation);
                        missile.AddComponent<SpruteTorpedo>().SetTarget(targetTr);
                        break;
                    }
                case TorpedoType.ShieldsBreaker:
                    {
                        missile = Instantiate(Global.Prefab.Torpedo, gameObject.transform.position, transform.rotation);
                        missile.AddComponent<ShieldBreakerTorpedo>().SetTarget(targetTr);
                        break;
                    }
                case TorpedoType.Thunderbolth:
                    {
                        missile = Instantiate(Global.Prefab.Torpedo, gameObject.transform.position, transform.rotation);
                        missile.AddComponent<ThunderbolthHeavyRocket>().SetTarget(targetTr);
                        break;
                    }
                case TorpedoType.Unitary:
                default:
                    {
                        missile = Instantiate(Global.Prefab.Torpedo, gameObject.transform.position, transform.rotation);
                        missile.AddComponent<UnitaryTorpedo>().SetTarget(targetTr);
                        break;
                    }
            }
            missile.GetComponent<Missile>().SetTeam(owner.Team);
            missile.GetComponent<Rigidbody>().AddForce(ownerBody.velocity, ForceMode.VelocityChange);
        }
    }
}
