using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpaceCommander
{

    public class MainMenu : MonoBehaviour
    {
    protected enum MenuWindow { Main, Levels, Options, About, Quit }
        public GUISkin Skin;
        private MenuWindow CurWin;
        UIWindowInfo[] Windows;
        private Vector2 scrollAboutPosition = Vector2.zero;
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
            //
            Vector2 W4x4 = UIUtil.GetWindow(4, 4);
            Vector2 W8x6 = UIUtil.GetWindow(8, 6);
            Vector2 W6x2 = UIUtil.GetWindow(6, 2);
            Windows[0] = new UIWindowInfo(new Rect(new Vector2((Screen.width - W4x4.x) / 2, ((Screen.height - W4x4.y) / 2)), W4x4));//main
            Windows[1] = new UIWindowInfo(new Rect(new Vector2((Screen.width - W6x2.x) / 2, ((Screen.height - W6x2.y) / 2)), W6x2));//question
            Windows[2] = new UIWindowInfo(new Rect(new Vector2((Screen.width - W8x6.x) / 2, ((Screen.height - W8x6.y) / 2)), W8x6));//options
            //
            this.gameObject.transform.Rotate(Vector3.up * 5 * Time.deltaTime);
        }
        void OnGUI()
        {
            GUI.skin = Skin;
            GUI.BeginGroup(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 100, 100, 100));
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
                        GUI.Window(2, Windows[2].rect, DrawOptionsW, "");
                        break;
                    }
                case MenuWindow.About:
                    {
                        GUI.Window(2, Windows[2].rect, DrawAboutW, "");
                        break;
                    }
                case MenuWindow.Quit:
                    {
                        GUI.Window(1, Windows[1].rect, DrawQuitW, "");
                        break;
                    }
            }
            GUI.EndGroup();
            UIUtil.Exclamation(new Rect(0, Screen.height - 70, 200, 50), "Jogo Deus");
            UIUtil.Exclamation(new Rect(Screen.width - 200, Screen.height - 70, 200, 50), "v. " + Application.version);
        }
        void DrawMainW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], "Space Comander");
            //GUI.color.a = window.UIAlpha;
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 100, 100, 200, 50), Global.Texts["Play"]))
            {
                CurWin = MenuWindow.Levels;
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 100, 150, 200, 50), Global.Texts["Options"]))
            {
                langChanged = false;
                CurWin = MenuWindow.Options;
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 100, 200, 200, 50), Global.Texts["About game"]))
            {
                CurWin = MenuWindow.About;
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 100, 250, 200, 50), Global.Texts["Quit"]))
            {
                CurWin = MenuWindow.Quit;
            }
        }
        void DrawLevelsW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], Global.Texts["Start level"]);
            //UIUtil.Label(new Rect(50, 10, 180, 43), "Выберите уровень");
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 100, 100, 200, 50), Global.Texts["Level"]+" 1"))
            {
                SceneManager.LoadScene(1);
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 100, 150, 200, 50), Global.Texts["Level"] + " 2"))
            {
                SceneManager.LoadScene(2);
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 100, 200, 200, 50), Global.Texts["Level"] + " 3"))
            {
                //Application.LoadLevel(3);
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 100, 250, 200, 50), Global.Texts["Back"]))
            {
                CurWin = MenuWindow.Main;
            }
        }
        void DrawOptionsW(int windowID)
        {
            float fBuffer;
            UIUtil.WindowTitle(Windows[windowID], Global.Texts["Options"]);

            GUI.BeginGroup(new Rect(Windows[windowID].CenterX - 170, 100, 340, 55));
            UIUtil.Label(new Rect(120, 0, 100, 20), Global.Texts["Sound"]);
            fBuffer = GUI.HorizontalSlider(new Rect(0, 40, 340, 13), Global.SoundLevel, 0.0f, 1f);
            if (Global.SoundLevel != fBuffer)
                Global.SoundLevel = fBuffer;
            GUI.EndGroup();

            GUI.BeginGroup(new Rect(Windows[windowID].CenterX - 170, 165, 340, 55));
            UIUtil.Label(new Rect(120, 0, 100, 20), Global.Texts["Music"]);
            fBuffer = GUI.HorizontalSlider(new Rect(0, 40, 340, 13), Global.MusicLevel, 0.0f, 1f);
            if (Global.MusicLevel != fBuffer)
                Global.MusicLevel = fBuffer;
            GUI.EndGroup();

            string[] radios = new string[2];
            radios[0] = "English";
            radios[1] = "Русский";
            int radioSelected = (int)Global.Localisation;
            GUI.BeginGroup(new Rect(77, 380, 100, 110));
            UIUtil.Label(new Rect(0, 0, 100, 20), Global.Texts["Language"]);
            radioSelected = UIUtil.ToggleList(new Rect(0, 40, 100, 74), radioSelected, radios);
            GUI.EndGroup();
            if (radioSelected != (int)Global.Localisation)
            {
                Global.Localisation = (Languages)radioSelected;
                langChanged = true;
            }

            if (Global.SettingsSaved)
            {
                if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - UIUtil.Scaled(100), Windows[windowID].Bottom - 100, UIUtil.Scaled(200), 50), Global.Texts["Back"]))
                {
                    CurWin = MenuWindow.Main;
                    if (langChanged)
                        Global.LoadTexts();
                }
            }
            else
            {
                if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - UIUtil.Scaled(100), Windows[windowID].Bottom - 100, UIUtil.Scaled(200), 50), Global.Texts["Save"]))
                {
                    Global.SaveSettings();
                }
            }
        }
        void DrawAboutW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], Global.Texts["About game"]);
            GUI.BeginGroup(new Rect(Windows[windowID].CenterX - UIUtil.Scaled(140), 100, UIUtil.Scaled(280), 150), GUI.skin.GetStyle("textContainer"));
            UIUtil.TextContainerTitle(new Rect(27, 10, UIUtil.Scaled(220), 20), Global.Texts["Development"]);
            UIUtil.TextContainerText(new Rect(27, 40, UIUtil.Scaled(220), 60), Global.Texts["Develop_content"]);
            GUI.EndGroup();
            Rect viewRect = new Rect(0, 0, UIUtil.Scaled(546), 600);
            scrollAboutPosition = GUI.BeginScrollView(new Rect(Windows[windowID].CenterX - UIUtil.Scaled(300), 260, UIUtil.Scaled(600), 230), scrollAboutPosition, viewRect);
            UIUtil.TextContainerTitle(new Rect(27, 10, UIUtil.Scaled(220), 20), Global.Texts["Story"]);
            UIUtil.TextContainerText(new Rect(27, 40, viewRect.width, viewRect.height), Global.Texts["Story_content"]);
            GUI.EndGroup();
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX + 130, Windows[windowID].Bottom - 100, 230, 50), Global.Texts["Developer page"]))
            {
                Application.OpenURL("https://vk.com/daniil.chikish");
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 100, Windows[windowID].Bottom - 100, 200, 50), Global.Texts["Back"]))
            {
                CurWin = MenuWindow.Main;
            }
        }
        void DrawQuitW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], Global.Texts["Quit_question"]);
            //UIUtil.Label(new Rect(50, 10, 180, 43), "Running in fear?");
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX + UIUtil.Scaled(10), 100, UIUtil.Scaled(180), 50), Global.Texts["Yes"]))
            {
                Application.Quit();
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - UIUtil.Scaled(190), 100, UIUtil.Scaled(180), 50), Global.Texts["No"]))
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
