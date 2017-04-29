using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PracticeProject
{

    public class MainMenu : MonoBehaviour
    {
    protected enum MenuWindow { Main, Levels, Options, About, Quit }
        public GUISkin Skin;
        private MenuWindow CurWin;
        UIWindowInfo[] Windows;

        void Start()
        {
            CurWin = MenuWindow.Main;
            Windows = new UIWindowInfo[5];
            Windows[0] = new UIWindowInfo(new Rect(Screen.width / 2 - 200, 100, 400, 400));//main
            Windows[1] = new UIWindowInfo(new Rect(Screen.width / 2 - 200, 100, 400, 400));//level
            Windows[2] = new UIWindowInfo(new Rect(Screen.width / 2 - 400, 50, 800, 600));//options
            Windows[3] = new UIWindowInfo(new Rect(Screen.width / 2 - 400, 50, 800, 600));//about
            Windows[4] = new UIWindowInfo(new Rect(Screen.width / 2 - 300, 200, 600, 200));//quit
            //Windows[5] = new SDWindowInfo(new Rect(0, Screen.height-100, 100, 100));//info
        }
        private void Update()
        {
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
                        GUI.Window(1, Windows[1].rect, DrawLevelsW, "");
                        break;
                    }
                case MenuWindow.Options:
                    {
                        GUI.Window(2, Windows[2].rect, DrawOptionsW, "");
                        break;
                    }
                case MenuWindow.About:
                    {
                        GUI.Window(3, Windows[3].rect, DrawAboutW, "");
                        break;
                    }
                case MenuWindow.Quit:
                    {
                        GUI.Window(4, Windows[4].rect, DrawQuitW, "");
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
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 90, 100, 180, 50), "Play"))
            {
                CurWin = MenuWindow.Levels;
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 90, 150, 180, 50), "Options"))
            {
                CurWin = MenuWindow.Options;
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 90, 200, 180, 50), "About game"))
            {
                CurWin = MenuWindow.About;
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 90, 250, 180, 50), "Quit"))
            {
                CurWin = MenuWindow.Quit;
            }
        }
        void DrawLevelsW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], "Start level");
            //UIUtil.Label(new Rect(50, 10, 180, 43), "Выберите уровень");
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 90, 100, 180, 50), "Level 1"))
            {
                Debug.Log("Уровень 1 загружен");
                //Application.LoadLevel(1);
                SceneManager.LoadScene(1);
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 90, 150, 180, 50), "Level 2"))
            {
                Debug.Log("Уровень 2 загружен");
                //Application.LoadLevel(2);
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 90, 200, 180, 50), "Level 3"))
            {
                Debug.Log("Уровень 3 загружен");
                //Application.LoadLevel(3);
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 90, 250, 180, 50), "Back"))
            {
                CurWin = MenuWindow.Main;
            }
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
                CurWin = MenuWindow.Main;
            }
        }
        void DrawAboutW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], "About game");
            GUI.BeginGroup(new Rect(Windows[windowID].CenterX - 110, 100, 220, 144), GUI.skin.GetStyle("textContainer"));
            UIUtil.TextContainerTitle(new Rect(27, 10, 220, 20), "Develop");
            UIUtil.TextContainerText(new Rect(27, 40, 220, 60), "Designer - Jogo Deus;\r\nScripting - Jogo Deus;\r\nModeling - Jogo Deus;\r\nTexturing - Open source;\r\nMusic - Open source.");
            GUI.EndGroup();
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 90, Windows[windowID].Bottom - 150, 180, 50), "Develiper page"))
            {
                Application.OpenURL("https://vk.com/daniil.chikish");
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 90, Windows[windowID].Bottom - 100, 180, 50), "Back"))
            {
                CurWin = MenuWindow.Main;
            }
        }
        void DrawQuitW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], "Running in fear?");
            //UIUtil.Label(new Rect(50, 10, 180, 43), "Running in fear?");
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX + 10, 100, 180, 50), "Yes"))
            {
                Application.Quit();
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - 190, 100, 180, 50), "No"))
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
