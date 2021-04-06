using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCDemoController : MonoBehaviour 
{
    [HideInInspector]
    public int Moving;
    [HideInInspector]
    public bool isEmitLerp;
    [HideInInspector]
    public Color EmitLerpValue;

    float TimeLerpStart;
    float TimeDuringLerp = 1.5f;
    Color StartVal;
    Color StoptVal;

    void Awake()
    {
        SCDemoStatic._SCDemoController = this;
    }

	// Use this for initialization
	void Start () 
    {
		
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (isEmitLerp)
        {
            float TimeStarted = Time.time - TimeLerpStart;
            float PercComplete = TimeStarted / TimeDuringLerp;
            EmitLerpValue = Color.Lerp(StartVal, StoptVal, PercComplete);
            if (PercComplete >= 1.0f)
            {
                isEmitLerp = false;
            }
        }
	}

    void StartEmitLerp(bool Mode)
    {
        isEmitLerp = true;
        TimeLerpStart = Time.time;
        if (Mode)
        {
            StartVal = new Color(2, 2, 2);
            StoptVal = new Color(7, 7, 7);
        }
        else
        {
            StoptVal = new Color(2, 2, 2);
            StartVal = new Color(7, 7, 7);
        }
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(50, 50, 150, 50), "MOVE A"))
        {
            Moving = 1;
            StartEmitLerp(true);
        }
        if (GUI.Button(new Rect(210, 50, 150, 50), "MOVE B"))
        {
            Moving = 2;
            StartEmitLerp(false);
        }

        GUI.TextField(new Rect(50, 110, 310, 20), "use WASD and mouse for move");
    }
}
