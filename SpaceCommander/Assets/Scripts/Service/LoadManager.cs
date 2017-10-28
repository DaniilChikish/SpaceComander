using DeusUtility.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SpaceCommander
{
    class LoadManager : MonoBehaviour
    {
        public Texture2D[] imagePool;
        public Texture2D black;
        private int imageIndex;
        [SerializeField]
        private Texture2D barBack;
        [SerializeField]
        private Texture2D barFill;
        private Rect mainRect;
        private AsyncOperation async;
        private float scale;
        public GUISkin Skin;
        public float fadeDir;
        public const float fadeDuration = 0.8f;
        private float alfa;
        public const int MenuIndex = 1;
        private float minLoadDelay;
        private float progress;
        public int CurrentSceneIndex { get { return SceneManager.GetActiveScene().buildIndex; } }
        public int ScenesCount { get { return SceneManager.sceneCountInBuildSettings; } }
        private void Start()
        {
            fadeDir = -1;
            alfa = 1;
            scale = Screen.width / (1280f / 1f);
            mainRect = new Rect(0, 0, Screen.width / scale, Screen.height / scale);
        }
        public void ReloadScene()
        {
            LoadScene(CurrentSceneIndex);
        }
        public void LoadNextScene()
        {
            LoadScene(CurrentSceneIndex + 1);
        }
        public void LoadMenuScene()
        {
            LoadScene(MenuIndex);
        }
        public void LoadScene(int sceneIndex)
        {
            StartCoroutine(LoadSceneCoroutine(sceneIndex));
            fadeDir = 1;
            minLoadDelay = fadeDuration;
            progress = 0;
            scale = Screen.width / (1280f / 1f);
            mainRect = new Rect(0, 0, Screen.width / scale, Screen.height / scale);
            imageIndex = Mathf.RoundToInt(UnityEngine.Random.Range(0, imagePool.Length - 1));
        }
        private void Update()
        {
            if (async!=null && minLoadDelay <= 0)
                async.allowSceneActivation = true;
            else minLoadDelay -= Time.unscaledDeltaTime;
        }
        private void OnGUI()
        {
            GUI.skin = Skin;
            GUI.depth = -100;
            alfa = Mathf.Clamp01(alfa + ((1f / fadeDuration) * fadeDir * Time.unscaledDeltaTime));
            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alfa);
            if (scale != 1)
                GUI.matrix = Matrix4x4.Scale(Vector3.one * scale);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), black);
            if (fadeDir == 1)
            {
                UIUtil.Image(UIUtil.GetRect(new Vector2(960, 540), PositionAnchor.Center, mainRect.size), imagePool[imageIndex]); //new Vector2(720, 405)
                GUI.DrawTexture(UIUtil.GetRect(new Vector2(barBack.width, barBack.height), PositionAnchor.Up, mainRect.size), barBack);
                GUI.DrawTexture(UIUtil.TransformBar(UIUtil.GetRect(new Vector2(barFill.width, barFill.height), PositionAnchor.Up, mainRect.size), progress), barFill);
            }
            UIUtil.Exclamation(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.LeftDown, mainRect.size), "Jogo Deus");
            UIUtil.Exclamation(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.RightDown, mainRect.size), "v. " + Application.version);
        }
        private IEnumerator LoadSceneCoroutine(int nextScene)
        {
            // Start an asynchronous operation to load the scene that was passed to the LoadNewScene coroutine.
            async = SceneManager.LoadSceneAsync(nextScene);
            async.allowSceneActivation = false;
            // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
            while (!async.isDone)
            {
                progress = async.progress / 0.9f;
                yield return null;
            }

        }
    }
}
