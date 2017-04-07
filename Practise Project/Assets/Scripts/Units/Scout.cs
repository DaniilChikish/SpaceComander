using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace PracticeProject
{
    public class Scout : SpaceShip
    {
        public bool jamming;//Make private after debug;
        public bool warpDrive;//Make private after debug;
        public new float Stealthness { get { if (jamming) return stealthness * 0.6f; else return stealthness; } }
        //private float cooldownInhibitor;
        public float cooldownJammer; //Make private after debug;
        public float cooldownWarp;//Make private after debug;
        public float cooldownMissileInhibitor;//Make private after debug;
        private bool idleFulag;
        protected override void StatsUp()
        {
            type = UnitClass.Scout;
            radarRange = 300; //set in child
            radarPover = 5;
            speed = 10; //set in child
            jamming = false;
            warpDrive = false;
            stealthness = 0.3f; //set in child
            radiolink = 2.5f;
            sortDelegate = SortEnemys;
        }
        protected override void Explosion()
        {
            GameObject blast = Instantiate(Global.ShipDieBlast, gameObject.transform.position, gameObject.transform.rotation);
            blast.GetComponent<Explosion>().StatUp(BlastType.SmallShip);
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
                speed = speed / 6;
                this.GetComponent<Rigidbody>().mass = this.GetComponent<Rigidbody>().mass * 10f;
                cooldownWarp = 15;
            }
            if (cooldownMissileInhibitor > 0)
                cooldownMissileInhibitor -= Time.deltaTime;
        }
        protected override bool RoleFunction()
        {
            return Warp();
        }
        protected override bool SelfDefenceFunction()
        {
            SelfDefenceFunctionBase();
            if (RadarWarningResiever() > 5)
                Jammer();
            MissileGuidanceInhibitor();
            return true;
        }
        //AI logick
        protected override bool CombatManeuverFunction()
        {
            switch (targetStatus)
            {
                case TargetStateType.Captured:
                    {
                        if (Warp())
                            return Rush();
                        else return ToSecondaryDistance();
                    }
                case TargetStateType.InPrimaryRange:
                    {
                        return Evasion();
                    }
                case TargetStateType.InSecondaryRange:
                    {
                        if (Warp())
                            return Rush();
                        else return ToPrimaryDistance();
                    }
                case TargetStateType.BehindABarrier:
                    {
                        return ToSecondaryDistance();
                    }
                default:
                    return false;
            }
        }
        protected override bool IdleManeuverFunction()
        {
            //Debug.Log("new loop");
            idleFulag = !idleFulag;
            if (idleFulag)
                return PatroolTriangle();
            else return PatroolPoint();
        }
        private bool PatroolTriangle()
        {
            //waitingBackCount = 40f;
            float dirflag;
            if (Randomizer.Uniform(-10, 10, 1)[0] > 0)
                dirflag = -1;
            else dirflag = 1;
            Vector3 point;
            float n = Convert.ToSingle(Randomizer.Uniform(10, 25, 1)[0]);
            float j = 1;
            for (float i = 1; i<8; i++)
            {
                point = (transform.forward * n * i) + (transform.right * j * dirflag * n) + new Vector3(0,0.5f,0) + this.transform.position;
                dirflag = dirflag * -1;
                Driver.MoveToQueue(point);
                j++;
            }
            Driver.MoveToQueue(this.transform.position);
            return true;
        }

        public override void SendTo(Vector3 destination)
        {
            Warp();
            orderBackCount = Vector3.Distance(this.transform.position, destination) / (this.GetComponent<NavMeshAgent>().speed * 0.9f);
            aiStatus = UnitStateType.UnderControl;
            Driver.MoveTo(destination);
        }
        public override void SendToQueue(Vector3 destination)
        {
            Warp();
            orderBackCount += Vector3.Distance(this.transform.position, destination) / (this.GetComponent<NavMeshAgent>().speed * 0.9f);
            aiStatus = UnitStateType.UnderControl;
            Driver.MoveToQueue(destination);
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
                        if (x.GetComponent<SelfguidedMissile>().target == gameObject.transform)
                        {
                            float distance = Vector3.Distance(x.transform.position, this.transform.position);
                            float multiplicator = Mathf.Pow(((-distance + (RadarRange * 0.5f)) * 0.02f), (1 / 3));
                            if (Randomizer.Uniform(0, 100, 1)[0] < 70 * multiplicator)
                            {
                                x.GetComponent<SelfguidedMissile>().target = null;
                                cooldownMissileInhibitor = 8;
                                return true;
                            }
                        }
                    }
            }
            return false;
        }
        private bool Warp()
        {
            if (!warpDrive && cooldownWarp <= 0)
            {
                //Debug.Log("WarpOn");
                warpDrive = true;
                speed = speed * 6;
                this.GetComponent<Rigidbody>().mass = this.GetComponent<Rigidbody>().mass / 10;
                Driver.UpdateSpeed();
                cooldownWarp = 1.4f;
                //waitingBackCount = 1.5f;
                return true;
            }
            else return false;
        }
        private int SortEnemys(IUnit x, IUnit y)
        {
            int xPriority;
            int yPriority;
            switch (x.Type)
            {
                case UnitClass.Command: //высший приоритет - командир
                    {
                        xPriority = 20;
                        break;
                    }
                case UnitClass.Recon: //жертва
                    {
                        xPriority = 10;
                        break;
                    }
                case UnitClass.Scout: //паритет
                    {
                        xPriority = 5;
                        break;
                    }
                case UnitClass.ECM: //хищник
                    {
                        xPriority = -5;
                        break;
                    }
                default: //более крупные цели не интересны
                    {
                        xPriority = 0;
                        break;
                    }
            }
            switch (y.Type)
            {
                case UnitClass.Command: //высший приоритет - командир
                    {
                        yPriority = 20;
                        break;
                    }
                case UnitClass.Recon: //жертва
                    {
                        yPriority = 10;
                        break;
                    }
                case UnitClass.Scout: //паритет
                    {
                        yPriority = 5;
                        break;
                    }
                case UnitClass.ECM: //хищник
                    {
                        yPriority = -5;
                        break;
                    }
                default: //более крупные цели не интересны
                    {
                        yPriority = 0;
                        break;
                    }
            }
            float xDictance = Vector3.Distance(this.transform.position, x.ObjectTransform.position);
            float yDistance = Vector3.Distance(this.transform.position, y.ObjectTransform.position);
            if ((xDictance - yDistance) > -100 && (xDictance - yDistance) < 100)
            { } //приоритет не меняется
            else
            {
                if (xDictance > yDistance)
                    yPriority += 5;
                else
                    xPriority += 5;
            }
            if (xPriority > yPriority)
                return -1;
            else return 1;
        }
    }
}
