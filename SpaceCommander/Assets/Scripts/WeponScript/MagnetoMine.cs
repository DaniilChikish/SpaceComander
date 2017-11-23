using UnityEngine;

namespace SpaceCommander.Mechanics.Weapons
{
    class MagnetoMine : MonoBehaviour
    {
        public Mechanics.Army team;
        public Transform target;
        public float speed;
        protected float detonateTimer;
        protected bool isArmed;
        public float ttl;
        public float lt;
        public float explosionRange;
        private float pingCount;

        private void Start()
        {
        }
        private void Update()
        {
            if (lt > ttl)
                Explode();
            else
                lt += Time.deltaTime;
            if (isArmed)
            {
                if (detonateTimer > 0)
                    detonateTimer -= Time.deltaTime;
                else Explode();
            }
            else if (pingCount <= 0)
            {
                pingCount = 0.1f;
                foreach (AI.SpaceShip x in General.GlobalController.Instance.unitList)
                {
                    if (x.Team != team && Vector3.Distance(x.transform.position, this.transform.position) < explosionRange)
                    {
                        Arm(x.transform);
                        break;
                    }
                }
            }
            else pingCount -= Time.deltaTime;
            if (target != null)
            {
                gameObject.GetComponent<Rigidbody>().AddForce((target.position - this.transform.position).normalized * speed, ForceMode.Acceleration);
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (!isArmed && other.gameObject.tag == "Unit")
            {
                //Debug.Log("triggered");
                if (other.gameObject.transform.GetComponent<Unit>().Team != this.team)
                    Arm(other.gameObject.transform);
            }
        }
        private void OnCollisionEnter(Collision other)
        {
            if (!isArmed && other.gameObject.tag == "Unit" && other.gameObject.transform.GetComponent<Unit>().Team != this.team)
                Explode();
        }
        public void Arm(Transform target)
        {
            if (this.target == null && lt > 2.5)
            {
                //Debug.Log("armed");
                this.target = target;
                detonateTimer = 0.5f;
                isArmed = true;
            }
        }

        public void Explode()
        {
            GameObject blast = Instantiate(General.GlobalController.Instance.Prefab.ExplosiveBlast, this.transform.position, this.transform.rotation);
            blast.GetComponent<Explosion>().StatUp(BlastType.UnitaryTorpedo);
            Destroy(gameObject);
        }
    }
}
