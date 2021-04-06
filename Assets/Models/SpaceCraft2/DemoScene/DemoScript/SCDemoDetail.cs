using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCDemoDetail : MonoBehaviour 
{
    public string NameIn;
    public string NameOut;
    int Mode;

	// Use this for initialization
	void Start () 
    {
		
	}
	
	// Update is called once per frame
	void Update () 
    {
        //animation
        if (GetComponent<Animation>())
        {
            if (!GetComponent<Animation>().isPlaying)
            {
                if (SCDemoStatic._SCDemoController.Moving == 1 & Mode != 1)
                {
                    Mode = 1;
                    if (NameIn != "") GetComponent<Animation>().Play(NameIn);
                }

                if (SCDemoStatic._SCDemoController.Moving == 2 & Mode != 2)
                {
                    Mode = 2;
                    if (NameOut != "") GetComponent<Animation>().Play(NameOut);
                }
            }
        }

        //emission lerp
        if (SCDemoStatic._SCDemoController.isEmitLerp)
        {
            if (GetComponent<Renderer>())
            {
                GetComponent<Renderer>().materials[0].SetColor("_EmissionColor", SCDemoStatic._SCDemoController.EmitLerpValue);
            }
        }
	}
}
