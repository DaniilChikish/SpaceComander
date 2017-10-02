using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DeusUtility.UI;

namespace SpaceCommander
{

    public class MainMenu : MonoBehaviour
    {
        protected enum MenuWindow { Main, Levels, Options, About, Quit }
        public GUISkin Skin;
        public float scale;
        public Vector2 screenRatio;
        public Rect mainRect;
        private MenuWindow CurWin;
        UIWindowInfo[] Windows;
        private Vector2 scrollLevelsPosition = Vector2.zero;
        private Vector2 scrollAboutPosition = Vector2.zero;
        private Vector2 scrollSpeed = Vector2.zero;
        private const float scrollSpeedFactor = 0.5f;
        private const float scrollDrag = 1;
        GlobalController Global;
        bool langChanged;
        void Start()
        {
            Global = FindObjectOfType<GlobalController>();
            Time.timeScale = 1;
            CurWin = MenuWindow.Main;
            Windows = new UIWindowInfo[5];
            //Windows[5] = new SDWindowInfo(new Rect(0, Screen.height-100, 100, 100));//info
        }
        private void Update()
        {
            if (scrollSpeed.magnitude > 1)
                scrollSpeed -= scrollSpeed.normalized * scrollDrag * Time.deltaTime;
            else scrollSpeed = Vector2.zero;
            //
            ScaleScreen();
            //
            this.gameObject.transform.Rotate(Vector3.up * 5 * Time.deltaTime);
        }
        public void ScaleScreen()
        {
            screenRatio = UIUtil.GetRatio();

            if (Global.Settings.StaticProportion)
            {
                scale = Screen.width / (1280f / 1f);
                mainRect = new Rect(0, 0, Screen.width / scale, Screen.height / scale);
            }
            else
                mainRect = new Rect(0, 0, Screen.width, Screen.height);

            Windows[0] = new UIWindowInfo(UIUtil.GetRect(new Vector2(400, 400), PositionAnchor.Center, mainRect.size));//main
            Windows[1] = new UIWindowInfo(UIUtil.GetRect(new Vector2(1100, 600), PositionAnchor.Center, mainRect.size));//options
            Windows[2] = new UIWindowInfo(UIUtil.GetRect(new Vector2(600, 200), PositionAnchor.Center, mainRect.size));//question
        }

