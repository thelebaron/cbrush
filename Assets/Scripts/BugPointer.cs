using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugPointerSaver : ScriptableObject
{
    //General idea: Point at area, press hotkey, log bug ingame or ineditor. Creates location, bug icon ingame(game checks and loads), user can quickly type report, set name, set game functionality which is problematic.
    //save as xml file?

    //public enum buglogger name etc
    public List<Vector3> BugPositions;
    //resolution: resolved/unresolved
    //Name
    //Comment
    //Change
}
