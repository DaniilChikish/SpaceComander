using SpaceCommander.Mechanics;
using UnityEngine;
namespace SpaceCommander.Mechanics.Weapons
{
    public class Railmortar : MagWeapon
    {
        public float DropImpulse;
        protected override void StatUp()
        {
            base.StatUp();
            type = WeaponType.Railmortar;
            audio.minDistance = 30;
            audio.maxDistance = 3000;
        }
        protected override void Shoot(Transform target)
        {
            Quaternion dispersionDelta = RandomDirectionNormal(Dispersion);
            GameObject mine = Instantiate(Global.Prefab.MagMine, gameObject.transform.position, this.transform.rotation * dispersionDelta);
            mine.GetComponent<MagnetoMine>().team = owner.Team;
            mine.GetComponent<Rigidbody>().AddForce(owner.Velocity + (DropImpulse * (1 + RoundspeedMultiplacator) * (dispersionDelta * this.transform.forward)), ForceMode.VelocityChange);
            ownerBody.AddForceAtPosition(-this.transform.forward * DropImpulse, this.transform.position, ForceMode.Impulse);
        }
    }
}
