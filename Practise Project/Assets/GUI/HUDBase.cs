using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpaceCommander
{

    public class HUDBase : MonoBehaviour
    {
        protected enum PauseWindow { Start, Main, Restart, Options, About, Quit, Victory, Defeat }
        private PauseWindow CurWin;
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
            Vector2 W4x4 = UIUtil.GetWindow(4, 4);
            Vector2 W8x6 = UIUtil.GetWindow(8, 6);
            Vector2 W6x2 = UIUtil.GetWindow(6, 2);
            Vector2 W6x2f5 = UIUtil.GetWindow(6, 2.5f);
            Windows[0] = new UIWindowInfo(new Rect(new Vector2((Screen.width - W4x4.x) / 2, ((Screen.height - W4x4.y) / 2)), W4x4));//main
            Windows[1] = new UIWindowInfo(new Rect(new Vector2((Screen.width - W6x2f5.x) / 2, ((Screen.height - W6x2f5.y) / 2)), W6x2f5));//question
            Windows[2] = new UIWindowInfo(new Rect(new Vector2((Screen.width - W8x6.x) / 2, ((Screen.height - W8x6.y) / 2)), W8x6));//options
            Windows[3] = new UIWindowInfo(new Rect(new Vector2((Screen.width - W6x2.x) / 2, (Screen.height - W6x2.y) / 2 + 100), W6x2));//victory
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
                    GUI.skin = Skin;
                    GUI.BeginGroup(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 100, 100, 100));
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
                    UIUtil.Exclamation(new Rect(0, Screen.height - 70, 200, 50), "Jogo Deus");
                    UIUtil.Exclamation(new Rect(Screen.width - 200, Screen.height - 70, 200, 50), "v. " + Application.version);
                }
            }
        }
        private void Victory()
        {
            CurWin = PauseWindow.Victory;
            GUI.DrawTexture(new Rect((Screen.width - VictoryBannerSize.x) / 2, (Screen.height - VictoryBannerSize.y) / 2 - 200, VictoryBannerSize.x, VictoryBannerSize.y), VictoryBanner);
            GUI.Window(3, Windows[3].rect, DrawVictoryQW, "");
        }
        void DrawVictoryQW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], Global.Texts["Good job!"]);
            //UIUtil.Label(new Rect(50, 10, 180, 43), "Running in fear?");
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX + 10, 100, 180, 50), Global.Texts["Next mission"]))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 190, 100, 180, 50), Global.Texts["Main menu"]))
            {
                SceneManager.LoadScene(0);
            }
        }
        private void Defeat()
        {
            CurWin = PauseWindow.Defeat;
            GUI.DrawTexture(new Rect((Screen.width - VictoryBannerSize.x) / 2, (Screen.height - VictoryBannerSize.y) / 2 - 200, VictoryBannerSize.x, VictoryBannerSize.y), DefeatBanner);
            GUI.Window(3, Windows[3].rect, DrawDefeatQW, "");
        }
        void DrawDefeatQW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], Global.Texts["You lose!"]);
            //UIUtil.Label(new Rect(50, 10, 180, 43), "Running in fear?");
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX + 10, 100, 180, 50), Global.Texts["Restart"]))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 190, 100, 180, 50), Global.Texts["Main menu"]))
            {
                SceneManager.LoadScene(0);
            }
        }
        private void DrawPauseW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], Global.Texts["Pause"]);
            //GUI.color.a = window.UIAlpha;
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 100, 100, 200, 50), Global.Texts["Restart"]))
            {
                CurWin = PauseWindow.Restart;
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 100, 150, 200, 50), Global.Texts["Options"]))
            {
                CurWin = PauseWindow.Options;
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 100, 200, 200, 50), Global.Texts["Briefing"]))
            {
                CurWin = PauseWindow.About;
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 100, 250, 200, 50), Global.Texts["Exit mission"]))
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
                    Global.Texts["Pause"]))
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
                    Global.Texts["Continue"]))
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

            if (Global.SettingsSaved)
            {
                if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - UIUtil.Scaled(100), Windows[windowID].Bottom - 100, UIUtil.Scaled(200), 50), Global.Texts["Back"]))
                {
                    CurWin = PauseWindow.Main;
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
            UIUtil.WindowTitle(Windows[windowID], Global.MissionName);
            Vector2 W6x4 = UIUtil.GetWindow(6, 4);
            GUI.BeginGroup(new Rect(new Vector2(Windows[windowID].CenterX - (W6x4.x / 2), 100), W6x4), GUI.skin.GetStyle("textContainer"));
            UIUtil.TextContainerTitle(new Rect(27, 10, UIUtil.Scaled(220), 20), Global.Texts["Targets"]);
            UIUtil.TextContainerText(new Rect(27, 40, UIUtil.Scaled(546), 60), Global.MissionBrief);
            GUI.EndGroup();
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - UIUtil.Scaled(90), Windows[windowID].Bottom - 100, UIUtil.Scaled(180), 50), Global.Texts["Back"]))
            {
                CurWin = PauseWindow.Main;
            }
        }
        void DrawStartW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], Global.MissionName);
            Vector2 W6x4 = UIUtil.GetWindow(6, 4);
            GUI.BeginGroup(new Rect(new Vector2(Windows[windowID].CenterX - (W6x4.x / 2), 100), W6x4), GUI.skin.GetStyle("textContainer"));
            UIUtil.TextContainerTitle(new Rect(27, 10, UIUtil.Scaled(220), 20), Global.Texts["Targets"]);
            UIUtil.TextContainerText(new Rect(27, 40, UIUtil.Scaled(546), 60), Global.MissionBrief);
            GUI.EndGroup();
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - UIUtil.Scaled(90), Windows[windowID].Bottom - 100, UIUtil.Scaled(180), 50), Global.Texts["Start"]))
            {
                CurWin = PauseWindow.Main;
                Continue();
            }
        }
        void DrawRestartW(int windowID)
        {
            //UIUtil.WindowTitle(Windows[windowID], "Restart mission?");
            //UIUtil.Label(new Rect(100, 100, 400, 43), "Do you think you'll do better from the beginning?");
            //if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX + 10, 150, 180, 50), "Yes"))
            //{
            //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            //}
            //if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 190, 150, 180, 50), "No"))
            //{
            //    CurWin = PauseWindow.Main;
            //}
            int ansver = Question(windowID, Global.Texts["Restart_question"], Global.Texts["Restart_discription"], Global.Texts["Yes"], Global.Texts["No"]);
            if (ansver == 1)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            else if (ansver == -1)
                CurWin = PauseWindow.Main;
        }
        void DrawQuitW(int windowID)
        {
            //UIUtil.WindowTitle(Windows[windowID], "Are you deserting?");
            //UIUtil.Label(new Rect(100, 100, 400, 43), "Another time, you'll have to start the mission from the beginning.");
            //if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX + 10, 150, 180, 50), "Yes"))
            //{
            //    Application.Quit();
            //}
            //if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 190, 150, 180, 50), "No"))
            //{
            //    CurWin = PauseWindow.Main;
            //}
            int ansver = Question(windowID, Global.Texts["ExitMission_question"], Global.Texts["ExitMission_discription"], Global.Texts["Yes"], Global.Texts["No"]);
            if (ansver == 1)
                SceneManager.LoadScene(0);
            else if (ansver == -1)
                CurWin = PauseWindow.Main;
        }
        int Question(int windowID, string title, string text, string var1, string var2)
        {
            UIUtil.WindowTitle(Windows[windowID], title);
            UIUtil.Label(new Rect(100, 100, UIUtil.Scaled(400), 43), text);
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX + UIUtil.Scaled(10), 150, UIUtil.Scaled(180), 50), var1))
            {
                return 1;
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - UIUtil.Scaled(190), 150, UIUtil.Scaled(180), 50), var2))
            {
                return -1;
            }
            return 0;
        }
    }
}//