using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PracticeProject
{
    public class TorpedoLauncher : Weapon
    {
        private SpaceShip owner;
        public TorpedoType AmmoType;
        public override void StatUp()
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            range = 250;
            ammo = 8;
            coolingTime = 20f;
            cooldown = 0;
            dispersion = 3f;
            shildBlinkTime = 1f;
            owner = this.transform.GetComponentInParent<SpaceShip>();
        }
        protected override void Shoot(Transform target)
        {
            GameObject torpedo;
            switch (AmmoType)
            {
                case TorpedoType.Unitary:
                    {
                        torpedo = Instantiate(Global.UnitaryTorpedo, gameObject.transform.position, transform.rotation);
                        break;
                    }
                case TorpedoType.Nuke:
                    {
                        torpedo = Instantiate(Global.NukeTorpedo, gameObject.transform.position, transform.rotation);
                        break;
                    }
                case TorpedoType.Sprute:
                    {
                        torpedo = Instantiate(Global.SpruteTorpedo, gameObject.transform.position, transform.rotation);
                        break;
                    }
                default:
                    {
                        torpedo = Instantiate(Global.UnitaryTorpedo, gameObject.transform.position, transform.rotation);
                        break;
                    }
            }
            torpedo.GetComponent<Torpedo>().SetTarget(target.position);
            torpedo.GetComponent<Torpedo>().SetTeam(owner.Team);
        }
    }
}
