using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PracticeProject
{
    class SpruteTorpedo : Torpedo
    {
        public override void Explode()
        {
            GameObject[] missile = new GameObject[6];
            float a = 0.7f;
            float h = Mathf.Sqrt((a * a) - (a * a / 4));
            missile[0] = Instantiate(Blast, gameObject.transform.position + transform.up * a, Quaternion.Euler(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, 180));
            missile[1] = Instantiate(Blast, gameObject.transform.position - transform.up * a, Quaternion.Euler(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, 0));
            missile[2] = Instantiate(Blast, gameObject.transform.position + transform.up * a/2 + transform.right * h, Quaternion.Euler(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, 130));
            missile[3] = Instantiate(Blast, gameObject.transform.position - transform.up * a/2 + transform.right * h, Quaternion.Euler(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, 60));
            missile[4] = Instantiate(Blast, gameObject.transform.position + transform.up * a/2 - transform.right * h, Quaternion.Euler(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, -130));
            missile[5] = Instantiate(Blast, gameObject.transform.position - transform.up * a/2 - transform.right * h, Quaternion.Euler(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, -60));
            Destroy(gameObject);
        }
    }
}
