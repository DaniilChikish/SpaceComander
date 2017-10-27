using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using UnityEngine;
namespace DeusUtility.UI
{
    public enum ScreenOrientation { Landscape, Portrait }
    public enum PositionAnchor { Up, Down, Left, Right, Center, LeftUp, LeftDown, RightUp, RightDown }
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
    /**
     * Фасад-утилиты и специализированные математические преобразования для отрисовки UI элеменов.
     * **/
    public static class UIUtil
    {
        public static float Scaled(float x)
        {
            if (Screen.height > 800)
                return x;
            else return x * 0.8f;
        }
        //public static Vector2 GetRectSize(float w, float h)
        //{
        //    if (Screen.width > Screen.height)
        //        if (Screen.height > 800)
        //            return new Vector2(w * 100, h * 100);
        //        else
        //            return new Vector2(w * 80, h * 80);
        //    else
        //                    if (Screen.height > 800)
        //        return new Vector2(w * 100, h * 100);
        //    else
        //        return new Vector2(w * 80, h * 80);
        //}
        public static Vector2 GetRatio()
        {
            return GetRatio(new Vector2(Screen.width, Screen.height));
        }
        /* Возвращает Vector2 со стандартной пропорцией экрана по заданному разширению */
        public static Vector2 GetRatio(Vector2 rectSize)
        {
            Vector2 rectProp = new Vector2();
            double ratio = rectSize.x / rectSize.y;
            {
                if (Math.Round(ratio, 5) == 1.77778) // 16x9
                {
                    rectProp.x = 16;
                    rectProp.y = 9;
                }
                else if (Math.Round(ratio, 5) == 1.6) // 16x10
                {
                    rectProp.x = 16;
                    rectProp.y = 10;
                }
                else if (Math.Round(ratio, 5) == 1.66667) // 5x3
                {
                    rectProp.x = 5;
                    rectProp.y = 3;
                }
                else if (Math.Round(ratio, 5) == 1.33334) // 4x3
                {
                    rectProp.x = 4;
                    rectProp.y = 3;
                }
                else if (Math.Round(ratio, 5) == 0.5625) // 9x16
                {
                    rectProp.x = 9;
                    rectProp.y = 16;
                }
                else if (Math.Round(ratio, 5) == 0.625) // 10x16
                {
                    rectProp.x = 10;
                    rectProp.y = 16;
                }
                else if (Math.Round(ratio, 5) == 0.6) // 3x5
                {
                    rectProp.x = 3;
                    rectProp.y = 5;
                }
                else if (Math.Round(ratio, 5) == 0.75) // 3x4
                {
                    rectProp.x = 3;
                    rectProp.y = 4;
                }
                else //default
                {
                    if (ratio > 1)
                    {
                        rectProp.x = 16;
                        rectProp.y = 9;
                    }
                    else
                    {
                        rectProp.x = 9;
                        rectProp.y = 16;
                    }
                }
            }
            return rectProp;
        }
        public static float GetProportionFactor()
        {
            return GetProportionFactor(new Vector2(Screen.width, Screen.height));
        }
        public static float GetProportionFactor(Vector2 etalon)
        {
            return GetProportionFactor(new Vector2(Screen.width, Screen.height), GetRatio());
        }
        public static float GetProportionFactor(Vector2 scaled, Vector2 rectProp)
        {
            if (scaled.x > scaled.y)
                return scaled.y / rectProp.y;
            else
                return scaled.x / rectProp.x;
        }
        public static Vector2 GetRectSize(float w, float h)
        {
            float prop = GetProportionFactor();
            return new Vector2(w * prop, h * prop);
        }
        public static Vector2 GetRectSize(Vector2 size, Vector2 parent)
        {
            float prop = GetProportionFactor(parent);
            return new Vector2(size.x * prop, size.y * prop);
        }
        public static Rect GetRect(Vector2 size, PositionAnchor anchor)
        {
            return GetRect(size, anchor, new Vector2(Screen.width, Screen.height), Vector2.zero);
        }
        public static Rect GetRect(Vector2 size, PositionAnchor anchor, Vector2 parent)
        {
            return GetRect(size, anchor, parent, Vector2.zero);
        }
        /* Возвращает Rect окна с привязкой к размерам родительского и относительной позиции */ 
        public static Rect GetRect(Vector2 size, PositionAnchor anchor, Vector2 parent, Vector2 positionRelAnchor)
        {
            Vector2 position = new Vector2();
            switch (anchor)
            {
                case PositionAnchor.Center:
                    {
                        position.x = (parent.x - size.x) / 2 + positionRelAnchor.x;
                        position.y = (parent.y - size.y) / 2 + positionRelAnchor.y;
                        break;
                    }
                case PositionAnchor.Down:
                    {
                        position.x = (parent.x - size.x) / 2 + positionRelAnchor.x;
                        position.y = (parent.y - size.y) + positionRelAnchor.y;
                        break;
                    }
                case PositionAnchor.Up:
                    {
                        position.x = (parent.x - size.x) / 2 + positionRelAnchor.x;
                        position.y = positionRelAnchor.y;
                        break;
                    }
                case PositionAnchor.Left:
                    {
                        position.x = positionRelAnchor.x;
                        position.y = (parent.y - size.y) / 2 + positionRelAnchor.y;
                        break;
                    }
                case PositionAnchor.Right:
                    {
                        position.x = (parent.x - size.x) + positionRelAnchor.x;
                        position.y = (parent.y - size.y) / 2 + positionRelAnchor.y;
                        break;
                    }
                case PositionAnchor.LeftUp:
                    {
                        position.x = positionRelAnchor.x;
                        position.y = positionRelAnchor.y;
                        break;
                    }
                case PositionAnchor.LeftDown:
                    {
                        position.x = positionRelAnchor.x;
                        position.y = (parent.y - size.y) + positionRelAnchor.y;
                        break;
                    }
                case PositionAnchor.RightUp:
                    {
                        position.x = (parent.x - size.x) + positionRelAnchor.x;
                        position.y = positionRelAnchor.y;
                        break;
                    }
                case PositionAnchor.RightDown:
                    {
                        position.x = (parent.x - size.x) + positionRelAnchor.x;
                        position.y = (parent.y - size.y) + positionRelAnchor.y;
                        break;
                    }
            }
            return new Rect(position, size);
        }
        //...//
        public static bool InArea(Vector2 position, Rect area)
        {
            return InArea(position, area, 1f);
        }

