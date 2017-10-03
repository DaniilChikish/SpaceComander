using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpaceCommander
{
    public class HUDUnitPrew : MonoBehaviour
    {
        public GUISkin Skin;
        public Vector2 Position;
        public Vector2 Size;
        private Vector4 border;
        public Texture2D Background;
        public RenderTexture UnitPrew;
        public bool Enabled;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            border.x = Size.x * 31 / 256f;
            border.y = Size.y * 39 / 256f;
            border.z = Size.x * 31 / 256f;
            border.w = Size.y * 39 / 256f;
            Size.y = Screen.height / 3.5f;
            Size.x = Size.y;
            Position.x = Screen.width - Size.x;
            Position.y = Screen.height - Size.y - Screen.height / 3f;
        }
        private void OnGUI()
        {
            GUI.skin = Skin;
            GUI.BeginGroup(new Rect(Position.x, Position.y, Size.x, Size.y));
            GUI.DrawTexture(new Rect(0, 0, Size.x, Size.y), Background);
            if (Enabled) GUI.DrawTexture(new Rect(border.x, border.y + 2, Size.x - border.x - border.z, Size.y - border.y - border.w), UnitPrew);
            GUI.EndGroup();
        }
    }
}
