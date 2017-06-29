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
    [Serializable]
    public class SerializeSettings
    {
        public Languages localisation;
        public float screenExpWidth;
        public float screenExpHeight;
        public float soundLevel;
        public float musicLevel;
    }
}
