using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml.Serialization;
using System;

namespace SpaceCommander
{
    [Serializable]
    public class SerializeData<TKey, TValue>: ISerializationCallbackReceiver 
    {
        //public Languages lang;
        [XmlIgnore]
        public Dictionary<TKey, TValue> Data;
        [SerializeField]
        public List<TKey> keys = new List<TKey>();

        [SerializeField]
        public List<TValue> values = new List<TValue>();

        public SerializeData() { }
        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            foreach (KeyValuePair<TKey, TValue> pair in Data)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
                Debug.Log(keys[keys.Count - 1] +" = "+ values[values.Count - 1]);
            }
        }

        // load dictionary from lists
        public void OnAfterDeserialize()
        {
            this.Data = new Dictionary<TKey, TValue>();

            if (keys.Count != values.Count)
                throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));

            for (int i = 0; i < keys.Count; i++)
                this.Data.Add(keys[i], values[i]);
        }
    }
    public class GameSettings
    {
        public bool SettingsSaved;

        private Languages localisation;
        private bool staticProportion;
        private float soundLevel;
        private float musicLevel;
        public GameSettings()
        {
            SetDefault();
        }
        public Languages Localisation { get { return localisation; } set { SettingsSaved = false; localisation = value; } }
        public float SoundLevel { get { return soundLevel; } set { SettingsSaved = false; soundLevel = value; } }
        public float MusicLevel { get { return musicLevel; } set { SettingsSaved = false; musicLevel = value; } }
        public bool StaticProportion { get { return staticProportion; } set { SettingsSaved = false; staticProportion = value; } }
        public void Load()
        {
            INIHandler settingsINI = new INIHandler(Application.streamingAssetsPath + "\\settings.dat");

            Localisation = (Languages)Convert.ToInt32(settingsINI.ReadINI(this.GetType().ToString() + ".Base", "Localisation"));
            SoundLevel = Convert.ToSingle(settingsINI.ReadINI(this.GetType().ToString() + ".Base", "SoundLevel"));
            MusicLevel = Convert.ToSingle(settingsINI.ReadINI(this.GetType().ToString() + ".Base", "MusicLevel"));
            StaticProportion = Convert.ToBoolean(settingsINI.ReadINI(this.GetType().ToString() + ".Base", "StaticProportion"));

            SettingsSaved = true;
        }
        public void Save()
        {
            INIHandler settingsINI = new INIHandler(Application.streamingAssetsPath + "\\settings.dat");

            settingsINI.Write(this.GetType().ToString() + ".Base", "Localisation", ((int)Localisation).ToString());
            settingsINI.Write(this.GetType().ToString() + ".Base", "SoundLevel", SoundLevel.ToString());
            settingsINI.Write(this.GetType().ToString() + ".Base", "MusicLevel", MusicLevel.ToString());
            settingsINI.Write(this.GetType().ToString() + ".Base", "StaticProportion", StaticProportion.ToString());
        }
        public void SetDefault()
        {
            Localisation = Languages.English;
            StaticProportion = true;
            MusicLevel = 100;
            SoundLevel = 100;
        }
    }
    public class TextINIHandler
    {
        private INIHandler dataStorage;
        private Dictionary<string, string> dataLocal;
        public TextINIHandler(string path)
        {
            dataStorage = new INIHandler(path);
            dataLocal = new Dictionary<string, string>();
        }
        public string GetText(string section, string key)
        {
            string value;
            if (!dataLocal.TryGetValue(key, out value))
            {
                value = dataStorage.ReadINI(section, key, 1024);
                value = value.Replace("\\r\\n", ((char)13).ToString() + ((char)10).ToString());
                dataLocal.Add(key, value);
            }
            return value;
        }
        public void SetText(string section, string key, string value)
        {
            dataStorage.Write(section, key, value);
        }

    }
}
