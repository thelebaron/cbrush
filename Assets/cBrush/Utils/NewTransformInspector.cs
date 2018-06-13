using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(Transform), true)]
public class NewTransformInspector : Editor
{
    private Vector3 m_StoredPosition;
    private Transform m_Transform;
    private bool m_Locked;
    private string m_LockStatusTrue = "Transform Locked";
    private string m_LockStatusFalse = "Transform is not Locked";
    private string m_LockPersistStatusTrue = "Persistent(Will save with scene)";
    private string m_LockPersistStatusFalse = "Not Persistent";
    private bool m_PendingLock;
    private bool m_PersistentLock;
    /// <summary>
    /// Draw the inspector widget.
    /// </summary>
    public override void OnInspectorGUI()
    {
        Transform t = (Transform)target;
        m_Transform = t;

        if (!m_Locked)
        {
            m_StoredPosition = t.position;
        }

        //Helper script, only add if we actually use this feature to avoid bunging up every damn transform
        var helper = t.GetComponent<TransformSceneHelper>() as TransformSceneHelper;
        if (helper == null)
        {
            helper = t.gameObject.AddComponent<TransformSceneHelper>() as TransformSceneHelper;
        }
        //Read lock value if not locked
        if (helper != null)
        {
            m_Locked = helper.LockEnabled;
        }
        helper.StoredPosition = m_StoredPosition;

        if (!m_Locked)
        { GUILayout.Label(m_LockStatusFalse); }
        else { GUILayout.Label(m_LockStatusTrue); }
        if (!m_PersistentLock)
        { GUILayout.Label(m_LockPersistStatusFalse); }
        else { GUILayout.Label(m_LockPersistStatusTrue); }


        float oldLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 15;

        EditorGUILayout.BeginHorizontal();
        bool resetPos = GUILayout.Button("P", GUILayout.Width(20f));
        Vector3 position = EditorGUILayout.Vector3Field("", t.localPosition);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        bool resetRot = GUILayout.Button("R", GUILayout.Width(20f));
        Vector3 eulerAngles = EditorGUILayout.Vector3Field("", t.localEulerAngles);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        bool resetScale = GUILayout.Button("S", GUILayout.Width(20f));
        Vector3 scale = EditorGUILayout.Vector3Field("", t.localScale);
        EditorGUILayout.EndHorizontal();


        EditorGUIUtility.labelWidth = 35;
        EditorGUILayout.LabelField("Lock settings");

        if (GUILayout.Button("Lock"))
        {
            m_Locked = !m_Locked;
            m_PendingLock = true;
            helper.LockEnabled = !helper.LockEnabled;
        }

        if (GUILayout.Button("Persistent Lock"))
        {
            m_PersistentLock = !m_PersistentLock;

            if (m_PersistentLock)
            {
                helper.hideFlags = HideFlags.None;
            }
            if (!m_PersistentLock)
            {
                helper.hideFlags = HideFlags.HideAndDontSave;
            }
            //Debug.Log(helper.hideFlags);
        }

        EditorGUIUtility.labelWidth = oldLabelWidth;

        if (resetPos) position = Vector3.zero;
        if (resetRot) eulerAngles = Vector3.zero;
        if (resetScale) scale = Vector3.one;

        if (GUI.changed)
        {
            Undo.RecordObject(t, "Transform Change");
            
            t.localEulerAngles = FixIfNaN(eulerAngles);
            t.localScale = FixIfNaN(scale);

            if (m_Locked)
            {
                t.localPosition = m_StoredPosition;
            }
            t.localPosition = FixIfNaN(position);
        }
    }

    private Vector3 FixIfNaN(Vector3 v)
    {
        if (float.IsNaN(v.x))
        {
            v.x = 0;
        }
        if (float.IsNaN(v.y))
        {
            v.y = 0;
        }
        if (float.IsNaN(v.z))
        {
            v.z = 0;
        }
        return v;
    }

    public void Update()
    {
        if (m_Locked)
        {
        }
        if (SceneView.onSceneGUIDelegate == null)
        {
            SceneView.onSceneGUIDelegate += OnScene;
            //Any other initialization code you would
            //normally place in Init() goes here instead.
        }

    }

    private static void OnScene(SceneView sceneview)
    {
        //var tr = Transform.position;
        EditorGUI.BeginChangeCheck();
        Debug.Log("move");
        //m_Transform.position = m_StoredPosition;

        EditorGUI.EndChangeCheck();
    }

}