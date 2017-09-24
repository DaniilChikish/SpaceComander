using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpaceCommander
{
    public class MoveTarget : MonoBehaviour
    {
        public Texture Target;
        private float ttl;
        //public int time;
        public int TargetFrameWidth;
        public int TargetFrameHeight;
        public int TargetFrameOffset;
        // Use this for initialization
        void Start()
        {
            ttl = 1.5f;
        }
        private void OnGUI()
        {
            Vector3 crd = Camera.main.WorldToScreenPoint(transform.position);
            crd.y = Screen.height - crd.y;

            //GUIStyle style = new GUIStyle();
            //style.fontSize = 12;
            //style.font = GuiProcessor.getI.rusfont;
            //style.normal.textColor = Color.cyan;
            //style.alignment = TextAnchor.MiddleCenter;
            //style.fontStyle = FontStyle.Italic;

            GUI.DrawTexture(new Rect(crd.x - TargetFrameWidth / 2, crd.y - TargetFrameOffset, TargetFrameWidth, TargetFrameHeight), Target);
            //GUI.Label(new Rect(crd.x - 120, crd.y - TargetFrameOffset, 240, 18), UnitName, style);
        }

        // Update is called once per frame
        void Update()
        {
            if (ttl > 0)
                ttl -= Time.deltaTime;
            else Destroy(this.gameObject);
        }
    }
}
