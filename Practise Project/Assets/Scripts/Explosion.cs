using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PracticeProject
{
    public class Explosion : MonoBehaviour
    {
        public BlastType type;
        private float waveSpeed; // Скорость волны
        private float maxRadius; // Радиус взрыва
        private float damage; // Урон
        public float Damage { get { return damage; } }
        public float MaxRadius { get { return maxRadius; } }
        void Start(){ }
        public void StatUp(BlastType type)
        {
            this.type = type;
            switch (type)
            {
                case BlastType.Missile:
                    {
                        maxRadius = 6f;
                        waveSpeed = 1f;
                        damage = 10;
                        break;
                    }
                case BlastType.UnitaryTorpedo:
                    {
                        maxRadius = 20f;
                        waveSpeed = 1.1f;
                        damage = 12f;
                        break;
                    }
                case BlastType.NukeTorpedo:
                    {
                        maxRadius = 40f;
                        waveSpeed = 1.3f;
                        damage = 20;
                        break;
                    }
                case BlastType.SmallShip:
                    {
                        maxRadius = 8f;
                        waveSpeed = 1.1f;
                        damage = 2;
                        break;
                    }
                case BlastType.MediumShip:
                    {
                        maxRadius = 12f;
                        waveSpeed = 1.1f;
                        damage = 3;
                        break;
                    }
                case BlastType.Corvette:
                    {
                        maxRadius = 20f;
                        waveSpeed = 1.1f;
                        damage = 4;
                        break;
                    }
                case BlastType.Shell:
                    {
                        maxRadius = 2f;
                        waveSpeed = 1.1f;
                        damage = 2;
                        break;
                    }
                case BlastType.ExplosiveShell:
                    {
                        maxRadius = 5f;
                        waveSpeed = 1f;
                        damage = 4;
                        break;
                    }

                default:
                    {
                        break;
                    }
            }
        }

        void Update()
        {
            if (transform.localScale.y >= maxRadius) Destroy(gameObject);
            else
            {
                transform.localScale += new Vector3(waveSpeed, waveSpeed, waveSpeed);
            }
        }
    }
}
