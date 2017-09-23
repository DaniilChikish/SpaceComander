﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpaceCommander
{
    public enum DriverStatus { Movement, Waiting};
    class SpaceMovementController : IDriver
    {
        private DriverStatus status;
        public DriverStatus Status { get { return status; } }
        public bool tridimensional = true;
        public float gridStep = 5f;
        public float accurancy = 7f;
        public float checkRadius = 5f;

        private SpaceShip walker;
        private Transform walkerTransform;
        private Rigidbody walkerBody;

        private float mainThrust;
        private float verticalShiftThrust;
        private float horisontalShiftThrust;

        private Queue<Vector3> path; //очередь путевых точек
        public Vector3 Velocity { get { return walkerBody.velocity; } }
        public int PathPoints
        {
            get
            {
                return path.Count;
            }
        }
        public Vector3 NextPoint { get { if (PathPoints > 0) return path.Peek(); else return Vector3.zero; } }
        //public float backCount; //время обновления пути.
        public SpaceMovementController(GameObject walker)
        {
            this.walkerTransform = walker.transform;
            path = new Queue<Vector3>();
            this.walker = walker.GetComponent<SpaceShip>();
            walkerTransform = walker.GetComponent<Transform>();
            walkerBody = walker.GetComponent<Rigidbody>();

            Debug.Log("Driver online");
        }
        public void Update()
        {
            if (path.Count > 0)
            {
                if ((path.Peek() - walkerTransform.position).magnitude < accurancy)
                {
                    path.Dequeue();
                }
                MoveSimple();
                Rotate();
            }
            else status = DriverStatus.Waiting;
        }
        public bool MoveTo(Vector3 destination)
        {
            ClearQueue();
            return MoveToQueue(destination);
        }
        public bool MoveToQueue(Vector3 destination)
        {
            Debug.Log("MoveTo " + destination);
            status = DriverStatus.Movement;
            if (!CanWalk(walkerTransform.position, destination))
            {
                Debug.Log("Find barier, build path");
                float accuracyFactor = (1 + (Vector3.Distance(walkerTransform.position, destination) / (accurancy * 10)));
                Vector3[] pathPoints = BuildPath(destination, accuracyFactor);
                if (pathPoints != null)
                {
                    Debug.Log("Path created");
                    foreach (Vector3 x in pathPoints)
                    {
                        Debug.Log(x);
                        path.Enqueue(x);
                    }
                    return true;
                }
                else Debug.Log("Path not created");
            }
            else
            {
                Debug.Log("Barier not finded");
                path.Enqueue(destination);
                return true;
            }
            return false;
        }

        private Vector3[] BuildPath(Vector3 destination, float accuracyFactor)
        {
            // Шаг 1.
            List<PathNode> closedSet = new List<PathNode>();
            List<PathNode> openSet = new List<PathNode>();
            int steps = 0;
            Vector3 startPos;
            if (path.Count > 0)
                startPos = path.Last();
            else
                startPos = walkerTransform.position;
            // Шаг 2.
            PathNode startNode = new PathNode()
            {
                Position = startPos,
                CameFrom = null,
                PathLengthFromStart = 0,
                HeuristicEstimatePathLength = GetHeuristicPathLength(walkerTransform.position, destination)
            };
            openSet.Add(startNode);
            PathNode currentNode;
            while (openSet.Count > 0 && steps < 1000)
            {
                steps++;
                // Шаг 3.
                currentNode = openSet.OrderBy(node => node.EstimateFullPathLength).First();
                // Шаг 4.
                if (Vector3.Distance(currentNode.Position, destination) <= accurancy * accuracyFactor)
                    return GetPathForNode(currentNode).ToArray();
                // Шаг 5.
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);
                // Шаг 6.
                List<PathNode> neighbours = GetNeighbours(currentNode, destination, accuracyFactor);
                foreach (PathNode neighbourNode in neighbours)
                {
                    // Шаг 7.
                    if (closedSet.Count(node => node.Position == neighbourNode.Position) > 0)
                        continue;
                    PathNode openNode = openSet.Find(node => node.Position == neighbourNode.Position);
                    // Шаг 8.
                    if (openNode == null)
                        openSet.Add(neighbourNode);
                    else if (openNode.PathLengthFromStart > neighbourNode.PathLengthFromStart)
                    {
                        // Шаг 9.
                        openNode.CameFrom = currentNode;
                        openNode.PathLengthFromStart = neighbourNode.PathLengthFromStart;
                    }
                }
            }
            // Шаг 10.
            return null;
        }

        private float GetHeuristicPathLength(Vector3 from, Vector3 to)
        {
            return Mathf.Abs(from.x - to.x) + Math.Abs(from.y - to.y) + Math.Abs(from.z - to.z);
        }

        private List<PathNode> GetNeighbours(PathNode pathNode, Vector3 destination, float accuracyFactor)
        {
            var result = new List<PathNode>();
            float gridStepLocal = gridStep * accuracyFactor;

            // Соседними точками являются соседние по стороне клетки.
            Vector3[] neighbourPoints = new Vector3[26];
            neighbourPoints[0] = new Vector3(pathNode.Position.x + gridStepLocal, pathNode.Position.y - gridStepLocal, pathNode.Position.z - gridStepLocal);
            neighbourPoints[1] = new Vector3(pathNode.Position.x + gridStepLocal, pathNode.Position.y,            pathNode.Position.z - gridStepLocal);
            neighbourPoints[2] = new Vector3(pathNode.Position.x + gridStepLocal, pathNode.Position.y + gridStepLocal, pathNode.Position.z - gridStepLocal);

            neighbourPoints[3] = new Vector3(pathNode.Position.x + gridStepLocal, pathNode.Position.y - gridStepLocal, pathNode.Position.z);
            neighbourPoints[4] = new Vector3(pathNode.Position.x + gridStepLocal, pathNode.Position.y,            pathNode.Position.z);
            neighbourPoints[5] = new Vector3(pathNode.Position.x + gridStepLocal, pathNode.Position.y + gridStepLocal, pathNode.Position.z);

            neighbourPoints[6] = new Vector3(pathNode.Position.x + gridStepLocal, pathNode.Position.y - gridStepLocal, pathNode.Position.z + gridStepLocal);
            neighbourPoints[7] = new Vector3(pathNode.Position.x + gridStepLocal, pathNode.Position.y,            pathNode.Position.z + gridStepLocal);
            neighbourPoints[8] = new Vector3(pathNode.Position.x + gridStepLocal, pathNode.Position.y + gridStepLocal, pathNode.Position.z + gridStepLocal);

            neighbourPoints[9] = new Vector3(pathNode.Position.x, pathNode.Position.y - gridStepLocal, pathNode.Position.z - gridStepLocal);
            neighbourPoints[10] = new Vector3(pathNode.Position.x, pathNode.Position.y,           pathNode.Position.z - gridStepLocal);
            neighbourPoints[11] = new Vector3(pathNode.Position.x + gridStepLocal, pathNode.Position.y + gridStepLocal, pathNode.Position.z - gridStepLocal);

            neighbourPoints[12] = new Vector3(pathNode.Position.x, pathNode.Position.y - gridStepLocal, pathNode.Position.z);
            //this pathNode
            neighbourPoints[13] = new Vector3(pathNode.Position.x, pathNode.Position.y + gridStepLocal, pathNode.Position.z);

            neighbourPoints[14] = new Vector3(pathNode.Position.x, pathNode.Position.y - gridStepLocal, pathNode.Position.z + gridStepLocal);
            neighbourPoints[15] = new Vector3(pathNode.Position.x, pathNode.Position.y,            pathNode.Position.z + gridStepLocal);
            neighbourPoints[16] = new Vector3(pathNode.Position.x, pathNode.Position.y + gridStepLocal, pathNode.Position.z + gridStepLocal);

            neighbourPoints[17] = new Vector3(pathNode.Position.x - gridStepLocal, pathNode.Position.y - gridStepLocal, pathNode.Position.z - gridStepLocal);
            neighbourPoints[18] = new Vector3(pathNode.Position.x - gridStepLocal, pathNode.Position.y,            pathNode.Position.z - gridStepLocal);
            neighbourPoints[19] = new Vector3(pathNode.Position.x - gridStepLocal, pathNode.Position.y + gridStepLocal, pathNode.Position.z - gridStepLocal);

            neighbourPoints[20] = new Vector3(pathNode.Position.x - gridStepLocal, pathNode.Position.y - gridStepLocal, pathNode.Position.z);
            neighbourPoints[21] = new Vector3(pathNode.Position.x - gridStepLocal, pathNode.Position.y,            pathNode.Position.z);
            neighbourPoints[22] = new Vector3(pathNode.Position.x - gridStepLocal, pathNode.Position.y + gridStepLocal, pathNode.Position.z);

            neighbourPoints[23] = new Vector3(pathNode.Position.x - gridStepLocal, pathNode.Position.y - gridStepLocal, pathNode.Position.z + gridStepLocal);
            neighbourPoints[24] = new Vector3(pathNode.Position.x - gridStepLocal, pathNode.Position.y,            pathNode.Position.z + gridStepLocal);
            neighbourPoints[25] = new Vector3(pathNode.Position.x - gridStepLocal, pathNode.Position.y + gridStepLocal, pathNode.Position.z + gridStepLocal);

            //debug
            GameObject worldpoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            worldpoint.GetComponent<SphereCollider>().enabled = false;
            worldpoint.AddComponent<SelfDestructor>().ttl = 1f;

            foreach (Vector3 point in neighbourPoints)
            {
                //debug
                GameObject.Instantiate(worldpoint, point, walkerTransform.rotation);

                // Проверяем, что по клетке можно ходить.
                if (!CanWalk(pathNode.Position, point))
                    continue;
                // Заполняем данные для точки маршрута.
                PathNode neighbourNode = new PathNode()
                {
                    Position = point,
                    CameFrom = pathNode,
                    PathLengthFromStart = pathNode.PathLengthFromStart + Vector3.Distance(pathNode.Position, point),
                    HeuristicEstimatePathLength = GetHeuristicPathLength(point, destination)
                };
                result.Add(neighbourNode);
            }
            return result;
        }

        private bool CanWalk(Vector3 position, Vector3 destination)
        { 
            RaycastHit[] hits = Physics.RaycastAll(position, (destination - position), (destination - position).magnitude); //9 is Terrain layer
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.tag == "Terrain")
                    return false;
            }
            return true;
        }

        private bool FreeSpace(Vector3 point)
        {
            return (Physics.OverlapSphere(point, checkRadius).Length == 0);
        }

        private List<Vector3> GetPathForNode(PathNode pathNode)
        {
            List<Vector3> result = new List<Vector3>();
            PathNode currentNode = pathNode;
            //debug
            GameObject worldpoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            worldpoint.GetComponent<SphereCollider>().enabled = false;
            worldpoint.AddComponent<SelfDestructor>().ttl = 15f;

            while (currentNode != null)
            {
                //debug
                GameObject.Instantiate(worldpoint, currentNode.Position, walkerTransform.rotation);

                result.Add(currentNode.Position);
                currentNode = currentNode.CameFrom;
            }
            result.Reverse();
            return result;
        }

        public void ClearQueue()
        {
            //backCount = 0;
            path.Clear();
        }
        private void MoveSimple()
        {
            if (path.Peek() != null)
                walkerTransform.Translate((path.Peek() - walkerTransform.position).normalized * walker.Speed * Time.deltaTime, Space.World);
        }
        private void Move()
        {
            float mainThrustLocal = ThrustAxis();
            if (mainThrust <= 2.5 && mainThrust >= -1)
                mainThrust += mainThrustLocal * Time.deltaTime;

            if (mainThrustLocal == 0)
            {
                if (mainThrust <= 0.3 && mainThrust >= -0.3)
                    mainThrust = mainThrust * 0.7f;
                else if (mainThrust > 0.3f)
                    mainThrust -= Time.deltaTime * 0.3f;
                else if (mainThrust < -0.3f)
                    mainThrust += Time.deltaTime * 0.5f;
            }
            if (mainThrust > 2.5) mainThrust = 2.5f;
            if (mainThrust < 0.0001 && mainThrust > -0.0001) mainThrust = 0;
            if (mainThrust < -1) mainThrust = -1;

            float horisontalShiftLocal = HorizontalShiftAxis();
            if (horisontalShiftThrust <= 1 && horisontalShiftThrust >= -1)
                horisontalShiftThrust += horisontalShiftLocal * Time.deltaTime;

            if (horisontalShiftLocal == 0)
            {
                if (horisontalShiftThrust <= 0.1 && horisontalShiftThrust >= -0.1)
                    horisontalShiftThrust = horisontalShiftThrust * 0.7f;
                else if (horisontalShiftThrust > 0.1f)
                    horisontalShiftThrust -= Time.deltaTime * 0.7f;
                else if (horisontalShiftThrust < -0.1f)
                    horisontalShiftThrust += Time.deltaTime * 0.7f;
            }
            if (horisontalShiftThrust > 1) horisontalShiftThrust = 1;
            if (horisontalShiftThrust < 0.0001 && horisontalShiftThrust > -0.0001) horisontalShiftThrust = 0;
            if (horisontalShiftThrust < -1) horisontalShiftThrust = -1;

            if (tridimensional)
            {
                float vertikalShiftLocal = VerticalShiftAxis();
                if (verticalShiftThrust <= 1 && verticalShiftThrust >= -1)
                    verticalShiftThrust += vertikalShiftLocal * Time.deltaTime;

                if (vertikalShiftLocal == 0)
                {
                    if (verticalShiftThrust <= 0.1 && verticalShiftThrust >= -0.1)
                        verticalShiftThrust = verticalShiftThrust * 0.7f;
                    else if (verticalShiftThrust > 0.1f)
                        verticalShiftThrust -= Time.deltaTime * 0.7f;
                    else if (verticalShiftThrust < -0.1f)
                        verticalShiftThrust += Time.deltaTime * 0.7f;
                }
                if (verticalShiftThrust > 1) verticalShiftThrust = 1;
                if (verticalShiftThrust < 0.0001 && verticalShiftThrust > -0.0001) verticalShiftThrust = 0;
                if (verticalShiftThrust < -1) verticalShiftThrust = -1;
            }
            Vector3 shiftLocal = (walkerTransform.right * horisontalShiftThrust * walker.ShiftSpeed + walkerTransform.up * verticalShiftThrust * walker.ShiftSpeed + walkerTransform.forward * mainThrust * walker.Speed) * 25;
            walkerBody.AddForce(shiftLocal, ForceMode.Acceleration);
        }

        private float ThrustAxis()
        {
            Vector3 targetMotion = walkerTransform.position - path.Peek();
            float pojection = Vector3.Project(targetMotion, walkerTransform.forward).magnitude;

            throw new NotImplementedException();
        }
        private float HorizontalShiftAxis()
        {
            throw new NotImplementedException();
        }
        private float VerticalShiftAxis()
        {
            throw new NotImplementedException();
        }

        private void Rotate()
        {
            if (walker.Gunner.Target == null)
            {
                Vector3 targetDirection = path.Peek() - walker.transform.position;
                if (targetDirection != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(targetDirection, new Vector3(0, 1, 0));
                    walker.transform.rotation = Quaternion.RotateTowards(walker.transform.rotation, targetRotation, Time.deltaTime * walker.RotationSpeed * 5.5f);
                }
            }
        }
    }
    public class PathNode
    {
        // Координаты точки на карте.
        public Vector3 Position { get; set; }
        // Длина пути от старта (G).
        public float PathLengthFromStart { get; set; }
        // Точка, из которой пришли в эту точку.
        public PathNode CameFrom { get; set; }
        // Примерное расстояние до цели (H).
        public float HeuristicEstimatePathLength { get; set; }
        // Ожидаемое полное расстояние до цели (F).
        public float EstimateFullPathLength
        {
            get
            {
                return this.PathLengthFromStart + this.HeuristicEstimatePathLength;
            }
        }
        public override string ToString()
        {
            return Position.ToString();
        }
    }
}
