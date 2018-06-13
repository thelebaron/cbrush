using System;
using UnityEditor;
using UnityEngine;

namespace cBrush
{
    public class cLibrary : EditorWindow
    {

        public static Vector2 m_HScrollBarPos;
        public static Vector2 m_VScrollBarPos;
        public static float m_ThumbSize = 100f;
        private GUIStyle m_boxStyle;
        private GUIStyle m_HeaderStyle;
        private GUIStyle m_ContentStyle;
        private GUIStyle m_wrapStyle;

        private static cLibraryGroup m_LibraryGroup;




        public static bool showWindow;
        public static GameObject gameObject;
        public static UnityEditor.Editor gameObjectEditor;
        public static Vector2 scrollPos;
        public static string t = "This is a string inside a Scroll view!";
        private static int m_RowSize = 10;
        private static int xSize;
        private static int ySize;

        [MenuItem("Tools/Library")]
        static void ShowWindow()
        {
            GetWindow<cLibrary>("Library Editor");
        }

        void OnGUI()
        {
            SetupBoxStyle();

            GUILayout.BeginVertical();

            m_RowSize = EditorGUILayout.IntSlider("Row size", m_RowSize, 1, 15);
            m_ThumbSize = EditorGUILayout.Slider("ThumbSize", m_ThumbSize, 1f, 350f);
            GUILayout.Label("Library:");
            m_LibraryGroup =
                (cLibraryGroup) EditorGUILayout.ObjectField("", m_LibraryGroup, typeof(cLibraryGroup),
                    true); //, GUILayout.MaxWidth(90)
            if (GUILayout.Button("Sort"))
            {
                Sort();
            }

            GUILayout.EndVertical();


            if (m_LibraryGroup == null)
                return;

            GUILayout.BeginHorizontal();






            //forloop block
            /*
            for (int i = 0; i < m_LibraryGroup.m_ItemList.Count; i++)
            {
    
    
                var path = "Assets/cLibrary/Resources/ThumbnailsGenerated/" + m_LibraryGroup.m_ItemList[i].name + ".png";
    
                Texture2D assetIcon = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
    
                if (GUILayout.Button(assetIcon, GUILayout.MaxWidth(m_ThumbSize), GUILayout.MaxHeight(m_ThumbSize)))
                {
    
                }
    
            }*/

            GUILayout.BeginVertical();
            Calculate();
            for (int i = 0, y = 0; y <= ySize; y++)
            {


                GUILayout.BeginHorizontal();
                for (int x = 0; x <= xSize && i < m_LibraryGroup.m_ItemList.Count; x++, i++)
                {
                    var path = "Assets/cLibrary/Resources/ThumbnailsGenerated/" + m_LibraryGroup.m_ItemList[i].name +
                               ".png";

                    Texture2D assetIcon = (Texture2D) AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));

                    if (GUILayout.Button(assetIcon, GUILayout.MaxWidth(m_ThumbSize), GUILayout.MaxHeight(m_ThumbSize)))
                    {
                        cBrushEditor.m_GameObject = m_LibraryGroup.m_ItemList[i];
                    }

                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            /*
    
            if (GUILayout.Button("SReloadWorkingScene"))
            {
                cSceneManagement.ReloadWorkingScene();
            }
    
    
            if (GUILayout.Button("Add"))
            {
                cThumbnailer.CreateThumbnail(m_GameObject);
                //AssetDatabase.Refresh();
                //SaveSettings();
            }
            if (GUILayout.Button("Edit"))
            {
                LoadSettings();
            }
    
    
            /// Row
            GUILayout.BeginHorizontal("", GUIStyle.none);
            m_GameObject = (GameObject)EditorGUILayout.ObjectField("", m_GameObject, typeof(GameObject), true, GUILayout.MaxWidth(90));
            GUILayout.Label("Radius:", GUILayout.MaxWidth(50));
            m_Radius = EditorGUILayout.Slider("", m_Radius, 1f, 50f, GUILayout.MaxWidth(120));
            GUILayout.Label("Intensity:", GUILayout.MaxWidth(55));
            m_PaintIntensity = EditorGUILayout.IntSlider("", m_PaintIntensity, 1, 100, GUILayout.MaxWidth(120));
            GUILayout.Label("Focal Shift:", GUILayout.MaxWidth(70));
            m_FocalShift = EditorGUILayout.Slider("", m_FocalShift, -1, 1, GUILayout.MaxWidth(120));
            m_CumulativeProbability = EditorGUILayout.CurveField("", m_CumulativeProbability, GUILayout.MaxWidth(40));
    
            GUILayout.Label("Paint surface:", GUILayout.MaxWidth(80));
            m_PaintSurface = (PaintSurface)EditorGUILayout.EnumPopup("", m_PaintSurface, GUILayout.MaxWidth(55));
            GUILayout.Label("Record session", GUILayout.MaxWidth(90));
            m_RecordSession = EditorGUILayout.Toggle("", m_RecordSession, GUILayout.MaxWidth(40));
    
            GUILayout.EndHorizontal();
    
            /// Row 
            GUILayout.BeginHorizontal("", GUIStyle.none);
            GUILayout.Label("Size:", GUILayout.MaxWidth(40));
            m_MinSize = EditorGUILayout.FloatField("", m_MinSize, GUILayout.MaxWidth(35));
            EditorGUILayout.MinMaxSlider(ref m_MinSize, ref m_MaxSize, m_MinLimit, m_MaxLimit, GUILayout.MaxWidth(60));
            m_MaxSize = EditorGUILayout.FloatField("", m_MaxSize, GUILayout.MaxWidth(35));
    
            GUILayout.Space(20);
            GUILayout.Space(20);
            GUILayout.Label("rX:", GUILayout.MaxWidth(20));
            m_MaxRotationX = EditorGUILayout.FloatField("", m_MaxRotationX, GUILayout.MaxWidth(30));
            GUILayout.Label("rY:", GUILayout.MaxWidth(20));
            m_MaxRotationY = EditorGUILayout.FloatField("", m_MaxRotationY, GUILayout.MaxWidth(30));
            GUILayout.Label("rZ:", GUILayout.MaxWidth(20));
            m_MaxRotationZ = EditorGUILayout.FloatField("", m_MaxRotationZ, GUILayout.MaxWidth(30));
            GUILayout.EndHorizontal();
    
    
            GUILayout.BeginHorizontal("", m_StatusBarStyle);
            GUILayout.Label("Status:");
            GUILayout.Label(myStatus);
            GUILayout.EndHorizontal();
            */
        }

