using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpaceCommander
{
    public class Explosion : MonoBehaviour
    {
        private const float soundSpeed = 343f; // m/s
        public BlastType type;
        private float ttl;
        private float waveLife;
        private float waveSpeed; // Скорость волны
        private float maxRadius; // Радиус взрыва
        private float damage; // Урон в секунду
        private SphereCollider colider;
        public float Damage { get { return damage * Time.deltaTime; } }
        public float MaxRadius { get { return maxRadius; } }
        void Start() { StatUp(type); }
        public void StatUp(BlastType type)
        {
            this.type = type;
            ttl = 10f;
            waveLife = 2f;
            float volumeScale = 1;
            colider = GetComponent<SphereCollider>();
            AudioSource sound;
            float soundDelay;
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
                        damage = 300f;
                        break;
                    }
                case BlastType.NukeTorpedo:
                    {
                        maxRadius = 1000f;
                        damage = 1000;
                        waveLife = 4f;
                        volumeScale = 2;
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
                        volumeScale = 0.5f;
                        break;
                    }
                case BlastType.ExplosiveShell:
                    {
                        maxRadius = 5f;
                        damage = 50;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            transform.localScale = new Vector3(maxRadius / 4, maxRadius / 4, maxRadius / 4);
            waveSpeed = maxRadius / ttl;
            sound = this.gameObject.GetComponent<AudioSource>();
            sound.minDistance = maxRadius;
            sound.maxDistance = maxRadius * 100f;
            sound.volume = FindObjectOfType<GlobalController>().Settings.SoundLevel * volumeScale;
            soundDelay = (Vector3.Distance(this.transform.position, Camera.main.transform.position) / soundSpeed);
            sound.PlayDelayed(soundDelay);
        }

        void Update()
        {
            if (transform.localScale.y < maxRadius)
                transform.localScale += new Vector3(waveSpeed * Time.deltaTime, waveSpeed * Time.deltaTime, waveSpeed * Time.deltaTime);
            if (ttl < 0)
                Destroy(gameObject);
            else ttl -= Time.deltaTime;
            if (waveLife <= 0)
                colider.enabled = false;
            else waveLife -= Time.deltaTime;
        }
    }
}
