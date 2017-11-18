using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
//using SpaceCommander.Units;
using DeusUtility.Random;
using DeusUtility.UI;
using SpaceCommander.Service;
using SpaceCommander.Scenarios;

namespace SpaceCommander
{
    public class GlobalController : MonoBehaviour
    {
        private static GlobalController instance;
        public void OnEnable()
        {
            Initialise();
            instance = this;
        }
        public static GlobalController GetInstance()
        {
            if (instance == null)
            {
                GameObject newGlobal = new GameObject();
                instance = newGlobal.AddComponent<GlobalController>();
                instance.Initialise();
            }
            return instance;
        }
        //private void Start()
        //{
        //    Initialise();
        //}
        public void Initialise()
        {
            if (!handlersInitialised)
                InitialiseHandlers();
            InitialiseReference();            
        }
        public List<Unit> unitList; // список 
        public List<SpaceShip> selectedList; // спиков выделенных объектов
        public Army playerArmy;

        public double[] RandomNormalPool;
        public double RandomNormalAverage;
        //public double[] RandomExponentPool;
        private float randomPoolBackCoount;
        //settings
        private static bool handlersInitialised;
        private static GameSettings settings;
        public GameSettings Settings { get { return settings; } set { settings = value; }}
        private static SpecINIHandler specIni;
        public SpecINIHandler SpecINI { get { return specIni; } private set { specIni = value; } }
        //texts
        private static TextINIHandler localTexts;
        public string Texts(string key)
        {
            return localTexts.GetText("Text." + Settings.Localisation.ToString(), key);
        }
        public SoundStorage Sound { get; private set; }
        public PrefabStorage Prefab { get; private set; }
        private Scenario Mission;
        public string MissionName
        {
            get
            {
                if (Mission != null)
                    return Mission.Name;
                else return SceneManager.GetActiveScene().name;
            }
        }
        public string MissionBrief
        {
            get
            {
                if (Mission != null)
                    return Mission.Brief;
                else return "Eliminate all enemies!";
            }
        }
        private ShipManualController manualController;
        public ShipManualController ManualController { get { return manualController; } }
        private void InitialiseHandlers()
        {
            LoadSettings();
            LoadTexts();
            LoadSpec();
            handlersInitialised = true;
        }
        private void InitialiseReference()
        {
            Sound = FindObjectOfType<SoundStorage>();
            Prefab = FindObjectOfType<PrefabStorage>();
            Mission = FindObjectOfType<Scenario>();
            manualController = FindObjectOfType<ShipManualController>();
        }
        private void LoadSettings()
        {
            settings = new GameSettings();
            Settings.Load();
        }
        private void LoadSpec()
        {
            SpecINI = new SpecINIHandler(Application.streamingAssetsPath + "\\spec.ini");
        }
        public void LoadTexts()
        {
            GlobalController.localTexts = new TextINIHandler(Application.streamingAssetsPath + "\\localisation_base.ini");
        }

        public void Update()
        {
            if (randomPoolBackCoount < 0)
            {
                RandomNormalPool = Randomizer.Normal(1, 1, 128, 0, 128);
                RandomNormalAverage = RandomNormalPool.Average();
                //RandomExponentPool = Randomizer.Exponential(7, 128, 0, 128);
                randomPoolBackCoount = 10;
            }
            else randomPoolBackCoount -= Time.deltaTime;
        }
        public int CheckVictory()
        {
            if (Mission != null)
                return Mission.CheckVictory();
            else
                return Scenario.DefaultOrder();
        }
    }
}
