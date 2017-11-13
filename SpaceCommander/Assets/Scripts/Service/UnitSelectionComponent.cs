using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
namespace SpaceCommander
{
    public class UnitSelectionComponent : MonoBehaviour
    {
        bool isSelecting = false;
        Vector3 mousePosition1;
        public GameObject destanationMarkerPrefab;
        public GameObject attackMarkerPrefab;
        GlobalController Global;
        private void Start()
        {
            Global = GlobalController.GetInstance();
        }
        void Update()
        {

            // If we press the left mouse button, begin selection and remember the location of the mouse
            if (Input.GetMouseButtonDown(0))
            {
                bool singleFinded = false;
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 10000))
                {
                    Debug.Log("hit " + hit.transform.name + " - " + hit.transform.tag);
                    SpaceShip unknownUnit = null;
                    Collider[] unknownColiders = Physics.OverlapSphere(hit.point, 10);
                    foreach (Collider x in unknownColiders)
                    {
                        unknownUnit = x.transform.GetComponentInParent<SpaceShip>();
                        if (unknownUnit != null && unknownUnit.Team == Global.playerArmy)
                        {
                            singleFinded = true;
                            ClearSelected();
                            //Global.selectedList.Add(unknownUnit);
                            unknownUnit.SelectUnit(true);
                            break;
                        }
                    }
                }
                if (!singleFinded&& !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                {
                    isSelecting = true;
                    mousePosition1 = Input.mousePosition;
                    ClearSelected();
                }
            }
            // If we let go of the left mouse button, end selection
            if (Input.GetMouseButtonUp(0))
            {
                var selectedObjects = new List<SpaceShip>();
                foreach (var selectableObject in FindObjectsOfType<SpaceShip>())
                {
                    if ((Global.playerArmy == selectableObject.GetComponent<SpaceShip>().Team) &&
                        (IsWithinSelectionBounds(selectableObject.gameObject)))
                    {
                        selectedObjects.Add(selectableObject);
                    }
                }

                var sb = new StringBuilder();
                sb.AppendLine(string.Format("Selecting [{0}] Units", selectedObjects.Count));
                foreach (var selectedObject in selectedObjects)
                    sb.AppendLine("-> " + selectedObject.gameObject.name);
                Debug.Log(sb.ToString());

                isSelecting = false;
            }

            // Highlight all objects within the selection box
            if (isSelecting)
            {
                foreach (var selectableObject in FindObjectsOfType<SpaceShip>())
                {
                    if ((Global.playerArmy == selectableObject.GetComponent<SpaceShip>().Team) && 
                        IsWithinSelectionBounds(selectableObject.gameObject))
                    {
                        if (selectableObject.isSelected == false)
                        {
                            selectableObject.gameObject.GetComponent<SpaceShip>().SelectUnit(true);
                        }
                    }
                }
            }
            if (Input.GetMouseButtonDown(1))
            {
                //Debug.Log("MouseButtonDown(1)");
                if (Global.selectedList.Count > 0)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 10000))
                    {
                        Unit unknownUnit = null;
                        Collider[] unknownColiders = Physics.OverlapSphere(hit.point, 10);
                        foreach (Collider x in unknownColiders)
                        {
                            unknownUnit = x.transform.GetComponentInParent<Unit>();
                            if (unknownUnit != null && unknownUnit.Team != Global.playerArmy)
                            {
                                Instantiate(attackMarkerPrefab, unknownUnit.transform.position, new Quaternion(), unknownUnit.transform);
                                foreach (SpaceShip y in Global.selectedList)
                                    y.AttackThat(unknownUnit);
                                break;
                            }
                        }
                        if (unknownUnit == null)
                        {
                            SendTo(hit.point);
                            Instantiate(destanationMarkerPrefab, hit.point, new Quaternion());
                        }
                    }
                }
            }
        }

        private void ClearSelected()
        {
            foreach (var selectableObject in Global.selectedList)
            {
                if (selectableObject.isSelected == true)
                {
                    selectableObject.gameObject.GetComponent<SpaceShip>().SelectUnit(false);
                }
            }
            Global.selectedList.Clear();
        }

        private void SendTo(Vector3 destination)
        {
            //Debug.Log("SendTo...");
            if (Global.selectedList.Count > 0)
                foreach (SpaceShip x in Global.selectedList)
                {
                    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                        x.SendToQueue(destination);
                    else
                        x.SendTo(destination);
                }
        }

        public bool IsWithinSelectionBounds(GameObject gameObject)
        {
            if (!isSelecting)
                return false;

            var camera = Camera.main;
            var viewportBounds = SelectionUtils.GetViewportBounds(camera, mousePosition1, Input.mousePosition);
            return viewportBounds.Contains(camera.WorldToViewportPoint(gameObject.transform.position));
        }

        void OnGUI()
        {
            if (isSelecting)
            {
                // Create a rect from both mouse positions
                var rect = SelectionUtils.GetScreenRect(mousePosition1, Input.mousePosition);
                SelectionUtils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
                SelectionUtils.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
            }
        }
    }
}