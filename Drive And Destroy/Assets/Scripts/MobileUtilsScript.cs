using UnityEngine;
using System.Collections;

public class MobileUtilsScript : MonoBehaviour
{

    private int FramesPerSec;
    private float frequency = 1.0f;
    private string fps;



    void Start()
    {
        StartCoroutine(FPS());
    }

    private IEnumerator FPS()
    {
        for (; ; )
        {
            // Capture frame-per-second
            int lastFrameCount = Time.frameCount;
            float lastTime = Time.realtimeSinceStartup;
            yield return new WaitForSeconds(frequency);
            float timeSpan = Time.realtimeSinceStartup - lastTime;
            int frameCount = Time.frameCount - lastFrameCount;

            // Display it

            fps = string.Format("FPS: {0}", Mathf.RoundToInt(frameCount / timeSpan));
        }
    }


    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        Rect rect = new Rect(0, 0, Screen.width * 95 / 100, Screen.height * 2 / 100);
        style.alignment = TextAnchor.UpperRight;
        style.fontSize = Screen.height * 4 / 100;
        style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        GUI.Label(rect, fps, style);
        //GUI.Label(new Rect(Screen.width - 100, 10, 200, 40), fps);
    }
}