        private void Calculate()
        {
            var Count = m_LibraryGroup.m_ItemList.Count;
            xSize = m_RowSize;
            ySize = Count / xSize;
        }

        private void OnInspectorUpdate()
        {

        }

        private void Sort()
        {

        }

        public bool IsDivisble(int x, int n)
        {
            return (x % n) == 0;
        }

        void SetupBoxStyle()
        {
            //GUI.skin.label.normal.textColor = Color.black;
            //Set up the box style
            if (m_boxStyle == null)
            {
                m_boxStyle = new GUIStyle(GUI.skin.box);
                m_boxStyle.normal.textColor = GUI.skin.label.normal.textColor;
                m_boxStyle.fontStyle = FontStyle.Bold;
                m_boxStyle.alignment = TextAnchor.UpperLeft;
            }

            if (m_ContentStyle == null)
            {
                m_ContentStyle = new GUIStyle(GUI.skin.box);
                m_ContentStyle.normal.textColor = GUI.skin.label.normal.textColor;
                m_ContentStyle.fontStyle = FontStyle.Normal;
                m_ContentStyle.alignment = TextAnchor.UpperLeft;

            }

            //Setup the wrap style
            if (m_wrapStyle == null)
            {
                m_wrapStyle = new GUIStyle(GUI.skin.label);
                m_wrapStyle.fontStyle = FontStyle.Normal;
                m_wrapStyle.wordWrap = true;
            }
        }
    }
}