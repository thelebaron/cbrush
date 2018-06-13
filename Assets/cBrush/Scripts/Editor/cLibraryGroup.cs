using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace cBrush
{
    public class cLibraryGroup : ScriptableObject
    {

        public GameObject ObjectToAdd;
        public string GroupName = "Library Group";
        public List<GameObject> m_ItemList = null;
        public List<Texture2D> m_ThumbList = null;
        public List<String> m_ThumbPathList = null;
        public Dictionary<GameObject, Texture2D> m_GroupList = null;

        public void Add()
        {
            m_ItemList.Add(ObjectToAdd);
            ObjectToAdd = null;
        }

        public void Reset()
        {
            m_ItemList = new List<GameObject>();
            m_GroupList = new Dictionary<GameObject, Texture2D>();
            m_ThumbList = new List<Texture2D>();
            m_ThumbPathList = new List<String>();
        }

        public void MakeThumbnails()
        {
            //cThumbnailer.CreateThumbnails(m_ItemList);
            for (int i = 0; i < m_ItemList.Count; i++)
            {
                cBrushThumbnailer.CreateThumbnail(m_ItemList[i].gameObject);
            }

            SetupDic();
        }

        public void SetupDic()
        {

            m_GroupList = new Dictionary<GameObject, Texture2D>();
            m_ThumbList = new List<Texture2D>();
            m_ThumbPathList = new List<String>();
            for (int i = 0; i < m_ItemList.Count; i++)
            {
                var path = "Assets/cLibrary/Resources/ThumbnailsGenerated/" + m_ItemList[i].name + ".png";
                Texture2D assetIcon = (Texture2D) AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
                m_ThumbPathList.Add(path);
                m_ThumbList.Add(assetIcon);
                m_GroupList.Add(m_ItemList[i].gameObject, assetIcon);
            }
        }

        public void ParseDirectory()
        {
            var myPath = AssetDatabase.GetAssetPath(this);
            myPath = Path.GetDirectoryName(myPath);

            DirectoryInfo dir = new DirectoryInfo(myPath);
            FileInfo[] info = dir.GetFiles("*.prefab");

            foreach (FileInfo f in info)
            {
                string n = f.Name;
                n = myPath + "/" + f.Name;
                Debug.Log(n);
                GameObject asset = (GameObject) AssetDatabase.LoadAssetAtPath(n, typeof(GameObject));
                m_ItemList.Add(asset);
            }



        }
    }
}
