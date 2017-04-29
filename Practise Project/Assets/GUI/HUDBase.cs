using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PracticeProject
{

    public class HUDBase : MonoBehaviour
    {
   protected  enum PauseWindow {Start, Main, Restart, Options, About, Quit, Victory, Defeat}
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
            Windows[0] = new UIWindowInfo(new Rect(Screen.width / 2 - 200, 100, 400, 400));//main
            Windows[1] = new UIWindowInfo(new Rect(Screen.width / 2 - 300, 200, 600, 250));//question
            Windows[2] = new UIWindowInfo(new Rect(Screen.width / 2 - 400, 50, 800, 600));//options
            Windows[3] = new UIWindowInfo(new Rect(Screen.width / 2 - 400, 50, 800, 600));//about
            Windows[4] = new UIWindowInfo(new Rect(Screen.width / 2 - 300, Screen.height - 400, 600, 200));//victory
            CurWin = PauseWindow.Start;
            Stop();
        }

        // Update is called once per frame
        void Update()
        {
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
                                GUI.Window(3, Windows[3].rect, DrawAboutW, "");
                                break;
                            }
                        case PauseWindow.Start:
                            {
                                GUI.Window(3, Windows[3].rect, DrawStartW, "");
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
            GUI.DrawTexture(new Rect((Screen.width- VictoryBannerSize.x) / 2 , (Screen.height- VictoryBannerSize.y) / 2  - 200, VictoryBannerSize.x, VictoryBannerSize.y), VictoryBanner);
            GUI.Window(4, Windows[4].rect, DrawVictoryQW, "");
        }
        void DrawVictoryQW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], "Good job!");
            //UIUtil.Label(new Rect(50, 10, 180, 43), "Running in fear?");
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX + 10, 100, 180, 50), "Next mission"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 190, 100, 180, 50), "Main menu"))
            {
                SceneManager.LoadScene(0);
            }
        }
        private void Defeat()
        {
            CurWin = PauseWindow.Defeat;
            GUI.DrawTexture(new Rect((Screen.width - VictoryBannerSize.x) / 2, (Screen.height - VictoryBannerSize.y) / 2 - 200, VictoryBannerSize.x, VictoryBannerSize.y), DefeatBanner);
            GUI.Window(4, Windows[4].rect, DrawDefeatQW, "");
        }
        void DrawDefeatQW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], "You lose!");
            //UIUtil.Label(new Rect(50, 10, 180, 43), "Running in fear?");
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX + 10, 100, 180, 50), "Restart"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 190, 100, 180, 50), "Main menu"))
            {
                SceneManager.LoadScene(0);
            }
        }
        private void DrawPauseW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], "Space Comander");
            //GUI.color.a = window.UIAlpha;
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 90, 100, 180, 50), "Restart"))
            {
                CurWin = PauseWindow.Restart;
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 90, 150, 180, 50), "Options"))
            {
                CurWin = PauseWindow.Options;
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 90, 200, 180, 50), "Briefing"))
            {
                CurWin = PauseWindow.About;
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 90, 250, 180, 50), "Exit mission"))
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
                    "Pause"))
                {
                    Debug.Log("Pause");
                    Stop();
                }
            }
            else
            {
                if (UIUtil.Button(new Rect(pauseButtonBoxBorder.x, pauseButtonBoxBorder.y,
                    pauseButtonBoxSize.x - pauseButtonBoxBorder.x - pauseButtonBoxBorder.z,  //width
                    pauseButtonBoxSize.y - pauseButtonBoxBorder.y - pauseButtonBoxBorder.w), //height
                    "Continue"))
                {
                    Debug.Log("Continue");
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
            if (CurWin == PauseWindow.Restart || CurWin == PauseWindow.Quit)
                CurWin = PauseWindow.Main;
        }

        void DrawOptionsW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], "Options");
            //UIUtil.Label(new Rect(50, 10, 180, 43), "Настройки Игры");
            //if (UIUtil.Button(new Rect(Windows[windowID].CenterX - 90, 100, 180, 50), "Игра"))
            //{
            //}
            //if (UIUtil.Button(new Rect(Windows[windowID].CenterX - 90, 150, 180, 50), "Аудио"))
            //{
            //}
            //if (UIUtil.Button(new Rect(Windows[windowID].CenterX - 90, 200, 180, 50), "Видео"))
            //{
            //}
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 90, Windows[windowID].Bottom - 100, 180, 50), "Back"))
            {
                CurWin = PauseWindow.Main;
            }
        }
        void DrawAboutW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], Global.MissionName);
            GUI.BeginGroup(new Rect(Windows[windowID].CenterX - 300, 100, 600, 400), GUI.skin.GetStyle("textContainer"));
            UIUtil.TextContainerTitle(new Rect(27, 10, 220, 20), "Targets");
            UIUtil.TextContainerText(new Rect(27, 40, 220, 60), Global.MissionBrief);
            GUI.EndGroup();
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 90, Windows[windowID].Bottom - 100, 180, 50), "Back"))
            {
                CurWin = PauseWindow.Main;
            }
        }
        void DrawStartW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], Global.MissionName);
            GUI.BeginGroup(new Rect(Windows[windowID].CenterX - 300, 100, 600, 400), GUI.skin.GetStyle("textContainer"));
            UIUtil.TextContainerTitle(new Rect(27, 10, 220, 20), "Targets");
            UIUtil.TextContainerText(new Rect(27, 40, 220, 60), Global.MissionBrief);
            GUI.EndGroup();
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 90, Windows[windowID].Bottom - 100, 180, 50), "Start"))
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
            int ansver = Question(windowID, "Restart mission?", "Do you think you'll do better from the beginning?", "Yes", "No");
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
            int ansver = Question(windowID, "Are you deserting?", "Another time, you'll have to start the mission from the beginning.", "Yes", "No");
            if (ansver == 1)
                SceneManager.LoadScene(0);
            else if (ansver == -1)
                CurWin = PauseWindow.Main;
        }
        int Question(int windowID, string title, string text, string var1, string var2)
        {
            UIUtil.WindowTitle(Windows[windowID], title);
            UIUtil.Label(new Rect(100, 100, 400, 43), text);
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX + 10, 150, 180, 50), var1))
            {
                return 1;
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 190, 150, 180, 50), var2))
            {
                return -1;
            }
            return 0;
        }
    }
}//