using DeusUtility.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SpaceCommander.Service
{
    class ZeroLoader : MonoBehaviour
    {
        public Texture2D image;
        [SerializeField]
        private int nextScene = 1;
        [SerializeField]
        private Text anyKeyText;
        [SerializeField]
        private Image topBar;
        [SerializeField]
        private Image botBar;
        private Rect mainRect;
        private bool blinkerEnabled;
        private float blinkerCount;
        private AsyncOperation async;
        private float scale;
        public GUISkin Skin;
        private void Start()
        {
                StartCoroutine(LoadNextScene());
            scale = Screen.width / (1280f / 1f);
            mainRect = new Rect(0, 0, Screen.width / scale, Screen.height / scale);
            blinkerEnabled = false;
            blinkerCount = 0;
        }
        private void Update()
        {
            if (blinkerEnabled)
            {
                blinkerCount += Time.deltaTime;
                float a = (blinkerCount % 1) / 1;
                anyKeyText.color = new Color(0, 1, 1, a);
                if (Input.anyKeyDown)
                    async.allowSceneActivation = true;
            }            
        }
        private void OnGUI()
        {
            GUI.skin = Skin;
            if (scale != 1)
                GUI.matrix = Matrix4x4.Scale(Vector3.one * scale);
            UIUtil.Image(UIUtil.GetRect(new Vector2(720, 405), PositionAnchor.Center, mainRect.size), image); //new Vector2(960, 540)
            UIUtil.Exclamation(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.LeftDown, mainRect.size), "Jogo Deus");
            UIUtil.Exclamation(UIUtil.GetRect(new Vector2(200, 50), PositionAnchor.RightDown, mainRect.size), "v. " + Application.version);
        }
        IEnumerator LoadNextScene()
        {
            // Start an asynchronous operation to load the scene that was passed to the LoadNewScene coroutine.
            async = SceneManager.LoadSceneAsync(nextScene);
            async.allowSceneActivation = false;
            // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
            while (!async.isDone)
            {
                float progress= async.progress / 0.9f;
                topBar.fillAmount = progress;
                botBar.fillAmount = progress;
                if (progress == 1)
                {
                    blinkerEnabled = true;
                }
                yield return null;
            }

        }
    }
}
