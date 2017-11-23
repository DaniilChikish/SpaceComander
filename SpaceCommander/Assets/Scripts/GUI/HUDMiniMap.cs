using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpaceCommander.UI
{
    public class HUDMiniMap : MonoBehaviour
    {
        public GUISkin Skin;
        public Vector2 Position;
        public Vector2 Size;
        private Vector4 border;
        public Texture2D Background;
        public RenderTexture Map;
        public Texture2D North;
        public Texture2D East;
        public Texture2D West;
        public Texture2D South;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            border.x = Size.x * 31f / 256f;
            border.y = Size.y * 39f / 256f;
            border.z = Size.x * 31f / 256f;
            border.w = Size.y * 39f / 256f;
            Size.y = Screen.height / 3f;
            Size.x = Size.y;
            Position.x = Screen.width - Size.x;
            Position.y = Screen.height - Size.y;
        }
        private void OnGUI()
        {
            GUI.skin = Skin;
            GUI.BeginGroup(new Rect(Position.x, Position.y, Size.x, Size.y));
            GUI.DrawTexture(new Rect(0, 0, Size.x, Size.y), Background);
            GUI.DrawTexture(new Rect(border.x, border.y + 2, Size.x - border.x - border.z, Size.y - border.y - border.w), Map);
            GUI.DrawTexture(new Rect(border.x + (Size.x - border.x - border.z) / 2 - 9, border.y, 18, 18), North);
            GUI.DrawTexture(new Rect(border.x + (Size.x - border.x - border.z) / 2 - 9, Size.y - border.w - 18, 18, 18), South);
            GUI.DrawTexture(new Rect(border.x, border.y + (Size.y - border.y - border.w) / 2 - 9, 18, 18), West);
            GUI.DrawTexture(new Rect(Size.x - border.z - 18, border.y + (Size.y - border.y - border.w) / 2 - 9, 18, 18), East);
            GUI.EndGroup();
        }
    }
}