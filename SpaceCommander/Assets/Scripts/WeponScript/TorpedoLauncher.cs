using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpaceCommander.Weapons
{
    public class TorpedoLauncher : ShellWeapon
    {
        private SpaceShip owner;
        public TorpedoType AmmoType;
        protected override void StatUp()
        {
            base.StatUp();
            type = WeaponType.Torpedo;
            owner = this.transform.GetComponentInParent<SpaceShip>();
        }
        protected override void Shoot(Transform target)
        {
            GameObject torpedo;
            switch (AmmoType)
            {
                case TorpedoType.Nuke:
                    {
                        torpedo = Instantiate(Global.Torpedo, gameObject.transform.position, transform.rotation);
                        torpedo.AddComponent<NukeTorpedo>();
                        break;
                    }
                case TorpedoType.Sprute:
                    {
                        torpedo = Instantiate(Global.Torpedo, gameObject.transform.position, transform.rotation);
                        torpedo.AddComponent<SpruteTorpedo>();
                        break;
                    }
                case TorpedoType.ShieldsBreaker:
                    {
                        torpedo = Instantiate(Global.Torpedo, gameObject.transform.position, transform.rotation);
                        torpedo.AddComponent<ShieldBreakerTorpedo>();
                        break;
                    }
                case TorpedoType.Unitary:
                default:
                    {
                        torpedo = Instantiate(Global.Torpedo, gameObject.transform.position, transform.rotation);
                        torpedo.AddComponent<UnitaryTorpedo>();
                        if (target == null)
                            torpedo.GetComponent<Torpedo>().SetMidpoint(this.transform.GetComponentInParent<Unit>().transform.position + this.transform.GetComponentInParent<Unit>().transform.forward * 100);
                        break;
                    }
            }
            if (target != null)
                torpedo.GetComponent<Torpedo>().SetTarget(target.position);
            else
                torpedo.GetComponent<Torpedo>().SetTarget(this.transform.GetComponentInParent<Unit>().transform.position + this.transform.GetComponentInParent<Unit>().transform.forward * Range);
            torpedo.GetComponent<Torpedo>().SetTeam(owner.Team);
        }
    }
}
