using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBlock : ScriptableObject
{
    public WallSegments wallSegments;
    public BuildingRule buildingRules;

    [System.Serializable]
    public class BuildingRule
    {
        public int SwapEvery;
    }

    [System.Serializable]
    public class PrefabProbability
    {
        public GameObject gameObject;
        [Range(0, 100)]
        public int Chance;
    }

    [System.Serializable]
    public class WallSegments
    {
        public PrefabProbability[] prefab;
    }

}
