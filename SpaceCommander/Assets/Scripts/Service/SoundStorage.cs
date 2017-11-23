using SpaceCommander.Mechanics;
using System;
using UnityEngine;

namespace SpaceCommander.Service
{
    public class SoundStorage : MonoBehaviour
    {
        public enum UISoundType { InClick, OutClick, Hover, Confirm, Denied}
        public enum SpecialSoundType { Hit}
        [Space]
        [Header("UI Sound")]
        [SerializeField]
        private AudioClip UIButtonInClick;
        [SerializeField]
        private AudioClip UIButtonHover;
        [SerializeField]
        private AudioClip UIButtonConfirm;
        [SerializeField]
        private AudioClip UIButtonOutClick;

        [Space]
        [Header("Weapon Sound")]
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

        [Space]
        [Header("Module Sound")]
        [SerializeField]
        private AudioClip DefaultModuleEnable;
        [SerializeField]
        private AudioClip DefaultModuleReady;

        [Space]
        [Header("Special Sound")]
        [SerializeField]
        private AudioClip armorHit;
        public void InstantUI(UISoundType type, Vector3 position, float volume)
        {
            float timeScale = Time.timeScale; //костыль
            Time.timeScale = 1;
            AudioSource.PlayClipAtPoint(GetUI(type), position, volume);
            Time.timeScale = timeScale;
        }
        public AudioClip GetUI(UISoundType type)
        {
            switch (type)
            {
                case UISoundType.Hover:
                    return UIButtonHover;
                case UISoundType.Confirm:
                    return UIButtonConfirm;
                case UISoundType.OutClick:
                    return UIButtonOutClick;
                case UISoundType.InClick:
                default:
                    return UIButtonInClick;
            }
        }
        public void InstantModule(Type type, SpellModuleState state, Vector3 position, float volume)
        {
            AudioSource.PlayClipAtPoint(GetModule(type, state), position, volume);
        }
        public AudioClip GetModule(Type type, SpellModuleState state)
        {
            switch (state)
            {
                case SpellModuleState.Ready:
                    {
                        return DefaultModuleReady;
                    }
                case SpellModuleState.Active:
                    {
                        return DefaultModuleEnable;
                    }
                default:
                    return DefaultModuleEnable;
            }
        }
        public void InstantShot(WeaponType type, Vector3 position, float volume)
        {
            AudioSource.PlayClipAtPoint(GetShot(type), position, volume);
        }
        public AudioClip GetShot(WeaponType type)
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
        public void InstantSpecial(SpecialSoundType type, Vector3 position, float volume)
        {
            AudioSource.PlayClipAtPoint(GetSpecial(type), position, volume);
        }
        public AudioClip GetSpecial(SpecialSoundType type)
        {
            switch (type)
            {
                case SpecialSoundType.Hit:
                default:
                    return armorHit;
            }
        }
    }
}
