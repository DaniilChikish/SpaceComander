using SpaceCommander.AI;
using SpaceCommander.General;
using SpaceCommander.Mechanics;
using UnityEngine;
namespace SpaceCommander.Test
{
    public class TestArmor : MonoBehaviour, IArmor
    {
        [SerializeField]
        public float maxHitpoints;
        [SerializeField]
        public float hitpoints;
        [SerializeField]
        public float shellResist;
        [SerializeField]
        public float energyResist;
        [SerializeField]
        public float blastResist;
        private float hitCount;
        private SpaceShip owner;
        public float Hitpoints { get { return hitpoints; } set { hitpoints = value; } }
        public float MaxHitpoints { get { return this.maxHitpoints; } }
        public float ShellResist { get { return this.shellResist * (1 + owner.ResistMultiplacator); } }
        public float EnergyResist { get { return this.energyResist * (1 + owner.ResistMultiplacator); } }
        public float BlastResist { get { return this.blastResist * (1 + owner.ResistMultiplacator); } }
        public void StatUp(float hitpoints, float maxHitpoints, float shellResist, float energyResist, float blastResist)
        { }
        void Start()
        {
            owner = transform.GetComponentInParent<SpaceShip>();
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
                        hitCount += 1;
                        float difference = Mathf.Clamp(collision.gameObject.GetComponent<IShell>().ArmorPiersing - ShellResist + 3, 0, 4f);
                        multiplicator = difference / 3;
                        if (!owner.ShieldOwerheat) multiplicator = multiplicator * (1 - owner.ShieldForce / owner.ShieldCampacity);
                        float damage = collision.gameObject.GetComponent<IShell>().Damage * multiplicator;
                        Debug.Log(owner.transform.name + " take " + damage + " damage by shell with " + collision.gameObject.GetComponent<IShell>().ArmorPiersing + " AP");
                        break;
                    }
                case "Energy":
                    {
                        hitCount += 3;
                        float difference = Mathf.Clamp(collision.gameObject.GetComponent<IEnergy>().ArmorPiersing - EnergyResist + 2, 0, 4f);
                        multiplicator = difference / 2;
                        if (!owner.ShieldOwerheat) multiplicator = multiplicator * 0.3f;
                        float damage = collision.gameObject.GetComponent<IEnergy>().Damage * multiplicator;
                        Debug.Log(owner.transform.name + " take " + damage + " damage by energy with " + collision.gameObject.GetComponent<IEnergy>().ArmorPiersing + " AP");
                        break;
                    }
                case "Unit":
                    {
                        Unit uncknown = collision.gameObject.GetComponent<Unit>();
                        Vector3 normalSum = Vector3.zero;
                        foreach (var x in collision.contacts)
                            normalSum += x.normal;
                        Vector3 kick = Vector3.Project(uncknown.Velocity - owner.Velocity, normalSum.normalized);
                        float damage = kick.magnitude;
                        Debug.Log(owner.transform.name + " take " + damage + " damage by collision");
                        break;
                    }
                case "Terrain":
                    {
                        Vector3 normalSum = Vector3.zero;
                        foreach (var x in collision.contacts)
                            normalSum += x.normal;
                        Vector3 kick = Vector3.Project(owner.Velocity, normalSum.normalized);
                        float damage = kick.magnitude;
                        Debug.Log(owner.transform.name + " take " + damage + " damage by collision");
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
                        float damage = collision.gameObject.GetComponent<IEnergy>().Damage * multiplicator * Time.deltaTime;
                        Debug.Log(owner.transform.name + " take " + damage + " damage by energy with " + collision.gameObject.GetComponent<IEnergy>().ArmorPiersing + " AP");
                        break;
                    }
                case "Explosion":
                    {
                        multiplicator = (1f - BlastResist) * Mathf.Pow(((-Vector3.Distance(this.gameObject.transform.position, collision.gameObject.transform.position) + collision.gameObject.GetComponent<Explosion>().MaxRadius) * 0.01f), (1f / 3f));
                        float damage = collision.gameObject.GetComponent<Explosion>().Damage * multiplicator;
                        Debug.Log(owner.transform.name + " take " + damage + " damage by explosion");
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
                        hitCount += 3;
                        float difference = Mathf.Clamp(trigger.gameObject.GetComponent<IEnergy>().ArmorPiersing - EnergyResist + 2, 0, 4f);
                        multiplicator = difference / 2;
                        if (!owner.ShieldOwerheat) multiplicator = multiplicator * 0.3f;
                        float damage = trigger.gameObject.GetComponent<IEnergy>().Damage * multiplicator * Time.deltaTime;
                        Debug.Log(owner.transform.name + " take " + damage + " damage by energy with " + trigger.gameObject.GetComponent<IEnergy>().ArmorPiersing + " AP");
                        break;
                    }
                case "Explosion":
                    {
                        float sizeFactor = 0;
                        if (owner.Type == UnitClass.LR_Corvette)
                            sizeFactor = 10;
                        else if (owner.Type == UnitClass.Guard_Corvette || owner.Type == UnitClass.Support_Corvette)
                            sizeFactor = 5;
                        if (Vector3.Distance(trigger.transform.position, this.transform.position) < trigger.gameObject.GetComponent<Explosion>().MaxRadius + sizeFactor)
                        {
                            multiplicator = (1 - BlastResist) * (trigger.gameObject.GetComponent<Explosion>().MaxRadius + sizeFactor - Vector3.Distance(trigger.transform.position, this.transform.position)) / trigger.gameObject.GetComponent<Explosion>().MaxRadius;
                            if (multiplicator > 1) multiplicator = 1;
                            if (multiplicator < 0) multiplicator = 0;
                            float damage = trigger.gameObject.GetComponent<Explosion>().Damage * multiplicator;
                            hitpoints -= damage;
                            Debug.Log(owner.transform.name + " take " + damage + " damage by explosion");
                        }
                        break;
                    }
            }
        }
    }
}
