using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    public TextMeshProUGUI fpsDisplay;
    public TextMeshProUGUI averageFPSDisplay;
    public TextMeshProUGUI minFPSDisplay;
    public TextMeshProUGUI maxFPSDisplay;

    int framesPassed = 0;
    float fpsTotal = 0f;
    float minFPS = Mathf.Infinity;
    float maxFPS = 0f;


    void Update()
    {
        float fps = 1 / Time.unscaledDeltaTime;
        fpsDisplay.text = "FPS: " + fps;

        fpsTotal += fps;
        framesPassed++;
        averageFPSDisplay.text = "AVG: " + (fpsTotal / framesPassed);

        if (fps > maxFPS && framesPassed > 10)
        {
            maxFPS = fps;
            maxFPSDisplay.text = "MAX: " + maxFPS;
        }
        if (fps < minFPS && framesPassed > 10)
        {
            minFPS = fps;
            minFPSDisplay.text = "MIN: " + minFPS;
        }
    }
}
