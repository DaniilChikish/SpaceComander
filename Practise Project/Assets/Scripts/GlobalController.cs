using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace PracticeProject
{
    public enum UnitClass { Figther, Bomber, Corvette };
    public enum UnitStateType { Move, Fire, MoveAndFire, Waiting, SkippingStep };
    //public enum ImpactType { ForestStaticImpact };
    //public enum TerrainType { Plain, Forest };
    public enum Team { Green, Red, Blue };
    public enum WeaponType { Cannon, Laser, Plazma, Missile, Torpedo }
    public enum BlastType { Plazma, Missile, Torpedo, Ship }
    public class GlobalController : MonoBehaviour
    {
        public List<GameObject> unitList; // список 
        public List<GameObject> selectedList; // спиков выделенных объектов
        public Team playerArmy;
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
        private Vector3 moveDestination;

        public float backCount;
        public MovementController(GameObject walker)
        {
            this.walker = walker;
            moveDestination = walker.transform.position;
            walker.GetComponent<NavMeshAgent>().speed = walker.GetComponent<Unit>().Speed;
            walker.GetComponent<NavMeshAgent>().acceleration = walker.GetComponent<Unit>().Speed * 1.6f;
            walker.GetComponent<NavMeshAgent>().angularSpeed = walker.GetComponent<Unit>().Speed * 3.3f;
            //Debug.Log("Driver online");
        }
        public void Update()
        {
            //if (Vector3.Distance(walker.transform.position, moveDestination) < 3)
            //    {
                    //Debug.Log("Distanse " + TacticControler.Distance(walker.transform.position, moveDestination)+" Stop.");
                    //walker.GetComponent<NavMeshAgent>().Stop();
            //    }
            if (backCount == 0)
            {
                walker.GetComponent<NavMeshAgent>().SetDestination(moveDestination);
                backCount = 1;
            }
            else backCount -= Time.deltaTime;
        }
        public bool MoveTo(Vector3 destination)
        {
            moveDestination = destination;
            walker.GetComponent<NavMeshAgent>().SetDestination(moveDestination);
            backCount = 100;            
            return false;
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
            //if (synchPrimary <= 0)
            //{
                if (indexPrimary >= primary.Length)
                    indexPrimary = 0;
                if (primary[indexPrimary].Cooldown <= 0)
                {
                    primary[indexPrimary].Fire(target.transform);
                    indexPrimary++;
                    return true;
                }
                else indexPrimary++;
            //}
            //else             Debug.Log(synchPrimary);
            return false;
        }
        public bool ShootHimSecondary(GameObject target)
        {
            //if (synchSecondary <= 0)
            //{
                if (indexSecondary >= secondary.Length)
                    indexSecondary = 0;
                if (secondary[indexSecondary].Cooldown <= 0)
                {
                    secondary[indexSecondary].Fire(target.transform);
                    indexSecondary++;
                    return true;
                }
                else indexSecondary++;
            //}
            //else             Debug.Log(synchSecondary);
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
                body.transform.rotation = Quaternion.Slerp(
                body.transform.rotation, targetRotation, Time.deltaTime * body.GetComponent<Unit>().Speed * 0.2f);
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
                //rotateDestination = Quaternion.Lerp(walker.transform.rotation, Quaternion.LookRotation(target - walker.transform.position), walker.GetComponent<Unit>().Speed * 0.3f);
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
