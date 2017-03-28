using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace PracticeProject
{
    public enum UnitClass { Scout, Recon, ECM, Figther, Bomber, Command, LR_Corvette, Guard_Corvette, Support_Corvette };
    public enum UnitStateType { MoveAI, AtackAI, UnderControl, Waiting};
    public enum TargetStateType { BehindABarrier,  InPramaryRange, InSecondaryRange, Captured}; 
    //public enum ImpactType { ForestStaticImpact };
    //public enum TerrainType { Plain, Forest };
    public enum Team { Green, Red, Blue };
    public enum WeaponType { Cannon, Laser, Plazma, Missile, Torpedo }
    public enum BlastType { Plazma, Missile, NukeTorpedo, SmallShip, MediumShip, Corvette }
    public class GlobalController : MonoBehaviour
    {
        public List<GameObject> unitList; // список 
        public List<GameObject> selectedList; // спиков выделенных объектов
        public Team playerArmy;
    }
    public abstract class Unit : MonoBehaviour
    {
        //base varibles
        protected UnitClass type;
        public UnitClass Type { get { return type; } }
        public UnitStateType aiStatus;
        public TargetStateType targetStus;
        public Team alliesArmy;
        public bool isSelected;
        public string UnitName;

        //selection
        public float NameFrameOffset;
        public Texture Selection;
        public float SelectionFrameWidth;
        public float SelectionFrameHeight;
        public float SelectionFrameOffset;

        //depend varibles
        public float Health;
        public GameObject DieBlast;
        public float Accurancy;
        public float Radiolink = 1.5f;

        //constants
        protected float maxHealth; //set in child
        protected float radarRange; //set in child
        protected float speed; //set in child
        public float MaxHealth { get { return maxHealth; } }
        public float RadarRange { get { return radarRange; } }
        public float Speed { get { return speed; } }
        public Team Army { get { return alliesArmy; } }
        //modules
        protected bool battleAIEnabled; //set in child
        protected bool selfDefenceAIEnabled; //set in child
        protected bool roleModuleEnabled;//set in child
        protected float stealthness; //set in child
        public float Stealthness { get { return stealthness; } }
        public float inhibition;
        protected bool detected;
        protected float cooldownDetected;
        //controllers
        protected MovementController Driver;
        protected ShootController Gunner;
        protected GlobalController Global;
        protected float synchAction;
        public float waitingBackCount; //Make private after debug;
        public List<GameObject> enemys;
        public GameObject CurrentTarget;

        protected void Start()
        {
            StatsUp();
            Health = MaxHealth;
            cooldownDetected = 0;
            UnitName = type.ToString();
            Driver = new MovementController(this.gameObject);
            Gunner = new ShootController(this.gameObject);
            Global = FindObjectsOfType<GlobalController>()[0];
            Global.unitList.Add(gameObject);
        }
        protected abstract void StatsUp();
        protected void Update()
        {
            if (Health < 0)
                Die();
            Driver.Update();
            Gunner.Update();
            //cooldowns
            DecrementBaseCounters();
            DecrementCounters();
            //action
            if (synchAction <= 0)
            {
                synchAction = 0.05f;
                if (waitingBackCount <= 0)
                {
                    aiStatus = UnitStateType.MoveAI;
                    ManeuverFunction();
                }
                else if (!BattleFunction())
                    aiStatus = UnitStateType.Waiting;
                if (roleModuleEnabled)
                    RoleFunction();
                if (selfDefenceAIEnabled)
                    SelfDefenceFunction();
            }
        }
        protected void DecrementBaseCounters()
        {
            synchAction -= Time.deltaTime;
            if (waitingBackCount>0)
            waitingBackCount -= Time.deltaTime;
            //else
            //{
            //    aiStatus = UnitStateType.MoveAI;
            //    waitingBackCount = 3;
            //    ManeuverFunction();
            //}
            if (this.alliesArmy != Global.playerArmy)
                cooldownDetected -= Time.deltaTime;
            if (cooldownDetected < 0)
                this.gameObject.transform.FindChild("MinimapPict").GetComponent<Renderer>().enabled = false;
            if (inhibition > 0)
                inhibition -= Time.deltaTime;
        }
        protected abstract void DecrementCounters();
        protected void OnGUI()
        {
            if (isSelected)
            {
                Vector3 crd = Camera.main.WorldToScreenPoint(transform.position);
                crd.y = Screen.height - crd.y;

                GUIStyle style = new GUIStyle();
                style.fontSize = 12;
                //style.font = GuiProcessor.getI.rusfont;
                style.normal.textColor = Color.cyan;
                style.alignment = TextAnchor.MiddleCenter;
                //style.fontStyle = FontStyle.Italic;

                GUI.DrawTexture(new Rect(crd.x - SelectionFrameWidth / 2, crd.y - SelectionFrameOffset, SelectionFrameWidth, SelectionFrameHeight), Selection);
                GUI.Label(new Rect(crd.x - 120, crd.y - NameFrameOffset, 240, 18), UnitName, style);
            }
        }

        protected void OnCollisionEnter(Collision collision)
        {
            switch (collision.gameObject.tag)
            {
                case "Shell":
                    {
                        this.Health = this.Health - collision.gameObject.GetComponent<Shell>().Damage;
                        break;
                    }

            }
        }
        protected void OnTriggerStay(Collider other)
        {
            switch (other.gameObject.tag)
            {
                case "Explosion":
                    {
                        float multiplicator = Mathf.Pow(((-Vector3.Distance(this.gameObject.transform.position, other.gameObject.transform.position) + other.gameObject.GetComponent<Explosion>().MaxRadius) * 0.01f), (1 / 3));
                        this.Health = this.Health - other.gameObject.GetComponent<Explosion>().Damage * multiplicator;
                        break;
                    }
            }
        }
        public void SelectUnit(bool isSelect)
        {
            if (isSelect)
            {
                gameObject.GetComponentInChildren<Camera>().enabled = true;
                Global.selectedList.Add(gameObject);
                isSelected = true;
            }
            else
            {
                gameObject.GetComponentInChildren<Camera>().enabled = false;
                isSelected = false;
            }
        }
        public void Die()
        {
            Instantiate(DieBlast, gameObject.transform.position, gameObject.transform.rotation);
            Global.selectedList.Remove(this.gameObject);
            Global.unitList.Remove(this.gameObject);
            Destroy(this.gameObject);
        }
        //AI logick
        protected bool BattleFunction()
        {
            if (CurrentTarget == null)//текущей цели нет
            {
                enemys = Scan();//поиск в зоне действия радара
                if (enemys.Count > 0)
                {
                    bool output = OpenFire(GetNearest());//огонь по ближайшей
                    CooperateFire();//целеуказание союзникам
                    return output;
                }
                else
                {
                    enemys = RequestScout();//запрос разведданных
                    if (enemys.Count > 0)
                        return OpenFire(GetNearest());
                    else//переходим в ожидение
                        return false;
                }
            }
            else
            {
                float distance = Vector3.Distance(this.transform.position, CurrentTarget.transform.position);
                if (distance < RadarRange)
                {
                    bool output = OpenFire(distance);
                    CooperateFire();
                    return output;
                }
                else
                {
                    if (!TargetScouting())
                    {
                        CurrentTarget = null;
                        Gunner.ResetAim();
                        return false;
                    }
                    else return OpenFire(distance);
                }
            }
        }
        protected virtual bool ManeuverFunction()
        {

            if (CurrentTarget == null)
            {
                return false;
            }
            else
            {
                switch (targetStus)
                {
                    case TargetStateType.Captured:
                        {
                            Debug.Log("AiManeur");
                            Debug.Log((CurrentTarget.transform.position - this.transform.position) * 0.1f);
                            Driver.MoveTo((CurrentTarget.transform.position - this.transform.position) * 0.1f);
                            return true;
                        }
                    case TargetStateType.InPramaryRange:
                        {
                            return true;
                        }
                    case TargetStateType.InSecondaryRange:
                        {
                            Debug.Log("AiManeur");
                            //Debug.Log((CurrentTarget.transform.position - this.transform.position) * 0.1f);
                            //Driver.MoveTo((CurrentTarget.transform.position - this.transform.position) * 0.1f);
                            //Driver.MoveTo(gameObject.transform.position + transform.forward * 5);
                            gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * Speed, ForceMode.Force);
                            return true;
                        }
                    case TargetStateType.BehindABarrier:
                        {
                            return false;
                        }
                    default:
                        return false;
                }
            }
        }
        protected abstract bool RoleFunction();
        protected abstract bool SelfDefenceFunction();

        protected bool OpenFire(float distance)
        {
            this.aiStatus = UnitStateType.AtackAI;
            RaycastHit hit;
            Physics.Raycast(this.transform.position, CurrentTarget.transform.position - this.transform.position, out hit);
            if (hit.transform==CurrentTarget.transform)
            {
                targetStus = TargetStateType.Captured;//наведение
                Gunner.SetAim(CurrentTarget);
                if (inhibition <= 0)//если не действует подавление оружия
                {
                    if (distance < Gunner.GetRangePrimary())//выбор оружия
                    {
                        targetStus = TargetStateType.InPramaryRange;
                        return Gunner.ShootHimPrimary(CurrentTarget);
                    }
                    else if (distance < Gunner.GetRangeSecondary())
                    {
                        targetStus = TargetStateType.InSecondaryRange;
                        return Gunner.ShootHimSecondary(CurrentTarget);
                    }
                }
            }
            return false;
        }
        protected List<GameObject> Scan()
        {
            List<GameObject> enemys = new List<GameObject>();
            foreach (GameObject x in Global.unitList)
            {
                float distance = Vector3.Distance(this.gameObject.transform.position, x.transform.position);
                if (distance < RadarRange)
                {
                    if (!x.GetComponent<Unit>().Allies(alliesArmy))
                    {
                        double multiplicator = Math.Pow(((-distance + RadarRange) * 0.02), (1 / 5)) * ((2 / (distance + 0.1)) + 1);
                        if (Randomizer.Uniform(0, 100, 1)[0] < x.GetComponent<Unit>().Stealthness * multiplicator * 100)
                            enemys.Add(x);
                    }
                }
            }
            return enemys;
        }
        public virtual bool Allies(Team army)
        {
            if (army == Global.playerArmy)
            {
                cooldownDetected = 1;
                this.gameObject.transform.FindChild("MinimapPict").GetComponent<Renderer>().enabled = true;
            }
            return (alliesArmy == army);
        }
        protected List<GameObject> RequestScout()
        {
            List<GameObject> enemys = new List<GameObject>();
            foreach (GameObject x in Global.unitList)
            {
                if (x.GetComponent<Unit>().alliesArmy == alliesArmy)
                {
                    float distance = Vector3.Distance(this.gameObject.transform.position, x.transform.position);
                    if (distance < RadarRange * Radiolink)
                    {
                        enemys.AddRange(x.GetComponent<Unit>().GetScout());
                    }
                }
            }
            return enemys;
        }
        public List<GameObject> GetScout()
        {
            return Scan();
        }
        protected bool TargetScouting()
        {
            List<GameObject> scoutingenemys = RequestScout();
            return scoutingenemys.Exists(x => CurrentTarget);
        }
        protected float GetNearest()
        {
            float minDistance = RadarRange;
            int minIndex = 0;
            float distance;
            for (int i = 0; i < enemys.Count; i++)
            {
                distance = Vector3.Distance(this.transform.position, enemys[i].transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    minIndex = i;
                }
            }
            CurrentTarget = enemys[minIndex];
            return minDistance;
        }
        protected void CooperateFire()
        {
            foreach (GameObject x in Global.unitList)
            {
                if (x.GetComponent<Unit>().alliesArmy == alliesArmy)
                {
                    float distance = Vector3.Distance(this.gameObject.transform.position, x.transform.position);
                    if (distance < RadarRange * Radiolink)
                    {
                        x.GetComponent<Unit>().GetFireSupport(CurrentTarget);
                    }
                }
            }
        }
        public void GetFireSupport(GameObject Target)
        {
            CurrentTarget = Target;
        }
        public virtual void SendTo(Vector3 destination)
        {
            waitingBackCount = 3;
            aiStatus = UnitStateType.UnderControl;
            Driver.MoveTo(destination);
        }
        public virtual void SendToQueue(Vector3 destination)
        {
            waitingBackCount = 3;
            aiStatus = UnitStateType.UnderControl;
            Driver.MoveToQueue(destination);
        }
    }
    public abstract class Weapon : MonoBehaviour
    {
        protected float range;
        public int ammo;
        protected float coolingTime;
        public float cooldown;
        public float dispersion;
        //public WeaponType Type;
        public GameObject ShellPrefab;
        public AudioClip ShootSound;

        public float Range { get { return range; } }
        public int Ammo { get { return ammo; } }
        public float Cooldown { get { return cooldown; } }
        public float CoolingTime { get { return coolingTime; } }

        protected virtual void Start(){  }

        // Update is called once per frame
        public void Update()
        {
            if (cooldown > 0)
                cooldown -= Time.deltaTime;
        }

        public bool Fire(Transform target)
        {
            if ((ammo > 0) && (cooldown <= 0))
            {
                //Debug.Log("Fire");
                Shoot(target);
                cooldown = coolingTime;
                ammo--;
            }
            return false;
        }
        protected virtual void Shoot(Transform target) { }
    }
    public abstract class Shell : MonoBehaviour
    {
        public float speed;
        public float damage;
        public float ttl;
        public float Speed { get { return speed; } }
        public float Damage { get { return damage; } }
        public GameObject Blast;
        // Use this for initialization
        protected abstract void Start();

        // Update is called once per frame
        public void Update()
        {
            if (ttl > 0)
                ttl -= Time.deltaTime;
            else
                Explode();
        }
        protected void OnCollisionEnter()
        {
            Explode();
        }
        protected void Explode()
        {
            if (Blast != null)
                Instantiate(Blast, this.transform.position, this.transform.rotation);
            Destroy(this.gameObject);
        }
    }
    public abstract class Torpedo : MonoBehaviour
    {
        public GameObject Blast;// префаб взрыва
        public Vector3 target;
        public Team allies;//свой-чужой
        public float Speed;// скорость ракеты      
        public float TurnSpeed;// скорость поворота ракеты            
        public float DropImpulse;//импульс сброса                  
        public float explosionRange; //расстояние детонации
        protected float lt;//продолжительность жизни
        public void Start()
        {
            gameObject.GetComponent<Rigidbody>().AddForce(-transform.up * DropImpulse, ForceMode.Impulse);
            lt = 0;
        }
        public virtual void Update()
        {
            if (target != null)//контроль курса
            {
                Quaternion targetRotation = Quaternion.LookRotation(target - this.transform.position, new Vector3(0, 1, 0));
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, Time.deltaTime * TurnSpeed);
            }
            //полет по прямой
            float multiplicator = Mathf.Pow((lt * 0.5f), (1 / 8)) * 0.7f;
            gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * Speed * multiplicator, ForceMode.Force);
            if (Vector3.Distance(this.transform.position, target) < explosionRange)
                Explode();
            else
                lt += Time.deltaTime;
        }
        public virtual void Explode()
        {
            Instantiate(Blast, this.transform.position, this.transform.rotation);
            Destroy(gameObject);
        }

        // взрываем при коллизии
        public virtual void OnCollisionEnter()
        {
            if (lt > 1)
                Explode();
        }
        public virtual void SetTarget(Vector3 target)
        {
            this.target = target;
        }
    }
    public static class TacticControler
    {
        public static double GetAngel(Vector3 A, Vector3 B)
        {
            return Math.Acos((A.x * B.x + A.y * B.y + A.z * B.z) / ((Math.Sqrt(A.x * A.x + A.y * A.y + A.z * A.z) * Math.Sqrt(B.x * B.x + B.y * B.y + B.z * B.z))));
        }
        //internal static double Distance(GameObject unitX, GameObject unitY)
        //{
        //    return Math.Sqrt(
        //        Math.Pow((unitX.transform.position.x - unitY.transform.position.x), 2) +
        //        Math.Pow((unitX.transform.position.y - unitY.transform.position.y), 2) +
        //        Math.Pow((unitX.transform.position.z - unitY.transform.position.z), 2)
        //        );
        //}
        //internal static double Distance(Vector3 A, Vector3 B)
        //{
        //    return Math.Sqrt(
        //        Math.Pow((A.x - B.x), 2) +
        //        Math.Pow((A.y - B.y), 2) +
        //        Math.Pow((A.z - B.z), 2)
        //        );
        //}
    }
    public class MovementController
    {
        private GameObject walker;
        //private Vector3 moveDestination;
        private Queue<Vector3> path;
        public float backCount;
        public MovementController(GameObject walker)
        {
            this.walker = walker;
            UpdateSpeed();
            path = new Queue<Vector3>();
            path.Enqueue(walker.transform.position);
            //Debug.Log("Driver online");
        }
        public void Update()
        {
            if (backCount <= 0)
            {
                UpdateSpeed();
                if (path.Count == 1)
                    walker.GetComponent<NavMeshAgent>().SetDestination(path.Peek());
                else
                {
                    if (Vector3.Distance(walker.transform.position, path.Peek())>10)
                        walker.GetComponent<NavMeshAgent>().SetDestination(path.Peek());
                    else
                        walker.GetComponent<NavMeshAgent>().SetDestination(path.Dequeue());
                }
                backCount = 0.2f;
            }
            else backCount -= Time.deltaTime;
        }
        public void UpdateSpeed()
        {
            walker.GetComponent<NavMeshAgent>().speed = walker.GetComponent<Unit>().Speed;
            walker.GetComponent<NavMeshAgent>().acceleration = walker.GetComponent<Unit>().Speed * 1.6f;
            walker.GetComponent<NavMeshAgent>().angularSpeed = walker.GetComponent<Unit>().Speed * 3.3f;
        }
        public bool MoveTo(Vector3 destination)
        {
            path.Clear();
            return MoveToQueue(destination);
        }
        public bool MoveToQueue(Vector3 destination)
        {
            path.Enqueue(destination);
            backCount = 0.2f;
            return true;
        }
        
        //public bool MoveTo(Vector3 destination)
        //{
        //    State = MovementState.Stering;
        //    destination.y = 0;
        //    dirAngel = GetAngel(walker.transform.ro)
        //    backCount = 1;
        //    walker.GetComponent<Unit>().state = UnitStateType.Move;

        //    return false;
        //}
        //public void Update()
        //{
        //    switch (State)
        //    {
        //        case MovementState.Acceleration:
        //            {
        //                if (backCount > 0)
        //                {
        //                    //Debug.Log("Breaking");
        //                    //walker.GetComponent<Unit>().state = UnitStateType.Waiting;
        //                    //walker.GetComponent<Rigidbody>().AddForce(-moveDirection, ForceMode.VelocityChange);
        //                    backCount -= Time.deltaTime;
        //                }
        //                else
        //                {
        //                    walker.GetComponent<Rigidbody>().AddForce(-Vector3.forward * walker.GetComponent<Unit>().speed, ForceMode.Acceleration);
        //                    State = MovementState.Breaking;
        //                    backCount = 100;
        //                }
        //                break;
        //            }
        //        case MovementState.Breaking:
        //            {
        //                if (backCount > 0)
        //                {
        //                    backCount -= Time.deltaTime;
        //                }
        //                else
        //                {
        //                    State = MovementState.Rest;
        //                }
        //                break;
        //            }
        //        case MovementState.Stering:
        //            {
        //                if (backCount > 0)
        //                {
        //                    var target = walker.transform.position - rotDirection;
        //                    target.y = 0;
        //                    walker.transform.rotation = Quaternion.LookRotation(target, Vector3.up);
        //                    backCount -= Time.deltaTime;
        //                }
        //                else
        //                {
        //                    walker.GetComponent<Rigidbody>().AddForce(Vector3.forward * walker.GetComponent<Unit>().speed, ForceMode.Acceleration);
        //                    State = MovementState.Acceleration;
        //                    backCount = 100;
        //                }
        //                break;
        //            }
        //        case MovementState.Rest:
        //            {
        //                if (backCount > 0)
        //                {
        //                    backCount -= Time.deltaTime;
        //                }
        //                else
        //                {

        //                }
        //                break;
        //            }
        //    }
        //}
        //private double GetAngel(Vector3 A, Vector3 B)
        //{
        //    return Math.Acos((A.x * B.x + A.y * B.y + A.z * B.z) / ((Math.Sqrt(A.x * A.x + A.y * A.y + A.z * A.z) * Math.Sqrt(B.x * B.x + B.y * B.y + B.z * B.z))));
        //}
    }
    public class ShootController
    {
        private Weapon[] primary;
        private Weapon[] secondary;
        public GameObject body;
        private GameObject aimTarget;
        //private float backCount;
        private float synchPrimary;
        private int indexPrimary;
        private float synchSecondary;
        private int indexSecondary;
        public ShootController(GameObject body)
        {
            this.body = body;
            this.primary = body.transform.FindChild("Primary").GetComponentsInChildren<Weapon>();
            synchPrimary = this.primary[0].CoolingTime / this.primary.Length;

            indexPrimary = 0;
            this.secondary = body.transform.FindChild("Secondary").GetComponentsInChildren<Weapon>();
            synchSecondary = this.secondary[0].CoolingTime / this.secondary.Length;

            indexSecondary = 0;
            //Debug.Log("Gunner online");
        }
        public bool ShootHimPrimary(GameObject target)
        {
            if (Vector3.Angle(target.transform.position - body.transform.position, body.transform.forward) < primary[0].dispersion * 5)
            {
                if (indexPrimary >= primary.Length)
                    indexPrimary = 0;
                if (primary[indexPrimary].Cooldown <= 0)
                {
                    bool output = primary[indexPrimary].Fire(target.transform);
                    indexPrimary++;
                    return output;
                }
                else indexPrimary++;
            }
            return false;
        }
        public bool ShootHimSecondary(GameObject target)
        {
            if (Vector3.Angle(target.transform.position - body.transform.position, body.transform.forward) < secondary[0].dispersion * 5)
            {
                if (indexSecondary >= secondary.Length)
                    indexSecondary = 0;
                if (secondary[indexSecondary].Cooldown <= 0)
                {
                    bool output = secondary[indexSecondary].Fire(target.transform);
                    indexSecondary++;
                    return output;
                }
                else indexSecondary++;
            }
            return false;
        }
        public void Update()
        {
            if (synchPrimary > 0)
                synchPrimary -= Time.deltaTime;
            else
                synchPrimary = this.primary[0].CoolingTime / this.primary.Length;
            if (synchSecondary > 0)
                synchSecondary -= Time.deltaTime;
            else
                synchSecondary = this.secondary[0].CoolingTime / this.secondary.Length;
            if (aimTarget != null)
            {
                var targetvelocity = aimTarget.GetComponent<Rigidbody>().velocity;
                var Distance = Vector3.Distance(body.transform.position, aimTarget.transform.position);
                var targetPoint = new Vector3(aimTarget.transform.position.x + targetvelocity.x * Mathf.Sqrt(Mathf.Pow(Distance, 2) / (Mathf.Pow(7500, 2) - Mathf.Pow(targetvelocity.x, 2))), aimTarget.transform.position.y + targetvelocity.y * Mathf.Sqrt(Mathf.Pow(Distance, 2) / (Mathf.Pow(7500, 2) - Mathf.Pow(targetvelocity.y, 2))), aimTarget.transform.position.z + targetvelocity.z * Mathf.Sqrt(Mathf.Pow(Distance, 2) / (Mathf.Pow(7500, 2) - Mathf.Pow(targetvelocity.z, 2))));//Поправку берём тут
                Quaternion targetRotation = Quaternion.LookRotation(targetPoint - body.transform.position, new Vector3(0, 1, 0));
                body.transform.rotation = Quaternion.Slerp(body.transform.rotation, targetRotation, Time.deltaTime * body.GetComponent<Unit>().Speed * 0.2f);
                //var forward = walker.transform.TransformDirection(Vector3.forward);
                //var targetDir = aimTarget - walker.transform.position;
                //if (Vector3.Angle(forward, targetDir) < shootAngleDistance)
            }
        }
        public bool SetAim(GameObject target)
        {
            if (aimTarget == null)
            {
                aimTarget = target;
                return true;
            }
            else return false;
        }
        public bool ResetAim()
        {
            aimTarget = null;
            return true;
        }
        public float GetRangePrimary()
        {
            return primary[0].Range;
        }
        public float GetRangeSecondary()
        {
            return secondary[0].Range;
        }
    }
    //public static class ObjectCreator
    //{
    //    public static Unit RestoreUnit(UnitType type, UnityEngine.Object GraficUnitRef)
    //    {
    //        string StatPath = type.ToString("F") + ".stat.dat";
    //        string[] stats = System.IO.File.ReadAllLines(StatPath);
    //        Unit newUnit = new Unit(
    //            Convert.ToInt32(stats[1]),
    //            Convert.ToInt32(stats[2]),
    //            Convert.ToDouble(stats[3]),
    //            Convert.ToDouble(stats[4]),
    //            Convert.ToInt32(stats[5]),
    //            Convert.ToInt32(stats[6]),
    //            Convert.ToInt32(stats[7]),
    //            Convert.ToInt32(stats[8]));
    //        newUnit.type = type;
    //        return newUnit;
    //    }
    //}
    public static class Randomizer
    {
        public static double[] Uniform(int minValue, int maxValue, int Count)
        {
            double[] Out = new double[Count];
            for (int i = 0; i < Count; i++)
            {
                Out[i] = Uniform(minValue, maxValue);
            }
            return Out;
        }
        public static double[] Exponential(double Rate, int Count, int minValue, int maxValue)
        {
            double[] stairWidth = new double[257];
            double[] stairHeight = new double[256];
            const double x1 = 7.69711747013104972;
            const double A = 3.9496598225815571993e-3; /// area under rectangle

            setupExpTables(ref stairWidth, ref stairHeight, x1, A);

            double[] Out = new double[Count];
            for (int i = 0; i < Count; i++)
            {
                Out[i] = Exponential(Rate, stairWidth, stairHeight, x1, minValue, maxValue);
            }

            return Out;
        }
        public static double[] Normal(double Mu, double Sigma, int Count, int minValue, int maxValue)
        {
            double[] stairWidth = new double[257];
            double[] stairHeight = new double[256];
            const double x1 = 3.6541528853610088;
            const double A = 4.92867323399e-3; /// area under rectangle

            setupNormalTables(ref stairWidth, ref stairHeight, x1, A);

            double[] Out = new double[Count];
            for (int i = 0; i < Count; i++)
            {
                Out[i] = Mu + NormalZiggurat(stairWidth, stairHeight, x1, minValue, maxValue) * Sigma;
            }

            return Out;
        }

        private static double Uniform(double A, double B)
        {
            return A + RandomDouble() * (B - A);// maxValue;
        }
        private static double MagiсUniform(double A, double B)
        {
            return A + RandomMagiсInt() * (B - A) / 256;
        }

        private static void setupExpTables(ref double[] stairWidth, ref double[] stairHeight, double x1, double A)
        {
            // coordinates of the implicit rectangle in base layer
            stairHeight[0] = Math.Exp(-x1);
            stairWidth[0] = A / stairHeight[0];
            // implicit value for the top layer
            stairWidth[256] = 0;
            for (int i = 1; i <= 255; ++i)
            {
                // such x_i that f(x_i) = y_{i-1}
                stairWidth[i] = -Math.Log(stairHeight[i - 1]);
                stairHeight[i] = stairHeight[i - 1] + A / stairWidth[i];
            }
        }
        private static void setupNormalTables(ref double[] stairWidth, ref double[] stairHeight, double x1, double A)
        {
            // coordinates of the implicit rectangle in base layer
            stairHeight[0] = Math.Exp(-.5 * x1 * x1);
            stairWidth[0] = A / stairHeight[0];
            // implicit value for the top layer
            stairWidth[256] = 0;
            for (int i = 1; i <= 255; ++i)
            {
                // such x_i that f(x_i) = y_{i-1}
                stairWidth[i] = Math.Sqrt(-2 * Math.Log(stairHeight[i - 1]));
                stairHeight[i] = stairHeight[i - 1] + A / stairWidth[i];
            }
        }

        private static double ExpZiggurat(double[] stairWidth, double[] stairHeight, double x1, int minValue, int maxValue)
        {
            int iter = 0;
            do
            {
                int stairId = RandomInt() & 255;
                double x = Uniform(0, stairWidth[stairId]); // get horizontal coordinate
                if (x < stairWidth[stairId + 1]) /// if we are under the upper stair - accept
                    return x;
                if (stairId == 0) // if we catch the tail
                    return x1 + ExpZiggurat(stairWidth, stairHeight, x1, minValue, maxValue);
                if (Uniform(stairHeight[stairId - 1], stairHeight[stairId]) < Math.Exp(-x)) // if we are under the curve - accept
                    return x;
                // rejection - go back
            } while (++iter <= 1e9); // one billion should be enough to be sure there is a bug
            return double.NaN; // fail due to some error
        }
        private static double NormalZiggurat(double[] stairWidth, double[] stairHeight, double x1, int minValue, int maxValue)
        {
            int iter = 0;
            do
            {
                int B = RandomMagiсInt();
                int stairId = B & 255;
                double x = MagiсUniform(0, stairWidth[stairId]); // get horizontal coordinate
                if (x < stairWidth[stairId + 1])
                    return ((int)B > 0) ? x : -x;
                if (stairId == 0) // handle the base layer
                {
                    double z = -1;
                    double y;
                    if (z > 0) // we don't have to generate another exponential variable as we already have one
                    {
                        x = Exponential(x1, stairWidth, stairHeight, x1, minValue, maxValue);
                        z -= 0.5 * x * x;
                    }
                    if (z <= 0) // if previous generation wasn't successful
                    {
                        do
                        {
                            x = Exponential(x1, stairWidth, stairHeight, x1, minValue, maxValue);
                            y = Exponential(1, stairWidth, stairHeight, x1, minValue, maxValue);
                            z = y - 0.5 * x * x; // we storage this value as after acceptance it becomes exponentially distributed
                        } while (z <= 0);
                    }
                    x += x1;
                    return ((int)B > 0) ? x : -x;
                }
                // handle the wedges of other stairs
                if (MagiсUniform(stairHeight[stairId - 1], stairHeight[stairId]) < Math.Exp(-.5 * x * x))
                    return ((int)B > 0) ? x : -x;
            } while (++iter <= 1e9); /// one billion should be enough
            return double.NaN; /// fail due to some error
        }
        private static double Exponential(double Rate, double[] stairWidth, double[] stairHeight, double x1, int minValue, int maxValue)
        {
            return ExpZiggurat(stairWidth, stairHeight, x1, minValue, maxValue) / Rate;
        }
        private static double RandomDouble()
        {
            System.Security.Cryptography.RNGCryptoServiceProvider rand = new System.Security.Cryptography.RNGCryptoServiceProvider();
            const int BUF_LENGTH = 1;
            byte[] buf = new byte[BUF_LENGTH];
            rand.GetBytes(buf);
            double Out = 0;
            foreach (byte x in buf)
                Out += x / 256.0;
            return Out;
        }
        private static int RandomInt()
        {
            System.Security.Cryptography.RNGCryptoServiceProvider rand = new System.Security.Cryptography.RNGCryptoServiceProvider();
            const int BUF_LENGTH = 1;
            byte[] buf = new byte[BUF_LENGTH];
            rand.GetBytes(buf);
            int Out = 0;
            foreach (byte x in buf)
                Out += x;
            return Out / BUF_LENGTH;
        }
        private static int RandomMagiсInt()
        {
            System.Security.Cryptography.RNGCryptoServiceProvider rand = new System.Security.Cryptography.RNGCryptoServiceProvider();
            const int BUF_LENGTH = 256;
            byte[] buf = new byte[BUF_LENGTH];
            rand.GetBytes(buf);
            int Out = 0;
            foreach (byte x in buf)
                Out += x;
            return Out / BUF_LENGTH;
        }
    }
}
