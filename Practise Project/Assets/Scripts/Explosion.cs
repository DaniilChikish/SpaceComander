using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PracticeProject
{
    public class Explosion : MonoBehaviour
    {
        public BlastType type;
        private float ttl;
        private float waveSpeed; // Скорость волны
        private float maxRadius; // Радиус взрыва
        private float damage; // Урон
        public float Damage { get { return damage * Time.deltaTime / 2; } }
        public float MaxRadius { get { return maxRadius; } }
        void Start() { }
        public void StatUp(BlastType type)
        {
            this.type = type;
            switch (type)
            {
                case BlastType.Missile:
                    {
                        maxRadius = 6f;
                        damage = 100f;
                        break;
                    }
                case BlastType.UnitaryTorpedo:
                    {
                        maxRadius = 30f;
                        damage = 150f;
                        break;
                    }
                case BlastType.NukeTorpedo:
                    {
                        maxRadius = 80f;
                        damage = 500;
                        break;
                    }
                case BlastType.SmallShip:
                    {
                        maxRadius = 18f;
                        damage = 20;
                        break;
                    }
                case BlastType.MediumShip:
                    {
                        maxRadius = 24f;
                        damage = 30;
                        break;
                    }
                case BlastType.Corvette:
                    {
                        maxRadius = 40f;
                        damage = 40;
                        break;
                    }
                case BlastType.Shell:
                    {
                        maxRadius = 2f;
                        damage = 20;
                        break;
                    }
                case BlastType.ExplosiveShell:
                    {
                        maxRadius = 5f;
                        damage = 40;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            ttl = 2f;
            transform.localScale = new Vector3(maxRadius / 4, maxRadius / 4, maxRadius / 4);
            //transform.GetComponentInChildren<ParticleSystem>().transform.localScale = new Vector3(maxRadius / 4, maxRadius / 4, maxRadius / 4);
            waveSpeed = maxRadius / ttl;
        }

        void Update()
        {
            if (transform.localScale.y < maxRadius)
                transform.localScale += new Vector3(waveSpeed * Time.deltaTime, waveSpeed * Time.deltaTime, waveSpeed * Time.deltaTime);
            if (ttl < 0)
                Destroy(gameObject);
            else ttl -= Time.deltaTime;
        }
    }
}
