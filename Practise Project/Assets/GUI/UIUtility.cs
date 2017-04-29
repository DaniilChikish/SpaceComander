using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace PracticeProject
{
    public class UIWindowInfo
    {
        public Rect rect;
        public float Alpha;
        public float UIAlpha;
        public bool Show;
        public float TimeToWait;
        public float Speed;
        public float CenterX { get { return rect.width / 2f; } }
        public float CenterY { get { return rect.height / 2f; } }
        public float Bottom { get { return rect.height; } }

        public UIWindowInfo(Rect rect, float alpha, float uialpha, bool show, int TimeToWait, float speed)
        {
            this.rect = rect;
            this.Alpha = alpha;
            this.UIAlpha = uialpha;
            this.Show = show;
            this.TimeToWait = TimeToWait;
            this.Speed = speed;
        }
        public UIWindowInfo(Rect rect)
        {
            this.rect = rect;
            this.Alpha = 0f;
            this.UIAlpha = 0f;
            this.Show = true;
            this.TimeToWait = 0f;
            this.Speed = 2.0f;
        }
    }
    public static class UIUtil
    {
        public static void WindowTitle(UIWindowInfo window, string text)
        {
            Vector2 bgOffset = new Vector2(36, 15);
            // Determine the width and height of the title background
            float bgWidth = window.rect.width - (2 * bgOffset.x);
            float bgHeight = 91;
            Rect windowTitleBGRect = new Rect(bgOffset.x, bgOffset.y, bgWidth, bgHeight);

            // Lay the title background
            GUI.Label(windowTitleBGRect, "", "windowTitleBackground");

            Vector2 titleOffset0 = new Vector2(0, 56);
            Vector2 titleOffset1 = new Vector2(2, 53);
            Vector2 titleOffset2 = new Vector2(-3, 57);
            Vector2 titleOffset3 = new Vector2(1, 55);
            Vector2 titleOffset4 = new Vector2(4, 53);
            Vector2 titleSize = new Vector2(window.rect.width, 29);

            // Draw the title
            GUI.Label(new Rect(titleOffset0.x, titleOffset0.y, titleSize.x, titleSize.y), text, "windowTitle");
            GUI.Label(new Rect(titleOffset1.x, titleOffset1.y, titleSize.x, titleSize.y), text, "windowTitleShadow");
            GUI.Label(new Rect(titleOffset2.x, titleOffset2.y, titleSize.x, titleSize.y), text, "windowTitleShadow");
            GUI.Label(new Rect(titleOffset3.x, titleOffset3.y, titleSize.x, titleSize.y), text, "windowTitleShadow");
            GUI.Label(new Rect(titleOffset4.x, titleOffset4.y, titleSize.x, titleSize.y), text, "windowTitleShadow");

            Vector2 olOffset = new Vector2(37, 41);
            // Determine the width and height of the title overlay
            float olWidth = window.rect.width - (2 * olOffset.x);
            float olHeight = 55;
            Rect windowTitleOLRect = new Rect(olOffset.x, olOffset.y, olWidth, olHeight);

            // Lay the title overlay
            GUI.Label(windowTitleOLRect, "", "windowTitleOverlay");
        }

        //public static void DoAnimation1(Vector2 offset)
        //{
        //    GUI.BeginGroup(new Rect(offset.x, offset.y, 128, 106));

        //    // Set the background
        //    GUI.Label(new Rect(0, 0, 128, 106), "", "animationBackground");

        //    // Draw the texture
        //    Rect position = new Rect(24, 14, 80, 78);
        //    Rect texCoords = new Rect(LoadingAnimation1TexOffset.x, LoadingAnimation1TexOffset.y, LoadingAnimation1TexSize.x, LoadingAnimation1TexSize.y);
        //    bool alpha = true;

        //    GUI.DrawTextureWithTexCoords(position, LoadingAnimation1, texCoords, alpha);

        //    // Do the percentage
        //    GUIStyle PercentageStyle = GUI.skin.GetStyle("animationPercentage");
        //    GUIStyle TextStyle = GUI.skin.GetStyle("animationText");
        //    GUIStyle TextShadowStyle = GUI.skin.GetStyle("animationTextShadow");

        //    DoTextWithShadow(new Rect(35, 33, 59, 25), GUIContent(LoadingAnimation1Percentage + "%"), PercentageStyle, PercentageStyle.normal.textColor, TextShadowStyle.normal.textColor, new Vector2(0.0f, 1.0f));
        //    DoTextWithShadow(new Rect(35, 56, 59, 15), GUIContent("loading"), TextStyle, TextStyle.normal.textColor, TextShadowStyle.normal.textColor, new Vector2(0.0f, 1.0f));

        //    GUI.EndGroup();
        //}

        public static Rect ScaleRect(Rect rect, float scale)
        {
            scale = scale * 100;

            Rect newRect = new Rect(0, 0, 0, 0);
            newRect.x = Mathf.CeilToInt((rect.x / 100) * scale);
            newRect.y = Mathf.CeilToInt((rect.y / 100) * scale);
            newRect.width = Mathf.CeilToInt((rect.width / 100) * scale);
            newRect.height = Mathf.CeilToInt((rect.height / 100) * scale);

            return newRect;
        }

        public static void Image(Vector2 offset, Texture2D imageTexture)
        {
            Vector2 frameSize = new Vector2(imageTexture.width + 8, imageTexture.height + 8);

            GUI.BeginGroup(new Rect(offset.x, offset.y, frameSize.x, frameSize.y));
            GUI.Label(new Rect(0, 0, frameSize.x, frameSize.y), "", "imageFrame");
            GUI.DrawTexture(new Rect(4, 4, imageTexture.width, imageTexture.height), imageTexture);
            GUI.EndGroup();
        }

        public static void TextWithShadow(Rect rect, GUIContent content, GUIStyle style, Color txtColor, Color shadowColor, Vector2 direction)
        {
            GUIStyle backupStyle = new GUIStyle(style);

            style.normal.textColor = shadowColor;
            rect.x += direction.x;
            rect.y += direction.y;
            GUI.Label(rect, content, style);

            style.normal.textColor = txtColor;
            rect.x -= direction.x;
            rect.y -= direction.y;
            GUI.Label(rect, content, style);

            style = backupStyle;
        }

        public static void TextStyle1(Rect r, string text)
        {
            GUIStyle TextStyle = GUI.skin.GetStyle("textStyle1");
            GUIStyle TextStyleShadow = GUI.skin.GetStyle("textStyle1Shadow");

            TextWithShadow(r, new GUIContent(text), TextStyle, TextStyle.normal.textColor, TextStyleShadow.normal.textColor, new Vector2(2.0f, 1.0f));
        }
        public static void Label(Rect r, string text)
        {
            GUIStyle LabelStyle = GUI.skin.GetStyle("label");
            GUIStyle LabelShadowStyle = GUI.skin.GetStyle("labelTextShadow");

            TextWithShadow(r, new GUIContent(text), LabelStyle, LabelStyle.normal.textColor, LabelShadowStyle.normal.textColor, new Vector2(1.0f, 1.0f));
        }
        public static void Exclamation(Rect r, string text)
        {
            GUIStyle ExclamationStyle = GUI.skin.GetStyle("exclamationMark");
            //GUIStyle InfoStyle = GUI.skin.GetStyle("infoContainer");
            GUI.BeginGroup(r, GUI.skin.GetStyle("infoContainer"));
            GUIStyle LabelStyle = GUI.skin.GetStyle("infoText");
            GUIStyle LabelShadowStyle = GUI.skin.GetStyle("labelTextShadow");
            TextWithShadow(new Rect(0, 0, r.width, r.height), new GUIContent(text), LabelStyle, LabelStyle.normal.textColor, LabelShadowStyle.normal.textColor, new Vector2(1.0f, 1.0f));
            GUI.EndGroup();
        }
        public static void TextStyle2(Rect r, string text)
        {
            GUIStyle TextStyle = GUI.skin.GetStyle("textStyle2");
            GUIStyle TextStyleShadow = GUI.skin.GetStyle("textStyle2Shadow");

            TextWithShadow(r, new GUIContent(text), TextStyle, TextStyle.normal.textColor, TextStyleShadow.normal.textColor, new Vector2(2.0f, 1.0f));
        }

        public static void Separator(Vector2 offset)
        {
            GUI.Label(new Rect(offset.x, offset.y, 340, 16), "", "separator");
        }

        public static bool Toggle(Vector2 offset, bool toggle, string text)
        {
            GUIStyle ToggleTextStyle = GUI.skin.GetStyle("toggleText");
            GUIStyle ToggleTextShadowStyle = GUI.skin.GetStyle("toggleTextShadow");

            GUI.BeginGroup(new Rect(offset.x, offset.y, 349, 146));
            toggle = GUI.Toggle(new Rect(0, 0, 32, 32), toggle, "");

            TextWithShadow(new Rect(39, 2, 278, 32), new GUIContent(text), ToggleTextStyle, ToggleTextStyle.normal.textColor, ToggleTextShadowStyle.normal.textColor, new Vector2(2.0f, 1.0f));
            GUI.EndGroup();

            return toggle;
        }

        // Displays a vertical list of toggles and returns the index of the selected item.
        public static int ToggleList(Rect offset, int selected, string[] items)
        {
            // Keep the selected index within the bounds of the items array
            selected = (selected < 0) ? 0 : (selected >= items.Length ? items.Length - 1 : selected);

            // Get the radio toggles style
            GUIStyle radioStyle = GUI.skin.GetStyle("radioToggle");
            GUIStyle ToggleTextStyle = GUI.skin.GetStyle("toggleText");
            GUIStyle ToggleTextShadowStyle = GUI.skin.GetStyle("toggleTextShadow");

            // Get the toggles height
            float height = radioStyle.fixedHeight;
            float width = radioStyle.fixedWidth;

            GUI.BeginGroup(new Rect(offset.x, offset.y, offset.width, (height * items.Length) + height));
            GUILayout.BeginVertical();

            float offsetY = 0;
            float textOffsetX = 37;

            for (int i = 0; i < items.Length; i++)
            {
                // Display toggle. Get if toggle changed.
                bool change = GUI.Toggle(new Rect(0, offsetY, width, height), selected == i, "", radioStyle);

                TextWithShadow(new Rect(textOffsetX, (offsetY + 3), (offset.width - textOffsetX), height), new GUIContent(items[i]), ToggleTextStyle, ToggleTextStyle.normal.textColor, ToggleTextShadowStyle.normal.textColor, new Vector2(1.0f, 1.0f));

                // If changed, set selected to current index.
                if (change)
                    selected = i;

                // Increase the offset for the next toggle
                offsetY = offsetY + (height + 8);
            }

            GUILayout.EndVertical();
            GUI.EndGroup();

            // Return the currently selected item's index
            return selected;
        }

        public static void TextContainerTitle(Rect r, string text)
        {
            GUIStyle TextStyle = GUI.skin.GetStyle("textContainerTitle");

            TextWithShadow(r, new GUIContent(text), TextStyle, TextStyle.normal.textColor, TextStyle.hover.textColor, new Vector2(2.0f, 1.0f));
        }

        public static void TextContainerText(Rect r, string text)
        {
            GUIStyle TextStyle = GUI.skin.GetStyle("textContainerText");

            TextWithShadow(r, new GUIContent(text), TextStyle, TextStyle.normal.textColor, TextStyle.hover.textColor, new Vector2(2.0f, 1.0f));
        }
        public static bool Button(Rect r, string content)
        {
            GUIStyle ButtonTextStyle = GUI.skin.GetStyle("buttonText");
            GUIStyle backupStyle = new GUIStyle(ButtonTextStyle);
            GUIStyle ShadowStyle = GUI.skin.GetStyle("buttonTextShadow");

            Rect size = new Rect(0, 0, r.width, r.height);
            Rect buttonRect = new Rect(8, 8, (r.width - (8 * 2)), (r.height - (8 * 2)));

            GUI.BeginGroup(r);

            // Do the button, exclude the overflow from the size
            bool result = GUI.Button(buttonRect, "");

            // Get the colors for diferrent scenarios
            Color color = (buttonRect.Contains(Event.current.mousePosition) && Input.GetMouseButton(0)) ? ButtonTextStyle.active.textColor : (buttonRect.Contains(Event.current.mousePosition) ? ButtonTextStyle.hover.textColor : ButtonTextStyle.normal.textColor);
            Color colorShadow = (buttonRect.Contains(Event.current.mousePosition) && Input.GetMouseButton(0)) ? ShadowStyle.active.textColor : (buttonRect.Contains(Event.current.mousePosition) ? ShadowStyle.hover.textColor : ShadowStyle.normal.textColor);
            Vector2 direction = new Vector2(0.0f, 1.0f);

            // Do the text
            TextWithShadow(size, new GUIContent(content), ButtonTextStyle, color, colorShadow, direction);

            GUI.EndGroup();

            // Restore the color
            ButtonTextStyle.normal.textColor = backupStyle.normal.textColor;

            return result;
        }
        public static bool ButtonBig(Rect r, string content)
        {
            GUIStyle ButtonTextStyle = GUI.skin.GetStyle("buttonText1");
            GUIStyle backupStyle = new GUIStyle(ButtonTextStyle);
            GUIStyle ShadowStyle = GUI.skin.GetStyle("buttonTextShadow");

            Rect size = new Rect(0, 0, r.width, r.height);
            Rect buttonRect = new Rect(13, 13, (r.width - (13 * 2)), (r.height - (13 * 2)));

            GUI.BeginGroup(r);

            // Do the button, exclude the overflow from the size
            bool result = GUI.Button(buttonRect, "");

            // Get the colors for diferrent scenarios
            Color color = (buttonRect.Contains(Event.current.mousePosition) && Input.GetMouseButton(0)) ? ButtonTextStyle.active.textColor : (buttonRect.Contains(Event.current.mousePosition) ? ButtonTextStyle.hover.textColor : ButtonTextStyle.normal.textColor);
            Color colorShadow = (buttonRect.Contains(Event.current.mousePosition) && Input.GetMouseButton(0)) ? ShadowStyle.active.textColor : (buttonRect.Contains(Event.current.mousePosition) ? ShadowStyle.hover.textColor : ShadowStyle.normal.textColor);
            Vector2 direction = new Vector2(0.0f, 1.0f);

            // Do the text
            TextWithShadow(size, new GUIContent(content), ButtonTextStyle, color, colorShadow, direction);

            GUI.EndGroup();

            // Restore the color
            ButtonTextStyle.normal.textColor = backupStyle.normal.textColor;

            return result;
        }
    }
}
