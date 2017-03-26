using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PracticeProject
{
    public class Explosion : MonoBehaviour {
        public BlastType Type;
        public float waveSpeed; // Скорость волны
        public float maxRadius; // Радиус взрыва
        public float damage; // Урон
        public float Damage { get { return damage; } }
        public float MaxRadius { get { return maxRadius; } }

        void Start()
        {
            switch (Type)
            {
                case BlastType.Missile:
                    {
                        maxRadius = 6f;
                        waveSpeed = 1f;
                        damage = 100;
                        break;
                    }
                case BlastType.Torpedo:
                    {
                        maxRadius = 10f;
                        waveSpeed = 1.1f;
                        damage = 200;
                        break;
                    }
                case BlastType.Ship:
                    {
                        maxRadius = 8f;
                        waveSpeed = 1.1f;
                        damage = 20;
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
