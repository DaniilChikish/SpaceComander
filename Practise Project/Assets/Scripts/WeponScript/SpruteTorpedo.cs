using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PracticeProject
{
    class SpruteTorpedo : Torpedo
    {
        public override void Explode()
        {
            List<GameObject> enemys = new List<GameObject>();
            foreach (GameObject x in FindObjectsOfType<GlobalController>()[0].unitList)
            {
                if (x.GetComponent<Unit>().alliesArmy == allies)
                {
                    float distance = Vector3.Distance(this.gameObject.transform.position, x.transform.position);
                    if (distance < explosionRange * 2)
                    {
                        enemys.Add(x);
                    }
                }
            }
            GameObject[] missile = new GameObject[6];
            float a = 0.7f;
            float h = Mathf.Sqrt((a * a) - (a * a / 4));
            missile[0] = Instantiate(Blast, gameObject.transform.position + transform.up * a, Quaternion.Euler(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, 180));
            missile[1] = Instantiate(Blast, gameObject.transform.position - transform.up * a, Quaternion.Euler(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, 0));
            missile[2] = Instantiate(Blast, gameObject.transform.position + transform.up * a/2 + transform.right * h, Quaternion.Euler(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, 130));
            missile[3] = Instantiate(Blast, gameObject.transform.position - transform.up * a/2 + transform.right * h, Quaternion.Euler(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, 60));
            missile[4] = Instantiate(Blast, gameObject.transform.position + transform.up * a/2 - transform.right * h, Quaternion.Euler(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, -130));
            missile[5] = Instantiate(Blast, gameObject.transform.position - transform.up * a/2 - transform.right * h, Quaternion.Euler(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, -60));
            if (enemys.Count > 0)
            {
                int j = 0, i = 0;
                for (i = 0; i < 6; i++)
                {
                    missile[i].GetComponent<Missile>().SetTarget(enemys[j].transform);
                    if (j < enemys.Count - 1)
                        j++;
                    else j = 0;
                }
            }
            Destroy(gameObject);
        }
    }
}
