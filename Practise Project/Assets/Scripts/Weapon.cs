using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace PracticeProject
{
    public class Weapon : MonoBehaviour
    {
        //protected float damage;
        protected float range;
        protected int ammo;
        protected int coolingTime;
        protected int cooldown;
        protected float dispersion;
        public WeaponType Type;
        public GameObject ShellPrefab;
        public AudioClip ShootSound;

        public float Range { get { return range; } }
        public int Ammo { get { return ammo; } }
        public int Cooldown { get { return cooldown; } }

        private void Start()
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            switch (Type)
            {
                case WeaponType.Cannon:
                    {
                        //damage = 30;
                        range = 50;
                        ammo = 2000;
                        coolingTime = 45;
                        cooldown = 0;
                        dispersion = 0.5f;//normal
                        break;
                    }
                case WeaponType.Laser:
                    {
                        //damage = 30;
                        range = 50;
                        ammo = Int32.MaxValue;
                        coolingTime = 300;
                        cooldown = 0;
                        dispersion = 0.001f;//exponential
                        break;
                    }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (cooldown > 0)
                cooldown--;
        }

        public bool Fire(Vector3 target)
        {
            if ((ammo > 0) && (cooldown == 0))
            {
                Debug.Log("Fire");
                Shoot(target);
                cooldown = coolingTime;
                ammo--;
            }
            return false;
        }
        private void Shoot(Vector3 target)
        {
            switch (Type)
            {
                case (WeaponType.Cannon):
                    {
                        Quaternion direction = transform.rotation;
                        double[] random = Randomizer.Normal(1, 1, 32, 0, 128);
                        double[] randomOffset = Randomizer.Uniform(0, 100, 2);
                        if (randomOffset[0] > 50)
                            direction.x = direction.x + (Convert.ToSingle(random[0] - (random.Min() + random.Max()) / 2) * dispersion);
                        else
                            direction.x = direction.x + (Convert.ToSingle(random[0] - (random.Min() + random.Max()) / 2) * -dispersion);
                        if (randomOffset[1] > 50)
                            direction.y = direction.y + (Convert.ToSingle(random[1] - (random.Min() + random.Max()) / 2) * dispersion);
                        else
                            direction.y = direction.y + (Convert.ToSingle(random[1] - (random.Min() + random.Max()) / 2) * -dispersion);
                        GameObject ShellInstance = Instantiate(ShellPrefab, gameObject.transform.position, direction);
                        break;
                    }
                case (WeaponType.Laser):
                    {
                        Quaternion direction = transform.rotation;
                        double[] random = Randomizer.Exponential(7, 32, 0, 128);
                        double[] randomOffset = Randomizer.Uniform(0, 100, 2);
                        if (randomOffset[0] > 50)
                            direction.x = direction.x + (Convert.ToSingle(random[0]) * dispersion);
                        else
                            direction.x = direction.x + (Convert.ToSingle(random[0]) * -dispersion);
                        if (randomOffset[1] > 50)
                            direction.y = direction.y + (Convert.ToSingle(random[1]) * dispersion);
                        else
                            direction.y = direction.y + (Convert.ToSingle(random[1]) * -dispersion);
                        GameObject ShellInstance = Instantiate(ShellPrefab, gameObject.transform.position, direction);
                        break;
                    }
            }
        }
    }
}
