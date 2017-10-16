using SpaceCommander;
using SpaceCommander.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.TestScripts
{
    class PlasmaSpawner : MonoBehaviour
    {
        public int firerate;
        public float backCount;
        private void Update()
        {
            if (backCount > 0)
                backCount -= Time.deltaTime;
            else
            {
                backCount = 60f / firerate;
                GameObject sphere = Instantiate(FindObjectOfType<GlobalController>().PlasmaSphere, gameObject.transform.position, transform.rotation);
                sphere.GetComponent<PlazmaSphere>().StatUp(EnergyType.Plazma);
            }
        }
    }
}
