using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
namespace PracticeProject
{
    public class UnitSelectionComponent : MonoBehaviour
    {
        bool isSelecting = false;
        Vector3 mousePosition1;
        public GameObject destanationMarkerPrefab;

        void Update()
        {

            // If we press the left mouse button, begin selection and remember the location of the mouse
            if (Input.GetMouseButtonDown(0))
            {
                isSelecting = true;
                mousePosition1 = Input.mousePosition;

                foreach (var selectableObject in FindObjectsOfType<SpaceShip>())
                {
                    if (selectableObject.isSelected == true)
                    {
                        selectableObject.gameObject.GetComponent<SpaceShip>().SelectUnit(false);
                        //Destroy(selectableObject.selectionCircle.gameObject);
                        //selectableObject.selectionCircle = null;
                    }
                }
                GameObject.Find("Gui").transform.FindChild("UnitPeviev").gameObject.SetActive(false);
                FindObjectsOfType<GlobalController>()[0].selectedList.Clear();
            }
            // If we let go of the left mouse button, end selection
            if (Input.GetMouseButtonUp(0))
            {
                var selectedObjects = new List<SpaceShip>();
                foreach (var selectableObject in FindObjectsOfType<SpaceShip>())
                {
                    if ((FindObjectsOfType<GlobalController>()[0].playerArmy == selectableObject.GetComponent<SpaceShip>().Team) &&
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
                    if ((FindObjectsOfType<GlobalController>()[0].playerArmy == selectableObject.GetComponent<SpaceShip>().Team) && 
                        IsWithinSelectionBounds(selectableObject.gameObject))
                    {
                        if (selectableObject.isSelected == false)
                        {
                            selectableObject.gameObject.GetComponent<SpaceShip>().SelectUnit(true);
                            GameObject.Find("Gui").transform.FindChild("UnitPeviev").gameObject.SetActive(true);
                        }
                    }
                }
            }
            if (Input.GetMouseButtonDown(1))
            {
                //Debug.Log("MouseButtonDown(1)");
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 10000))
                {
                    SendTo(hit.point);
                    Instantiate(destanationMarkerPrefab, hit.point, new Quaternion());
                }
            }
        }

        private void SendTo(Vector3 destination)
        {
            //Debug.Log("SendTo...");
            if (FindObjectsOfType<GlobalController>()[0].selectedList.Count > 0)
                foreach (SpaceShip x in FindObjectsOfType<GlobalController>()[0].selectedList)
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
            var viewportBounds = Utils.GetViewportBounds(camera, mousePosition1, Input.mousePosition);
            return viewportBounds.Contains(camera.WorldToViewportPoint(gameObject.transform.position));
        }

        void OnGUI()
        {
            if (isSelecting)
            {
                // Create a rect from both mouse positions
                var rect = Utils.GetScreenRect(mousePosition1, Input.mousePosition);
                Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
                Utils.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
            }
        }
    }
}