using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PracticeProject
{
    class UnitaryTorpedo : Torpedo
    {
        public override void Explode()
        {
            GameObject blast = Instantiate(FindObjectOfType<GlobalController>().ExplosiveBlast, this.transform.position, this.transform.rotation);
            blast.GetComponent<Explosion>().StatUp(BlastType.UnitaryTorpedo);
            Destroy(gameObject);
        }
    }
}
