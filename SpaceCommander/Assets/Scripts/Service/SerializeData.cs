using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml.Serialization;
using System;
using System.Text;

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
        private float iconsScale;
        private string[] users;
        private UISettings aliesUISettings;
        private UISettings selectedUISettings;
        private UISettings enemyUISettings;
        public GameSettings()
        {
            SetDefault();
        }
        public Languages Localisation { get { return localisation; } set { SettingsSaved = false; localisation = value; } }
        public float SoundLevel { get { return soundLevel; } set { SettingsSaved = false; soundLevel = value; } }
        public float MusicLevel { get { return musicLevel; } set { SettingsSaved = false; musicLevel = value; } }
        public bool StaticProportion { get { return staticProportion; } set { SettingsSaved = false; staticProportion = value; } }
        public float IconsScale { get { return iconsScale; } set { SettingsSaved = false; iconsScale = value; } }
        public UISettings AliesUI { get { return aliesUISettings; } set { SettingsSaved = false; aliesUISettings = value; } }
        public UISettings SelectedUI { get { return selectedUISettings; } set { SettingsSaved = false; selectedUISettings = value; } }
        public UISettings EnemyUI { get { return enemyUISettings; } set { SettingsSaved = false; enemyUISettings = value; } }

        public void Load()
        {
            INIHandler settingsINI = new INIHandler(Application.streamingAssetsPath + "\\settings.dat");

            Localisation = (Languages)Convert.ToInt32(settingsINI.ReadINI(this.GetType().ToString() + ".Base", "Localisation", "0"));
            SoundLevel = Convert.ToSingle(settingsINI.ReadINI(this.GetType().ToString() + ".Base", "SoundLevel", "100"));
            MusicLevel = Convert.ToSingle(settingsINI.ReadINI(this.GetType().ToString() + ".Base", "MusicLevel", "100"));
            StaticProportion = Convert.ToBoolean(settingsINI.ReadINI(this.GetType().ToString() + ".Base", "StaticProportion", "True"));

            string buffer;
            buffer = settingsINI.ReadINI(this.GetType().ToString() + ".Base", "Users");
            if (buffer == "") users = new string[0];
            else
            {
                List<string> listBuff = new List<string>(buffer.Split('#'));
                listBuff.RemoveAt(listBuff.Count - 1);
                users = listBuff.ToArray();
            }

            UISettings UIbuff = new UISettings();
            UIbuff.ShowUnitFrame = Convert.ToBoolean(settingsINI.ReadINI(this.GetType().ToString() + ".UI.Alies", "ShowUnitFrame", "True"));
            UIbuff.ShowUnitIcon = Convert.ToBoolean(settingsINI.ReadINI(this.GetType().ToString() + ".UI.Alies", "ShowUnitIcon", "True"));
            UIbuff.ShowUnitName = Convert.ToBoolean(settingsINI.ReadINI(this.GetType().ToString() + ".UI.Alies", "ShowUnitName", "False"));
            UIbuff.ShowUnitStatus = Convert.ToBoolean(settingsINI.ReadINI(this.GetType().ToString() + ".UI.Alies", "ShowUnitStatus", "False"));
            AliesUI = UIbuff;

            UIbuff.ShowUnitFrame = Convert.ToBoolean(settingsINI.ReadINI(this.GetType().ToString() + ".UI.Selected", "ShowUnitFrame", "True"));
            UIbuff.ShowUnitIcon = Convert.ToBoolean(settingsINI.ReadINI(this.GetType().ToString() + ".UI.Selected", "ShowUnitIcon", "True"));
            UIbuff.ShowUnitName = Convert.ToBoolean(settingsINI.ReadINI(this.GetType().ToString() + ".UI.Selected", "ShowUnitName", "True"));
            UIbuff.ShowUnitStatus = Convert.ToBoolean(settingsINI.ReadINI(this.GetType().ToString() + ".UI.Selected", "ShowUnitStatus", "True"));
            SelectedUI = UIbuff;

            UIbuff.ShowUnitFrame = Convert.ToBoolean(settingsINI.ReadINI(this.GetType().ToString() + ".UI.Enemy", "ShowUnitFrame", "True"));
            UIbuff.ShowUnitIcon = Convert.ToBoolean(settingsINI.ReadINI(this.GetType().ToString() + ".UI.Enemy", "ShowUnitIcon", "True"));
            UIbuff.ShowUnitName = Convert.ToBoolean(settingsINI.ReadINI(this.GetType().ToString() + ".UI.Enemy", "ShowUnitName", "False"));
            UIbuff.ShowUnitStatus = Convert.ToBoolean(settingsINI.ReadINI(this.GetType().ToString() + ".UI.Enemy", "ShowUnitStatus", "False"));
            EnemyUI = UIbuff;

            SettingsSaved = true;
        }
        public void Save()
        {
            INIHandler settingsINI = new INIHandler(Application.streamingAssetsPath + "\\settings.dat");

            settingsINI.Write(this.GetType().ToString() + ".Base", "Localisation", ((int)Localisation).ToString());
            settingsINI.Write(this.GetType().ToString() + ".Base", "SoundLevel", SoundLevel.ToString());
            settingsINI.Write(this.GetType().ToString() + ".Base", "MusicLevel", MusicLevel.ToString());
            settingsINI.Write(this.GetType().ToString() + ".Base", "StaticProportion", StaticProportion.ToString());
            StringBuilder userList = new StringBuilder();
            foreach (string u in users)
                userList.Append(u + "#");
            settingsINI.Write(this.GetType().ToString() + ".Base", "Users", userList.ToString());

            settingsINI.Write(this.GetType().ToString() + ".UI.Alies", "ShowUnitFrame", AliesUI.ShowUnitFrame.ToString());
            settingsINI.Write(this.GetType().ToString() + ".UI.Alies", "ShowUnitIcon", AliesUI.ShowUnitIcon.ToString());
            settingsINI.Write(this.GetType().ToString() + ".UI.Alies", "ShowUnitName", AliesUI.ShowUnitName.ToString());
            settingsINI.Write(this.GetType().ToString() + ".UI.Alies", "ShowUnitStatus", AliesUI.ShowUnitStatus.ToString());

            settingsINI.Write(this.GetType().ToString() + ".UI.Selected", "ShowUnitFrame", SelectedUI.ShowUnitFrame.ToString());
            settingsINI.Write(this.GetType().ToString() + ".UI.Selected", "ShowUnitIcon", SelectedUI.ShowUnitIcon.ToString());
            settingsINI.Write(this.GetType().ToString() + ".UI.Selected", "ShowUnitName", SelectedUI.ShowUnitName.ToString());
            settingsINI.Write(this.GetType().ToString() + ".UI.Selected", "ShowUnitStatus", SelectedUI.ShowUnitStatus.ToString());

            settingsINI.Write(this.GetType().ToString() + ".UI.Enemy", "ShowUnitFrame", EnemyUI.ShowUnitFrame.ToString());
            settingsINI.Write(this.GetType().ToString() + ".UI.Enemy", "ShowUnitIcon", EnemyUI.ShowUnitIcon.ToString());
            settingsINI.Write(this.GetType().ToString() + ".UI.Enemy", "ShowUnitName", EnemyUI.ShowUnitName.ToString());
            settingsINI.Write(this.GetType().ToString() + ".UI.Enemy", "ShowUnitStatus", EnemyUI.ShowUnitStatus.ToString());
        }
        public void SetDefault()
        {
            localisation = Languages.English;
            staticProportion = true;
            musicLevel = 100;
            soundLevel = 100;
            iconsScale = 0.6f;
            users = new string[0];

            aliesUISettings = new UISettings();
            aliesUISettings.ShowUnitFrame = true;
            aliesUISettings.ShowUnitIcon = true;
            aliesUISettings.ShowUnitName = false;
            aliesUISettings.ShowUnitStatus = false;

            selectedUISettings = new UISettings();
            selectedUISettings.ShowUnitFrame = true;
            selectedUISettings.ShowUnitIcon = true;
            selectedUISettings.ShowUnitName = true;
            selectedUISettings.ShowUnitStatus = true;

            enemyUISettings = new UISettings();
            enemyUISettings.ShowUnitFrame = true;
            enemyUISettings.ShowUnitIcon = true;
            enemyUISettings.ShowUnitName = false;
            enemyUISettings.ShowUnitStatus = false;
        }
        public void AddUser(string userName)
        {
            List<string> listBuff = new List<string>(users);
            listBuff.Add(userName);
            users = listBuff.ToArray();
        }
        public void RemoveUser(string userName)
        {
            List<string> listBuff = new List<string>(users);
            listBuff.Remove(userName);
            users = listBuff.ToArray();
        }
        public GameSettings Copy()
        {
            GameSettings copy = new GameSettings();
            copy.Localisation = this.Localisation;
            copy.MusicLevel = this.MusicLevel;
            copy.SoundLevel = this.SoundLevel;
            copy.StaticProportion = this.StaticProportion;
            copy.AliesUI = this.AliesUI.Copy();
            copy.SelectedUI = this.SelectedUI.Copy();
            copy.EnemyUI = this.EnemyUI.Copy();
            return copy;
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != this.GetType())
                return base.Equals(obj);
            else
            {
                return (
                       (this.Localisation == ((GameSettings)obj).Localisation)
                    && (this.SoundLevel == ((GameSettings)obj).SoundLevel)
                    && (this.MusicLevel == ((GameSettings)obj).MusicLevel)
                    && (this.StaticProportion == ((GameSettings)obj).StaticProportion)
                    && (this.AliesUI.Equals(((GameSettings)obj).AliesUI))
                    && (this.SelectedUI.Equals(((GameSettings)obj).SelectedUI))
                    && (this.EnemyUI.Equals(((GameSettings)obj).EnemyUI))
                    );
            }
        }
    }
    public struct UISettings
    {
        public bool ShowUnitName;
        public bool ShowUnitStatus;
        public bool ShowUnitIcon;
        public bool ShowUnitFrame;
        public UISettings Copy()
        {
            UISettings copy = new UISettings();
            copy.ShowUnitFrame = this.ShowUnitFrame;
            copy.ShowUnitIcon = this.ShowUnitIcon;
            copy.ShowUnitName = this.ShowUnitName;
            copy.ShowUnitStatus = this.ShowUnitStatus;
            return copy;
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != this.GetType())
                return base.Equals(obj);
            else return (this.ShowUnitName == ((UISettings)obj).ShowUnitName
                    && this.ShowUnitStatus == ((UISettings)obj).ShowUnitStatus
                    && this.ShowUnitIcon == ((UISettings)obj).ShowUnitIcon
                    && this.ShowUnitFrame == ((UISettings)obj).ShowUnitFrame);
        }
    }
    public class UserProfile
    {
        private string username;
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
                if (value != "")
                {
                    value = value.Replace("\\r\\n", ((char)13).ToString() + ((char)10).ToString());
                    dataLocal.Add(key, value);
                }
                else value = key;
            }
            return value;
        }
        public void SetText(string section, string key, string value)
        {
            dataStorage.Write(section, key, value);
        }

    }
}
