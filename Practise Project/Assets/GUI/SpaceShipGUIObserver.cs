using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using DeusUtility.UI;

namespace SpaceCommander
{
    public enum ObserverMode { None, Half, Full}
    public class SpaceShipGUIObserver : MonoBehaviour
    {
        //public Texture2D healthPanel;
        //public Texture2D healthBarBack;
        //public Texture2D healthBarFill;
        //public Texture2D shieldBarBack;
        //public Texture2D shieldBarFill;
        private const float MovDuratuon = 0.8f;
        private Image ShieldBar;
        private Text ShieldCount;
        private Image HealthBar;
        private Text HealthCount;
        public Texture2D slotBack;
        public Texture2D slotOverlay;
        public Texture2D slotCooldown;
        public Texture2D slotActive;
        public Texture2D MissoleTrapSpellIcon;
        public Texture2D JammerSpellIcon;
        public Texture2D DefaultSpellIcon;
        public ISpaceShipObservable observable;
        private HUDBase hud;
        private GlobalController Global;
        public GUIStyle localStyle;
        private float statusBackCount;
        private float previevBackCount;
        private GameObject canvas;
        private GameObject status;
        private GameObject previev;
        private GameObject spellPanel;
        private ObserverMode mode;
        private bool StatusIsOpen;
        private bool PrevievIsOpen;
        private float openedY;
        private void OnEnable()
        {
            StatusIsOpen = true;
            PrevievIsOpen = true;
            hud = FindObjectOfType<HUDBase>();
            Global = FindObjectOfType<GlobalController>();
            localStyle = new GUIStyle();
            localStyle.normal.background = slotBack;
            localStyle.padding.top = 10;
            localStyle.padding.bottom = 10;
            localStyle.padding.left = 10;
            localStyle.padding.right = 10;
            canvas = GameObject.Find("Canvas");
            status = GameObject.Find("StatusPanel");
            openedY = status.transform.position.y;
            previev = GameObject.Find("UnitPreviewPanel");
            spellPanel = GameObject.Find("SpellButtons");
            ShieldBar = GameObject.Find("ShieldBar").GetComponent<Image>();
            ShieldCount = GameObject.Find("ShieldCount").GetComponent<Text>();
            HealthBar = GameObject.Find("HealthBar").GetComponent<Image>();
            HealthCount = GameObject.Find("HealthCount").GetComponent<Text>();
        }
        public void SetObservable(ISpaceShipObservable observable)
        {
            this.observable = observable;
            mode = ObserverMode.Half;
            statusBackCount = MovDuratuon;
        }
        private void Update()
        {
            if (Global.selectedList.Count == 1)
            {
                observable = Global.selectedList[0];
                if (false)//observable status under hand control
                {
                    mode = ObserverMode.Full;
                    if (PrevievIsOpen && previevBackCount <= 0)
                    {
                        previevBackCount = MovDuratuon;
                        PrevievIsOpen = false;
                    }
                }
                else
                {
                    mode = ObserverMode.Half;
                    if (!PrevievIsOpen && previevBackCount <= 0)
                    {
                        previevBackCount = MovDuratuon;
                        PrevievIsOpen = true;
                    }
                }
                if (!StatusIsOpen && statusBackCount <= 0)
                {
                    statusBackCount = MovDuratuon;
                    StatusIsOpen = true;
                }
            }
            else
            {
                observable = null;
                mode = ObserverMode.None;
                if (StatusIsOpen && statusBackCount <= 0)
                {
                    statusBackCount = MovDuratuon;
                    StatusIsOpen = false;
                }
                if (PrevievIsOpen && previevBackCount <= 0)
                {
                    previevBackCount = MovDuratuon;
                    PrevievIsOpen = false;
                }
            }
            if (mode == ObserverMode.None)
            {
                if (statusBackCount > 0)
                {
                    statusBackCount -= Time.deltaTime;
                    float speedFactor = Mathf.Cos((MovDuratuon - statusBackCount) * Mathf.PI * 2 / MovDuratuon + Mathf.PI) + 1;
                    float statusSpeedMotion = status.GetComponent<RectTransform>().rect.height*hud.scale / MovDuratuon * speedFactor;
                    status.transform.Translate(0, -statusSpeedMotion * Time.deltaTime, 0);
                }
            }
            else
            {
                if (statusBackCount > 0)
                {
                    statusBackCount -= Time.deltaTime;
                    float speedFactor = Mathf.Cos((MovDuratuon - statusBackCount) * Mathf.PI * 2 / MovDuratuon + Mathf.PI) + 1;
                    float statusSpeedMotion = status.GetComponent<RectTransform>().rect.height * hud.scale / MovDuratuon * speedFactor;
                    status.transform.Translate(0, statusSpeedMotion * Time.deltaTime, 0);
                }
            }
            if (mode == ObserverMode.Half)
            {
                if (previevBackCount > 0)
                {
                    previevBackCount -= Time.deltaTime;
                    float speedFactor = Mathf.Cos((MovDuratuon - previevBackCount) * Mathf.PI * 2 / MovDuratuon + Mathf.PI) + 1;
                    float previevSpeedMotion = previev.GetComponent<RectTransform>().rect.height * hud.scale / MovDuratuon * speedFactor;
                    previev.transform.Translate(-previevSpeedMotion * Time.deltaTime, 0, 0);
                }
            }
            else
            {
                if (previevBackCount > 0)
                {
                    previevBackCount -= Time.deltaTime;
                    float speedFactor = Mathf.Cos((MovDuratuon - previevBackCount) * Mathf.PI * 2 / MovDuratuon + Mathf.PI) + 1;
                    float previevSpeedMotion = previev.GetComponent<RectTransform>().rect.height * hud.scale / MovDuratuon * speedFactor;
                    previev.transform.Translate(previevSpeedMotion * Time.deltaTime, 0, 0);
                }
            }
        }
        private void OnGUI()
        {
            GUI.skin = hud.Skin;
            if (Global.StaticProportion && hud.scale != 1)
                GUI.matrix = Matrix4x4.Scale(Vector3.one * hud.scale);
            if (mode != ObserverMode.None)
            {
                //modules
                if (observable.Module != null && observable.Module.Length > 0)
                {
                    int i = 0;
                    foreach (SpellModule m in observable.Module)
                    {
                        float xPos = -(74 * (observable.Module.Length-1))/2 + (74 * i);
                        float yPos = ((openedY * hud.scale - status.transform.position.y) - 10);
                        Rect spellRect = UIUtil.GetRect(new Vector2(68, 68), PositionAnchor.Down, hud.mainRect.size, new Vector2(xPos, yPos));
                        Rect spellOver = UIUtil.GetRect(new Vector2(48, 48), PositionAnchor.Down, hud.mainRect.size, new Vector2(xPos, yPos - 10));
                        float current;
                        Texture2D spellIcon;
                        if (m.GetType() == typeof(MissileTrapLauncher))
                            spellIcon = MissoleTrapSpellIcon;
                        else if (m.GetType() == typeof(Jammer))
                            spellIcon = JammerSpellIcon;
                        else spellIcon = DefaultSpellIcon;
                        switch (m.State)
                        {
                            case SpellModuleState.Active:
                                {
                                    current = 1 - (m.BackCount / m.ActiveTime);
                                    GUI.Button(spellRect, spellIcon, localStyle);
                                    GUI.DrawTexture(spellOver, ProgressUpdate(current, slotActive));
                                    break;
                                }
                            case SpellModuleState.Cooldown:
                                {
                                    current = 1 - (m.BackCount / m.CoolingTime);
                                    GUI.Button(spellRect, spellIcon, localStyle);
                                    GUI.DrawTexture(spellOver, ProgressUpdate(current, slotCooldown)); break;
                                }
                            case SpellModuleState.Ready:
                                {
                                    current = 0f;
                                    if (GUI.Button(spellRect, spellIcon, localStyle) && mode == ObserverMode.Full)
                                        m.Enable();
                                    //GUI.DrawTexture(spellRect, ProgressUpdate(current, slotActive));
                                    break;
                                }
                        }
                        i++;
                    }
                }
                //health
                {
                    ShieldBar.fillAmount = observable.ShieldForce / observable.ShieldCampacity;
                    ShieldCount.text = Mathf.Round(observable.ShieldForce).ToString();
                    HealthBar.fillAmount = observable.Health / observable.MaxHealth;
                    HealthCount.text = Mathf.Round(observable.Health).ToString();
                    //Rect panelRect = UIUtil.GetRect(new Vector2(healthPanel.width, healthPanel.height), PositionAnchor.LeftDown, hud.mainRect.size, new Vector2(10, -10));
                    //GUI.BeginGroup(panelRect, healthPanel);
                    //{
                    //    Rect healthBackRect = UIUtil.GetRect(new Vector2(healthBarBack.width, healthBarBack.height), PositionAnchor.LeftUp, panelRect.size, new Vector2(4, 4));
                    //    GUI.BeginGroup(healthBackRect, healthBarBack);

                    //}
                    //GUI.EndGroup();
                }
            }
        }
        Texture2D ProgressUpdate(float progress, Texture2D tex)
        {
            Texture2D thisTex = new Texture2D(tex.width, tex.height);
            Vector2 centre = new Vector2(Mathf.Ceil(thisTex.width / 2), Mathf.Ceil(thisTex.height / 2)); //find the centre pixel
            for (int y = 0; y < thisTex.height; y++){
                for (int x = 0; x < thisTex.width; x++){
                    var angle = Mathf.Atan2(x - centre.x, y - centre.y) * Mathf.Rad2Deg; //find the angle between the centre and this pixel (between -180 and 180)
                    if (angle < 0)
                    {
                        angle += 360; //change angles to go from 0 to 360
                    }
                    var pixColor = tex.GetPixel(x, y);
                    if (angle <= progress * 360.0)
                    { //if the angle is less than the progress angle blend the overlay colour
                        pixColor = new Color(0,0,0,0);
                        thisTex.SetPixel(x, y, pixColor);
                    }
                    else
                    {
                        thisTex.SetPixel(x, y, pixColor);
                    }
                }
            }
            thisTex.Apply(); //apply the cahnges we made to the texture
            return thisTex;
        }
        public Rect TransformBar(Rect origin, float progress)
        {
            Rect outp = new Rect(origin);
            outp.width = outp.width * progress;
            return outp;
        }
    }
}
