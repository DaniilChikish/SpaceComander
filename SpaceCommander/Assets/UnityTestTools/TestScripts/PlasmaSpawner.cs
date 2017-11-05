using SpaceCommander;
using SpaceCommander.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpaceCommander.Test
{
    class PlasmaSpawner : MonoBehaviour
    {
        public int firerate;
        public float backCount;
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
                GameObject sphere = Instantiate(Global.Prefab.PlasmaSphere, gameObject.transform.position, transform.rotation);
                sphere.GetComponent<PlazmaSphere>().StatUp(EnergyType.Plazma);
            }
        }
    }
}
