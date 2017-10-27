using SpaceCommander;
using SpaceCommander.Weapons;
using UnityEngine;

namespace SpaceCommander.Test
{
    class ShellSpawner : MonoBehaviour
    {
        public int firerate;
        public float backCount;
        public float speed;
        public float damage;
        public float armorPiersing;
        public bool canRicochet;
        public GameObject explosionPrefab;
        public float mass;
        public float scale;
        public float DamageMultiplacator;
        public float RoundspeedMultiplacator;
        public float APMultiplacator;
        public float ShellmassMultiplacator;
        private GlobalController Global;
        private void Start()
        {
            Global = FindObjectOfType<GlobalController>();
        }
        private void Update()
        {
            if (backCount > 0)
                backCount -= Time.deltaTime;
            else
            {
                backCount = 60f / firerate;
                GameObject shell = Instantiate(Global.UnitaryShell, gameObject.transform.position, transform.rotation);
                shell.transform.localScale = shell.transform.localScale * scale;
                shell.GetComponent<IShell>().StatUp((speed * (1 + RoundspeedMultiplacator) * this.transform.forward), damage * (1 + DamageMultiplacator), armorPiersing * (1 + APMultiplacator), mass * (1 + ShellmassMultiplacator), canRicochet, explosionPrefab);
            }
        }
    }
}
