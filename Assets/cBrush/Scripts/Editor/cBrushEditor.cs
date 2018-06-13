
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
    public class CBrushEditor : EditorWindow
    {
        public static Vector2 GuiPositionDifferential;
        public static Vector2 CurrentGuiPosition;
        public static Vector2 LastGuiPosition;

        public static bool Init;
        public static bool RecordSession;
        public static Color GizmoBrushColour;
        public static Color GizmoBrushFocalColour;
        float m_SizeMultiplier = 1.0f;
        public static float Radius;
        public static int Intensity = 25;
        public static float FocalShift = 0;


        public static int PaintIntensity;
        public const float MinLimit = 0f;
        public const float MaxLimit = 1f;

        public static float MinSize = 0.5f;
        public static float MaxSize = 1f;
        public static Vector3 PainterPosition;
        public static Vector3 NormalDirection;
        public static RaycastHit RayCastHitInfo;
        public static string MyStatus;
        public DataHolder dataHolder;
        public static GameObject GameObject;
        public static bool PaintMode;
        public static GameObject DragObject;

        public static bool UseNormalDirection = true;
        public static bool RandomizePosition;
        public static bool RandomizeRotation;
        public static bool RandomizeScale;
        public static float MaxRotationX;
        public static float MaxRotationY;
        public static float MaxRotationZ;
        public static AnimationCurve CumulativeProbability;

        private GUIStyle m_BoxStyle;
        private GUIStyle m_HeaderStyle;
        private GUIStyle m_ContentStyle;
        private GUIStyle m_WrapStyle;
        private GUIStyle m_HorizontalStyle;
        private GUIStyle m_StatusBarStyle;
        public static int BrushModeNum;

        public enum StrokeOptions
        {
            Dots = 0,
            Spray = 1,
            DragDot = 2,
            DragRect = 3,
            Freehand = 4
        };

        public static StrokeOptions m_StrokeOptions;

        public enum PaintSurfaceType
        {
            All,
            Selected
        };

        public static PaintSurfaceType m_PaintSurface;

        // Controls section
        public static bool HotKeyShift;
        public static bool HotKeyControl;

        //Icons
        public static Texture2D ButtonIconBrushMode;
        public static Texture2D ButtonIconNormal;
        public static Texture2D ButtonIconStroke;
        public static Texture2D ButtonIconRandomizePosition;
        public static Texture2D ButtonIconRandomizeRotation;
        public static Texture2D ButtonIconRandomizeScale;
        public static Texture2D ButtonIconGrid;

        //stroke vars
        public static bool m_StrokeDragDot;

        [MenuItem("Tools/cBrush %g", false, 1)]
        public static void ShowManager()
        {
            var manager = GetWindow<CBrushEditor>(false, "cBrush");
            manager.Show();
        }

        [System.Serializable]
        public class DataHolder : MonoBehaviour
        {
            [SerializeField] public bool DefaultBool = false;
            [SerializeField] public GameObject m_GameObject;
        }

        void OnGUI()
        {
            SetupHorizontalBoxStyle();
            SetupStatusBoxStyle();
            SetupBoxStyle();


            // Row 
            GUILayout.BeginHorizontal("", m_HorizontalStyle);
            //GUILayout.Height(175);
            if (GUI.Button(new Rect(10, 10, 25, 25), ButtonIconBrushMode, GUIStyle.none))
            {
                ToggleBrushMode();
                UpdateBrushMode();
            }

            if (GUI.Button(new Rect(40, 10, 25, 25), ButtonIconNormal, GUIStyle.none))
            {
                UseNormalDirection = !UseNormalDirection;
                UpdateBrushNormal();
            }

            if (GUI.Button(new Rect(70, 10, 25, 25), ButtonIconStroke, GUIStyle.none))
            {
                ToggleStrokeMode();
                UpdateBrushStroke();
            }

            if (GUI.Button(new Rect(100, 10, 25, 25), ButtonIconRandomizePosition, GUIStyle.none))
            {
                RandomizePosition = !RandomizePosition;
                UpdateBrushRandomizePosition();
            }

            if (GUI.Button(new Rect(130, 10, 25, 25), ButtonIconRandomizeRotation, GUIStyle.none))
            {
                RandomizeRotation = !RandomizeRotation;
                UpdateBrushRandomizeRotation();
            }

            if (GUI.Button(new Rect(160, 10, 25, 25), ButtonIconRandomizeScale, GUIStyle.none))
            {
                RandomizeScale = !RandomizeScale;
                UpdateBrushRandomizeScale();
            }

            if (GUI.Button(new Rect(190, 10, 25, 25), ButtonIconGrid, GUIStyle.none))
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
            GameObject = (GameObject) EditorGUILayout.ObjectField("", GameObject, typeof(GameObject), true,
                GUILayout.MaxWidth(90));
            if (GUILayout.Button("Add(N)"))//not yet working
            {
                cBrushThumbnailer.CreateThumbnail(GameObject);
                //cThumbnailer.CreateThumbnail()
                //AssetDatabase.Refresh();
                //SaveSettings();
            }

            if (GUILayout.Button("LoadSettings(N)")) //not yet working
            {
                LoadSettings();
            }

            GUILayout.EndHorizontal();

            //
            //
            /***
             *  New Row
             */
            // Row
            GUILayout.BeginHorizontal("", GUIStyle.none);
            GameObject = (GameObject) EditorGUILayout.ObjectField("", GameObject, typeof(GameObject), true,
                GUILayout.MaxWidth(90));
            GUILayout.Label("Radius:", GUILayout.MaxWidth(50));
            Radius = EditorGUILayout.Slider("", Radius, 1f, 50f, GUILayout.MaxWidth(120));
            GUILayout.Label("Intensity:", GUILayout.MaxWidth(55));
            PaintIntensity = EditorGUILayout.IntSlider("", PaintIntensity, 1, 100, GUILayout.MaxWidth(120));
            GUILayout.Label("Focal Shift:", GUILayout.MaxWidth(70));
            FocalShift = EditorGUILayout.Slider("", FocalShift, -1, 1, GUILayout.MaxWidth(120));
            CumulativeProbability = EditorGUILayout.CurveField("", CumulativeProbability, GUILayout.MaxWidth(40));

            GUILayout.Label("Paint surface:", GUILayout.MaxWidth(80));
            m_PaintSurface = (PaintSurfaceType) EditorGUILayout.EnumPopup("", m_PaintSurface, GUILayout.MaxWidth(55));
            GUILayout.Label("Record session", GUILayout.MaxWidth(90));
            RecordSession = EditorGUILayout.Toggle("", RecordSession, GUILayout.MaxWidth(40));

            GUILayout.EndHorizontal();

            /// Row 
            GUILayout.BeginHorizontal("", GUIStyle.none);
            GUILayout.Label("Size:", GUILayout.MaxWidth(40));
            MinSize = EditorGUILayout.FloatField("", MinSize, GUILayout.MaxWidth(35));
            EditorGUILayout.MinMaxSlider(ref MinSize, ref MaxSize, MinLimit, MaxLimit, GUILayout.MaxWidth(60));
            MaxSize = EditorGUILayout.FloatField("", MaxSize, GUILayout.MaxWidth(35));

            GUILayout.Space(20);
            GUILayout.Space(20);
            GUILayout.Label("rX:", GUILayout.MaxWidth(20));
            MaxRotationX = EditorGUILayout.FloatField("", MaxRotationX, GUILayout.MaxWidth(30));
            GUILayout.Label("rY:", GUILayout.MaxWidth(20));
            MaxRotationY = EditorGUILayout.FloatField("", MaxRotationY, GUILayout.MaxWidth(30));
            GUILayout.Label("rZ:", GUILayout.MaxWidth(20));
            MaxRotationZ = EditorGUILayout.FloatField("", MaxRotationZ, GUILayout.MaxWidth(30));
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal("", m_StatusBarStyle);
            GUILayout.Label("Status:");
            GUILayout.Label(MyStatus);
            GUILayout.EndHorizontal();

        }



        void OnEnable()
        {
            InitialiseBigBrushEngine();
            SceneView.onSceneGUIDelegate += OnScene;
            //if (dataHolder == null) dataHolder = new DataHolder();
        }

        void InitialiseBigBrushEngine()
        {
            if (!Init)
            {
                Init = true;
                GizmoBrushColour = Color.green;
                GizmoBrushFocalColour = Color.blue;
                CumulativeProbability = AnimationCurve.Linear(0, 0, 10, 10);
                bool init = false;
    
                MyStatus = "Welcome!";

                UpdateBrushMode();
                UpdateBrushNormal();
                UpdateBrushRandomizePosition();
                UpdateBrushRandomizeRotation();
                UpdateBrushRandomizeScale();
                UpdateBrushStroke();
                UpdateGrid();

            }
        }

        private void OnSceneGUI()
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
            Handles.DrawSolidArc(PainterPosition, Vector3.up, -Vector3.right, 180, 3);

            EditorGUI.EndChangeCheck();
        }

        private void OnInspectorUpdate()
        {
                    //Debug.Log("OnInspectorUpdate");
            /*
                    UpdateBrushMode();
                    UpdateBrushNormal();
                    UpdateBrushRandomizePosition();
                    UpdateBrushRandomizeRotation();
                    UpdateBrushRandomizeScale();
                    UpdateBrushStroke();
                    */
        }


        private void UpdateGrid()
        {

            if (cGridUtility.ShowGrid)
            {
                ButtonIconGrid =
                    (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/cBrush/Resources/Icons/cbrush_ico_grid_on.png",
                        typeof(Texture2D));
                MyStatus = "Grid on";
            }
            else
            {
                ButtonIconGrid =
                    (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/cBrush/Resources/Icons/cbrush_ico_grid_off.png",
                        typeof(Texture2D));
                MyStatus = "Grid off";
            }

            OnInspectorUpdate();
        }


        private void UpdateBrushStroke()
        {
            switch (m_StrokeOptions)
            {
                case StrokeOptions.Freehand:
                {
                    ButtonIconStroke = (Texture2D) AssetDatabase.LoadAssetAtPath(
                        "Assets/cBrush/Resources/Icons/cbrush_ico_Stroke_Freehand.png", typeof(Texture2D));
                    MyStatus = "Stoke set to Freehand";
                    break;
                }
                case StrokeOptions.DragRect:
                {
                    ButtonIconStroke = (Texture2D) AssetDatabase.LoadAssetAtPath(
                        "Assets/cBrush/Resources/Icons/cbrush_ico_Stroke_Dragrect.png", typeof(Texture2D));
                    MyStatus = "Stoke set to DragRect";
                    break;
                }
                case StrokeOptions.DragDot:
                {
                    ButtonIconStroke = (Texture2D) AssetDatabase.LoadAssetAtPath(
                        "Assets/cBrush/Resources/Icons/cbrush_ico_Stroke_DragDot.png", typeof(Texture2D));
                    MyStatus = "Stoke set to DragDot";
                    break;
                }
                case StrokeOptions.Spray:
                {
                    ButtonIconStroke =
                        (Texture2D) AssetDatabase.LoadAssetAtPath(
                            "Assets/cBrush/Resources/Icons/cbrush_ico_Stroke_Spray.png", typeof(Texture2D));
                    MyStatus = "Stoke set to Spray";
                    break;
                }
                case StrokeOptions.Dots:
                {
                    ButtonIconStroke =
                        (Texture2D) AssetDatabase.LoadAssetAtPath(
                            "Assets/cBrush/Resources/Icons/cbrush_ico_Stroke_Dots.png", typeof(Texture2D));
                    MyStatus = "Stoke set to Dots";
                    break;
                }
            }
        }

        private void UpdateBrushNormal()
        {
            switch (UseNormalDirection)
            {
                case true:
                {
                    ButtonIconNormal =
                        (Texture2D) AssetDatabase.LoadAssetAtPath(
                            "Assets/cBrush/Resources/Icons/cbrush_ico_Normal_On.png", typeof(Texture2D));
                    MyStatus = "Use normal direction on";
                    break;
                }
                case false:
                {
                    ButtonIconNormal =
                        (Texture2D) AssetDatabase.LoadAssetAtPath(
                            "Assets/cBrush/Resources/Icons/cbrush_ico_Normal_Off.png", typeof(Texture2D));
                    MyStatus = "Use normal direction off";
                    break;
                }
            }
        }

        private void UpdateBrushRandomizePosition()
        {
            switch (RandomizePosition)
            {
                case true:
                {
                    ButtonIconRandomizePosition = (Texture2D) AssetDatabase.LoadAssetAtPath(
                        "Assets/cBrush/Resources/Icons/cbrush_ico_randomize_pos_on.png", typeof(Texture2D));
                    MyStatus = "Randomize position on";
                    break;
                }
                case false:
                {
                    ButtonIconRandomizePosition = (Texture2D) AssetDatabase.LoadAssetAtPath(
                        "Assets/cBrush/Resources/Icons/cbrush_ico_randomize_pos_off.png", typeof(Texture2D));
                    MyStatus = "Randomize position off";
                    break;
                }
            }
        }

        private void UpdateBrushRandomizeRotation()
        {
            switch (RandomizeRotation)
            {
                case true:
                {
                    ButtonIconRandomizeRotation = (Texture2D) AssetDatabase.LoadAssetAtPath(
                        "Assets/cBrush/Resources/Icons/cbrush_ico_randomize_rotation_on.png", typeof(Texture2D));
                    MyStatus = "Randomize rotation on";
                    break;
                }
                case false:
                {
                    ButtonIconRandomizeRotation = (Texture2D) AssetDatabase.LoadAssetAtPath(
                        "Assets/cBrush/Resources/Icons/cbrush_ico_randomize_rotation_off.png", typeof(Texture2D));
                    MyStatus = "Randomize rotation off";
                    break;
                }
            }
        }

        private void UpdateBrushRandomizeScale()
        {
            switch (RandomizeScale)
            {
                case true:
                {
                    ButtonIconRandomizeScale = (Texture2D) AssetDatabase.LoadAssetAtPath(
                        "Assets/cBrush/Resources/Icons/cbrush_ico_randomize_scale_on.png", typeof(Texture2D));
                    MyStatus = "Randomize scale on";
                    break;
                }
                case false:
                {
                    ButtonIconRandomizeScale = (Texture2D) AssetDatabase.LoadAssetAtPath(
                        "Assets/cBrush/Resources/Icons/cbrush_ico_randomize_scale_off.png", typeof(Texture2D));
                    MyStatus = "Randomize scale off";
                    break;
                }
            }
        }

        private void UpdateBrushMode()
        {
            
            switch (BrushModeNum)
            {
                case 2:
                {
                    ButtonIconBrushMode =
                        (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/cBrush/Resources/Icons/cbrush_ico_Select.png",
                            typeof(Texture2D));
                    PaintMode = false;
                    MyStatus = "Select";
                    DragObject = null;
                    GameObject = null;
                    break;
                }
                case 1:
                {
                    ButtonIconBrushMode =
                        (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/cBrush/Resources/Icons/cbrush_ico_Nudge.png",
                            typeof(Texture2D));
                    PaintMode = false;
                    MyStatus = "Nudge";
                    break;
                }
                default:
                {
                    
                    ButtonIconBrushMode =
                        (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/cBrush/Resources/Icons/cbrush_ico_Paint.png",
                            typeof(Texture2D));
                    PaintMode = true;
                    MyStatus = "Paint";
                    break;
                }

            }
        }

        void ToggleBrushMode()
        {
            BrushModeNum += 1;

            if (BrushModeNum > 2)
                BrushModeNum = 0;
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

                MyStatus = "Alt";
                return;

            }

            Event e = Event.current;


            if (Event.current.shift)
            {
                HotKeyShift = true;
            }
            else
            {
                HotKeyShift = false;
            }

            if (Event.current.control)
            {
                HotKeyControl = true;
            }
            else
            {
                HotKeyControl = false;
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
                CurrentGuiPosition = Event.current.mousePosition;
                if (LastGuiPosition != CurrentGuiPosition)
                {
                    if (CurrentGuiPosition.x > LastGuiPosition.x)
                    {
                        GuiPositionDifferential.x = 1;
                    }

                    if (CurrentGuiPosition.x < LastGuiPosition.x)
                    {
                        GuiPositionDifferential.x = -1;
                    }

                    if (CurrentGuiPosition.y > LastGuiPosition.y)
                    {
                        GuiPositionDifferential.y = 1;
                    }

                    if (CurrentGuiPosition.y < LastGuiPosition.y)
                    {
                        GuiPositionDifferential.y = -1;
                    }

                }

                LastGuiPosition = CurrentGuiPosition;

                Ray ray = HandleUtility.GUIPointToWorldRay(CurrentGuiPosition);
                if (Physics.Raycast(ray, out hit))
                {
                    RayCastHitInfo = hit;
                    PainterPosition = hit.point;

                    if (UseNormalDirection)
                        NormalDirection = hit.normal;
                    else
                        NormalDirection = Vector3.up;

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

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && PaintMode)
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

            if (PaintMode)
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));


            EditorGUI.EndChangeCheck();
        }


        private static void UpdatePaintDrag()
        {
            if (DragObject != null)
            {
                DragObject.transform.position = PainterPosition;
                DragObject.transform.rotation =
                    Quaternion.FromToRotation(DragObject.transform.up, NormalDirection) *
                    DragObject.transform.rotation;

                if (HotKeyShift && Tools.current.ToString() == "Scale")
                {
                    DragObject.transform.localScale += new Vector3(0.01f, 0.01f, 0.01f);
                }

                if (HotKeyControl && Tools.current.ToString() == "Scale")
                {
                    DragObject.transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
                }

                if (HotKeyShift && Tools.current.ToString() == "Rotate")
                {
                    DragObject.transform.Rotate(Vector3.up * (Time.deltaTime + 1), Space.World);
                }

                if (HotKeyControl && Tools.current.ToString() == "Rotate")
                {
                    DragObject.transform.Rotate(Vector3.up * -(Time.deltaTime + 1), Space.World);
                }
            }
        }


        private static void StrokeFreehand()
        {
            //clear out any drag paint object
            if (DragObject != null)
            {
                DragObject.layer = 0;
                DragObject = null;
                DestroyImmediate(DragObject);
            }

            if (GameObject != null)
            {
                for (int i = 0; i < PaintIntensity; i++)
                {
                    var go = Instantiate(GameObject, PainterPosition, Quaternion.identity);
                    var tag = go.AddComponent<BigBrushTag>() as BigBrushTag;
                    tag.hideFlags = HideFlags.HideAndDontSave;
                    //void record session
                    //m_BigBrushSessionRecorder.SessionObjects.Add(go);

                    if (UseNormalDirection)
                        go.transform.rotation = Quaternion.FromToRotation(go.transform.up, NormalDirection) *
                                                go.transform.rotation;

                    var randomradius = RandomCurve(Radius);
                    //var x = RandomCurve(go.transform.position.x);
                    var y = go.transform.position.y;
                    //var z = RandomCurve(go.transform.position.z);
                    var pos = go.transform.position + UnityEngine.Random.insideUnitSphere * randomradius;
                    pos.y = y;

                    go.transform.position = pos;

                    AwesomeExtensions.SetObjectRotation(go.transform, RandomizeRotation, MaxRotationX,
                        MaxRotationY, MaxRotationZ);
                    AwesomeExtensions.SetObjectScale(go.transform, RandomizeScale, MinSize, MaxSize);
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
            if (DragObject != null)
            {
                DragObject.layer = 0;
                DragObject = null;
                return;
            }

            if (DragObject == null)
            {
                var go = Instantiate(GameObject, PainterPosition, Quaternion.identity);
                var tag = go.AddComponent<BigBrushTag>() as BigBrushTag;
                tag.hideFlags = HideFlags.HideAndDontSave;
                DragObject = go;
                DragObject.layer = 2;
            }

        }

        private static void StrokeDots()
        {
            //clear out any drag paint object
            if (DragObject != null)
            {
                DragObject.layer = 0;
                DragObject = null;
                DestroyImmediate(DragObject);
            }

            if (GameObject != null)
            {
                for (int i = 0; i < PaintIntensity; i++)
                {
                    var go = Instantiate(GameObject, PainterPosition, Quaternion.identity);
                    var tag = go.AddComponent<BigBrushTag>() as BigBrushTag;
                    tag.hideFlags = HideFlags.HideAndDontSave;
                    //void record session
                    //m_BigBrushSessionRecorder.SessionObjects.Add(go);

                    if (UseNormalDirection)
                        go.transform.rotation = Quaternion.FromToRotation(go.transform.up, NormalDirection) * go.transform.rotation;

                    var randomradius = RandomCurve(Radius);
                    //var x = RandomCurve(go.transform.position.x);
                    var y = go.transform.position.y;
                    //var z = RandomCurve(go.transform.position.z);
                    var pos = go.transform.position + UnityEngine.Random.insideUnitSphere * randomradius;
                    pos.y = y;

                    go.transform.position = pos;

                    AwesomeExtensions.SetObjectRotation(go.transform, RandomizeRotation, MaxRotationX,
                        MaxRotationY, MaxRotationZ);
                    AwesomeExtensions.SetObjectScale(go.transform, RandomizeScale, MinSize, MaxSize);
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
            return num * (float) CumulativeProbability.Evaluate(UnityEngine.Random.value);
        }

        public static void DrawBigBrushGizmo()
        {
            if (PaintMode)
            {
                Handles.color = GizmoBrushColour;

                //Quaternion.LookRotation(new Vector3(0, 180, 1)) flat circle
                var thicken1 = Radius * 1.005f;
                var thicken2 = Radius * 1.01f;
                var thicken3 = Radius * 1.02f;
                var thicken4 = Radius * 1.03f;
                var thicken11 = Radius * 1.005f;
                var thicken21 = Radius * 1.015f;
                var thicken31 = Radius * 1.025f;
                var thicken41 = Radius * 1.035f;

                if (NormalDirection == Vector3.zero)
                    NormalDirection = Vector3.up;

                //show a line thats always up
                var lineEndUpPos = PainterPosition + Vector3.up;
                Handles.DrawLine(PainterPosition, lineEndUpPos);

                //show a line thats always reflects normal direction, to help gauge space
                var lineEndPos = PainterPosition + RayCastHitInfo.normal;
                Handles.DrawLine(PainterPosition, lineEndPos);

                Handles.CircleHandleCap(0, PainterPosition, Quaternion.LookRotation(NormalDirection), Radius,
                    EventType.Repaint);
                Handles.CircleHandleCap(0, PainterPosition, Quaternion.LookRotation(NormalDirection), Radius,
                    EventType.Repaint);
                Handles.CircleHandleCap(0, PainterPosition, Quaternion.LookRotation(NormalDirection), Radius,
                    EventType.Repaint);
                Handles.CircleHandleCap(0, PainterPosition, Quaternion.LookRotation(NormalDirection), thicken1,
                    EventType.Repaint);
                Handles.CircleHandleCap(0, PainterPosition, Quaternion.LookRotation(NormalDirection), thicken2,
                    EventType.Repaint);
                Handles.CircleHandleCap(0, PainterPosition, Quaternion.LookRotation(NormalDirection), thicken3,
                    EventType.Repaint);
                Handles.CircleHandleCap(0, PainterPosition, Quaternion.LookRotation(NormalDirection), thicken4,
                    EventType.Repaint);
                Handles.CircleHandleCap(0, PainterPosition, Quaternion.LookRotation(NormalDirection), thicken11,
                    EventType.Repaint);
                Handles.CircleHandleCap(0, PainterPosition, Quaternion.LookRotation(NormalDirection), thicken21,
                    EventType.Repaint);
                Handles.CircleHandleCap(0, PainterPosition, Quaternion.LookRotation(NormalDirection), thicken31,
                    EventType.Repaint);
                Handles.CircleHandleCap(0, PainterPosition, Quaternion.LookRotation(NormalDirection), thicken41,
                    EventType.Repaint);


                //Handles.DrawSolidArc(m_PainterPosition, Vector3.up, -Vector3.right, 180, 3);

                Handles.color = GizmoBrushFocalColour;
                var focalSize = 0.5f;

                if (AwesomeExtensions.IsPositive(FocalShift))
                {
                    float OldMax = 1;
                    float OldMin = 0.1f;
                    float NewMax = 1;
                    float NewMin = 0.5f;
                    float OldRange = (OldMax - OldMin);
                    float NewRange = (NewMax - NewMin);
                    float NewValue = (((FocalShift - OldMin) * NewRange) / OldRange) + NewMin;
                    Debug.Log(NewValue);

                    focalSize = NewValue;
                }

                if (AwesomeExtensions.IsNegative(FocalShift))
                {
                    //float OldMax = 1; float OldMin = 0.1f; float NewMax = 0.5f; float NewMin = 0.01f;
                    float OldMax = 0;
                    float OldMin = -1f;
                    float NewMax = 0.5f;
                    float NewMin = 0f;
                    float OldRange = (OldMax - OldMin);
                    float NewRange = (NewMax - NewMin);
                    float NewValue = (((FocalShift - OldMin) * NewRange) / OldRange) + NewMin;
                    Debug.Log(NewValue);
                    //float remap = 0.5f - NewValue;
                    focalSize = NewValue;
                }




                focalSize = Radius * focalSize;
                //focalSize += m_FocalShift;

                //  (m_Radius * 0.5f) * (m_FocalShift * 1);
                //Debug.Log(focalSize);

                Handles.CircleHandleCap(0, PainterPosition, Quaternion.LookRotation(NormalDirection), focalSize,
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
            if (m_BoxStyle == null)
            {
                m_BoxStyle = new GUIStyle(GUI.skin.box);
                m_BoxStyle.normal.textColor = GUI.skin.label.normal.textColor;
                m_BoxStyle.fontStyle = FontStyle.Bold;
                m_BoxStyle.alignment = TextAnchor.UpperLeft;
            }

            if (m_ContentStyle == null)
            {
                m_ContentStyle = new GUIStyle(GUI.skin.box);
                m_ContentStyle.normal.textColor = GUI.skin.label.normal.textColor;
                m_ContentStyle.fontStyle = FontStyle.Normal;
                m_ContentStyle.alignment = TextAnchor.UpperLeft;

            }

            //Setup the wrap style
            if (m_WrapStyle == null)
            {
                m_WrapStyle = new GUIStyle(GUI.skin.label);
                m_WrapStyle.fontStyle = FontStyle.Normal;
                m_WrapStyle.wordWrap = true;
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
            if (m_WrapStyle == null)
            {
                m_WrapStyle = new GUIStyle(GUI.skin.label);
                m_WrapStyle.fontStyle = FontStyle.Normal;
                m_WrapStyle.wordWrap = true;
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
            if (m_WrapStyle == null)
            {
                m_WrapStyle = new GUIStyle(GUI.skin.label);
                m_WrapStyle.fontStyle = FontStyle.Normal;
                m_WrapStyle.wordWrap = true;
            }
        }
    }
}

