﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpaceCommander
{
    public enum DriverStatus { Navigating, Maneuvering, Waiting};
    class SpaceMovementController : IDriver
    {
        private DriverStatus status;
        private bool calculating;
        public DriverStatus Status { get { return status; } }
        public bool tridimensional = true;
        public float gridStep = 5f;
        public float accurancy = 10f;

        private SpaceShip walker;
        private Transform walkerTransform;
        private Rigidbody walkerBody;
        private GlobalController Global;

        private float mainThrust;
        private float verticalShiftThrust;
        private float horisontalShiftThrust;

        private Queue<Vector3> path; //очередь путевых точек
        List<PathNode> closedSet = new List<PathNode>();
        List<PathNode> openSet = new List<PathNode>();
        private Vector3 navDestination;
        private float navAccurancyFactor;
        private int calculatingStep;
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
            Global = GameObject.FindObjectOfType<GlobalController>();
            Debug.Log("Driver online");
        }
        public void Update()
        {
        }
        public void FixedUpdate()
        {
            if (calculating && openSet.Count > 0 && calculatingStep < 240)
                PathfindingStep(navDestination, navAccurancyFactor);
            else calculating = false;
            switch (status)
            {
                case DriverStatus.Navigating:
                    {
                        if (path.Count > 0)
                        {
                            //MoveSimple();
                            Move();
                            Rotate();
                            if ((path.Peek() - walkerTransform.position).magnitude < accurancy)
                            {
                                path.Dequeue();
                            }
                        }
                        else
                        {
                            status = DriverStatus.Waiting;

                        }
                        break;
                    }
                case DriverStatus.Maneuvering:
                    {
                        Move();
                        break;
                    }
                default:
                    {
                        Move();
                        if (!walker.ManualControl && walker.Gunner.Target == null)
                            Stabilisation();
                        break;
                    }
            }
        }
        public bool MoveTo(Vector3 destination)
        {
            ClearQueue();
            return MoveToQueue(destination);
        }
        public bool MoveToQueue(Vector3 destination)
        {
            //Debug.Log("MoveTo " + destination);
            status = DriverStatus.Navigating;
            Vector3 last;
            if (path.Count > 0)
                last = path.Last();
            else
                last = walkerTransform.position;
            if (!CanWalk(last, destination))
            {
                PathfindingBegin(destination);
                return true;
            }
            else
            {
                //Debug.Log("Barier not finded");
                path.Enqueue(destination);
                return true;
            }
        }
        public void BuildPathArrows()
        {
            if (walker.isSelected && path.Count > 0)
            {
                Vector3[] pathLocal = path.ToArray();
                GameObject arrow;
                float dist;

                arrow = GameObject.Instantiate(Global.pathArrow);
                dist = Vector3.Distance(walkerTransform.position, pathLocal[0]);
                arrow.transform.localScale = new Vector3(1, 1, dist);
                arrow.transform.position = walkerTransform.position + (pathLocal[0] - walkerTransform.position).normalized * dist / 2;
                arrow.transform.rotation = Quaternion.LookRotation((pathLocal[0] - walkerTransform.position), new Vector3(0, 1, 0));
                arrow.AddComponent<Service.SelfDestructor>().ttl = 2f;

                for (int i = 0; i + 1 < pathLocal.Length; i++)
                {
                    arrow = GameObject.Instantiate(Global.pathArrow);
                    dist = Vector3.Distance(pathLocal[i], pathLocal[i + 1]);
                    arrow.transform.localScale = new Vector3(1, 1, dist);
                    arrow.transform.position = pathLocal[i] + (pathLocal[i + 1] - pathLocal[i]).normalized * dist / 2;
                    arrow.transform.forward = (pathLocal[i + 1] - pathLocal[i]);
                    arrow.AddComponent<Service.SelfDestructor>().ttl = 2f;
                }
            }
        }
        private void PathfindingBegin(Vector3 destination)
        {
            navAccurancyFactor = (1 + (Vector3.Distance(walkerTransform.position, destination) / (accurancy * 5)));
            navDestination = destination;
            calculatingStep = 0;
            calculating = true;
            closedSet = new List<PathNode>();
            openSet = new List<PathNode>();
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
        }
        private void PathfindingStep(Vector3 destination, float accuracyFactor)
        {
            calculatingStep++;
            PathNode currentNode;
            // Шаг 3.
            currentNode = openSet.OrderBy(node => node.EstimateFullPathLength).First();
            // Шаг 4.
            if (Vector3.Distance(currentNode.Position, destination) <= accurancy * accuracyFactor)
            {
                PathfindingComplete(CollapsePath(GetPathForNode(currentNode)).ToArray());
                return;
            }
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
        private void PathfindingComplete(Vector3[] pathPoints)
        {
            if (pathPoints != null)
            {
                status = DriverStatus.Navigating;
                foreach (Vector3 x in pathPoints)
                {
                    //Debug.Log(x);
                    path.Enqueue(x);
                }
                BuildPathArrows();
            }
            calculating = false;
        }
        private float GetHeuristicPathLength(Vector3 from, Vector3 to)
        {
            return Mathf.Abs(from.x - to.x) + Math.Abs(from.y - to.y) + Math.Abs(from.z - to.z);//Vector3.Distance(from, to);//
        }
        private List<PathNode> GetNeighbours(PathNode pathNode, Vector3 destination, float accuracyFactor)
        {
            var result = new List<PathNode>();
            float gridStepLocal = gridStep * accuracyFactor;

            // Соседними точками являются соседние по стороне клетки.
            Vector3[] neighbourPoints = new Vector3[27];
            neighbourPoints[0] = new Vector3(pathNode.Position.x + gridStepLocal, pathNode.Position.y - gridStepLocal, pathNode.Position.z - gridStepLocal);
            neighbourPoints[1] = new Vector3(pathNode.Position.x + gridStepLocal, pathNode.Position.y, pathNode.Position.z - gridStepLocal);
            neighbourPoints[2] = new Vector3(pathNode.Position.x + gridStepLocal, pathNode.Position.y + gridStepLocal, pathNode.Position.z - gridStepLocal);

            neighbourPoints[3] = new Vector3(pathNode.Position.x + gridStepLocal, pathNode.Position.y - gridStepLocal, pathNode.Position.z);
            neighbourPoints[4] = new Vector3(pathNode.Position.x + gridStepLocal, pathNode.Position.y, pathNode.Position.z);
            neighbourPoints[5] = new Vector3(pathNode.Position.x + gridStepLocal, pathNode.Position.y + gridStepLocal, pathNode.Position.z);

            neighbourPoints[6] = new Vector3(pathNode.Position.x + gridStepLocal, pathNode.Position.y - gridStepLocal, pathNode.Position.z + gridStepLocal);
            neighbourPoints[7] = new Vector3(pathNode.Position.x + gridStepLocal, pathNode.Position.y, pathNode.Position.z + gridStepLocal);
            neighbourPoints[8] = new Vector3(pathNode.Position.x + gridStepLocal, pathNode.Position.y + gridStepLocal, pathNode.Position.z + gridStepLocal);

            neighbourPoints[9] = new Vector3(pathNode.Position.x, pathNode.Position.y - gridStepLocal, pathNode.Position.z - gridStepLocal);
            neighbourPoints[10] = new Vector3(pathNode.Position.x, pathNode.Position.y, pathNode.Position.z - gridStepLocal);
            neighbourPoints[11] = new Vector3(pathNode.Position.x, pathNode.Position.y + gridStepLocal, pathNode.Position.z - gridStepLocal);

            neighbourPoints[12] = new Vector3(pathNode.Position.x, pathNode.Position.y - gridStepLocal, pathNode.Position.z);
            //this pathNode
            neighbourPoints[13] = new Vector3(pathNode.Position.x, pathNode.Position.y + gridStepLocal, pathNode.Position.z);

            neighbourPoints[14] = new Vector3(pathNode.Position.x, pathNode.Position.y - gridStepLocal, pathNode.Position.z + gridStepLocal);
            neighbourPoints[15] = new Vector3(pathNode.Position.x, pathNode.Position.y, pathNode.Position.z + gridStepLocal);
            neighbourPoints[16] = new Vector3(pathNode.Position.x, pathNode.Position.y + gridStepLocal, pathNode.Position.z + gridStepLocal);

            neighbourPoints[17] = new Vector3(pathNode.Position.x - gridStepLocal, pathNode.Position.y - gridStepLocal, pathNode.Position.z - gridStepLocal);
            neighbourPoints[18] = new Vector3(pathNode.Position.x - gridStepLocal, pathNode.Position.y, pathNode.Position.z - gridStepLocal);
            neighbourPoints[19] = new Vector3(pathNode.Position.x - gridStepLocal, pathNode.Position.y + gridStepLocal, pathNode.Position.z - gridStepLocal);

            neighbourPoints[20] = new Vector3(pathNode.Position.x - gridStepLocal, pathNode.Position.y - gridStepLocal, pathNode.Position.z);
            neighbourPoints[21] = new Vector3(pathNode.Position.x - gridStepLocal, pathNode.Position.y, pathNode.Position.z);
            neighbourPoints[22] = new Vector3(pathNode.Position.x - gridStepLocal, pathNode.Position.y + gridStepLocal, pathNode.Position.z);

            neighbourPoints[23] = new Vector3(pathNode.Position.x - gridStepLocal, pathNode.Position.y - gridStepLocal, pathNode.Position.z + gridStepLocal);
            neighbourPoints[24] = new Vector3(pathNode.Position.x - gridStepLocal, pathNode.Position.y, pathNode.Position.z + gridStepLocal);
            neighbourPoints[25] = new Vector3(pathNode.Position.x - gridStepLocal, pathNode.Position.y + gridStepLocal, pathNode.Position.z + gridStepLocal);

            //neighbourPoints[0] = pathNode.Position + (gridStepLocal * ((-1 * walkerTransform.forward) + (-1 * walkerTransform.right) + (-1 * walkerTransform.up)));
            //neighbourPoints[1] = pathNode.Position + (gridStepLocal * ((-1 * walkerTransform.forward) + (-1 * walkerTransform.right) + (0 * walkerTransform.up)));
            //neighbourPoints[2] = pathNode.Position + (gridStepLocal * ((-1 * walkerTransform.forward) + (-1 * walkerTransform.right) + (1 * walkerTransform.up)));

            //neighbourPoints[3] = pathNode.Position + (gridStepLocal * ((-1 * walkerTransform.forward) + (0 * walkerTransform.right) + (-1 * walkerTransform.up)));
            //neighbourPoints[4] = pathNode.Position + (gridStepLocal * ((-1 * walkerTransform.forward) + (0 * walkerTransform.right) + (0 * walkerTransform.up)));
            //neighbourPoints[5] = pathNode.Position + (gridStepLocal * ((-1 * walkerTransform.forward) + (0 * walkerTransform.right) + (1 * walkerTransform.up)));

            //neighbourPoints[6] = pathNode.Position + (gridStepLocal * ((-1 * walkerTransform.forward) + (1 * walkerTransform.right) + (-1 * walkerTransform.up)));
            //neighbourPoints[7] = pathNode.Position + (gridStepLocal * ((-1 * walkerTransform.forward) + (1 * walkerTransform.right) + (0 * walkerTransform.up)));
            //neighbourPoints[8] = pathNode.Position + (gridStepLocal * ((-1 * walkerTransform.forward) + (1 * walkerTransform.right) + (1 * walkerTransform.up)));
            ////
            //neighbourPoints[9] = pathNode.Position + (gridStepLocal * ((0 * walkerTransform.forward) + (-1 * walkerTransform.right) + (-1 * walkerTransform.up)));
            //neighbourPoints[10] = pathNode.Position + (gridStepLocal * ((0 * walkerTransform.forward) + (-1 * walkerTransform.right) + (0 * walkerTransform.up)));
            //neighbourPoints[11] = pathNode.Position + (gridStepLocal * ((0 * walkerTransform.forward) + (-1 * walkerTransform.right) + (1 * walkerTransform.up)));

            //neighbourPoints[12] = pathNode.Position + (gridStepLocal * ((0 * walkerTransform.forward) + (0 * walkerTransform.right) + (-1 * walkerTransform.up)));
            ////this
            //neighbourPoints[13] = pathNode.Position + (gridStepLocal * ((0 * walkerTransform.forward) + (0 * walkerTransform.right) + (1 * walkerTransform.up)));

            //neighbourPoints[14] = pathNode.Position + (gridStepLocal * ((0 * walkerTransform.forward) + (1 * walkerTransform.right) + (-1 * walkerTransform.up)));
            //neighbourPoints[15] = pathNode.Position + (gridStepLocal * ((0 * walkerTransform.forward) + (1 * walkerTransform.right) + (0 * walkerTransform.up)));
            //neighbourPoints[16] = pathNode.Position + (gridStepLocal * ((0 * walkerTransform.forward) + (1 * walkerTransform.right) + (1 * walkerTransform.up)));
            ////
            //neighbourPoints[17] = pathNode.Position + (gridStepLocal * ((-1 * walkerTransform.forward) + (-1 * walkerTransform.right) + (-1 * walkerTransform.up)));
            //neighbourPoints[18] = pathNode.Position + (gridStepLocal * ((-1 * walkerTransform.forward) + (-1 * walkerTransform.right) + (0 * walkerTransform.up)));
            //neighbourPoints[19] = pathNode.Position + (gridStepLocal * ((-1 * walkerTransform.forward) + (-1 * walkerTransform.right) + (1 * walkerTransform.up)));

            //neighbourPoints[20] = pathNode.Position + (gridStepLocal * ((-1 * walkerTransform.forward) + (0 * walkerTransform.right) + (-1 * walkerTransform.up)));
            //neighbourPoints[21] = pathNode.Position + (gridStepLocal * ((-1 * walkerTransform.forward) + (0 * walkerTransform.right) + (0 * walkerTransform.up)));
            //neighbourPoints[22] = pathNode.Position + (gridStepLocal * ((-1 * walkerTransform.forward) + (0 * walkerTransform.right) + (1 * walkerTransform.up)));

            //neighbourPoints[23] = pathNode.Position + (gridStepLocal * ((-1 * walkerTransform.forward) + (1 * walkerTransform.right) + (-1 * walkerTransform.up)));
            //neighbourPoints[24] = pathNode.Position + (gridStepLocal * ((-1 * walkerTransform.forward) + (1 * walkerTransform.right) + (0 * walkerTransform.up)));
            //neighbourPoints[25] = pathNode.Position + (gridStepLocal * ((-1 * walkerTransform.forward) + (1 * walkerTransform.right) + (1 * walkerTransform.up)));
            neighbourPoints[26] = destination;
            //
            //neighbourPoints[26] = pathNode.Position + (gridStepLocal * ((2 * walkerTransform.forward) + (-1 * walkerTransform.right) + (-1 * walkerTransform.up)));
            //neighbourPoints[27] = pathNode.Position + (gridStepLocal * ((2 * walkerTransform.forward) + (-1 * walkerTransform.right) + (0 * walkerTransform.up)));
            //neighbourPoints[28] = pathNode.Position + (gridStepLocal * ((2 * walkerTransform.forward) + (-1 * walkerTransform.right) + (1 * walkerTransform.up)));

            //neighbourPoints[29] = pathNode.Position + (gridStepLocal * ((2 * walkerTransform.forward) + (0 * walkerTransform.right) + (-1 * walkerTransform.up)));
            //neighbourPoints[30] = pathNode.Position + (gridStepLocal * ((2 * walkerTransform.forward) + (0 * walkerTransform.right) + (0 * walkerTransform.up)));
            //neighbourPoints[31] = pathNode.Position + (gridStepLocal * ((2 * walkerTransform.forward) + (0 * walkerTransform.right) + (1 * walkerTransform.up)));

            //neighbourPoints[32] = pathNode.Position + (gridStepLocal * ((2 * walkerTransform.forward) + (1 * walkerTransform.right) + (-1 * walkerTransform.up)));
            //neighbourPoints[33] = pathNode.Position + (gridStepLocal * ((2 * walkerTransform.forward) + (1 * walkerTransform.right) + (0 * walkerTransform.up)));
            //neighbourPoints[34] = pathNode.Position + (gridStepLocal * ((2 * walkerTransform.forward) + (1 * walkerTransform.right) + (1 * walkerTransform.up)));
            //debug
            //GameObject worldpoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //worldpoint.GetComponent<SphereCollider>().enabled = false;
            //worldpoint.AddComponent<Service.SelfDestructor>().ttl = 1f;

            foreach (Vector3 point in neighbourPoints)
            {
                // Проверяем, что по клетке можно ходить.
                if (!CanWalk(pathNode.Position, point) || !FreeSpace(point, gridStepLocal * 0.5f))
                    continue;
                // Заполняем данные для точки маршрута.
                //debug
                //GameObject.Instantiate(worldpoint, point, walkerTransform.rotation);

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
        private bool CanWalk(Vector3 position, Vector3 destination, float accuracyFactor)
        {
            Vector3[] directions = new Vector3[6];
            directions[0] = Vector3.forward;//forward
            directions[1] = -Vector3.forward;//backward
            directions[2] = Vector3.right;//right
            directions[3] = -Vector3.right;//left
            directions[4] = Vector3.up;//up
            directions[5] = -Vector3.up;//down
            foreach (Vector3 direction in directions)
            {
                if (!CanWalk(position + gridStep * 0.5f * accuracyFactor * direction, destination))
                    return false;
            }
            return true;
        }
        private bool FreeSpace(Vector3 point, float radius)
        {
            Collider[] hits = Physics.OverlapSphere(point, radius);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].tag == "Terrain")
                    return false;
            }
            return true;
        }
        private List<PathNode> GetNeighboursJump(PathNode pathNode, Vector3 destination, float accuracyFactor)
        {
            Vector3[] directions = new Vector3[27];
            List<PathNode> result = new List<PathNode>();
            directions[0] = Vector3.forward;//forward
            directions[1] = -Vector3.forward;//backward
            directions[2] = Vector3.right;//right
            directions[3] = -Vector3.right;//left
            directions[4] = Vector3.up;//up
            directions[5] = -Vector3.up;//down

            directions[6] =  Vector3.right + Vector3.up;
            directions[7] =  Vector3.right + -Vector3.up;
            directions[8] = -Vector3.right + Vector3.up;
            directions[9] = -Vector3.right + -Vector3.up;

            directions[10] = Vector3.forward + Vector3.right;
            directions[11] = Vector3.forward + -Vector3.right;
            directions[12] = Vector3.forward + Vector3.up;
            directions[13] = Vector3.forward + -Vector3.up;
            directions[14] = Vector3.forward + Vector3.right + Vector3.up;
            directions[15] = Vector3.forward + Vector3.right + -Vector3.up;
            directions[16] = Vector3.forward + -Vector3.right + Vector3.up;
            directions[17] = Vector3.forward + -Vector3.right + -Vector3.up;

            directions[18] = -Vector3.forward + Vector3.right;
            directions[19] = -Vector3.forward + -Vector3.right;
            directions[20] = -Vector3.forward + Vector3.up;
            directions[21] = -Vector3.forward + -Vector3.up;
            directions[22] = -Vector3.forward + Vector3.right + Vector3.up;
            directions[23] = -Vector3.forward + Vector3.right + -Vector3.up;
            directions[24] = -Vector3.forward + -Vector3.right + Vector3.up;
            directions[25] = -Vector3.forward + -Vector3.right + -Vector3.up;

            directions[26] = (destination - pathNode.Position).normalized;

            //debug
            GameObject worldpoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            worldpoint.GetComponent<SphereCollider>().enabled = false;
            worldpoint.AddComponent<Service.SelfDestructor>().ttl = 1f;

            foreach (Vector3 direction in directions)
            {
                //The only difference between this GetNeighbours and the other one 
                //is that this one calls Jump here.
                Vector3 point = Jump(pathNode.Position + direction * gridStep, direction, destination);
                if (point != Vector3.zero)
                {
                    // Проверяем, что по клетке можно ходить.
                    if (!CanWalk(pathNode.Position, point) || !FreeSpace(point, gridStep * accuracyFactor * 0.5f))
                        continue;
                    // Заполняем данные для точки маршрута.
                    //debug
                    GameObject.Instantiate(worldpoint, point, walkerTransform.rotation);

                    PathNode neighbourNode = new PathNode()
                    {
                        Position = point,
                        CameFrom = pathNode,
                        PathLengthFromStart = pathNode.PathLengthFromStart + Vector3.Distance(pathNode.Position, point),
                        HeuristicEstimatePathLength = GetHeuristicPathLength(point, destination)
                    };
                    result.Add(neighbourNode);
                }
            }
            return result;
        }
        public Vector3 Jump(Vector3 current, Vector3 direction, Vector3 destination)
        {
            // Position of new node we are going to consider:
            Vector3 next = current + direction * gridStep * navAccurancyFactor;

            // If it's blocked we can't jump here
            if (!CanWalk(current, next) || !FreeSpace(current, gridStep * navAccurancyFactor)||next.magnitude > 2500)
                return Vector3.zero;

            // If the node is the goal return it
            if (Vector3.Distance(next, destination) < accurancy * navAccurancyFactor)
                return next;

            if (!FreeSpace(next, gridStep * navAccurancyFactor))
                return next;
            // If forced neighbor was not found try next jump point
            return Jump(next, direction, destination);
        }
    private List<Vector3> GetPathForNode(PathNode pathNode)
        {
            List<Vector3> result = new List<Vector3>();
            PathNode currentNode = pathNode;
            
            //debug
            //GameObject worldpoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //worldpoint.GetComponent<SphereCollider>().enabled = false;
            //worldpoint.AddComponent<SelfDestructor>().ttl = 15f;

            while (currentNode != null)
            {
                //debug
                //GameObject.Instantiate(worldpoint, currentNode.Position, walkerTransform.rotation);

                result.Add(currentNode.Position);
                currentNode = currentNode.CameFrom;
            }
            result.Reverse();
            return result;
        }
        private List<Vector3> CollapsePath(List<Vector3> path)
        {
            List<Vector3> collapsedPath = path;
            int i = 0;
            while ((i+2) < collapsedPath.Count)
            {
                if (CanWalk(collapsedPath[i], collapsedPath[i + 2], navAccurancyFactor))
                    collapsedPath.Remove(collapsedPath[i + 1]);
                else i = i + 1;
            }

            return collapsedPath;
        }
        public void ClearQueue()
        {
            //backCount = 0;
            path.Clear();
        }
        private void MoveSimple()
        {
              walkerTransform.Translate((path.Peek() - walkerTransform.position).normalized * walker.Speed * Time.deltaTime, Space.World);
            //walkerBody.velocity = (path.Peek() - walkerTransform.position).normalized * walker.Speed;

            //    Vector3 targetMotion = (path.Peek() - walkerTransform.position).normalized;
            //    float mainThrustLocal = Vector3.Project(targetMotion, walkerTransform.forward).magnitude;
            //    float horisontalShiftLocal = Vector3.Project(targetMotion, walkerTransform.right).magnitude;
            //    float vertikalShiftLocal = Vector3.Project(targetMotion, walkerTransform.up).magnitude;
            //    Vector3 velocityLocal = walkerTransform.right * horisontalShiftLocal * walker.ShiftSpeed + walkerTransform.up * vertikalShiftLocal * walker.ShiftSpeed + walkerTransform.forward * mainThrustLocal * walker.Speed;
            //    walkerBody.velocity = velocityLocal;
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
            //Debug.Log("\t mT = " + mainThrust + "\t hT = " + horisontalShiftThrust + "\t vT = " + verticalShiftThrust);
            Vector3 shiftLocal = (walkerTransform.right * horisontalShiftThrust * walker.ShiftSpeed + walkerTransform.up * verticalShiftThrust * walker.ShiftSpeed + walkerTransform.forward * mainThrust * walker.Speed) * 25;
            walkerBody.AddForce(shiftLocal, ForceMode.Acceleration);
        }

        private float ThrustAxis()
        {
            float sign;
                float velocityPojection = Vector3.Project(walkerBody.velocity, walkerTransform.forward).magnitude;
            if (path.Count != 0)
            {
                Vector3 targetMotion = path.Peek() - walkerTransform.position;
                if (Vector3.Angle(targetMotion, walkerTransform.forward) < 90)
                    sign = 1;
                else sign = -1;
                float targetPojection = Vector3.Project(targetMotion, walkerTransform.forward).magnitude;
                if (targetPojection > 0.1f)
                    //if (Mathf.Abs(targetPojection) > (Mathf.Abs(velocityPojection) * 2.5))
                        return 1f * sign;
            }
            else
            {
                if (Vector3.Angle(walkerBody.velocity, walkerTransform.forward) < 90)
                    sign = 2;
                else sign = -2;
                return -Mathf.Clamp01(velocityPojection) * sign;
            }
            //else return 0.2f * (Mathf.Sign(targetPojection));
            return 0;
        }
        private float HorizontalShiftAxis()
        {
            float sign;
                float velocityPojection = Vector3.Project(walkerBody.velocity, walkerTransform.right).magnitude;
            if (path.Count != 0)
            {
                Vector3 targetMotion = path.Peek() - walkerTransform.position;
                if (Vector3.Angle(targetMotion, walkerTransform.right) < 90)
                    sign = 1;
                else sign = -1;
                float targetPojection = Vector3.Project(targetMotion, walkerTransform.right).magnitude;
                if (targetPojection > 0.1f)
                    if (Mathf.Abs(targetPojection) > Mathf.Abs(velocityPojection))
                        return 0.5f * sign;
            }
            else
            {
                if (Vector3.Angle(walkerBody.velocity, walkerTransform.right) < 90)
                    sign = 1;
                else sign = -1;
                return -Mathf.Clamp01(velocityPojection) * sign;
            }
            return 0;
        }
        private float VerticalShiftAxis()
        {
            float sign;
                float velocityPojection = Vector3.Project(walkerBody.velocity, walkerTransform.up).magnitude;
            if (path.Count != 0)
            {
                Vector3 targetMotion = path.Peek() - walkerTransform.position;
                if (Vector3.Angle(targetMotion, walkerTransform.up) < 90)
                    sign = 1;
                else sign = -1;
                float targetPojection = Vector3.Project(targetMotion, walkerTransform.up).magnitude;
                if (targetPojection > 0.1f)
                    if (Mathf.Abs(targetPojection) > Mathf.Abs(velocityPojection))
                        return 0.5f * sign;
            }
            else
            {
                if (Vector3.Angle(walkerBody.velocity, walkerTransform.up) < 90)
                    sign = 1;
                else sign = -1;
                return -Mathf.Clamp01(velocityPojection) * sign;
            }
            return 0;
        }

        private void Rotate()
        {
            if (walker.Gunner.Target == null)
            {
                Vector3 targetDirection = path.Peek() - walker.transform.position;
                if (targetDirection != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(targetDirection, new Vector3(0, 1, 0));
                    walker.transform.rotation = Quaternion.RotateTowards(walker.transform.rotation, targetRotation, Time.deltaTime * walker.RotationSpeed);
                }
            }
        }
        private void Stabilisation()
        {
            Quaternion rotDest = Quaternion.Euler(0, walkerTransform.rotation.eulerAngles.y, 0);
            if (Quaternion.Angle(walkerTransform.rotation, rotDest) > 0.1f)
                walkerTransform.rotation = Quaternion.RotateTowards(walkerTransform.rotation, rotDest, Time.deltaTime * walker.RotationSpeed * 0.6f);
            else walkerTransform.rotation = rotDest;

            Collider[] hits = Physics.OverlapSphere(walkerTransform.position, gridStep);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].tag == "Terrain")
                    walkerTransform.position = Vector3.MoveTowards(walkerTransform.position, (hits[i].transform.position - walkerTransform.position) * -gridStep, Time.deltaTime * walker.ShiftSpeed * 0.2f);
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
