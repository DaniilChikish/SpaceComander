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
        private Vector2 scrollAboutPosition = Vector2.zero;

        void Start()
        {
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
            Windows[0] = new UIWindowInfo(new Rect(new Vector2((Screen.width - W4x4.x) / 2, 100), W4x4));//main
            Windows[1] = new UIWindowInfo(new Rect(new Vector2((Screen.width - W6x2.x) / 2, (Screen.height - W6x2.y) / 2 + 100), W6x2));//question
            Windows[2] = new UIWindowInfo(new Rect(new Vector2((Screen.width - W8x6.x) / 2, 50), W8x6));//options
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
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - UIUtil.Scaled(90), Windows[windowID].Bottom - 100, UIUtil.Scaled(180), 50), "Back"))
            {
                CurWin = MenuWindow.Main;
            }
        }
        void DrawAboutW(int windowID)
        {
            UIUtil.WindowTitle(Windows[windowID], "About game");
            GUI.BeginGroup(new Rect(Windows[windowID].CenterX - UIUtil.Scaled(140), 100, UIUtil.Scaled(280), 150), GUI.skin.GetStyle("textContainer"));
            UIUtil.TextContainerTitle(new Rect(27, 10, UIUtil.Scaled(220), 20), "Develop");
            UIUtil.TextContainerText(new Rect(27, 40, UIUtil.Scaled(220), 60), "Designer - Jogo Deus;\r\nScripting - Jogo Deus;\r\nModeling - Jogo Deus;\r\nTexturing - Open source;\r\nMusic - Open source.");
            GUI.EndGroup();
            Rect viewRect = new Rect(0, 0, UIUtil.Scaled(546), 600);
            scrollAboutPosition = GUI.BeginScrollView(new Rect(Windows[windowID].CenterX - UIUtil.Scaled(300), 260, UIUtil.Scaled(600), 230), scrollAboutPosition, viewRect);
            UIUtil.TextContainerTitle(new Rect(27, 10, UIUtil.Scaled(220), 20), "Story");
            UIUtil.TextContainerText(new Rect(27, 40, viewRect.width, viewRect.height), "   Year 2406'.\r\n" +
"In deep space beyond the boundaries of the jurisdiction of the galactic government the struggle of conglomerates for resource - rich planets reaches astronomical proportions." +
"Their interests are defended by huge space fleets of mercenaries." +
"The object of the dispute was the planet Glies-876-d. It is' a harsh and dangerous world unsuitable for life but hiding endless minerals in it'.");
            GUI.EndGroup();
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX + 110, Windows[windowID].Bottom - 100, 200, 50), "Developer page"))
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
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX + UIUtil.Scaled(10), 100, UIUtil.Scaled(180), 50), "Yes"))
            {
                Application.Quit();
            }
            if (UIUtil.ButtonBig(new Rect(Windows[windowID].CenterX - UIUtil.Scaled(190), 100, UIUtil.Scaled(180), 50), "No"))
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
