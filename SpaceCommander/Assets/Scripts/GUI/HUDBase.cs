using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;
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
        public Texture2D UnitStatBack;
        public Texture2D UnitArmorLine;
        public Texture2D UnitShieldLine;
        private Vector4 pauseButtonBoxBorder;
        private Vector2 pauseButtonBoxSize;
        private bool Pause;
        private UIWindowInfo[] Windows;
        private GlobalController Global;
        private LoadManager Loader;
        private SpaceShipGUIObserver observer;
        private bool langChanged;
        private GameSettings settingsLocal;
        private int victory;
        private float checkVictoryCount;
        private const float checkVictoryRate = 1;
        // Use this for initialization
        void Start()
        {
            //Debug.Log("HUD started");
            Global = GlobalController.GetInstance();
            Loader = FindObjectOfType<LoadManager>();
            observer = FindObjectOfType<SpaceShipGUIObserver>();
            VictoryBannerSize.x = 1784f / 2f;
            VictoryBannerSize.y = 758f / 2f;
            pauseButtonBoxSize.x = 108f;
            pauseButtonBoxSize.y = 50f;
            pauseButtonBoxBorder.x = pauseButtonBoxSize.x * 4f / 88f;
            pauseButtonBoxBorder.y = pauseButtonBoxSize.y * 8f / 40f;
            pauseButtonBoxBorder.z = pauseButtonBoxSize.x * 4f / 88f;
            pauseButtonBoxBorder.w = pauseButtonBoxSize.y * 8f / 40f;
            Windows = new UIWindowInfo[5];

            //
            if (Global.Settings.StaticProportion)
            {
                scale = Screen.width / (1280f / 1f);
                mainRect = new Rect(0, 0, Screen.width / scale, Screen.height / scale);
            }
            else
                mainRect = new Rect(0, 0, Screen.width, Screen.height);

            Windows[0] = new UIWindowInfo(UIUtil.GetRect(new Vector2(400, 400), PositionAnchor.Center, mainRect.size));//main
            Windows[1] = new UIWindowInfo(UIUtil.GetRect(new Vector2(600, 250f), PositionAnchor.Center, mainRect.size));//question
            Windows[2] = new UIWindowInfo(UIUtil.GetRect(new Vector2(800, 600), PositionAnchor.Center, mainRect.size));//options
            Windows[3] = new UIWindowInfo(UIUtil.GetRect(new Vector2(600, 200), PositionAnchor.Center, mainRect.size, new Vector2(0, 120)));//victory

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

            Windows[0].rect = UIUtil.GetRect(new Vector2(400, 400), PositionAnchor.Center, mainRect.size);//main
            Windows[1].rect = UIUtil.GetRect(new Vector2(600, 250f), PositionAnchor.Center, mainRect.size);//question
            Windows[2].rect = UIUtil.GetRect(new Vector2(800, 600), PositionAnchor.Center, mainRect.size);//options
            Windows[3].rect = UIUtil.GetRect(new Vector2(600, 200), PositionAnchor.Center, mainRect.size, new Vector2(0, 120));//victory

            if (checkVictoryCount <= 0)
            {
                victory = Global.CheckVictory();
                checkVictoryCount = checkVictoryRate;
            }
            else
                checkVictoryCount -= Time.deltaTime;

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
                                GUI.Window(2, Windows[2].rect, DrawBriefW, "");
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
                        default: break;
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
            if (observer.Mode == ObserverMode.Full)
                observer.SwichHandControl();
            GUI.DrawTexture(UIUtil.GetRect(VictoryBannerSize, PositionAnchor.Center, mainRect.size, new Vector2(0, -100)), VictoryBanner);
            GUI.Window(3, Windows[3].rect, DrawVictoryQW, "");
        }
        void DrawVictoryQW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], Global.Texts("Good job!"));
            //UIUtil.Label(new Rect(50, 10, 180, 43), "Running in fear?");
            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(180, 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(100, -50)), Global.Texts("Next mission")))
            {
                Loader.LoadNextScene();
            }
            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(180, 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(-100, -50)), Global.Texts("Main menu")))
            {
                Loader.LoadMenuScene();
            }
        }
        private void Defeat()
        {
            CurWin = PauseWindow.Defeat;
            if (observer.Mode == ObserverMode.Full)
                observer.SwichHandControl();
            GUI.DrawTexture(UIUtil.GetRect(VictoryBannerSize, PositionAnchor.Center, mainRect.size, new Vector2(0, -100)), DefeatBanner);
            GUI.Window(3, Windows[3].rect, DrawDefeatQW, "");
        }
        void DrawDefeatQW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], Global.Texts("You lose!"));
            //UIUtil.Label(new Rect(50, 10, 180, 43), "Running in fear?");
            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(180, 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(100, -50)), Global.Texts("Restart")))
            {
                Loader.ReloadScene();
            }
            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(180, 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(-100, -50)), Global.Texts("Main menu")))
            {
                Loader.LoadMenuScene();
            }
        }
        private void DrawPauseW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], Global.Texts("Pause"));
            //GUI.color.a = window.UIAlpha;
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 100, 100, 200, 50), Global.Texts("Restart")))
            {
                ClickSound();
                CurWin = PauseWindow.Restart;
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 100, 150, 200, 50), Global.Texts("Options")))
            {
                ClickSound();
                langChanged = false;
                settingsLocal = Global.Settings.Copy();
                CurWin = PauseWindow.Options;
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 100, 200, 200, 50), Global.Texts("Briefing")))
            {
                ClickSound();
                CurWin = PauseWindow.About;
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 100, 250, 200, 50), Global.Texts("Exit mission")))
            {
                ClickSound();
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
            Time.timeScale = 0.01f;
            Pause = true;
        }
        private void Continue()
        {
            Time.timeScale = 1f;
            Pause = false;
            if (CurWin == PauseWindow.Restart || CurWin == PauseWindow.Quit || CurWin == PauseWindow.Start)
                CurWin = PauseWindow.Main;
        }

        void DrawOptionsW(int windowID)
        {
            float fBuffer;
            UIUtil.WindowTitle(Windows[windowID], Global.Texts("Options"));

            GUI.BeginGroup(UIUtil.GetRect(new Vector2(340, 55), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 100)));
            UIUtil.Label(new Rect(120, 0, 100, 20), Global.Texts("Sound"));
            fBuffer = GUI.HorizontalSlider(new Rect(0, 40, 340, 13), settingsLocal.SoundLevel, 0.0f, 1f);
            if (settingsLocal.SoundLevel != fBuffer)
                settingsLocal.SoundLevel = fBuffer;
            GUI.EndGroup();

            GUI.BeginGroup(UIUtil.GetRect(new Vector2(340, 55), PositionAnchor.Up, Windows[windowID].rect.size, new Vector2(0, 165)));
            UIUtil.Label(new Rect(120, 0, 100, 20), Global.Texts("Music"));
            fBuffer = GUI.HorizontalSlider(new Rect(0, 40, 340, 13), settingsLocal.MusicLevel, 0.0f, 1f);
            if (settingsLocal.MusicLevel != fBuffer)
                settingsLocal.MusicLevel = fBuffer;
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
            UIbuff.ShowUnitFrame = UIUtil.Toggle(new Vector2(35, 40), settingsLocal.AliesUI.ShowUnitFrame, "");
            UIbuff.ShowUnitIcon = UIUtil.Toggle(new Vector2(35, 65), settingsLocal.AliesUI.ShowUnitIcon, "");
            UIbuff.ShowUnitName = UIUtil.Toggle(new Vector2(35, 90), settingsLocal.AliesUI.ShowUnitName, "");
            UIbuff.ShowUnitStatus = UIUtil.Toggle(new Vector2(35, 115), settingsLocal.AliesUI.ShowUnitStatus, "");
            GUI.EndGroup();
            if (!settingsLocal.AliesUI.Equals(UIbuff))
                settingsLocal.AliesUI = UIbuff;

            GUI.BeginGroup(new Rect(300, 37, 100, 145));
            UIUtil.Label(new Rect(0, 0, 100, 25), Global.Texts("Selected"));
            UIbuff.ShowUnitFrame = UIUtil.Toggle(new Vector2(35, 40), settingsLocal.SelectedUI.ShowUnitFrame, "");
            UIbuff.ShowUnitIcon = UIUtil.Toggle(new Vector2(35, 65), settingsLocal.SelectedUI.ShowUnitIcon, "");
            UIbuff.ShowUnitName = UIUtil.Toggle(new Vector2(35, 90), settingsLocal.SelectedUI.ShowUnitName, "");
            UIbuff.ShowUnitStatus = UIUtil.Toggle(new Vector2(35, 115), settingsLocal.SelectedUI.ShowUnitStatus, "");
            GUI.EndGroup();
            if (!settingsLocal.SelectedUI.Equals(UIbuff))
                settingsLocal.SelectedUI = UIbuff;

            GUI.BeginGroup(new Rect(400, 37, 100, 145));
            UIUtil.Label(new Rect(0, 0, 100, 25), Global.Texts("Enemys"));
            UIbuff.ShowUnitFrame = UIUtil.Toggle(new Vector2(35, 40), settingsLocal.EnemyUI.ShowUnitFrame, "");
            UIbuff.ShowUnitIcon = UIUtil.Toggle(new Vector2(35, 65), settingsLocal.EnemyUI.ShowUnitIcon, "");
            UIbuff.ShowUnitName = UIUtil.Toggle(new Vector2(35, 90), settingsLocal.EnemyUI.ShowUnitName, "");
            UIbuff.ShowUnitStatus = UIUtil.Toggle(new Vector2(35, 115), settingsLocal.EnemyUI.ShowUnitStatus, "");
            GUI.EndGroup();
            if (!settingsLocal.EnemyUI.Equals(UIbuff))
                settingsLocal.EnemyUI = UIbuff;
            GUI.EndGroup();
            {
                string[] radios = new string[2];
                radios[0] = "English";
                radios[1] = "Русский";
                int langueageRadioSelected = (int)settingsLocal.Localisation;
                GUI.BeginGroup(UIUtil.GetRect(new Vector2(150, 110), PositionAnchor.LeftDown, Windows[windowID].rect.size, new Vector2(100, -100)));
                UIUtil.Label(new Rect(0, 0, 150, 20), Global.Texts("Language"));
                langueageRadioSelected = UIUtil.ToggleList(new Rect(10, 40, 140, 74), langueageRadioSelected, radios);
                GUI.EndGroup();
                if (langueageRadioSelected != (int)settingsLocal.Localisation)
                {
                    settingsLocal.Localisation = (Languages)langueageRadioSelected;
                    langChanged = true;
                }
            }

            if (false)
            {
                string[] radios = new string[2];
                radios[0] = "Static size";
                radios[1] = "Static proportion";
                int screenRadioSelected;
                if (settingsLocal.StaticProportion)
                    screenRadioSelected = 1;
                else screenRadioSelected = 0;
                GUI.BeginGroup(UIUtil.GetRect(new Vector2(150, 110), PositionAnchor.RightDown, Windows[windowID].rect.size, new Vector2(-100, -100)));
                UIUtil.Label(new Rect(0, 0, 150, 20), Global.Texts("UI scale"));
                int screenRadioBuffer = UIUtil.ToggleList(new Rect(0, 40, 150, 74), screenRadioSelected, radios);
                GUI.EndGroup();
                if (screenRadioSelected != screenRadioBuffer)
                {
                    settingsLocal.StaticProportion = (screenRadioBuffer == 1);
                }
            }

            if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(0, -50)), Global.Texts("Back")))
            {
                ClickSound();
                CurWin = PauseWindow.Main;
            }
            if (!settingsLocal.Equals(Global.Settings))
            {
                if (UIUtil.ButtonBig(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.Down, Windows[windowID].rect.size, new Vector2(220, -50)), Global.Texts("Save")))
                {
                    ConfirmSound();
                    Global.Settings = settingsLocal.Copy();
                    Global.Settings.Save();
                    if (langChanged)
                        Global.LoadTexts();
                }
            }
        }
        void DrawBriefW(int windowID)
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
                ClickSound();
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
                ConfirmSound();
                Continue();
            }
        }
        void DrawRestartW(int windowID)
        {
            int ansver = Question(windowID, Global.Texts("Restart_question"), Global.Texts("Restart_discription"), Global.Texts("Yes"), Global.Texts("No"));
            if (ansver == 1)
            {
                Loader.ReloadScene();
                ConfirmSound();
            }
            else if (ansver == -1)
            {
                CurWin = PauseWindow.Main;
                DeniedSound();
            }
        }
        void DrawQuitW(int windowID)
        {
            int ansver = Question(windowID, Global.Texts("ExitMission_question"), Global.Texts("ExitMission_discription"), Global.Texts("Yes"), Global.Texts("No"));
            if (ansver == 1)
            {
                Loader.LoadMenuScene();
                ConfirmSound();
            }
            else if (ansver == -1)
            {
                CurWin = PauseWindow.Main;
                DeniedSound();
            }
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
        private void ClickSound()
        {
            Global.Sound.InstantUI(Service.SoundStorage.UISoundType.InClick, Camera.main.transform.position, Global.Settings.SoundLevel);
        }
        private void HoverSound()
        {
            Global.Sound.InstantUI(Service.SoundStorage.UISoundType.Hover, Camera.main.transform.position, Global.Settings.SoundLevel);
        }
        private void ConfirmSound()
        {
            Global.Sound.InstantUI(Service.SoundStorage.UISoundType.Confirm, Camera.main.transform.position, Global.Settings.SoundLevel);
        }
        private void DeniedSound()
        {
            Global.Sound.InstantUI(Service.SoundStorage.UISoundType.OutClick, Camera.main.transform.position, Global.Settings.SoundLevel);
        }
    }
}//