        public static bool InArea(Vector2 position, Rect area, float GuiScale)
        {
            position.y = Screen.height - position.y;
            position = position / GuiScale;
            if ((position.x > area.position.x &&
                 position.y > area.position.y) &&
                (position.x < area.position.x + area.size.x &&
                 position.y < area.position.y + area.size.y))
                return true;
            else return false;
        }
        public static Vector2 TouchScroll(Vector2 scrollPosition, Rect area)
        {
            return TouchScroll(scrollPosition, area, 1f, 1f);
        }
        public static Vector2 TouchScroll(Vector2 scrollPosition, Rect area, float speedFactor)
        {
            return TouchScroll(scrollPosition, area, speedFactor, 1f);
        }

        public static Vector2 TouchScroll(Vector2 scrollPosition, Rect area, float speedFactor, float GuiScale)
        {
            if (Input.touches.Length > 0)
            {
                Touch t = Input.GetTouch(0);
                Vector2 outp = scrollPosition;
                if (InArea(t.position, area, GuiScale))
                    outp += (t.deltaPosition * speedFactor);
                return outp;
            }
            else return scrollPosition;
        }
        public static Vector2 MouseScroll(Vector2 scrollPosition, Rect area)
        {
            return MouseScroll(scrollPosition, area, 1f, 1f);
        }
        public static Vector2 MouseScroll(Vector2 scrollPosition, Rect area, float speedFactor)
        {
            return MouseScroll(scrollPosition, area, speedFactor, 1f);
        }
        public static Vector2 MouseScroll(Vector2 scrollPosition, Rect area, float speedFactor, float GuiScale)
        {
            Vector2 outp = scrollPosition;
            if (InArea(Input.mousePosition, area, GuiScale))
                outp += (Input.mouseScrollDelta * speedFactor);
            return outp;
        }
        public static int GetAndroidTouchKeyboardHeight()
        {
            using (AndroidJavaClass UnityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject View = UnityClass.GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mUnityPlayer").Call<AndroidJavaObject>("getView");

                using (AndroidJavaObject Rct = new AndroidJavaObject("android.graphics.Rect"))
                {
                    View.Call("getWindowVisibleDisplayFrame", Rct);

                    return Screen.height - Rct.Call<int>("height");
                }
            }
        }
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
        public static void Image(Vector2 offset, Vector2 size, Texture2D imageTexture)
        {
            Vector2 frameSize = new Vector2(size.x + 8, size.y + 8);

            GUI.BeginGroup(new Rect(offset.x, offset.y, frameSize.x, frameSize.y));
            GUI.Label(new Rect(0, 0, frameSize.x, frameSize.y), "", "imageFrame");
            GUI.DrawTexture(new Rect(4, 4, size.x, size.y), imageTexture);
            GUI.EndGroup();
        }
        public static void Image(Rect frame, Texture2D imageTexture)
        {
            Vector2 size = new Vector2(frame.width - 8, frame.height - 8);

            GUI.BeginGroup(frame);
            GUI.Label(new Rect(0, 0, frame.width, frame.height), "", "imageFrame");
            GUI.DrawTexture(new Rect(4, 4, size.x, size.y), imageTexture);
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
        public static Texture2D ProgressUpdate(float progress, Texture2D tex)
        {
            Texture2D thisTex = new Texture2D(tex.width, tex.height);
            Vector2 centre = new Vector2(Mathf.Ceil(thisTex.width / 2f), Mathf.Ceil(thisTex.height / 2f)); //find the centre pixel
            for (int y = 0; y < thisTex.height; y++)
            {
                for (int x = 0; x < thisTex.width; x++)
                {
                    var angle = Mathf.Atan2(x - centre.x, y - centre.y) * Mathf.Rad2Deg; //find the angle between the centre and this pixel (between -180 and 180)
                    if (angle < 0)
                    {
                        angle += 360; //change angles to go from 0 to 360
                    }
                    var pixColor = tex.GetPixel(x, y);
                    if (angle <= progress * 360.0)
                    { //if the angle is less than the progress angle blend the overlay colour
                        pixColor = new Color(0, 0, 0, 0);
                        thisTex.SetPixel(x, y, pixColor);
                    }
                    else
                    {
                        thisTex.SetPixel(x, y, pixColor);
                    }
                }
            }
            thisTex.Apply(); //apply the cahnges we made to the texture
            return thisTex;
        }
        public static Rect TransformBar(Rect origin, float progress)
        {
            Rect outp = new Rect(origin);
            outp.width = outp.width * progress;
            return outp;
        }
        public static Vector3 WorldToScreenFrame(Vector3 worldPosition, float border, out bool outOfBorder)
        {
            outOfBorder = false;
            bool outOfPlane = false;
            Vector3 crd = Camera.main.WorldToScreenPoint(worldPosition);
            crd.y = Screen.height - crd.y;

            if (crd.x > Screen.width - border || crd.x < border || crd.y > Screen.height - (border - 35) || crd.y < border)
                outOfBorder = true;
            if (crd.z < 0)
                outOfPlane = true;
            //crd.z = 0;

            if (outOfBorder || outOfPlane)
            {
                Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2);
                Vector3 originC = crd - center;
                Vector3 vC = originC;
                float a = center.x - border;
                float b = center.y - border;
                float r = 0;

                //float comX = a, comY = b;

                if (vC.x < 0) vC.x = -vC.x;
                if (vC.y < 0) vC.y = -vC.y;
                if (vC.x / vC.y > a / b || vC.x / vC.y < -(a / b))
                {
                    r = vC.magnitude * a / vC.x;
                }
                else
                {
                    r = vC.magnitude * b / vC.y;
                }
                crd = center + originC.normalized * r;
            }
            if (outOfPlane)
            {
                crd.x = Screen.width - crd.x;
                crd.y = Screen.height - border;
                outOfBorder = true;
            }
            return crd;
        }
        public static Vector3 WorldToScreenCircle(Vector3 worldPosition, float border, out bool outOfBorder)
        {
            outOfBorder = false;
            bool outOfPlane = false;
            Vector3 crd = Camera.main.WorldToScreenPoint(worldPosition);
            crd.y = Screen.height - crd.y;

            Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2);
            Vector3 originC = crd - center;
            Vector3 vC = originC;
            float a = center.x - border;
            float b = center.y - border;
            float r = b;

