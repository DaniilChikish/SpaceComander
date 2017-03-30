using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PracticeProject
{
    public class ECM : Unit
    {
        public bool jamming;//Make private after debug;
        public new float Stealthness { get { if (jamming) return stealthness * 0.4f; else return stealthness; } }
        public float cooldownJammer; //Make private after debug;
        public float cooldownWeaponInhibitor;//Make private after debug;
        public float cooldownMissileInhibitor;//Make private after debug;
        protected override void StatsUp()
        {
            type = UnitClass.Scout;
            maxHealth = 100; //set in child
            radarRange = 150; //set in child
            radarPover = 5;
            speed = 9; //set in child
            jamming = false;
            stealthness = 0.2f; //set in child
            radiolink = 1.1f;
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
            if (cooldownWeaponInhibitor > 0)
                cooldownWeaponInhibitor -= Time.deltaTime;
            if (cooldownMissileInhibitor > 0)
                cooldownMissileInhibitor -= Time.deltaTime;
        }
        //AI logick
        protected override bool BattleManeuverFunction()
        {
            switch (targetStatus)
            {
                case TargetStateType.Captured:
                    {
                        return ShortenDistance();
                    }
                case TargetStateType.InPrimaryRange:
                    {
                        return IncreaseDistance();
                    }
                case TargetStateType.InSecondaryRange:
                    {
                        if (WeaponInhibitor(CurrentTarget))
                            return Rush();
                        else
                        return ShortenDistance();
                    }
                case TargetStateType.BehindABarrier:
                    {
                        return Rush();
                    }
                default:
                    return false;
            }
        }
        protected override bool RoleFunction()
        {
            if (CurrentTarget != null)
            {
                WeaponInhibitor(CurrentTarget);
                return true;
            }
            else return false;
        }
        protected override bool SelfDefenceFunction()
        {
            SelfDefenceFunctionBase();
            if (RadarWarningResiever() > 5)
                Jammer();
            MissileGuidanceInhibitor();
            return true;
        }
        protected override int RadarWarningResiever()
        {
            capByTarget.Clear();
            foreach (GameObject x in enemys)
            {
                if (x.GetComponent<Unit>().CurrentTarget == this)
                    capByTarget.Add(x);
                if (jamming)
                {
                    x.GetComponent<Unit>().CurrentTarget = null;
                    if (cooldownWeaponInhibitor < 0)
                        x.GetComponent<Unit>().inhibition = 0.1f;
                }
            }
            capByTarget.Sort(delegate (GameObject x, GameObject y)
            {
                if (Vector3.Distance(this.transform.position, x.transform.position) > Vector3.Distance(this.transform.position, y.transform.position))
                    return 1;
                else return -1;
            });
            return capByTarget.Count;
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
                                cooldownMissileInhibitor = 3.5f;
                                return true;
                            }
                        }
                    }
            }
            return false;
        }
        private bool WeaponInhibitor(GameObject target)
        {
            if (cooldownWeaponInhibitor <= 0)
            {
                target.GetComponent<Unit>().inhibition = 4f;
                cooldownWeaponInhibitor = 10f;
                return true;
            }
            return false;
        }
    }
}
