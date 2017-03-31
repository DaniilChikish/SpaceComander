using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PracticeProject
{
    public class Command : Unit
    {
        public bool jamming;//Make private after debug;
        public new float Stealthness { get { if (jamming) return stealthness * 0.8f; else return stealthness; } }
        public float cooldownJammer; //Make private after debug;
        public float cooldownMissileInhibitor;//Make private after debug;
        protected override void StatsUp()
        {
            type = UnitClass.Command;
            maxHealth = 150; //set in child
            radarRange = 300; //set in child
            speed = 12; //set in child
            battleAIEnabled = true; //set in child
            selfDefenceModuleEnabled = true; //set in child
            roleModuleEnabled = true; //set in child
            jamming = false;
            stealthness = 0.2f; //set in child
        }
        protected override void DecrementCounters()
        {
            if (cooldownJammer > 0)
                cooldownJammer -= Time.deltaTime;
            if (jamming && cooldownJammer <= 0)
            {
                jamming = false;
                cooldownJammer = 5;
            }
            if (cooldownMissileInhibitor > 0)
                cooldownMissileInhibitor -= Time.deltaTime;
        }
        //AI logick
        //protected override bool ManeuverFunction()
        //{
        //    return false;
        //}
        protected override bool RoleFunction()
        {
            if (CurrentTarget != null)
            {

                return true;
            }
            else return false;
        }
        protected override bool SelfDefenceFunction()
        {
            if (RadarWarningResiever() > 5)
                Jammer();
            MissileGuidanceInhibitor();
            return true;
        }
        private void Jammer()
        {
            if (!jamming && cooldownJammer <= 0)
            {
                jamming = true;
                cooldownJammer = 3f;
            }
        }
        private bool MissileGuidanceInhibitor()
        {
            if (cooldownMissileInhibitor <= 0)
            {
                GameObject[] missiles = GameObject.FindGameObjectsWithTag("Missile");
                if (missiles.Length > 0)
                    foreach (GameObject x in missiles)
                    {
                        if (x.GetComponent<Missile>().target == gameObject.transform)
                        {
                            float distance = Vector3.Distance(x.transform.position, this.transform.position);
                            float multiplicator = Mathf.Pow(((-distance + (RadarRange * 0.5f)) * 0.02f), (1 / 3));
                            if (Randomizer.Uniform(0, 100, 1)[0] < 70 * multiplicator)
                            {
                                x.GetComponent<Missile>().target = null;
                                cooldownMissileInhibitor = 4;
                                return true;
                            }
                        }
                    }
            }
            return false;
        }
    }
}
