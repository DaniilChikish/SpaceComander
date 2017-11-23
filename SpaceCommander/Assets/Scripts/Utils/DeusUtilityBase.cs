using System.Collections.Generic;
using UnityEngine;

namespace DeusUtility
{
    public static class GameObjectUtility
    {
        public static List<GameObject> GetChildObjectByTag(Transform parent, string tag)
        {
            List<GameObject> actors = new List<GameObject>();
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (child.tag == tag)
                {
                    actors.Add(child.gameObject);
                }
                if (child.childCount > 0)
                {
                   actors.AddRange(GetChildObjectByTag(child, tag));
                }
            }
            return actors;
        }
        public static List<GameObject> GetChildObjectByName(Transform parent, string name)
        {
            List<GameObject> actors = new List<GameObject>();
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (child.name == name)
                {
                    actors.Add(child.gameObject);
                }
                if (child.childCount > 0)
                {
                    actors.AddRange(GetChildObjectByName(child, name));
                }
            }
            return actors;
        }
    }
}
