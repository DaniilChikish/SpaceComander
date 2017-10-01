using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DeusUtility.UI;

namespace SpaceCommander
{
    public class HUDBase : MonoBehaviour
    {
        protected enum PauseWindow { Start, Main, Restart, Options, About, Quit, Victory, Defeat }
        private PauseWindow CurWin;
        public Rect mainRect;
        public Vector2 screenRatio;
        public float scale;
        public GUISkin Skin;
        public Texture2D VictoryBanner;
        public Texture2D DefeatBanner;
        public Vector2 VictoryBannerSize;
        public Texture2D PauseButtonBox;
        private Vector4 pauseButtonBoxBorder;
        private Vector2 pauseButtonBoxSize;
        private bool Pause;
        private UIWindowInfo[] Windows;
        GlobalController Global;
        // Use this for initialization
        void Start()
        {
            //Debug.Log("HUD started");
            Global = FindObjectOfType<GlobalController>();
            VictoryBannerSize.x = 1784 / 2;
            VictoryBannerSize.y = 758 / 2;
            pauseButtonBoxSize.x = 108;
            pauseButtonBoxSize.y = 50;
            pauseButtonBoxBorder.x = pauseButtonBoxSize.x * 4 / 88;
            pauseButtonBoxBorder.y = pauseButtonBoxSize.y * 8 / 40;
            pauseButtonBoxBorder.z = pauseButtonBoxSize.x * 4 / 88;
            pauseButtonBoxBorder.w = pauseButtonBoxSize.y * 8 / 40;
            Windows = new UIWindowInfo[5];
            CurWin = PauseWindow.Start;
            Stop();
        }

        // Update is called once per frame
        void Update()
        {
            //
            if (Global.Settings.StaticProportion)
            {
                scale = Screen.width / (1280f / 1f);
                mainRect = new Rect(0, 0, Screen.width / scale, Screen.height / scale);
            }
            else
                mainRect = new Rect(0, 0, Screen.width, Screen.height);


            Vector2 W4x4 = new Vector2(400, 400);
            Vector2 W8x6 = new Vector2(800, 600);
            Vector2 W6x2 = new Vector2(600, 200);
            Vector2 W6x2f5 = new Vector2(600, 250f);
            Windows[0] = new UIWindowInfo(UIUtil.GetRect(W4x4, PositionAnchor.Center, mainRect.size));//main
            Windows[1] = new UIWindowInfo(UIUtil.GetRect(W6x2f5, PositionAnchor.Center, mainRect.size));//question
            Windows[2] = new UIWindowInfo(UIUtil.GetRect(W8x6, PositionAnchor.Center, mainRect.size));//options
            Windows[3] = new UIWindowInfo(UIUtil.GetRect(W6x2, PositionAnchor.Center, mainRect.size, new Vector2(0, 120)));//victory
            //

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (Pause)
                    Continue();
                else Stop();
            }
        }

