using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PracticeProject
{
    public class Scout : Unit
    {
        public bool jamming;//Make private after debug;
        public bool warpDrive;//Make private after debug;
        public new float Stealthness { get { if (jamming) return stealthness * 0.6f; else return stealthness; } }
        //private float cooldownInhibitor;
        public float cooldownJammer; //Make private after debug;
        public float cooldownWarp;//Make private after debug;
        public float cooldownMissileInhibitor;//Make private after debug;
        protected override void StatsUp()
        {
            type = UnitClass.Scout;
            maxHealth = 100; //set in child
            radarRange = 300; //set in child
            speed = 12; //set in child
            battleAIEnabled = true; //set in child
            selfDefenceAIEnabled = true; //set in child
            roleModuleEnabled = true; //set in child
            jamming = false;
            warpDrive = false;
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
            if (cooldownWarp > 0)
                cooldownWarp -= Time.deltaTime;
            if (warpDrive && cooldownWarp <= 0)
            {
                //Debug.Log("WarpOff");
                warpDrive = false;
                speed = speed * 0.1f;
                this.GetComponent<Rigidbody>().mass = this.GetComponent<Rigidbody>().mass * 10f;
                cooldownWarp = 6;
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
            //Warp();
            return false;
        }
        protected override bool SelfDefenceFunction()
        {
            RadarWarningResiever();
            MissileGuidanceInhibitor();
            return true;
        }
        public override void SendTo(Vector3 destination)
        {
            Warp();
            waitingBackCount = 3;
            aiStatus = UnitStateType.UnderControl;
            Driver.MoveTo(destination);
        }
        public override void SendToQueue(Vector3 destination)
        {
            Warp();
            waitingBackCount = 3;
            aiStatus = UnitStateType.UnderControl;
            Driver.MoveToQueue(destination);
        }
        private void RadarWarningResiever()
        {
            int CapByTarget = 0;
            foreach (GameObject x in enemys)
            {
                if (x.GetComponent<Unit>().CurrentTarget == this)
                    CapByTarget++;
            }
            if (CapByTarget > 5)
                Jammer();
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
                                cooldownMissileInhibitor = 8;
                                return true;
                            }
                        }
                    }
            }
            return false;
        }
        private void Warp()
        {
            if (!warpDrive&&cooldownWarp <= 0)
            {
                //Debug.Log("WarpOn");
                warpDrive = true;
                speed = speed * 10;
                this.GetComponent<Rigidbody>().mass = this.GetComponent<Rigidbody>().mass * 0.1f;
                Driver.UpdateSpeed();
                cooldownWarp = 2f;
            }
        }
    }
}
