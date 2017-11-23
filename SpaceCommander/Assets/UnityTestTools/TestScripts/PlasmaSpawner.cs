using SpaceCommander.Mechanics;
using SpaceCommander.Mechanics.Weapons;
using UnityEngine;

namespace SpaceCommander.Test
{
    class PlasmaSpawner : MonoBehaviour
    {
        public int firerate;
        public float backCount;
        private void Start()
        {
        }
        private void Update()
        {
            if (backCount > 0)
                backCount -= Time.deltaTime;
            else
            {
                backCount = 60f / firerate;
                GameObject sphere = Instantiate(General.GlobalController.Instance.Prefab.PlasmaSphere, gameObject.transform.position, transform.rotation);
                sphere.GetComponent<PlazmaSphere>().StatUp(EnergyType.Plazma);
            }
        }
    }
}
