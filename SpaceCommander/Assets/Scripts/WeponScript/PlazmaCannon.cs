using SpaceCommander.Mechanics;
using UnityEngine;

namespace SpaceCommander.Mechanics.Weapons
{
    public class PlazmaCannon : EnergyWeapon
    {
        protected override void StatUp()
        {
            base.StatUp();
            type = WeaponType.Plazma;
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        protected override void Shoot(Transform target)
        {
            heat += 5;
            Quaternion dispersionDelta = RandomDirectionNormal(Dispersion);
            GameObject sphere = Instantiate(Global.Prefab.PlasmaSphere, gameObject.transform.position, transform.rotation * dispersionDelta);
            sphere.GetComponent<PlazmaSphere>().SetTarget(target);
            sphere.GetComponent<PlazmaSphere>().StatUp(EnergyType.Plazma);
        }
    }
}
