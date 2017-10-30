using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpaceCommander.Service
{
    public class SoundStorage : MonoBehaviour
    {
        [SerializeField]
        private AudioClip cannonShot;
        [SerializeField]
        private AudioClip chaingunShot;
        [SerializeField]
        private AudioClip laserShot;
        [SerializeField]
        private AudioClip machineShot;
        [SerializeField]
        private AudioClip magnitoShot;
        [SerializeField]
        private AudioClip missileShot;
        [SerializeField]
        private AudioClip plasmaShot;
        [SerializeField]
        private AudioClip railgunShot;
        [SerializeField]
        private AudioClip mortarShot;
        [SerializeField]
        private AudioClip torpedoShot;
        [SerializeField]
        private AudioClip shotgunShot;
        public void InstantShot(WeaponType type, Vector3 position, float volume)
        {
            AudioSource.PlayClipAtPoint(GetClip(type), position, volume);
        }
        public AudioClip GetClip(WeaponType type)
        {
            switch (type)
            {
                case WeaponType.Cannon:
                    return cannonShot;
                case WeaponType.Chaingun:
                    return chaingunShot;
                case WeaponType.Laser:
                    return laserShot;
                case WeaponType.MachineCannon:
                    return machineShot;
                case WeaponType.MagnetohydrodynamicGun:
                    return magnitoShot;
                case WeaponType.Missile:
                    return missileShot;
                case WeaponType.Plazma:
                    return plasmaShot;
                case WeaponType.Railgun:
                    return railgunShot;
                case WeaponType.Railmortar:
                    return mortarShot;
                case WeaponType.ShootCannon:
                    return shotgunShot;
                case WeaponType.Torpedo:
                    return torpedoShot;
                default:
                    return cannonShot;
            }
        }
    }
}
