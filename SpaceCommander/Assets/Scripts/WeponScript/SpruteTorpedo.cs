using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCommander.Weapons
{
    class SpruteTorpedo : SelfguidedMissile
    {
        protected override void Start()
        {
            base.Start();
            body.AddForce(-transform.up * dropImpulse, ForceMode.Impulse);
        }
        public override void Explode()
        {
            GameObject[] missile = new GameObject[6];
            float a = 0.7f;
            float h = Mathf.Sqrt((a * a) - (a * a / 4));
            missile[0] = Instantiate(Global.Missile, gameObject.transform.position + transform.up * a, Quaternion.Euler(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, 180));
            missile[1] = Instantiate(Global.Missile, gameObject.transform.position - transform.up * a, Quaternion.Euler(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, 0));
            missile[2] = Instantiate(Global.Missile, gameObject.transform.position + transform.up * a / 2 + transform.right * h, Quaternion.Euler(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, 130));
            missile[3] = Instantiate(Global.Missile, gameObject.transform.position - transform.up * a / 2 + transform.right * h, Quaternion.Euler(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, 60));
            missile[4] = Instantiate(Global.Missile, gameObject.transform.position + transform.up * a / 2 - transform.right * h, Quaternion.Euler(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, -130));
            missile[5] = Instantiate(Global.Missile, gameObject.transform.position - transform.up * a / 2 - transform.right * h, Quaternion.Euler(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, -60));
            List<SpaceShip> enemys = new List<SpaceShip>();
            foreach (SpaceShip x in Global.unitList)
            {
                float distance = Vector3.Distance(this.gameObject.transform.position, x.transform.position);
                if (distance < explosionRange * 3)
                {
                    if (!x.Allies(this.Team))
                        enemys.Add(x);
                }
            }
            if (enemys.Count > 0)
            {
                //Debug.Log(enemys.Count + " enemy finded");
                int i = 0;
                foreach (GameObject M in missile)
                {
                    M.AddComponent<InterceptorMissile>().SetTarget(enemys[i].transform);
                    i++;
                    if (i >= enemys.Count)
                        i = 0;
                }
            }
            else
            {
                foreach (GameObject M in missile)
                    M.AddComponent<InterceptorMissile>().SetTarget(null);
            }
            Destroy(gameObject);
        }
    }
}