            if (vC.x < 0) vC.x = -vC.x;
            if (vC.y < 0) vC.y = -vC.y;
            //float e2 = 1f - (b * b) / (a * a);
            //float angle = Vector3.Angle(Vector3.ProjectOnPlane(vC, Vector3.forward), Vector3.right);
            //r = b / (1 - (e2 * Mathf.Cos(angle) * Mathf.Cos(angle)));

            if (Vector3.ProjectOnPlane(vC, Vector3.forward).magnitude > r)
                outOfBorder = true;
            if (crd.z < 0)
                outOfPlane = true;

            if (outOfBorder || outOfPlane)
            {
                crd = center + Vector3.ProjectOnPlane(originC, Vector3.forward).normalized * r;
            }
            if (outOfPlane)
            {
                crd.x = Screen.width - crd.x;
                crd.y = Screen.height - crd.y;
                outOfBorder = true;
            }
            return crd;
        }

    }
    /**
     * Утилита для парсинга входных строк
     * **/
    public static class ValidString
    {
        public static string NumberInt(string text)
        {
            if (text == "")
                return "0";
            else
                return Regex.Replace(text, "[\\D]", "");
        }
        /* Удаляет из строки символы кроме цифр и преобразует в целое без знака */
        public static int FormatUnsigned(string text)
        {
            int outp;
            if (text == "")
                outp = 0;
            else
            {
                string outpS = Regex.Replace(text, "[\\D]", "");
                try
                {
                    outp = Convert.ToInt32(outpS);
                }
                catch (OverflowException)
                {
                    outp = Int32.MaxValue;
                }
            }
            return outp;
        }
        public static int FormatUnsignedRange(string text, int minVal, int maxVal)
        {
            int outp = FormatUnsigned(text);
            if (outp > maxVal)
                return maxVal;
            else if (outp < minVal)
                return minVal;
            else return outp;
        }
        public static string ReplaceChar(string input, char fromC, char toC)
        {
            string outp = "";
            for (int i = 0; i < input.Length; i++)
                if (input[i] == fromC)
                    outp += toC;
                else outp += input[i];
            return outp;
        }
    }
}
