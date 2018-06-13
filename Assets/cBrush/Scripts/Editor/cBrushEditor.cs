
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace cBrush
{


    [System.Serializable]
    public class cBrushEditor : EditorWindow
    {
        public static Vector2 m_HScrollBarPos;
        public static Vector2 m_VScrollBarPos;
        public static Vector2 m_CurrentGuiPosition;
        public static Vector2 m_LastGuiPosition;

        public static Vector2
            m_GuiPositionDifferential; // simple check: if x or y increasing, 1, if decreasing 0 (else 0)

        public static bool m_Init;
        public static bool m_RecordSession;
        public static BigBrushSettings m_cBrushSettings;
        public static Color m_GizmoBrushColour;
        public static Color m_GizmoBrushFocalColour;
        float sizeMultiplier = 1.0f;
        public static float m_Radius;
        public static int m_Intensity = 25;
        public static float m_FocalShift = 0;
        public static float m_FocalShiftMinLimit = 0.0f;
        public static float m_FocalShiftMaxLimit = 1.0f;
        public static float m_FocalShiftMinSize = 0.0f;
        public static float m_FocalShiftMaxSize = 1.0f;


        public static int m_PaintIntensity;
        public static float m_BrushSize = 1f;
        public static float m_MinLimit = 0f;
        public static float m_MaxLimit = 1f;

        public static float m_MinSize = 0.5f;
        public static float m_MaxSize = 1f;
        public static Vector3 m_PainterPosition;
        public static Vector3 m_NormalDirection;
        public static RaycastHit m_RayCastHitInfo;
        public static string myString;
        public static string myStatus;
        public DataHolder dataHolder = null;
        public static GameObject m_GameObject = null;
        public static bool m_PaintMode;
        public static bool m_GatherMode;
        public static GameObject m_DragObject;
        public static bool m_DragObjectRelease;
        public static Vector3 m_LastPosition;
        public static Vector3 m_LastScale;
        public static Quaternion m_LastRotation;

        public static bool m_UseNormalDirection = true;
        public static bool m_Randomize = false;
        public static bool m_RandomizePosition;
        public static bool m_RandomizeRotation;
        public static bool m_RandomizeScale;
        public static float m_MaxRotationX;
        public static float m_MaxRotationY;
        public static float m_MaxRotationZ;
        public static AnimationCurve m_CumulativeProbability;
        public static float m_DepthOffset;

        private GUIStyle m_boxStyle;
        private GUIStyle m_HeaderStyle;
        private GUIStyle m_ContentStyle;
        private GUIStyle m_wrapStyle;
        private GUIStyle m_HorizontalStyle;
        private GUIStyle m_StatusBarStyle;
        public static BigBrushSessionRecorder m_BigBrushSessionRecorder;
        public static int m_BrushModeNum = 0;
        public static string[] m_BrushMode = new string[] {"Paint", "Gather", "Select",};

        public enum StrokeOptions
        {
            Dots = 0,
            Spray = 1,
            DragDot = 2,
            DragRect = 3,
            Freehand = 4
        };

        public static StrokeOptions m_StrokeOptions;

        public enum PaintSurface
        {
            All,
            Selected
        };

        public static PaintSurface m_PaintSurface;

        // Controls section
        public static bool m_HotKeyPosition;
        public static bool m_HotKeyRotation;
        public static bool m_HotKeyScale;
        public static bool m_HotKeyShift;
        public static bool m_HotKeyControl;

        //Icons
        public static GUISkin _editorSkin;
        public static Texture2D buttonIconBrushMode;
        public static Texture2D buttonIconNormal;
        public static Texture2D buttonIconRandomize;
        public static Texture2D buttonIconStroke;
        public static Texture2D buttonIconRandomizePosition;
        public static Texture2D buttonIconRandomizeRotation;
        public static Texture2D buttonIconRandomizeScale;
        public static Texture2D buttonIconGrid;

        //stroke vars
        public static bool m_StrokeDragDot;

        [MenuItem("Tools/cBrush %g", false, 1)]
        public static void ShowManager()
        {
            var manager = EditorWindow.GetWindow<cBrushEditor>(false, "cBrush");
            manager.Show();
        }

        [System.Serializable]
        public class DataHolder : MonoBehaviour
        {
            [SerializeField] public bool defaultBool = false;
            [SerializeField] public GameObject m_GameObject;
        }

        void OnGUI()
        {
            SetupHorizontalBoxStyle();
            SetupStatusBoxStyle();
            SetupBoxStyle();


            /// Row 
            GUILayout.BeginHorizontal("", m_HorizontalStyle);
            //GUILayout.Height(175);
            if (GUI.Button(new Rect(10, 10, 25, 25), buttonIconBrushMode, GUIStyle.none))
            {
                ToggleBrushMode();
                UpdateBrushMode();
            }

            if (GUI.Button(new Rect(40, 10, 25, 25), buttonIconNormal, GUIStyle.none))
            {
                m_UseNormalDirection = !m_UseNormalDirection;
                UpdateBrushNormal();
            }

            if (GUI.Button(new Rect(70, 10, 25, 25), buttonIconStroke, GUIStyle.none))
            {
                ToggleStrokeMode();
                UpdateBrushStroke();
            }

            if (GUI.Button(new Rect(100, 10, 25, 25), buttonIconRandomizePosition, GUIStyle.none))
            {
                m_RandomizePosition = !m_RandomizePosition;
                UpdateBrushRandomizePosition();
            }

            if (GUI.Button(new Rect(130, 10, 25, 25), buttonIconRandomizeRotation, GUIStyle.none))
            {
                m_RandomizeRotation = !m_RandomizeRotation;
                UpdateBrushRandomizeRotation();
            }

            if (GUI.Button(new Rect(160, 10, 25, 25), buttonIconRandomizeScale, GUIStyle.none))
            {
                m_RandomizeScale = !m_RandomizeScale;
                UpdateBrushRandomizeScale();
            }

            if (GUI.Button(new Rect(190, 10, 25, 25), buttonIconGrid, GUIStyle.none))
            {
                cGridUtility.ShowGrid = !cGridUtility.ShowGrid;
                UpdateGrid();
            }


            GUILayout.Space(125);
            GUILayout.Space(125);
            GUILayout.Space(125);
            GUILayout.Space(125);

            if (GUILayout.Button("SNewScene"))
            {
                cSceneManagement.NewScene();
            }

            if (GUILayout.Button("SReloadWorkingScene"))
            {
                cSceneManagement.ReloadWorkingScene();
            }

            GUILayout.Label("MultiObject:", GUILayout.MaxWidth(50));
            m_GameObject = (GameObject) EditorGUILayout.ObjectField("", m_GameObject, typeof(GameObject), true,
                GUILayout.MaxWidth(90));
            if (GUILayout.Button("Add"))
            {
                cBrushThumbnailer.CreateThumbnail(m_GameObject);
                //cThumbnailer.CreateThumbnail()
                //AssetDatabase.Refresh();
                //SaveSettings();
            }

            if (GUILayout.Button("Edit"))
            {
                LoadSettings();
            }

            GUILayout.EndHorizontal();

            /// Row
            GUILayout.BeginHorizontal("", GUIStyle.none);
            m_GameObject = (GameObject) EditorGUILayout.ObjectField("", m_GameObject, typeof(GameObject), true,
                GUILayout.MaxWidth(90));
            GUILayout.Label("Radius:", GUILayout.MaxWidth(50));
            m_Radius = EditorGUILayout.Slider("", m_Radius, 1f, 50f, GUILayout.MaxWidth(120));
            GUILayout.Label("Intensity:", GUILayout.MaxWidth(55));
            m_PaintIntensity = EditorGUILayout.IntSlider("", m_PaintIntensity, 1, 100, GUILayout.MaxWidth(120));
            GUILayout.Label("Focal Shift:", GUILayout.MaxWidth(70));
            m_FocalShift = EditorGUILayout.Slider("", m_FocalShift, -1, 1, GUILayout.MaxWidth(120));
            m_CumulativeProbability = EditorGUILayout.CurveField("", m_CumulativeProbability, GUILayout.MaxWidth(40));

            GUILayout.Label("Paint surface:", GUILayout.MaxWidth(80));
            m_PaintSurface = (PaintSurface) EditorGUILayout.EnumPopup("", m_PaintSurface, GUILayout.MaxWidth(55));
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

        }



        void OnEnable()
        {
            InitialiseBigBrushEngine();
            if (dataHolder == null) dataHolder = new DataHolder();
        }

        void InitialiseBigBrushEngine()
        {
            if (!m_Init)
            {
                m_Init = true;
                m_GizmoBrushColour = Color.green;
                m_GizmoBrushFocalColour = Color.blue;
                m_CumulativeProbability = AnimationCurve.Linear(0, 0, 10, 10);

                bool init = false;
                if (EditorApplication.timeSinceStartup < 1 && !init)
                    ScriptableObjectUtility.CreateAssetPath<BigBrushSessionRecorder>(
                        "Assets/Resources/BigBrushSession/");

                BigBrushSettings m_BigBrushSettings =
                    (BigBrushSettings) AssetDatabase.LoadAssetAtPath("Assets/BigBrush/DefaultBigBrushSettings.asset",
                        typeof(BigBrushSettings));
                myStatus = "Welcome!";

                UpdateBrushMode();
                UpdateBrushNormal();
                UpdateBrushRandomizePosition();
                UpdateBrushRandomizeRotation();
                UpdateBrushRandomizeScale();
                UpdateBrushStroke();
                UpdateGrid();

            }
        }

        void OnSceneGUI()
        {
            EditorGUI.BeginChangeCheck();
            Debug.Log("OnSceneGUI");

            Handles.BeginGUI();
            if (GUILayout.Button("Foo"))
            {
                Debug.Log("Bar");
            }

            Handles.EndGUI();

            Handles.color = Color.red;
            Handles.DrawSolidArc(m_PainterPosition, Vector3.up, -Vector3.right, 180, 3);

            EditorGUI.EndChangeCheck();
        }

        private void OnInspectorUpdate()
        {
            /*
                    UpdateBrushMode();
                    UpdateBrushNormal();
                    UpdateBrushRandomizePosition();
                    UpdateBrushRandomizeRotation();
                    UpdateBrushRandomizeScale();
                    UpdateBrushStroke();*/
        }


        private void UpdateGrid()
        {

            if (cGridUtility.ShowGrid)
            {
                buttonIconGrid =
                    (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/cBrush/Resources/Icons/cbrush_ico_grid_on.png",
                        typeof(Texture2D));
                myStatus = "Grid on";
            }
            else
            {
                buttonIconGrid =
                    (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/cBrush/Resources/Icons/cbrush_ico_grid_off.png",
                        typeof(Texture2D));
                myStatus = "Grid off";
            }

            OnInspectorUpdate();
        }


        private void UpdateBrushStroke()
        {
            switch (m_StrokeOptions)
            {
                case StrokeOptions.Freehand:
                {
                    buttonIconStroke = (Texture2D) AssetDatabase.LoadAssetAtPath(
                        "Assets/cBrush/Resources/Icons/cbrush_ico_Stroke_Freehand.png", typeof(Texture2D));
                    myStatus = "Stoke set to Freehand";
                    break;
                }
                case StrokeOptions.DragRect:
                {
                    buttonIconStroke = (Texture2D) AssetDatabase.LoadAssetAtPath(
                        "Assets/cBrush/Resources/Icons/cbrush_ico_Stroke_Dragrect.png", typeof(Texture2D));
                    myStatus = "Stoke set to DragRect";
                    break;
                }
                case StrokeOptions.DragDot:
                {
                    buttonIconStroke = (Texture2D) AssetDatabase.LoadAssetAtPath(
                        "Assets/cBrush/Resources/Icons/cbrush_ico_Stroke_DragDot.png", typeof(Texture2D));
                    myStatus = "Stoke set to DragDot";
                    break;
                }
                case StrokeOptions.Spray:
                {
                    buttonIconStroke =
                        (Texture2D) AssetDatabase.LoadAssetAtPath(
                            "Assets/cBrush/Resources/Icons/cbrush_ico_Stroke_Spray.png", typeof(Texture2D));
                    myStatus = "Stoke set to Spray";
                    break;
                }
                case StrokeOptions.Dots:
                {
                    buttonIconStroke =
                        (Texture2D) AssetDatabase.LoadAssetAtPath(
                            "Assets/cBrush/Resources/Icons/cbrush_ico_Stroke_Dots.png", typeof(Texture2D));
                    myStatus = "Stoke set to Dots";
                    break;
                }
            }
        }

        private void UpdateBrushNormal()
        {
            switch (m_UseNormalDirection)
            {
                case true:
                {
                    buttonIconNormal =
                        (Texture2D) AssetDatabase.LoadAssetAtPath(
                            "Assets/cBrush/Resources/Icons/cbrush_ico_Normal_On.png", typeof(Texture2D));
                    myStatus = "Use normal direction on";
                    break;
                }
                case false:
                {
                    buttonIconNormal =
                        (Texture2D) AssetDatabase.LoadAssetAtPath(
                            "Assets/cBrush/Resources/Icons/cbrush_ico_Normal_Off.png", typeof(Texture2D));
                    myStatus = "Use normal direction off";
                    break;
                }
            }
        }

        private void UpdateBrushRandomizePosition()
        {
            switch (m_RandomizePosition)
            {
                case true:
                {
                    buttonIconRandomizePosition = (Texture2D) AssetDatabase.LoadAssetAtPath(
                        "Assets/cBrush/Resources/Icons/cbrush_ico_randomize_pos_on.png", typeof(Texture2D));
                    myStatus = "Randomize position on";
                    break;
                }
                case false:
                {
                    buttonIconRandomizePosition = (Texture2D) AssetDatabase.LoadAssetAtPath(
                        "Assets/cBrush/Resources/Icons/cbrush_ico_randomize_pos_off.png", typeof(Texture2D));
                    myStatus = "Randomize position off";
                    break;
                }
            }
        }

        private void UpdateBrushRandomizeRotation()
        {
            switch (m_RandomizeRotation)
            {
                case true:
                {
                    buttonIconRandomizeRotation = (Texture2D) AssetDatabase.LoadAssetAtPath(
                        "Assets/cBrush/Resources/Icons/cbrush_ico_randomize_rotation_on.png", typeof(Texture2D));
                    myStatus = "Randomize rotation on";
                    break;
                }
                case false:
                {
                    buttonIconRandomizeRotation = (Texture2D) AssetDatabase.LoadAssetAtPath(
                        "Assets/cBrush/Resources/Icons/cbrush_ico_randomize_rotation_off.png", typeof(Texture2D));
                    myStatus = "Randomize rotation off";
                    break;
                }
            }
        }

        private void UpdateBrushRandomizeScale()
        {
            switch (m_RandomizeScale)
            {
                case true:
                {
                    buttonIconRandomizeScale = (Texture2D) AssetDatabase.LoadAssetAtPath(
                        "Assets/cBrush/Resources/Icons/cbrush_ico_randomize_scale_on.png", typeof(Texture2D));
                    myStatus = "Randomize scale on";
                    break;
                }
                case false:
                {
                    buttonIconRandomizeScale = (Texture2D) AssetDatabase.LoadAssetAtPath(
                        "Assets/cBrush/Resources/Icons/cbrush_ico_randomize_scale_off.png", typeof(Texture2D));
                    myStatus = "Randomize scale off";
                    break;
                }
            }
        }

        private void UpdateBrushMode()
        {
            switch (m_BrushModeNum)
            {
                case 2:
                {
                    buttonIconBrushMode =
                        (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/cBrush/Resources/Icons/cbrush_ico_Select.png",
                            typeof(Texture2D));
                    m_PaintMode = false;
                    myStatus = "Select";
                    break;
                }
                case 1:
                {
                    buttonIconBrushMode =
                        (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/cBrush/Resources/Icons/cbrush_ico_Nudge.png",
                            typeof(Texture2D));
                    m_PaintMode = false;
                    myStatus = "Nudge";
                    break;
                }
                default:
                {
                    buttonIconBrushMode =
                        (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/cBrush/Resources/Icons/cbrush_ico_Paint.png",
                            typeof(Texture2D));
                    m_PaintMode = true;
                    myStatus = "Paint";
                    break;
                }

            }
        }

        void ToggleBrushMode()
        {
            m_BrushModeNum += 1;

            if (m_BrushModeNum > 2)
                m_BrushModeNum = 0;
            //OnInspectorUpdate();
        }

        void ToggleStrokeMode()
        {
            m_StrokeOptions = m_StrokeOptions.Next();

            //OnInspectorUpdate();
        }

        private static void OnScene(SceneView sceneview)
        {

            EditorGUI.BeginChangeCheck();

            Handles.BeginGUI();

            Handles.EndGUI();


            //Draws the brush circle
            DrawBigBrushGizmo();

            if (Event.current.alt == true)
            {

                myStatus = "Alt";
                return;

            }

            Event e = Event.current;


            if (Event.current.shift)
            {
                m_HotKeyShift = true;
            }
            else
            {
                m_HotKeyShift = false;
            }

            if (Event.current.control)
            {
                m_HotKeyControl = true;
            }
            else
            {
                m_HotKeyControl = false;
            }


            /*
             * //unreal mousemove old stuff
            //if (Event.current.capsLock == true) 
            if (Event.current.type == EventType.MouseDrag && Event.current.button == 0)
            {
                Vector2 guiPosition = Event.current.mousePosition;
    
                
                var c = SceneView.GetAllSceneCameras();
                Vector3 position = SceneView.lastActiveSceneView.pivot;
    
                Quaternion direction = SceneView.lastActiveSceneView.rotation;
                Vector3 targetForward = direction * Vector3.forward;
                targetForward.x = position.x;
                targetForward.z = position.z;
                targetForward.y = 0;
                position += (targetForward * 1f);
    
                SceneView.lastActiveSceneView.pivot = position;
                SceneView.lastActiveSceneView.Repaint();
            }
            */


            RaycastHit hit;
            if (Event.current.type != EventType.MouseUp)
            {
                m_CurrentGuiPosition = Event.current.mousePosition;
                if (m_LastGuiPosition != m_CurrentGuiPosition)
                {
                    if (m_CurrentGuiPosition.x > m_LastGuiPosition.x)
                    {
                        m_GuiPositionDifferential.x = 1;
                    }

                    if (m_CurrentGuiPosition.x < m_LastGuiPosition.x)
                    {
                        m_GuiPositionDifferential.x = -1;
                    }

                    if (m_CurrentGuiPosition.y > m_LastGuiPosition.y)
                    {
                        m_GuiPositionDifferential.y = 1;
                    }

                    if (m_CurrentGuiPosition.y < m_LastGuiPosition.y)
                    {
                        m_GuiPositionDifferential.y = -1;
                    }

                }

                m_LastGuiPosition = m_CurrentGuiPosition;

                Ray ray = HandleUtility.GUIPointToWorldRay(m_CurrentGuiPosition);
                if (Physics.Raycast(ray, out hit))
                {
                    m_RayCastHitInfo = hit;
                    m_PainterPosition = hit.point;

                    if (m_UseNormalDirection)
                        m_NormalDirection = hit.normal;
                    else
                        m_NormalDirection = Vector3.up;

                    UpdatePaintDrag();

                    //force scene update
                    HandleUtility.Repaint();
                }
            }

            //Event.current.type == EventType.MouseDrag && Event.current.button == 0
            if (Event.current.type == EventType.MouseDrag && m_StrokeOptions == StrokeOptions.Freehand)
            {
                StrokeFreehand();
            }

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && m_PaintMode)
            {
                if (m_StrokeOptions == StrokeOptions.Dots)
                {
                    StrokeDots();
                }

                if (m_StrokeOptions == StrokeOptions.DragDot)
                {
                    StrokeDragDot();
                }

                if (m_StrokeOptions == StrokeOptions.Freehand)
                {
                    StrokeFreehand();
                }


            }




            /*
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                Vector2 guiPosition = Event.current.mousePosition;
                Ray ray = HandleUtility.GUIPointToWorldRay(guiPosition);
                if (Physics.Raycast(ray, out hit))
                {
                    m_PainterPosition = hit.point;
                    Debug.Log("Found an object - distance: " + hit.distance);
                    HandleUtility.Repaint(); //force scene update
                }
            }
            */
            //Debug.Log("This event opens up so many possibilities.");

            if (m_PaintMode)
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));


            EditorGUI.EndChangeCheck();
        }


        private static void UpdatePaintDrag()
        {
            if (m_DragObject != null)
            {
                m_DragObject.transform.position = m_PainterPosition;
                m_DragObject.transform.rotation =
                    Quaternion.FromToRotation(m_DragObject.transform.up, m_NormalDirection) *
                    m_DragObject.transform.rotation;

                if (m_HotKeyShift && Tools.current.ToString() == "Scale")
                {
                    m_DragObject.transform.localScale += new Vector3(0.01f, 0.01f, 0.01f);
                }

                if (m_HotKeyControl && Tools.current.ToString() == "Scale")
                {
                    m_DragObject.transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
                }

                if (m_HotKeyShift && Tools.current.ToString() == "Rotate")
                {
                    m_DragObject.transform.Rotate(Vector3.up * (Time.deltaTime + 1), Space.World);
                }

                if (m_HotKeyControl && Tools.current.ToString() == "Rotate")
                {
                    m_DragObject.transform.Rotate(Vector3.up * -(Time.deltaTime + 1), Space.World);
                }
            }
        }


        private static void StrokeFreehand()
        {
            //clear out any drag paint object
            if (m_DragObject != null)
            {
                m_DragObject.layer = 0;
                m_DragObject = null;
                DestroyImmediate(m_DragObject);
            }

            if (m_GameObject != null)
            {
                for (int i = 0; i < m_PaintIntensity; i++)
                {
                    var go = Instantiate(m_GameObject, m_PainterPosition, Quaternion.identity);
                    var tag = go.AddComponent<BigBrushTag>() as BigBrushTag;
                    tag.hideFlags = HideFlags.HideAndDontSave;
                    //void record session
                    //m_BigBrushSessionRecorder.SessionObjects.Add(go);

                    if (m_UseNormalDirection)
                        go.transform.rotation = Quaternion.FromToRotation(go.transform.up, m_NormalDirection) *
                                                go.transform.rotation;

                    var randomradius = RandomCurve(m_Radius);
                    //var x = RandomCurve(go.transform.position.x);
                    var y = go.transform.position.y;
                    //var z = RandomCurve(go.transform.position.z);
                    var pos = go.transform.position + UnityEngine.Random.insideUnitSphere * randomradius;
                    pos.y = y;

                    go.transform.position = pos;

                    AwesomeExtensions.SetObjectRotation(go.transform, m_RandomizeRotation, m_MaxRotationX,
                        m_MaxRotationY, m_MaxRotationZ);
                    AwesomeExtensions.SetObjectScale(go.transform, m_RandomizeScale, m_MinSize, m_MaxSize);
                    go.layer = 0;
                }
            }

        }

        private IEnumerator FreehandStroke()
        {
            yield return new WaitForSeconds(1);
        }

        private static void StrokeDragDot()
        {
            if (m_DragObject != null)
            {
                m_DragObject.layer = 0;
                m_DragObject = null;
                return;
            }

            if (m_DragObject == null)
            {
                var go = Instantiate(m_GameObject, m_PainterPosition, Quaternion.identity);
                var tag = go.AddComponent<BigBrushTag>() as BigBrushTag;
                tag.hideFlags = HideFlags.HideAndDontSave;
                m_DragObject = go;
                m_DragObject.layer = 2;
            }

        }

        private static void StrokeDots()
        {
            //clear out any drag paint object
            if (m_DragObject != null)
            {
                m_DragObject.layer = 0;
                m_DragObject = null;
                DestroyImmediate(m_DragObject);
            }

            if (m_GameObject != null)
            {
                for (int i = 0; i < m_PaintIntensity; i++)
                {
                    var go = Instantiate(m_GameObject, m_PainterPosition, Quaternion.identity);
                    var tag = go.AddComponent<BigBrushTag>() as BigBrushTag;
                    tag.hideFlags = HideFlags.HideAndDontSave;
                    //void record session
                    //m_BigBrushSessionRecorder.SessionObjects.Add(go);

                    if (m_UseNormalDirection)
                        go.transform.rotation = Quaternion.FromToRotation(go.transform.up, m_NormalDirection) *
                                                go.transform.rotation;

                    var randomradius = RandomCurve(m_Radius);
                    //var x = RandomCurve(go.transform.position.x);
                    var y = go.transform.position.y;
                    //var z = RandomCurve(go.transform.position.z);
                    var pos = go.transform.position + UnityEngine.Random.insideUnitSphere * randomradius;
                    pos.y = y;

                    go.transform.position = pos;

                    AwesomeExtensions.SetObjectRotation(go.transform, m_RandomizeRotation, m_MaxRotationX,
                        m_MaxRotationY, m_MaxRotationZ);
                    AwesomeExtensions.SetObjectScale(go.transform, m_RandomizeScale, m_MinSize, m_MaxSize);
                    go.layer = 0;
                }
            }
        }

        public void OnDestroy()
        {
            SceneView.onSceneGUIDelegate -= OnScene;
        }

        public void Update()
        {
            if (SceneView.onSceneGUIDelegate == null)
            {
                SceneView.onSceneGUIDelegate += OnScene;
                //Any other initialization code you would
                //normally place in Init() goes here instead.
            }

        }

        public static float RandomCurve(float num)
        {
            return num * (float) m_CumulativeProbability.Evaluate(UnityEngine.Random.value);
        }

        public static void DrawBigBrushGizmo()
        {
            if (m_PaintMode)
            {

                Handles.color = m_GizmoBrushColour;

                //Quaternion.LookRotation(new Vector3(0, 180, 1)) flat circle
                var thicken1 = m_Radius * 1.005f;
                var thicken2 = m_Radius * 1.01f;
                var thicken3 = m_Radius * 1.02f;
                var thicken4 = m_Radius * 1.03f;
                var thicken11 = m_Radius * 1.005f;
                var thicken21 = m_Radius * 1.015f;
                var thicken31 = m_Radius * 1.025f;
                var thicken41 = m_Radius * 1.035f;

                if (m_NormalDirection == Vector3.zero)
                    m_NormalDirection = Vector3.up;

                //show a line thats always up
                var lineEndUpPos = m_PainterPosition + Vector3.up;
                Handles.DrawLine(m_PainterPosition, lineEndUpPos);

                //show a line thats always reflects normal direction, to help gauge space
                var lineEndPos = m_PainterPosition + m_RayCastHitInfo.normal;
                Handles.DrawLine(m_PainterPosition, lineEndPos);

                Handles.CircleHandleCap(0, m_PainterPosition, Quaternion.LookRotation(m_NormalDirection), m_Radius,
                    EventType.Repaint);
                Handles.CircleHandleCap(0, m_PainterPosition, Quaternion.LookRotation(m_NormalDirection), m_Radius,
                    EventType.Repaint);
                Handles.CircleHandleCap(0, m_PainterPosition, Quaternion.LookRotation(m_NormalDirection), m_Radius,
                    EventType.Repaint);
                Handles.CircleHandleCap(0, m_PainterPosition, Quaternion.LookRotation(m_NormalDirection), thicken1,
                    EventType.Repaint);
                Handles.CircleHandleCap(0, m_PainterPosition, Quaternion.LookRotation(m_NormalDirection), thicken2,
                    EventType.Repaint);
                Handles.CircleHandleCap(0, m_PainterPosition, Quaternion.LookRotation(m_NormalDirection), thicken3,
                    EventType.Repaint);
                Handles.CircleHandleCap(0, m_PainterPosition, Quaternion.LookRotation(m_NormalDirection), thicken4,
                    EventType.Repaint);
                Handles.CircleHandleCap(0, m_PainterPosition, Quaternion.LookRotation(m_NormalDirection), thicken11,
                    EventType.Repaint);
                Handles.CircleHandleCap(0, m_PainterPosition, Quaternion.LookRotation(m_NormalDirection), thicken21,
                    EventType.Repaint);
                Handles.CircleHandleCap(0, m_PainterPosition, Quaternion.LookRotation(m_NormalDirection), thicken31,
                    EventType.Repaint);
                Handles.CircleHandleCap(0, m_PainterPosition, Quaternion.LookRotation(m_NormalDirection), thicken41,
                    EventType.Repaint);


                //Handles.DrawSolidArc(m_PainterPosition, Vector3.up, -Vector3.right, 180, 3);

                Handles.color = m_GizmoBrushFocalColour;
                var focalSize = 0.5f;

                if (AwesomeExtensions.IsPositive(m_FocalShift))
                {
                    float OldMax = 1;
                    float OldMin = 0.1f;
                    float NewMax = 1;
                    float NewMin = 0.5f;
                    float OldRange = (OldMax - OldMin);
                    float NewRange = (NewMax - NewMin);
                    float NewValue = (((m_FocalShift - OldMin) * NewRange) / OldRange) + NewMin;
                    Debug.Log(NewValue);

                    focalSize = NewValue;
                }

                if (AwesomeExtensions.IsNegative(m_FocalShift))
                {
                    //float OldMax = 1; float OldMin = 0.1f; float NewMax = 0.5f; float NewMin = 0.01f;
                    float OldMax = 0;
                    float OldMin = -1f;
                    float NewMax = 0.5f;
                    float NewMin = 0f;
                    float OldRange = (OldMax - OldMin);
                    float NewRange = (NewMax - NewMin);
                    float NewValue = (((m_FocalShift - OldMin) * NewRange) / OldRange) + NewMin;
                    Debug.Log(NewValue);
                    //float remap = 0.5f - NewValue;
                    focalSize = NewValue;
                }




                focalSize = m_Radius * focalSize;
                //focalSize += m_FocalShift;

                //  (m_Radius * 0.5f) * (m_FocalShift * 1);
                //Debug.Log(focalSize);

                Handles.CircleHandleCap(0, m_PainterPosition, Quaternion.LookRotation(m_NormalDirection), focalSize,
                    EventType.Repaint);
            }

        }


        private void LoadSettings()
        {
            throw new NotImplementedException();
        }

        private void SaveSettings()
        {
            throw new NotImplementedException();
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

        void SetupHorizontalBoxStyle()
        {
            //GUI.skin.label.normal.textColor = Color.black;
            //Set up the box style
            if (m_HorizontalStyle == null)
            {
                m_HorizontalStyle = new GUIStyle(GUI.skin.box);
                m_HorizontalStyle.normal.textColor = GUI.skin.label.normal.textColor;
                m_HorizontalStyle.fontStyle = FontStyle.Bold;
                m_HorizontalStyle.alignment = TextAnchor.UpperLeft;
                m_HorizontalStyle.fixedHeight = 45;
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

        void SetupStatusBoxStyle()
        {
            //GUI.skin.label.normal.textColor = Color.black;
            //Set up the box style
            if (m_StatusBarStyle == null)
            {
                m_StatusBarStyle = new GUIStyle(GUI.skin.box);
                m_StatusBarStyle.normal.textColor = GUI.skin.label.normal.textColor;
                m_StatusBarStyle.fontStyle = FontStyle.Bold;
                m_StatusBarStyle.alignment = TextAnchor.UpperLeft;
                m_StatusBarStyle.fixedHeight = 1;
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

