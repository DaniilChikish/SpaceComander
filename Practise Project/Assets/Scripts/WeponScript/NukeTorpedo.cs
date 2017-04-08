using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PracticeProject
{
    class NukeTorpedo : Torpedo
    {
        public override void Explode()
        {
            GameObject blast = Instantiate(FindObjectOfType<GlobalController>().NukeBlast, this.transform.position, this.transform.rotation);
            blast.GetComponent<Explosion>().StatUp(BlastType.NukeTorpedo);
            Destroy(gameObject);
        }
    }
}
