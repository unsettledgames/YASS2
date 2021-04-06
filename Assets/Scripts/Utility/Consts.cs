using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Consts : MonoBehaviour
{
    public static Consts Instance;
    [System.Serializable]
    public class GameObjectMatrix
    {
        public GameObject[] row;
    }

    [System.Serializable]
    public class GOInt
    {
        public GameObject go;
        public int integer;
    }

    public static class Directions
    {
        public const int RIGHT = 0;
        public const int DOWN = 1;
        public const int LEFT = 2;
        public const int UP = 3;
    }

    public AnimationCurve customBounceCurve;

    [Header("Utility")]
    public static AnimationCurve easeCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    public static AnimationCurve bounceCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

    public static float enemyOutlineWidth = 5;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (customBounceCurve != null)
        {
            bounceCurve = customBounceCurve;
        }
    }
}
