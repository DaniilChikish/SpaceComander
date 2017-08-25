using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpaceCommander.Weapons
{
    public class TorpedoLauncher : RoundWeapon
    {
        private SpaceShip owner;
        public TorpedoType AmmoType;
        public override void StatUp()
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            range = 350;
            ammo = 16;
            firerate = 6;
            reloadingTime = 45;
            dispersion = 3f;
            shildBlinkTime = 1f;
            owner = this.transform.GetComponentInParent<SpaceShip>();
        }
        protected override void Shoot(Transform target)
        {
            GameObject torpedo;
            switch (AmmoType)
            {
                case TorpedoType.Nuke:
                    {
                        ammo -= 4;
                        torpedo = Instantiate(Global.Torpedo, gameObject.transform.position, transform.rotation);
                        torpedo.AddComponent<NukeTorpedo>();
                        break;
                    }
                case TorpedoType.Sprute:
                    {
                        ammo -= 2;
                        torpedo = Instantiate(Global.Torpedo, gameObject.transform.position, transform.rotation);
                        torpedo.AddComponent<SpruteTorpedo>();
                        break;
                    }
                case TorpedoType.Unitary:
                default:
                    {
                        ammo -= 1;
                        torpedo = Instantiate(Global.Torpedo, gameObject.transform.position, transform.rotation);
                        torpedo.AddComponent<UnitaryTorpedo>();
                        break;
                    }
            }
            torpedo.GetComponent<Torpedo>().SetTarget(target.position);
            torpedo.GetComponent<Torpedo>().SetTeam(owner.Team);
        }
    }
}
