using UnityEngine;

public class TimeScript : MonoBehaviour
{
    // 2D arrays: [segmentIndex, sphereIndex]

    // Row = time segment (0-10, 10-20, 20-30)

    // Col = sphere id

    // Example: startPositions[1, 25] -> sphere 25 start position in 10-20s segment
    static int numSphere = 200; 
    Vector3[,] startPositions;

    Vector3[,] endPositions;

    void Start()

    {

        timeFlags = new float[] { 0f, 10f, 20f, 30f };

        segmentCount = timeFlags.Length - 1;

        spheres = new GameObject[numSphere];

        startPositions = new Vector3[segmentCount, numSphere];

        endPositions = new Vector3[segmentCount, numSphere];

		//....

        for (int i = 0; i < numSphere; i++)

        {
            float r = 10f;

            // Segment 0 : 0s -> 10s

            startPositions[0, i] = new Vector3(r * Random.Range(-1f, 1f), r * Random.Range(-1f, 1f), r * Random.Range(-1f, 1f));

            endPositions[0, i] = new Vector3(r * Mathf.Cos(i * 2 * Mathf.PI / numSphere * 2) * Mathf.Cos(i * 2 * Mathf.PI / numSphere), r * Mathf.Cos(i * 2 * Mathf.PI / numSphere * 2) * Mathf.Sin(i * 2 * Mathf.PI / numSphere));

 

            // Segment 1 : 10s -> 20s

            startPositions[1, i] = new Vector3(r * Random.Range(-1f, 1f), r * Random.Range(-1f, 1f), r * Random.Range(-1f, 1f));

            endPositions[1, i] = new Vector3(r * Mathf.Cos(i * 2 * Mathf.PI / numSphere * 3) * Mathf.Cos(i * 2 * Mathf.PI / numSphere), r * Mathf.Cos(i * 2 * Mathf.PI / numSphere * 3) * Mathf.Sin(i * 2 * Mathf.PI / numSphere));

            // Segment 2 : 20s -> 30s

            startPositions[2, i] = new Vector3(r * Random.Range(-1f, 1f), r * Random.Range(-1f, 1f), r * Random.Range(-1f, 1f));

            endPositions[2, i] = new Vector3(r * Mathf.Cos(i * 2 * Mathf.PI / numSphere * 4) * Mathf.Cos(i * 2 * Mathf.PI / numSphere), r * Mathf.Cos(i * 2 * Mathf.PI / numSphere * 4) * Mathf.Sin(i * 2 * Mathf.PI / numSphere));

			//....

        }
		//...

    }

    
	  void Update()

    {
        float currentTime = Time.time;

        if (currentTime < timeFlags[1])
        {
            for (int i =0; i < numSphere; i++){
                // Lerp : Linearly interpolates between two points.
                // https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Vector3.Lerp.html
                // Vector3.Lerp(startPosition, endPosition, lerpFraction)
                
                // lerpFraction variable defines the point between startPosition and endPosition (0~1)
                lerpFraction = Mathf.Sin(time) * 0.5f + 0.5f;

                // Lerp logic. Update position       
                t = i* 2 * Mathf.PI / numSphere;
                spheres[i].transform.position = Vector3.Lerp(startPosition[i], endPosition[i], lerpFraction);
                float scale = 1f + AudioSpectrum.audioAmp;
                spheres[i].transform.localScale = new Vector3(scale, 1f, 1f);
                spheres[i].transform.Rotate(AudioSpectrum.audioAmp, 1f, 1f);
                
                // Color Update over time
                Renderer sphereRenderer = spheres[i].GetComponent<Renderer>();
                // float hue = (float)i / numSphere; // Hue cycles through 0 to 1
                Color color = Color.HSVToRGB(Mathf.Abs(1f * Mathf.Cos(time)), Mathf.Cos(AudioSpectrum.audioAmp / 10f), 2f + Mathf.Cos(time)); // Full saturation and brightness
                // Switched Hue with 1f to make the color of the shape constant
                sphereRenderer.material.color = color;
            }
        }

        else if (currentTime < timeFlags[2])
        {
            for (int i =0; i < numSphere; i++){
                // Lerp : Linearly interpolates between two points.
                // https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Vector3.Lerp.html
                // Vector3.Lerp(startPosition, endPosition, lerpFraction)
                
                // lerpFraction variable defines the point between startPosition and endPosition (0~1)
                lerpFraction = Mathf.Sin(time) * 0.5f + 0.5f;

                // Lerp logic. Update position       
                t = i* 2 * Mathf.PI / numSphere;
                spheres[i].transform.position = Vector3.Lerp(startPosition[i], endPosition[i], lerpFraction);
                float scale = 1f + AudioSpectrum.audioAmp;
                spheres[i].transform.localScale = new Vector3(scale, 1f, 1f);
                spheres[i].transform.Rotate(AudioSpectrum.audioAmp, 1f, 1f);
                
                // Color Update over time
                Renderer sphereRenderer = spheres[i].GetComponent<Renderer>();
                // float hue = (float)i / numSphere; // Hue cycles through 0 to 1
                Color color = Color.HSVToRGB(Mathf.Abs(.5f * Mathf.Cos(time)), Mathf.Cos(AudioSpectrum.audioAmp / 10f), 2f + Mathf.Cos(time)); // Full saturation and brightness
                // Switched Hue with 1f to make the color of the shape constant
                sphereRenderer.material.color = color;
            }
        }

        else if (currentTime < timeFlags[3])
        {
            for (int i =0; i < numSphere; i++){
                // Lerp : Linearly interpolates between two points.
                // https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Vector3.Lerp.html
                // Vector3.Lerp(startPosition, endPosition, lerpFraction)
                
                // lerpFraction variable defines the point between startPosition and endPosition (0~1)
                lerpFraction = Mathf.Sin(time) * 0.5f + 0.5f;

                // Lerp logic. Update position       
                t = i* 2 * Mathf.PI / numSphere;
                spheres[i].transform.position = Vector3.Lerp(startPosition[i], endPosition[i], lerpFraction);
                float scale = 1f + AudioSpectrum.audioAmp;
                spheres[i].transform.localScale = new Vector3(scale, 1f, 1f);
                spheres[i].transform.Rotate(AudioSpectrum.audioAmp, 1f, 1f);
                
                // Color Update over time
                Renderer sphereRenderer = spheres[i].GetComponent<Renderer>();
                // float hue = (float)i / numSphere; // Hue cycles through 0 to 1
                Color color = Color.HSVToRGB(Mathf.Abs(.7f * Mathf.Cos(time)), Mathf.Cos(AudioSpectrum.audioAmp / 10f), 2f + Mathf.Cos(time)); // Full saturation and brightness
                // Switched Hue with 1f to make the color of the shape constant
                sphereRenderer.material.color = color;
            }
        }
    }
}
