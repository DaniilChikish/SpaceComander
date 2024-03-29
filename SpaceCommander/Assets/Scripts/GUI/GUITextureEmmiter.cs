﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpaceCommander.UI
{
    public class GUITextureEmmiter : MonoBehaviour
    {
        public Texture Target;
        //public int time;
        public int TargetFrameWidth;
        public int TargetFrameHeight;
        public int TargetFrameOffset;
        // Use this for initialization
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

            GUI.DrawTexture(new Rect(crd.x - TargetFrameWidth / 2f, crd.y - TargetFrameOffset, TargetFrameWidth, TargetFrameHeight), Target);
            //GUI.Label(new Rect(crd.x - 120, crd.y - TargetFrameOffset, 240, 18), UnitName, style);
        }
    }
}
