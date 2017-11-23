using SpaceCommander.Mechanics;
using UnityEngine;
namespace SpaceCommander.Mechanics.Weapons
{
    public class Railgun : ShellWeapon {
        private float accumulate;
        protected override void StatUp()
        {
            base.StatUp();
            type = WeaponType.Railgun;
            audio.minDistance = 10 * 2;
            audio.maxDistance = 2000 * 2;
        }
        protected override void UpdateLocal()
        {
            if (backCount <= 0 && accumulate < 5)
            {
                accumulate += Time.deltaTime;
                owner.ShieldForce -= owner.ShieldRecharging * 0.25f * Time.deltaTime;
            }
            RoundSpeed = 400 * accumulate;
        }
        protected override void Shoot(Transform target)
        {
            float damage = 100f * accumulate;
            float armorPiersing = 3f + (1 * accumulate);
            float mass = 400f;
            accumulate = 1;

            Quaternion dispersionDelta = RandomDirectionNormal(Dispersion);
            GameObject shell = Instantiate(Global.Prefab.RailgunShell, gameObject.transform.position, this.transform.rotation * dispersionDelta);

            shell.GetComponent<General.IShell>().StatUp(owner.Velocity + (RoundSpeed * (dispersionDelta * this.transform.forward)), damage * (1 + DamageMultiplacator), armorPiersing * (1 + APMultiplacator), mass * (1 + ShellmassMultiplacator), true, null);
            ownerBody.AddForceAtPosition(-this.transform.forward * mass * RoundSpeed, this.transform.position, ForceMode.Impulse);
        }
    }
}
