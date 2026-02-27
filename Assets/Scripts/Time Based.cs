using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBased : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    GameObject[] spheres;
    static int numSphere = 200;
    float time = 0f;
    float lerpFraction;
    int segmentCount;
    float[] timeFlags;
    Vector3[] initPos;
    Vector3[,] startPositions;
    Vector3[,] endPositions;
    void Start()
    {
        timeFlags = new float[] { 0f, 10f, 20f, 30f };
        segmentCount = timeFlags.Length - 1;
        spheres = new GameObject[numSphere];
        initPos = new Vector3[numSphere];
        startPositions = new Vector3[segmentCount, numSphere];
        endPositions = new Vector3[segmentCount, numSphere];
        for (int i = 0; i < numSphere; i++)
        {
            float r = 10f;
            // Segment 0 : 0s -> 10s
            startPositions[0, i] = new Vector3(r * Random.Range(-1f, 1f), r * Random.Range(-1f, 1f), r * Random.Range(-1f, 1f));;
            endPositions[0, i] = new Vector3(r * Random.Range(-1f, 1f), r * Random.Range(-1f, 1f), r * Random.Range(-1f, 1f));;

            // Segment 1 : 10s -> 20s
            startPositions[1, i] = new Vector3(r * Random.Range(-2f, 0f), r * Random.Range(-2f, 0f), r * Random.Range(-2f, 0f));;
            endPositions[1, i] = new Vector3(r * Random.Range(-2f, 0f), r * Random.Range(-2f, 0f), r * Random.Range(-2f, 0f));;

            // Segment 2 : 20s -> 30s
            startPositions[2, i] = new Vector3(r * Random.Range(-3f, -1f), r * Random.Range(-3f, -1f), r * Random.Range(-3f, -1f));;
            endPositions[2, i] = new Vector3(r * Random.Range(-3f, -1f), r * Random.Range(-3f, -1f), r * Random.Range(-3f, -1f));;
        }
        /*
        
        Test: Sphere cluster should translate down to the left after every 10 seconds.
        Result: Succss!

        */

        // Spheres From Git Repo
        for (int i =0; i < numSphere; i++){
            // Draw primitive elements:
            // https://docs.unity3d.com/6000.0/Documentation/ScriptReference/GameObject.CreatePrimitive.html
            spheres[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            // Position
            initPos[i] = startPositions[0,i];
            spheres[i].transform.position = initPos[i];
            spheres[i].transform.localRotation = Quaternion.EulerAngles(Random.Range(-180f, 180f), Random.Range(-180f, 180f), Random.Range(-180f, 180f));
            spheres[i].transform.localScale = new Vector3(Random.Range(0.1f, 0.3f), Random.Range(0.1f, 0.3f), Random.Range(0.1f, 0.3f));
            // Color
            // Get the renderer of the spheres and assign colors.
            Renderer sphereRenderer = spheres[i].GetComponent<Renderer>();
            // HSV color space: https://en.wikipedia.org/wiki/HSL_and_HSV
            float hue = (float)i / numSphere * 2; // Hue cycles through 0 to 1
            Color color = Color.HSVToRGB(hue, 1f, 1f); // Full saturation and brightness
            sphereRenderer.material.color = color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        float currentTime = Time.time;

        if (currentTime < timeFlags[1])
        {
			for (int i =0; i < numSphere; i++)
            {
                lerpFraction = Mathf.Sin(time) * 0.5f + 0.5f;

                spheres[i].transform.position = Vector3.Lerp(startPositions[0,i], endPositions[0,i], lerpFraction);
            }
        }

        else if (currentTime < timeFlags[2])
        {
            for (int i =0; i < numSphere; i++)
            {
                lerpFraction = Mathf.Sin(time) * 0.5f + 0.5f;

                spheres[i].transform.position = Vector3.Lerp(startPositions[1,i], endPositions[1,i], lerpFraction);
            }
        }

        else if (currentTime < timeFlags[3])
        {
            for (int i =0; i < numSphere; i++)
            {
                lerpFraction = Mathf.Sin(time) * 0.5f + 0.5f;

                spheres[i].transform.position = Vector3.Lerp(startPositions[2,i], endPositions[2,i], lerpFraction);
            }
        }
        /*
        else
        {

        }
        */
    }
}
