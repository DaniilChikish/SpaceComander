using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpaceCommander
{
    public class Armor : MonoBehaviour, IArmor
    {
        [SerializeField]
        private float maxHitpoints;
        [SerializeField]
        private float hitpoints;
        [SerializeField]
        private float shellResist;
        [SerializeField]
        private float energyResist;
        [SerializeField]
        private float blastResist;
        private float hitCount;
        private SpaceShip owner;
        public float Hitpoints { get { return hitpoints; } set { hitpoints = value; } }
        public float MaxHitpoints { get { return this.maxHitpoints; } }
        public float ShellResist { get { return this.shellResist; } }
        public float EnergyResist { get { return this.energyResist; } }
        public float BlastResist { get { return this.blastResist; } }
        private void Start()
        {
            owner = transform.GetComponentInParent<SpaceShip>();
        }
        public void StatUp(float hitpoints, float maxHitpoints, float shellResist, float energyResist, float blastResist)
        {
            this.hitpoints = hitpoints;
            this.maxHitpoints = maxHitpoints;
            this.shellResist = shellResist;
            this.energyResist = energyResist;
            this.blastResist = blastResist;
        }
        // Update is called once per frame
        void Update()
        {
            if (hitpoints < 0)
                owner.Die();
            else if (hitpoints < maxHitpoints * 0.1)
                hitpoints -= Time.deltaTime;
            else if (hitpoints < maxHitpoints * 0.3)
                owner.ArmorCriticalAlarm();
        }
        protected void OnCollisionEnter(Collision collision)
        {
            float multiplicator;
            switch (collision.gameObject.tag)
            {
                case "Shell":
                    {
                        //Global.Sound.InstantHit(collision.contacts[0].point, Global.Settings.SoundLevel);
                        float difference = Mathf.Clamp(collision.gameObject.GetComponent<IShell>().ArmorPiersing - ShellResist + 3, 0, 4f);
                        multiplicator = difference / 3;
                        if (!owner.ShieldOwerheat) multiplicator = multiplicator * (1 - owner.ShieldForce / owner.ShieldCampacity);
                        float damage = collision.gameObject.GetComponent<IShell>().Damage * multiplicator;
                        this.hitpoints -= damage;
                        break;
                    }
                case "Energy":
                    {
                        hitCount += 3;
                        float difference = Mathf.Clamp(collision.gameObject.GetComponent<IEnergy>().ArmorPiersing - EnergyResist + 2, 0, 4f);
                        multiplicator = difference / 2;
                        if (!owner.ShieldOwerheat) multiplicator = multiplicator * 0.3f;
                        float damage = collision.gameObject.GetComponent<IEnergy>().Damage * multiplicator;
                        this.hitpoints -= damage;
                        break;
                    }
                case "Unit":
                    {
                        Unit uncknown = collision.gameObject.GetComponent<Unit>();
                        Vector3 normalSum = Vector3.zero;
                        foreach (var x in collision.contacts)
                            normalSum += x.normal;
                        Vector3 kick = Vector3.Project(uncknown.Velocity - owner.Velocity, normalSum.normalized);
                        this.hitpoints -= kick.magnitude;
                        break;
                    }
                case "Terrain":
                    {
                        Vector3 normalSum = Vector3.zero;
                        foreach (var x in collision.contacts)
                            normalSum += x.normal;
                        Vector3 kick = Vector3.Project(owner.Velocity, normalSum.normalized);
                        this.hitpoints -= kick.magnitude;
                        //this.transform.position = Vector3.MoveTowards(this.transform.position, normalSum * 5, Time.deltaTime * owner.ShiftSpeed * 0.2f);
                        break;
                    }
            }
        }
        protected void OnCollisionStay(Collision collision)
        {
            float multiplicator;
            switch (collision.gameObject.tag)
            {
                case "Energy":
                    {
                        hitCount += 3;
                        float difference = Mathf.Clamp(collision.gameObject.GetComponent<IEnergy>().ArmorPiersing - EnergyResist + 2, 0, 4f);
                        multiplicator = difference / 2;
                        if (!owner.ShieldOwerheat) multiplicator = multiplicator * 0.3f;
                        float damage = collision.gameObject.GetComponent<IEnergy>().Damage * multiplicator;
                        this.hitpoints -= damage;
                        break;
                    }
                case "Explosion":
                    {
                        multiplicator = (1f - BlastResist) * Mathf.Pow(((-Vector3.Distance(this.gameObject.transform.position, collision.gameObject.transform.position) + collision.gameObject.GetComponent<Explosion>().MaxRadius) * 0.01f), (1f / 3f));
                        this.hitpoints -= collision.gameObject.GetComponent<Explosion>().Damage * multiplicator;
                        break;
                    }
            }
        }
        protected void OnTriggerStay(Collider trigger)
        {
            float multiplicator;
            switch (trigger.gameObject.tag)
            {
                case "Energy":
                    {
                        //Debug.Log(this.owner.name + ": Laser pierse by" + trigger.gameObject.GetComponentInParent<SpaceShip>().name);
                        hitCount += 3;
                        float difference = Mathf.Clamp(trigger.gameObject.GetComponent<IEnergy>().ArmorPiersing - EnergyResist + 2, 0, 4f);
                        multiplicator = difference / 2;
                        if (!owner.ShieldOwerheat) multiplicator = multiplicator * 0.3f;
                        float damage = trigger.gameObject.GetComponent<IEnergy>().Damage * multiplicator;
                        this.hitpoints -= damage;
                        break;
                    }
                case "Explosion":
                    {
                        float sizeFactor = 0;
                        if (owner.Type == UnitClass.LR_Corvette)
                            sizeFactor = 30;
                        else if (owner.Type == UnitClass.Guard_Corvette || owner.Type == UnitClass.Support_Corvette)
                            sizeFactor = 15;
                        if (Vector3.Distance(trigger.transform.position, this.transform.position) < trigger.gameObject.GetComponent<Explosion>().MaxRadius + sizeFactor)
                        {
                            multiplicator = Mathf.Clamp01((1 - BlastResist) * (trigger.gameObject.GetComponent<Explosion>().MaxRadius + sizeFactor - Vector3.Distance(trigger.transform.position, this.transform.position)) / trigger.gameObject.GetComponent<Explosion>().MaxRadius);
                            this.hitpoints -= trigger.gameObject.GetComponent<Explosion>().Damage * multiplicator;
                        }
                        break;
                    }
            }
        }
    }
}