        private void OnGUI()
        {
            GUI.skin = Skin;
            if (Global.Settings.StaticProportion && scale != 1)
                GUI.matrix = Matrix4x4.Scale(Vector3.one * scale);
            int victory = Global.CheckVictory();
            if (victory == 1)
                Victory();
            else if (victory == -1)
                Defeat();
            else
            {
                DrawPauseButton();
                if (Pause)
                {
                    GUI.BeginGroup(mainRect);
                    switch (CurWin)
                    {
                        case PauseWindow.Main:
                            {
                                GUI.Window(0, Windows[0].rect, DrawPauseW, "");
                                break;
                            }
                        case PauseWindow.Restart:
                            {
                                GUI.Window(1, Windows[1].rect, DrawRestartW, "");
                                break;
                            }
                        case PauseWindow.Options:
                            {
                                GUI.Window(2, Windows[2].rect, DrawOptionsW, "");
                                break;
                            }
                        case PauseWindow.About:
                            {
                                GUI.Window(2, Windows[2].rect, DrawAboutW, "");
                                break;
                            }
                        case PauseWindow.Start:
                            {
                                GUI.Window(2, Windows[2].rect, DrawStartW, "");
                                break;
                            }
                        case PauseWindow.Quit:
                            {
                                GUI.Window(1, Windows[1].rect, DrawQuitW, "");
                                break;
                            }
                    }
                    GUI.EndGroup();
                    UIUtil.Exclamation(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.LeftDown, mainRect.size), "Jogo Deus");
                    UIUtil.Exclamation(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.RightDown, mainRect.size), "v. " + Application.version);
                }
            }
        }
        private void Victory()
        {
            CurWin = PauseWindow.Victory;
            GUI.DrawTexture(UIUtil.GetRect(VictoryBannerSize, PositionAnchor.Center, mainRect.size, new Vector2(0, -100)), VictoryBanner);
            GUI.Window(3, Windows[3].rect, DrawVictoryQW, "");
        }
        void DrawVictoryQW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], Global.Texts("Good job!"));
            //UIUtil.Label(new Rect(50, 10, 180, 43), "Running in fear?");
            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(180, 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(100, -50)), Global.Texts("Next mission")))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(180, 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(-100, -50)), Global.Texts("Main menu")))
            {
                SceneManager.LoadScene(0);
            }
        }
        private void Defeat()
        {
            CurWin = PauseWindow.Defeat;
            GUI.DrawTexture(UIUtil.GetRect(VictoryBannerSize, PositionAnchor.Center, mainRect.size, new Vector2(0, -100)), DefeatBanner);
            GUI.Window(3, Windows[3].rect, DrawDefeatQW, "");
        }
        void DrawDefeatQW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], Global.Texts("You lose!"));
            //UIUtil.Label(new Rect(50, 10, 180, 43), "Running in fear?");
            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(180, 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(100, -50)), Global.Texts("Restart")))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(180, 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(-100, -50)), Global.Texts("Main menu")))
            {
                SceneManager.LoadScene(0);
            }
        }
        private void DrawPauseW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], Global.Texts("Pause"));
            //GUI.color.a = window.UIAlpha;
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 100, 100, 200, 50), Global.Texts("Restart")))
            {
                CurWin = PauseWindow.Restart;
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 100, 150, 200, 50), Global.Texts("Options")))
            {
                CurWin = PauseWindow.Options;
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 100, 200, 200, 50), Global.Texts("Briefing")))
            {
                CurWin = PauseWindow.About;
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 100, 250, 200, 50), Global.Texts("Exit mission")))
            {
                CurWin = PauseWindow.Quit;
            }
        }

        private void DrawPauseButton()
        {
            GUI.BeginGroup(new Rect(new Vector2(0, 0), pauseButtonBoxSize));
            GUI.DrawTexture(new Rect(new Vector2(0, 0), pauseButtonBoxSize), PauseButtonBox);
            if (!Pause)
            {
                if (UIUtil.Button(new Rect(pauseButtonBoxBorder.x, pauseButtonBoxBorder.y,
                    pauseButtonBoxSize.x - pauseButtonBoxBorder.x - pauseButtonBoxBorder.z,  //width
                    pauseButtonBoxSize.y - pauseButtonBoxBorder.y - pauseButtonBoxBorder.w), //height
                    Global.Texts("Pause")))
                {
                    //Debug.Log("Pause");
                    Stop();
                }
            }
            else
            {
                if (UIUtil.Button(new Rect(pauseButtonBoxBorder.x, pauseButtonBoxBorder.y,
                    pauseButtonBoxSize.x - pauseButtonBoxBorder.x - pauseButtonBoxBorder.z,  //width
                    pauseButtonBoxSize.y - pauseButtonBoxBorder.y - pauseButtonBoxBorder.w), //height
                    Global.Texts("Continue")))
                {
                    //Debug.Log("Continue");
                    Continue();
                }
            }
            GUI.EndGroup();
        }

        private void Stop()
        {
            Time.timeScale = 0;
            Pause = true;
        }
        private void Continue()
        {
            Time.timeScale = 1;
            Pause = false;
            if (CurWin == PauseWindow.Restart || CurWin == PauseWindow.Quit || CurWin == PauseWindow.Start)
                CurWin = PauseWindow.Main;
        }

        void DrawOptionsW(int windowID)
        {
            float fBuffer;
            UIUtil.WindowTitle(Windows[windowID], Global.Texts("Options"));

            GUI.BeginGroup(new Rect(Windows[windowID].CenterX - 170, 100, 340, 55));
            UIUtil.Label(new Rect(120, 0, 100, 20), Global.Texts("Sound"));
            fBuffer = GUI.HorizontalSlider(new Rect(0, 40, 340, 13), Global.Settings.SoundLevel, 0.0f, 1f);
            if (Global.Settings.SoundLevel != fBuffer)
                Global.Settings.SoundLevel = fBuffer;
            GUI.EndGroup();

            GUI.BeginGroup(new Rect(Windows[windowID].CenterX - 170, 165, 340, 55));
            UIUtil.Label(new Rect(120, 0, 100, 20), Global.Texts("Music"));
            fBuffer = GUI.HorizontalSlider(new Rect(0, 40, 340, 13), Global.Settings.MusicLevel, 0.0f, 1f);
            if (Global.Settings.MusicLevel != fBuffer)
                Global.Settings.MusicLevel = fBuffer;
            GUI.EndGroup();

            if (Screen.width != 1280)
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

            if (Global.Settings.SettingsSaved)
            {
                if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - UIUtil.Scaled(100), Windows[windowID].Bottom - 100, UIUtil.Scaled(200), 50), Global.Texts("Back")))
                {
                    CurWin = PauseWindow.Main;
                }
            }
            else
            {
                if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - UIUtil.Scaled(100), Windows[windowID].Bottom - 100, UIUtil.Scaled(200), 50), Global.Texts("Save")))
                {
                    Global.Settings.Save();
                }
            }
        }
        void DrawAboutW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], Global.MissionName);
            Vector2 W6x4 = new Vector2(600, 400);
            GUI.BeginGroup(new Rect(new Vector2(Windows[windowID].CenterX - (W6x4.x / 2), 100), W6x4), GUI.skin.GetStyle("textContainer"));
            UIUtil.TextContainerTitle(new Rect(27, 10, UIUtil.Scaled(220), 20), Global.Texts("Targets"));
            UIUtil.TextContainerText(new Rect(27, 40, UIUtil.Scaled(546), 60), Global.MissionBrief);
            GUI.EndGroup();
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - UIUtil.Scaled(90), Windows[windowID].Bottom - 100, UIUtil.Scaled(180), 50), Global.Texts("Back")))
            {
                CurWin = PauseWindow.Main;
            }
        }
        void DrawStartW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], Global.MissionName);
            Vector2 W6x4 = new Vector2(600, 400);
            GUI.BeginGroup(new Rect(new Vector2(Windows[windowID].CenterX - (W6x4.x / 2), 100), W6x4), GUI.skin.GetStyle("textContainer"));
            UIUtil.TextContainerTitle(new Rect(27, 10, UIUtil.Scaled(220), 20), Global.Texts("Targets"));
            UIUtil.TextContainerText(new Rect(27, 40, UIUtil.Scaled(546), 60), Global.MissionBrief);
            GUI.EndGroup();
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - UIUtil.Scaled(90), Windows[windowID].Bottom - 100, UIUtil.Scaled(180), 50), Global.Texts("Start")))
            {
                CurWin = PauseWindow.Main;
                Continue();
            }
        }
        void DrawRestartW(int windowID)
        {
            int ansver = Question(windowID, Global.Texts("Restart_question"), Global.Texts("Restart_discription"), Global.Texts("Yes"), Global.Texts("No"));
            if (ansver == 1)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            else if (ansver == -1)
                CurWin = PauseWindow.Main;
        }
        void DrawQuitW(int windowID)
        {
            int ansver = Question(windowID, Global.Texts("ExitMission_question"), Global.Texts("ExitMission_discription"), Global.Texts("Yes"), Global.Texts("No"));
            if (ansver == 1)
                SceneManager.LoadScene(0);
            else if (ansver == -1)
                CurWin = PauseWindow.Main;
        }
        int Question(int windowID, string title, string text, string var1, string var2)
        {
            UIUtil.WindowTitle(Windows[windowID], title);
            UIUtil.Label(new Rect(100, 100, UIUtil.Scaled(400), 43), text);
            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(180, 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(100, -50)), var1))
            {
                return 1;
            }
            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(180, 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(-100, -50)), var2))
            {
                return -1;
            }
            return 0;
        }
        public void ShowExclamation(string text)
        {
            GUI.skin = Skin;
            if (Global.Settings.StaticProportion && scale != 1)
                GUI.matrix = Matrix4x4.Scale(Vector3.one * scale);
            UIUtil.Exclamation(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.Up, mainRect.size, new Vector2(0, 10)), text);
        }
    }
}//