        void OnGUI()
        {
            GUI.skin = Skin;
            if (Global.Settings.StaticProportion &&scale != 1)
                GUI.matrix = Matrix4x4.Scale(Vector3.one * scale);
            GUI.BeginGroup(mainRect);
            switch (CurWin)
            {
                case MenuWindow.Main:
                    {
                        GUI.Window(0, Windows[0].rect, DrawMainW, "");
                        break;
                    }
                case MenuWindow.Levels:
                    {
                        GUI.Window(0, Windows[0].rect, DrawLevelsW, "");
                        break;
                    }
                case MenuWindow.Options:
                    {
                        GUI.Window(1, Windows[1].rect, DrawOptionsW, "");
                        break;
                    }
                case MenuWindow.About:
                    {
                        GUI.Window(1, Windows[1].rect, DrawAboutW, "");
                        break;
                    }
                case MenuWindow.Quit:
                    {
                        GUI.Window(2, Windows[2].rect, DrawQuitW, "");
                        break;
                    }
            }
            GUI.EndGroup();
            UIUtil.Exclamation(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.LeftDown, mainRect.size), "Jogo Deus");
            UIUtil.Exclamation(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.RightDown, mainRect.size), "v. " + Application.version);
        }
        void DrawMainW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], "Space Comander");
            //GUI.color.a = window.UIAlpha;
            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 100)), Global.Texts("Play")))
            {
                CurWin = MenuWindow.Levels;
            }
            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 150)), Global.Texts("Options")))
            {
                langChanged = false;
                CurWin = MenuWindow.Options;
            }
            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 200)), Global.Texts("About game")))
            {
                CurWin = MenuWindow.About;
            }
            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(0, -50)), Global.Texts("Quit")))
            {
                CurWin = MenuWindow.Quit;
            }
        }
        void DrawLevelsW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], Global.Texts("Start level"));
            //UIUtil.Label(new Rect(50, 10, 180, 43), "Выберите уровень");
            Rect scrollContent = new Rect(0, 0, 270, 50 * SceneManager.sceneCountInBuildSettings);
            Rect scrollView = UIUtil.GetRect(new Vector2(280, 180), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 110));
            scrollLevelsPosition += scrollSpeed;
            Vector2 speedBuff = UIUtil.MouseScroll(scrollLevelsPosition, scrollView, scrollSpeedFactor, scale) - scrollLevelsPosition;
            if (speedBuff != Vector2.zero)
                scrollSpeed = speedBuff;
            scrollLevelsPosition = GUI.BeginScrollView(scrollView, scrollLevelsPosition, scrollContent);
            for (int i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.Up, scrollContent.size, new Vector2(5, (i - 1) * 50 + 5)), Global.Texts("Level") + " " + i))
                {
                    SceneManager.LoadScene(i);
                }
            }
            GUI.EndScrollView();
            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(0, -50)), Global.Texts("Back")))
            {
                CurWin = MenuWindow.Main;
            }
        }
        void DrawOptionsW(int windowID)
        {
            float fBuffer;
            UIUtil.WindowTitle(Windows[windowID], Global.Texts("Options"));

            GUI.BeginGroup(UIUtil.GetRect(new Vector2(340, 55), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 100)));
            UIUtil.Label(new Rect(120, 0, 100, 20), Global.Texts("Sound"));
            fBuffer = GUI.HorizontalSlider(new Rect(0, 40, 340, 13), Global.Settings.SoundLevel, 0.0f, 100f);
            if (Global.Settings.SoundLevel != fBuffer)
                Global.Settings.SoundLevel = fBuffer;
            GUI.EndGroup();

            GUI.BeginGroup(UIUtil.GetRect(new Vector2(340, 55), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 165)));
            UIUtil.Label(new Rect(120, 0, 100, 20), Global.Texts("Music"));
            fBuffer = GUI.HorizontalSlider(new Rect(0, 40, 340, 13), Global.Settings.MusicLevel, 0.0f, 100f);
            if (Global.Settings.MusicLevel != fBuffer)
                Global.Settings.MusicLevel = fBuffer;
            GUI.EndGroup();

            GUI.BeginGroup(UIUtil.GetRect(new Vector2(500, 180), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 210)));
            UIUtil.TextStyle2(new Rect(0, 30, 500, 30), Global.Texts("UI Settings"));
            GUI.BeginGroup(new Rect(0, 60, 200, 145));
            UIUtil.TextStyle1(new Rect(10, 25, 180, 25), Global.Texts("Show unit Frame"));
            UIUtil.TextStyle1(new Rect(10, 50, 180, 25), Global.Texts("Show unit Icon"));
            UIUtil.TextStyle1(new Rect(10, 75, 180, 25), Global.Texts("Show unit Name"));
            UIUtil.TextStyle1(new Rect(10, 100, 180, 25), Global.Texts("Show unit Status"));
            GUI.EndGroup();

            UISettings UIbuff = new UISettings();
            GUI.BeginGroup(new Rect(200, 37, 100, 145));
            UIUtil.Label(new Rect(0, 0, 100, 25), Global.Texts("Allies"));
            UIbuff.ShowUnitFrame = UIUtil.Toggle(new Vector2(35, 40), Global.Settings.AliesUI.ShowUnitFrame, "");
            UIbuff.ShowUnitIcon = UIUtil.Toggle(new Vector2(35, 65), Global.Settings.AliesUI.ShowUnitIcon, "");
            UIbuff.ShowUnitName = UIUtil.Toggle(new Vector2(35, 90), Global.Settings.AliesUI.ShowUnitName, "");
            UIbuff.ShowUnitStatus = UIUtil.Toggle(new Vector2(35, 115), Global.Settings.AliesUI.ShowUnitStatus, "");
            GUI.EndGroup();
            if (!Global.Settings.AliesUI.Equals(UIbuff))
                Global.Settings.AliesUI = UIbuff;

            GUI.BeginGroup(new Rect(300, 37, 100, 145));
            UIUtil.Label(new Rect(0, 0, 100, 25), Global.Texts("Selected"));
            UIbuff.ShowUnitFrame = UIUtil.Toggle(new Vector2(35, 40), Global.Settings.SelectedUI.ShowUnitFrame, "");
            UIbuff.ShowUnitIcon = UIUtil.Toggle(new Vector2(35, 65), Global.Settings.SelectedUI.ShowUnitIcon, "");
            UIbuff.ShowUnitName = UIUtil.Toggle(new Vector2(35, 90), Global.Settings.SelectedUI.ShowUnitName, "");
            UIbuff.ShowUnitStatus = UIUtil.Toggle(new Vector2(35, 115), Global.Settings.SelectedUI.ShowUnitStatus, "");
            GUI.EndGroup();
            if (!Global.Settings.SelectedUI.Equals(UIbuff))
                Global.Settings.SelectedUI = UIbuff;

            GUI.BeginGroup(new Rect(400, 37, 100, 145));
            UIUtil.Label(new Rect(0, 0, 100, 25), Global.Texts("Enemys"));
            UIbuff.ShowUnitFrame = UIUtil.Toggle(new Vector2(35, 40), Global.Settings.EnemyUI.ShowUnitFrame, "");
            UIbuff.ShowUnitIcon = UIUtil.Toggle(new Vector2(35, 65), Global.Settings.EnemyUI.ShowUnitIcon, "");
            UIbuff.ShowUnitName = UIUtil.Toggle(new Vector2(35, 90), Global.Settings.EnemyUI.ShowUnitName, "");
            UIbuff.ShowUnitStatus = UIUtil.Toggle(new Vector2(35, 115), Global.Settings.EnemyUI.ShowUnitStatus, "");
            GUI.EndGroup();
            if (!Global.Settings.EnemyUI.Equals(UIbuff))
                Global.Settings.EnemyUI = UIbuff;
            GUI.EndGroup();
            {
                string[] radios = new string[2];
                radios[0] = "English";
                radios[1] = "Русский";
                int langueageRadioSelected = (int)Global.Settings.Localisation;
                GUI.BeginGroup(UIUtil.GetRect(new Vector2(150, 110), PositionAnchor.LeftDown, Windows[windowID].rect.size, new Vector2(100, -100)));
                UIUtil.Label(new Rect(0, 0, 150, 20), Global.Texts("Language"));
                langueageRadioSelected = UIUtil.ToggleList(new Rect(10, 40, 140, 74), langueageRadioSelected, radios);
                GUI.EndGroup();
                if (langueageRadioSelected != (int)Global.Settings.Localisation)
                {
                    Global.Settings.Localisation = (Languages)langueageRadioSelected;
                    langChanged = true;
                }
            }

            if (false)
            {
                string[] radios = new string[2];
                radios[0] = "Static size";
                radios[1] = "Static proportion";
                int screenRadioSelected;
                if (Global.Settings.StaticProportion)
                    screenRadioSelected = 1;
                else screenRadioSelected = 0;
                GUI.BeginGroup(UIUtil.GetRect(new Vector2(150, 110), PositionAnchor.RightDown, Windows[windowID].rect.size, new Vector2(-100, -100)));
                UIUtil.Label(new Rect(0, 0, 150, 20), Global.Texts("UI scale"));
                int screenRadioBuffer = UIUtil.ToggleList(new Rect(0, 40, 150, 74), screenRadioSelected, radios);
                GUI.EndGroup();
                if (screenRadioSelected != screenRadioBuffer)
                {
                    Global.Settings.StaticProportion = (screenRadioBuffer == 1);
                }
            }

            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(0, -50)), Global.Texts("Back")))
            {
                CurWin = MenuWindow.Main;
                if (langChanged)
                    Global.LoadTexts();
            }
            if (!Global.Settings.SettingsSaved)
            {
                if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(220, -50)), Global.Texts("Save")))
                {
                    Global.Settings.Save();
                }
            }
        }
        void DrawAboutW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], Global.Texts("About game"));
            GUI.BeginGroup(new Rect(Windows[windowID].CenterX - UIUtil.Scaled(140), 100, UIUtil.Scaled(280), 150), GUI.skin.GetStyle("textContainer"));
            UIUtil.TextContainerTitle(new Rect(27, 10, UIUtil.Scaled(220), 20), Global.Texts("Development"));
            UIUtil.TextContainerText(new Rect(27, 40, UIUtil.Scaled(220), 60), Global.Texts("Develop_content"));
            GUI.EndGroup();
            Rect scrollContent = new Rect(0, 0, 546, 400);
            Rect scrollView = UIUtil.GetRect(new Vector2(600, 230), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 260));

            scrollAboutPosition += scrollSpeed;
            Vector2 speedBuff = UIUtil.MouseScroll(scrollAboutPosition, scrollView, scrollSpeedFactor, scale) - scrollAboutPosition;
            if (speedBuff != Vector2.zero)
                scrollSpeed = speedBuff;

            scrollAboutPosition = GUI.BeginScrollView(scrollView, scrollAboutPosition, scrollContent);
            {
                UIUtil.TextContainerTitle(new Rect(27, 10, UIUtil.Scaled(220), 20), Global.Texts("Story"));
                UIUtil.TextContainerText(new Rect(27, 40, scrollContent.width, scrollContent.height), Global.Texts("Story_content"));
            }
            GUI.EndScrollView();
            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(230, 50), PositionAnchor.RightDown, Windows[windowID].rect.size, new Vector2(-50, -50)), Global.Texts("Developer page")))
            {
                Application.OpenURL("https://www.linkedin.com/in/%D0%B4%D0%B0%D0%BD%D0%B8%D0%B8%D0%BB-%D1%87%D0%B8%D0%BA%D0%B8%D1%88-5809a2108/");
            }
            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(0, -50)), Global.Texts("Back")))
            {
                CurWin = MenuWindow.Main;
            }
        }
        void DrawQuitW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], Global.Texts("Quit_question"));
            //UIUtil.Label(new Rect(50, 10, 180, 43), "Running in fear?");
            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(180, 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(100, -50)), Global.Texts("Yes")))
            {
                Application.Quit();
            }
            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(180, 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(-100, -50)), Global.Texts("No")))
            {
                CurWin = MenuWindow.Main;
            }
        }
        //void DrawInfo(int windowID)
        //{
        //    UIUtil.Exclamation(new Rect(0, Screen.height - 200, 100, 200), "Jogo Deus");
        //}
    }
}
