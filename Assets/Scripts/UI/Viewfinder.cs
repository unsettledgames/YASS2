using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Viewfinder : MonoBehaviour
{
    public float animationSpeed = 5;
    public float maxScale = 1;

    private PlayerShipController player;
    private bool isVisible;
    private float currentOpacity;
    private float currentScale;

    private Image renderer;

    private Coroutine currentScaleRoutine;
    private Coroutine currentOpacityRoutine;

    // Start is called before the first frame update
    void Start()
    {
        isVisible = false;
        currentScale = 0;
        currentOpacity = 0;

        player = FrequentlyAccessed.Instance.playerController;
        renderer = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentPos = player.GetCurrentTargetPosition();

        if (currentPos != Vector3.zero)
        {
            transform.position = currentPos;

            if (!isVisible)
                MakeVisible();
        }
        else if (isVisible)
            MakeInvisible();
    }

    private void MakeVisible()
    {
        if (currentScaleRoutine != null)
            StopCoroutine(currentScaleRoutine);
        if (currentOpacityRoutine != null)
            StopCoroutine(currentOpacityRoutine);

        currentOpacityRoutine = StartCoroutine(MovementUtility.SlideFloat(currentOpacity, 1, animationSpeed, 
            Consts.easeCurve, UpdateOpacity, null, null));
        currentScaleRoutine = StartCoroutine(MovementUtility.SlideFloat(currentScale, maxScale, 
            animationSpeed, Consts.easeCurve, UpdateScale, null, null));

        isVisible = true;
    }

    private void MakeInvisible()
    {
        if (currentScaleRoutine != null)
            StopCoroutine(currentScaleRoutine);
        if (currentOpacityRoutine != null)
            StopCoroutine(currentOpacityRoutine);

        currentOpacityRoutine = StartCoroutine(MovementUtility.SlideFloat(currentOpacity, 0, animationSpeed, 
            Consts.easeCurve, UpdateOpacity, null, null));
        currentScaleRoutine = StartCoroutine(MovementUtility.SlideFloat(currentScale, 0, animationSpeed, 
            Consts.easeCurve, UpdateScale, null, null));

        isVisible = false;
    }

    private void UpdateOpacity(float val)
    {
        Color toSet = renderer.color;
        toSet.a = val;
        renderer.color = toSet;

        currentOpacity = val;
    }

    private void UpdateScale(float scale)
    {
        Vector3 toSet = Vector3.one * scale;

        currentScale = scale;
        transform.localScale = toSet;
    }
}
