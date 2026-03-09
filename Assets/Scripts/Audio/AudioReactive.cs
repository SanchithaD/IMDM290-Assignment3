// UMD IMDM290 
// Instructor: Myungin Lee
// All the same Lerp but using audio

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioReactive : MonoBehaviour
{
    // Shapes
    GameObject[] flowerSpheres;
    GameObject[] starSpheres;
    GameObject[] seaweedSpheres;
    GameObject[] bubbleSpheres;
    GameObject[] waveParticles;
    
    static int numSphere = 200;
    static int numSeaweedSpheres = 100;
    static int numBubbles = 50;
    static int numWaveParticles = 150;
    
    float time = 0f;
    float lerpFraction;
    float spinSpeed = 0f;
    
    // Scene timing matched to Grass Skirt Chase (first 35 seconds)
    float[] timeFlags;
    int currentScene = 0;
    
    // Position arrays
    Vector3[] flowerStartPos, flowerEndPos;
    Vector3[] starStartPos, starEndPos;
    Vector3[] seaweedPositions;
    Vector3[] bubblePositions;
    Vector3[] wavePositions;
    float flowerX = 0f;
    float flowerY = 0f;
    float starX = 20f;
    float starY = 0f;
    float chaseSpeed = 0f;
    bool isRunningRight = true;
    
    // bounce variables
    float bounceTime = 0f;
    float wiggleAmount = 0f;
    
    // Stop flag
    bool isStopped = false;
    float stopTime = 35f;

    // Start is called before the first frame update
    void Start()
    {
        // Scene breakdown:
        // 0-3s: Waves appear
        // 3-9s: Flower forms from cloud, bouncing to the beat
        // 9-15s: Star appears from right
        // 15-19s: Flower and star zigzag across screen
        // 19-24s: Seaweed appears, chase continues
        // 24-26s: Everything moving wild
        // 26-31s: Flower chases star left
        // 31-33s: Spiral together
        // 33-35s: Cooldown and fade out
        
        timeFlags = new float[] { 0f, 3f, 9f, 15f, 19f, 24f, 26f, 28f, 31f, 33f };
        
        InitializeFlower();
        InitializeStar();
        InitializeSeaweed();
        InitializeBubbles();
        InitializeWaves();
    }
    
    void InitializeFlower()
    {
        flowerSpheres = new GameObject[numSphere];
        flowerStartPos = new Vector3[numSphere];
        flowerEndPos = new Vector3[numSphere];
        
        for (int i = 0; i < numSphere; i++)
        {
            float t = i * 2 * Mathf.PI / numSphere;
            float r = 6f;
            
            // Start scattered
            flowerStartPos[i] = new Vector3(
                r * Random.Range(-2f, 2f),
                r * Random.Range(-2f, 2f),
                Random.Range(-1f, 1f)
            );
            
            // End as flower 
            flowerEndPos[i] = new Vector3(
                r * Mathf.Cos(t * 2) * Mathf.Cos(t),
                r * Mathf.Cos(t * 2) * Mathf.Sin(t),
                0f
            );
            
            flowerSpheres[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            flowerSpheres[i].transform.position = new Vector3(-50f, 0f, 0f); // Hidden initially
            flowerSpheres[i].transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            
            Renderer rend = flowerSpheres[i].GetComponent<Renderer>();
            // Yellow flower colors
            float hue = Random.Range(0.12f, 0.18f); // Yellows
            rend.material.color = Color.HSVToRGB(hue, 0.9f, 1f);
        }
    }
    
    void InitializeStar()
    {
        starSpheres = new GameObject[numSphere];
        starStartPos = new Vector3[numSphere];
        starEndPos = new Vector3[numSphere];
        
        for (int i = 0; i < numSphere; i++)
        {
            float t = i * 2 * Mathf.PI / numSphere;
            float r = 5f;
            
            // Star pattern
            float starR = r * (1f + 0.6f * Mathf.Cos(5 * t));
            
            starStartPos[i] = new Vector3(30f, Random.Range(-5f, 5f), 0f);
            starEndPos[i] = new Vector3(
                starR * Mathf.Cos(t),
                starR * Mathf.Sin(t),
                0f
            );
            
            starSpheres[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            starSpheres[i].transform.position = new Vector3(50f, 0f, 0f); // Hidden initially
            starSpheres[i].transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            
            Renderer rend = starSpheres[i].GetComponent<Renderer>();
            // Pink star colors
            float hue = Random.Range(0.9f, 0.98f); // Pinks and magentas
            rend.material.color = Color.HSVToRGB(hue, 0.8f, 1f);
        }
    }
    
    void InitializeSeaweed()
    {
        seaweedSpheres = new GameObject[numSeaweedSpheres];
        seaweedPositions = new Vector3[numSeaweedSpheres];
        
        for (int i = 0; i < numSeaweedSpheres; i++)
        {
            // Create seaweed on the sides
            float side = (i < numSeaweedSpheres / 2) ? -1f : 1f;
            float treeIndex = i % (numSeaweedSpheres / 2);
            float t = treeIndex * Mathf.PI / (numSeaweedSpheres / 2);
            
            float x = side * (12f + 3f * Mathf.Cos(t * 3));
            float y = -5f + 10f * (float)treeIndex / (numSeaweedSpheres / 2);
            
            seaweedPositions[i] = new Vector3(x, y, 2f);
            
            seaweedSpheres[i] = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            seaweedSpheres[i].transform.position = new Vector3(side * 30f, y, 5f); // Off screen
            seaweedSpheres[i].transform.localScale = new Vector3(0.3f, 0.6f, 0.3f);
            
            Renderer rend = seaweedSpheres[i].GetComponent<Renderer>();
            // Green seaweed colors
            float hue = Random.Range(0.25f, 0.35f);
            rend.material.color = Color.HSVToRGB(hue, 0.7f, 0.8f);
        }
    }
    
    void InitializeBubbles()
    {
        bubbleSpheres = new GameObject[numBubbles];
        bubblePositions = new Vector3[numBubbles];
        
        for (int i = 0; i < numBubbles; i++)
        {
            bubblePositions[i] = new Vector3(
                Random.Range(-15f, 15f),
                Random.Range(-10f, 10f),
                Random.Range(-2f, 2f)
            );
            
            bubbleSpheres[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bubbleSpheres[i].transform.position = bubblePositions[i];
            bubbleSpheres[i].transform.localScale = Vector3.zero; // Start invisible
            
            Renderer rend = bubbleSpheres[i].GetComponent<Renderer>();
            // Transparent blue bubbles
            rend.material.color = new Color(0.5f, 0.8f, 1f, 0.5f);
        }
    }
    
    void InitializeWaves()
    {
        waveParticles = new GameObject[numWaveParticles];
        wavePositions = new Vector3[numWaveParticles];
        
        for (int i = 0; i < numWaveParticles; i++)
        {
            float x = -20f + 40f * (float)i / numWaveParticles;
            wavePositions[i] = new Vector3(x, -8f, 1f);
            
            waveParticles[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            waveParticles[i].transform.position = wavePositions[i];
            waveParticles[i].transform.localScale = new Vector3(0.4f, 0.3f, 0.3f);
            
            Renderer rend = waveParticles[i].GetComponent<Renderer>();
            // Ocean blue
            float hue = Random.Range(0.5f, 0.6f);
            rend.material.color = Color.HSVToRGB(hue, 0.6f, 0.9f);
        }
    }

    void Update()
    {
        float currentTime = Time.time;
        
        // Stop after 35 seconds
        if (currentTime >= stopTime)
        {
            if (!isStopped)
            {
                isStopped = true;
            }
            return;
        }
        
        // Audio-reactive time flow
        float audioMult = 1f + AudioSpectrum.audioAmp * 3f;
        time += Time.deltaTime * audioMult;
        bounceTime += Time.deltaTime * (2f + AudioSpectrum.audioAmp * 5f);
        
        // Update chase dynamics based on audio
        chaseSpeed = 2f + AudioSpectrum.audioAmp * 8f;
        wiggleAmount = AudioSpectrum.audioAmp * 2f;
        spinSpeed = AudioSpectrum.audioAmp * 400f;
        
        // Always update waves
        UpdateWaves();
        
        if (currentTime < timeFlags[1])
        {
            // Scene 0
            Scene0_Intro(currentTime);
        }
        else if (currentTime < timeFlags[2])
        {
            // Scene 1
            Scene1_FlowerAppears(currentTime);
        }
        else if (currentTime < timeFlags[3])
        {
            // Scene 2
            Scene2_ChaseBegins(currentTime);
        }
        else if (currentTime < timeFlags[4])
        {
            // Scene 3
            Scene3_ChaseIntensifies(currentTime);
        }
        else if (currentTime < timeFlags[5])
        {
            // Scene 4: Seaweed comes
            Scene4_SeaweedScene(currentTime);
        }
        else if (currentTime < timeFlags[6])
        {
            // Scene 5: Chaos
            Scene5_Chaos(currentTime);
        }
        else if (currentTime < timeFlags[7])
        {
            // Scene 6: Chase reversal
            Scene6_ChaseReversal(currentTime);
        }
        else if (currentTime < timeFlags[8])
        {
            // Scene 7: Grand finale spin
            Scene7_GrandFinale(currentTime);
        }
        else if (currentTime < timeFlags[9])
        {
            // Scene 8: Cooldown
            Scene8_Cooldown(currentTime);
        }
        
        // Update bubbles throughout
        UpdateBubbles();
    }
    
    int GetCurrentScene(float t)
    {
        for (int i = timeFlags.Length - 1; i >= 0; i--)
        {
            if (t >= timeFlags[i]) return i;
        }
        return 0;
    }
    
    
    void Scene0_Intro(float currentTime)
    {
        float progress = currentTime / timeFlags[1];
        
        // Waves fade in
        for (int i = 0; i < numWaveParticles; i++)
        {
            float scale = 0.3f * progress;
            waveParticles[i].transform.localScale = new Vector3(0.4f * progress, scale, scale);
        }
        
        // Hide flower and star
        HideFlower();
        HideStar();
    }
    
    void Scene1_FlowerAppears(float currentTime)
    {
        float segmentStart = timeFlags[1];
        float segmentDuration = timeFlags[2] - timeFlags[1];
        float progress = (currentTime - segmentStart) / segmentDuration;
        
        lerpFraction = EaseOutBounce(progress);
        
        // Flower forms at center-left of screen
        flowerX = -5f;
        flowerY = 0f;
        
        for (int i = 0; i < numSphere; i++)
        {
            // Form from cloud to flower with bouncy easing
            Vector3 localPos = Vector3.Lerp(flowerStartPos[i], flowerEndPos[i], lerpFraction);
            
            // Add bounce to the beat
            float bounce = Mathf.Sin(bounceTime * 4f + i * 0.1f) * wiggleAmount;
            localPos.y += bounce;
            
            // Apply world position
            flowerSpheres[i].transform.position = new Vector3(localPos.x + flowerX, localPos.y + flowerY, localPos.z);
            
            // Spin with audio
            flowerSpheres[i].transform.Rotate(spinSpeed * Time.deltaTime * 0.1f, spinSpeed * Time.deltaTime * 0.05f, 0f);
            
            // Pulse scale with beat
            float scale = 0.3f + AudioSpectrum.audioAmp * 0.3f;
            flowerSpheres[i].transform.localScale = new Vector3(scale, scale, scale);
            
            // Color pulse
            UpdateFlowerColor(i);
        }
        
        // Star still hidden
        HideStar();
    }
    
    void Scene2_ChaseBegins(float currentTime)
    {
        float segmentStart = timeFlags[2];
        float segmentDuration = timeFlags[3] - timeFlags[2];
        float progress = (currentTime - segmentStart) / segmentDuration;
        
        flowerX = Mathf.Lerp(-5f, 5f, progress);
        flowerY = Mathf.Sin(bounceTime * 3f) * 1.5f;
        
        // Star enters from off-screen right and chases
        float starFormProgress = Mathf.Clamp01(progress * 2f); // Form in first half
        starX = Mathf.Lerp(18f, 0f, EaseOutCubic(progress));
        starY = Mathf.Sin(bounceTime * 4f) * 1f;
        
        // Update flower
        UpdateFlowerRunning(flowerX, flowerY, true);
        
        // Update star
        for (int i = 0; i < numSphere; i++)
        {
            float t = i * 2 * Mathf.PI / numSphere;
            float r = 5f * EaseOutElastic(starFormProgress);
            float starR = r * (1f + 0.6f * Mathf.Cos(5 * t));
            
            float x = starR * Mathf.Cos(t) + starX;
            float y = starR * Mathf.Sin(t) + starY;
            
            y += Mathf.Sin(bounceTime * 6f + i * 0.2f) * wiggleAmount * 1.5f;
            
            starSpheres[i].transform.position = new Vector3(x, y, 0f);
            starSpheres[i].transform.Rotate(spinSpeed * Time.deltaTime * 0.2f, 0f, spinSpeed * Time.deltaTime * 0.1f);
            
            Renderer rend = starSpheres[i].GetComponent<Renderer>();
            float hue = 0.92f - AudioSpectrum.audioAmp * 0.05f; // More magenta when loud
            rend.material.color = Color.HSVToRGB(Mathf.Max(0.85f, hue), 0.85f, 1f);
        }
    }
    
    void Scene3_ChaseIntensifies(float currentTime)
    {
        float segmentStart = timeFlags[3];
        float segmentDuration = timeFlags[4] - timeFlags[3];
        float progress = (currentTime - segmentStart) / segmentDuration;
        
        // Zig-zag chase pattern across screen
        float zigzag = Mathf.Sin(progress * Mathf.PI * 4f) * 4f;
        
        // Flower runs right across screen
        flowerX = Mathf.Lerp(5f, 12f, progress);
        flowerY = zigzag;
        
        // Star chases behind
        float chaseGap = 6f - AudioSpectrum.audioAmp * 2f;
        starX = flowerX - chaseGap;
        starY = zigzag * 0.8f;
        
        UpdateFlowerRunning(flowerX, flowerY, true);
        UpdateStarChasing(starX, starY);
    }
    
    void Scene4_SeaweedScene(float currentTime)
    {
        float segmentStart = timeFlags[4];
        float segmentDuration = timeFlags[5] - timeFlags[4];
        float progress = (currentTime - segmentStart) / segmentDuration;
        
        float seaweedProgress = EaseOutCubic(Mathf.Clamp01(progress * 2f));
        
        for (int i = 0; i < numSeaweedSpheres; i++)
        {
            float side = (i < numSeaweedSpheres / 2) ? -1f : 1f;
            Vector3 targetPos = seaweedPositions[i];
            Vector3 startPos = new Vector3(side * 25f, targetPos.y, 5f);
            
            Vector3 pos = Vector3.Lerp(startPos, targetPos, seaweedProgress);
            
            // Sway with audio
            pos.x += Mathf.Sin(bounceTime * 2f + i * 0.3f) * wiggleAmount * 0.5f;
            
            seaweedSpheres[i].transform.position = pos;
            
            // Green color pulse
            Renderer rend = seaweedSpheres[i].GetComponent<Renderer>();
            float brightness = 0.7f + AudioSpectrum.audioAmp * 0.3f;
            rend.material.color = Color.HSVToRGB(0.3f, 0.7f, brightness);
        }
        
        // Chase continues
        flowerX = 12f + Mathf.Sin(progress * Mathf.PI * 2f) * 8f;
        flowerY = Mathf.Cos(progress * Mathf.PI * 3f) * 3f;
        
        float chaseGap = 5f - AudioSpectrum.audioAmp * 1.5f;
        starX = flowerX - chaseGap;
        starY = flowerY * 0.9f;
        
        UpdateFlowerRunning(flowerX, flowerY, flowerX > starX);
        UpdateStarChasing(starX, starY);
    }
    
    void Scene5_Chaos(float currentTime)
    {
        float segmentStart = timeFlags[5];
        float segmentDuration = timeFlags[6] - timeFlags[5];
        float progress = (currentTime - segmentStart) / segmentDuration;
        
        // Everything bouncing wildly
        float chaos = 1f + AudioSpectrum.audioAmp * 3f;
        
        // Flower goes crazy - spinning and bouncing
        for (int i = 0; i < numSphere; i++)
        {
            float t = i * 2 * Mathf.PI / numSphere;
            float r = 6f * (1f + AudioSpectrum.audioAmp);
            
            float x = r * Mathf.Cos(t * 2) * Mathf.Cos(t + time);
            float y = r * Mathf.Cos(t * 2) * Mathf.Sin(t + time);
            
            x += Mathf.Sin(bounceTime * 5f + i * 0.2f) * chaos * 2f;
            y += Mathf.Cos(bounceTime * 4f + i * 0.3f) * chaos * 2f;
            
            flowerSpheres[i].transform.position = new Vector3(x - 5f, y, 0f);
            flowerSpheres[i].transform.Rotate(spinSpeed * Time.deltaTime, spinSpeed * Time.deltaTime, spinSpeed * Time.deltaTime * 0.5f);
            
            // Rainbow colors during chaos
            Renderer rend = flowerSpheres[i].GetComponent<Renderer>();
            float hue = (time * 0.5f + (float)i / numSphere) % 1f;
            rend.material.color = Color.HSVToRGB(hue, 0.8f, 1f);
        }
        
        for (int i = 0; i < numSphere; i++)
        {
            float t = i * 2 * Mathf.PI / numSphere;
            float r = 5f * (1f + 0.6f * Mathf.Cos(5 * t)) * (1f + AudioSpectrum.audioAmp);
            
            float x = r * Mathf.Cos(t - time * 0.5f);
            float y = r * Mathf.Sin(t - time * 0.5f);
            
            x += Mathf.Cos(bounceTime * 6f + i * 0.15f) * chaos * 2f;
            y += Mathf.Sin(bounceTime * 5f + i * 0.25f) * chaos * 2f;
            
            starSpheres[i].transform.position = new Vector3(x + 5f, y, 0f);
            starSpheres[i].transform.Rotate(spinSpeed * Time.deltaTime * 1.5f, spinSpeed * Time.deltaTime, 0f);
            
            Renderer rend = starSpheres[i].GetComponent<Renderer>();
            float hue = (time * 0.3f + (float)i / numSphere + 0.5f) % 1f;
            rend.material.color = Color.HSVToRGB(hue, 0.9f, 1f);
        }
        
        for (int i = 0; i < numSeaweedSpheres; i++)
        {
            Vector3 pos = seaweedPositions[i];
            pos.x += Mathf.Sin(bounceTime * 3f + i * 0.4f) * chaos;
            pos.y += Mathf.Cos(bounceTime * 2f + i * 0.3f) * chaos * 0.5f;
            seaweedSpheres[i].transform.position = pos;
        }
    }
    
    void Scene6_ChaseReversal(float currentTime)
    {
        float segmentStart = timeFlags[6];
        float segmentDuration = timeFlags[7] - timeFlags[6];
        float progress = (currentTime - segmentStart) / segmentDuration;
        
        // Star runs away to the left
        starX = Mathf.Lerp(5f, -12f, progress);
        starY = Mathf.Sin(bounceTime * 5f) * 2f;
        
        // Flower chases from behind
        float chaseGap = 5f - AudioSpectrum.audioAmp * 2f;
        flowerX = starX + chaseGap;
        flowerY = Mathf.Sin(bounceTime * 5f + 0.3f) * 2f;
        
        UpdateFlowerRunning(flowerX, flowerY, false); // Facing left now!
        UpdateStarRunning(starX, starY); // Star is running now!
        
        for (int i = 0; i < numSeaweedSpheres; i++)
        {
            Vector3 pos = seaweedPositions[i];
            float side = (i < numSeaweedSpheres / 2) ? -1f : 1f;
            pos.x += side * Mathf.Sin(progress * Mathf.PI) * 2f; // Lean in
            seaweedSpheres[i].transform.position = pos;
        }
    }
    
    void Scene7_GrandFinale(float currentTime)
    {
        float segmentStart = timeFlags[7];
        float segmentDuration = timeFlags[8] - timeFlags[7];
        float progress = (currentTime - segmentStart) / segmentDuration;
        
        // Everything spirals together
        float spiralSpeed = time * (2f + AudioSpectrum.audioAmp * 5f);
        float expandRadius = 8f + AudioSpectrum.audioAmp * 5f;
        
        // Flower forms outer ring
        for (int i = 0; i < numSphere; i++)
        {
            float t = i * 2 * Mathf.PI / numSphere + spiralSpeed;
            float r = expandRadius;
            
            float x = r * Mathf.Cos(t);
            float y = r * Mathf.Sin(t);
            
            flowerSpheres[i].transform.position = new Vector3(x, y, 0f);
            flowerSpheres[i].transform.Rotate(spinSpeed * Time.deltaTime, spinSpeed * Time.deltaTime, 0f);
            
            // Golden yellow flower color
            Renderer rend = flowerSpheres[i].GetComponent<Renderer>();
            float hue = 0.14f + Mathf.Sin(time + i * 0.1f) * 0.03f;
            rend.material.color = Color.HSVToRGB(hue, 0.9f, 1f);
        }
        
        // Star forms inner ring
        for (int i = 0; i < numSphere; i++)
        {
            float t = i * 2 * Mathf.PI / numSphere - spiralSpeed;
            float r = expandRadius * 0.5f;
            
            float x = r * Mathf.Cos(t);
            float y = r * Mathf.Sin(t);
            
            starSpheres[i].transform.position = new Vector3(x, y, 0f);
            starSpheres[i].transform.Rotate(0f, spinSpeed * Time.deltaTime, spinSpeed * Time.deltaTime);
            
            // Pink/magenta star color
            Renderer rend = starSpheres[i].GetComponent<Renderer>();
            float hue = 0.92f + Mathf.Cos(time + i * 0.1f) * 0.03f;
            rend.material.color = Color.HSVToRGB(hue, 0.8f, 1f);
        }
        
        // Seaweed form outermost ring
        for (int i = 0; i < numSeaweedSpheres; i++)
        {
            float t = i * 2 * Mathf.PI / numSeaweedSpheres + spiralSpeed * 0.5f;
            float r = expandRadius * 1.5f;
            
            float x = r * Mathf.Cos(t);
            float y = r * Mathf.Sin(t);
            
            seaweedSpheres[i].transform.position = new Vector3(x, y, 2f);
        }
        
        // Bubbles form center
        for (int i = 0; i < numBubbles; i++)
        {
            float t = i * 2 * Mathf.PI / numBubbles + spiralSpeed * 2f;
            float r = expandRadius * 0.25f;
            
            bubbleSpheres[i].transform.position = new Vector3(r * Mathf.Cos(t), r * Mathf.Sin(t), -1f);
            float scale = 0.3f + AudioSpectrum.audioAmp * 0.5f;
            bubbleSpheres[i].transform.localScale = new Vector3(scale, scale, scale);
        }
    }
    
    void Scene8_Cooldown(float currentTime)
    {
        float segmentStart = timeFlags[8];
        float segmentDuration = timeFlags[9] - timeFlags[8];
        float progress = (currentTime - segmentStart) / segmentDuration;
        
        // Everything slowly disperses and fades out
        float disperseSpeed = 5f * progress;
        float fadeScale = 1f - progress;
        
        for (int i = 0; i < numSphere; i++)
        {
            Vector3 pos = flowerSpheres[i].transform.position;
            // Float outward and up
            pos += new Vector3(
                Mathf.Sin(i * 0.1f) * disperseSpeed * Time.deltaTime,
                disperseSpeed * Time.deltaTime * 0.5f,
                0f
            );
            flowerSpheres[i].transform.position = pos;
            
            // Fade out (scale down)
            float scale = 0.3f * fadeScale;
            flowerSpheres[i].transform.localScale = new Vector3(scale, scale, scale);
        }
        
        for (int i = 0; i < numSphere; i++)
        {
            Vector3 pos = starSpheres[i].transform.position;
            pos += new Vector3(
                Mathf.Cos(i * 0.15f) * disperseSpeed * Time.deltaTime,
                disperseSpeed * Time.deltaTime * 0.5f,
                0f
            );
            starSpheres[i].transform.position = pos;
            
            float scale = 0.25f * fadeScale;
            starSpheres[i].transform.localScale = new Vector3(scale, scale, scale);
        }
        
        // Fade Seaweeds
        for (int i = 0; i < numSeaweedSpheres; i++)
        {
            float scale = 0.3f * fadeScale;
            seaweedSpheres[i].transform.localScale = new Vector3(scale, 0.6f * fadeScale, scale);
        }
        
        // Fade waves
        for (int i = 0; i < numWaveParticles; i++)
        {
            float scale = 0.3f * fadeScale;
            waveParticles[i].transform.localScale = new Vector3(0.4f * fadeScale, scale, scale);
        }
    }
        
    void HideFlower()
    {
        for (int i = 0; i < numSphere; i++)
        {
            flowerSpheres[i].transform.position = new Vector3(-50f, 0f, 0f);
        }
    }
    
    void HideStar()
    {
        for (int i = 0; i < numSphere; i++)
        {
            starSpheres[i].transform.position = new Vector3(50f, 0f, 0f);
        }
    }
    
    void UpdateFlowerRunning(float xPos, float yPos, bool facingRight)
    {
        float runCycle = bounceTime * 8f;
        float legBounce = Mathf.Abs(Mathf.Sin(runCycle)) * 1.5f;
        
        // Rotation based on audio frequency
        float rotationAngle = time * spinSpeed * 0.005f;
        if (!facingRight) rotationAngle = -rotationAngle;
        
        for (int i = 0; i < numSphere; i++)
        {
            float t = i * 2 * Mathf.PI / numSphere;
            float r = 6f * (1f + AudioSpectrum.audioAmp * 0.3f);
            
            // Flower shape
            float localX = r * Mathf.Cos(t * 2) * Mathf.Cos(t);
            float localY = r * Mathf.Cos(t * 2) * Mathf.Sin(t);
            
            // Apply rotation
            float rotX = localX * Mathf.Cos(rotationAngle) - localY * Mathf.Sin(rotationAngle);
            float rotY = localX * Mathf.Sin(rotationAngle) + localY * Mathf.Cos(rotationAngle);
            
            // Squash and stretch for running
            float squash = 1f + Mathf.Sin(runCycle) * 0.2f;
            rotX *= squash;
            rotY *= (2f - squash);
            
            // Flip if facing left
            if (!facingRight) rotX = -rotX;
            
            // Add bounce
            rotY += legBounce;
            
            // Apply world position
            flowerSpheres[i].transform.position = new Vector3(rotX + xPos, rotY + yPos, 0f);
            flowerSpheres[i].transform.Rotate(spinSpeed * Time.deltaTime * 0.1f, spinSpeed * Time.deltaTime * 0.05f, 0f);
            
            UpdateFlowerColor(i);
        }
    }
    
    void UpdateStarChasing(float xPos, float yPos)
    {
        float chaseCycle = bounceTime * 6f;
        float bounce = Mathf.Abs(Mathf.Sin(chaseCycle)) * 2f;
        
        // Rotation - opposite direction, faster when loud
        float rotationAngle = -time * spinSpeed * 0.006f;
        
        for (int i = 0; i < numSphere; i++)
        {
            float t = i * 2 * Mathf.PI / numSphere;
            float r = 5f * (1f + 0.6f * Mathf.Cos(5 * t)) * (1f + AudioSpectrum.audioAmp * 0.4f);
            
            float localX = r * Mathf.Cos(t);
            float localY = r * Mathf.Sin(t);
            
            // Apply rotation
            float rotX = localX * Mathf.Cos(rotationAngle) - localY * Mathf.Sin(rotationAngle);
            float rotY = localX * Mathf.Sin(rotationAngle) + localY * Mathf.Cos(rotationAngle);
            
            // Angry wobble
            rotX *= 1f + Mathf.Sin(chaseCycle * 2f) * 0.15f;
            rotY += bounce;
            
            starSpheres[i].transform.position = new Vector3(rotX + xPos, rotY + yPos, 0f);
            starSpheres[i].transform.Rotate(spinSpeed * Time.deltaTime * 0.2f, 0f, spinSpeed * Time.deltaTime * 0.15f);
            
            // Angry pink/magenta color - gets more intense when loud
            Renderer rend = starSpheres[i].GetComponent<Renderer>();
            float hue = 0.92f - AudioSpectrum.audioAmp * 0.04f; // Shifts toward red/magenta when loud
            rend.material.color = Color.HSVToRGB(Mathf.Max(0.85f, hue), 0.9f, 1f);
        }
    }
    
    void UpdateStarRunning(float xPos, float yPos)
    {
        float runCycle = bounceTime * 7f;
        float bounce = Mathf.Abs(Mathf.Sin(runCycle)) * 2f;
        
        // spinning
        float rotationAngle = time * spinSpeed * 0.008f;
        
        for (int i = 0; i < numSphere; i++)
        {
            float t = i * 2 * Mathf.PI / numSphere;
            float r = 5f * (1f + 0.6f * Mathf.Cos(5 * t));
            
            float localX = -r * Mathf.Cos(t); // Flipped - running left
            float localY = r * Mathf.Sin(t);
            
            // Apply rotation
            float rotX = localX * Mathf.Cos(rotationAngle) - localY * Mathf.Sin(rotationAngle);
            float rotY = localX * Mathf.Sin(rotationAngle) + localY * Mathf.Cos(rotationAngle);
            
            rotY += bounce;
            
            // Scared wobble
            float scared = Mathf.Sin(bounceTime * 10f + i * 0.1f) * AudioSpectrum.audioAmp;
            rotX += scared;
            
            starSpheres[i].transform.position = new Vector3(rotX + xPos, rotY + yPos, 0f);
            
            // lighter pink color
            Renderer rend = starSpheres[i].GetComponent<Renderer>();
            float hue = 0.95f + AudioSpectrum.audioAmp * 0.03f; // Lighter pink when scared
            rend.material.color = Color.HSVToRGB(hue % 1f, 0.7f, 1f);
        }
    }
    
    void UpdateFlowerColor(int i)
    {
        Renderer rend = flowerSpheres[i].GetComponent<Renderer>();
        // Yellow color with slight orange shift when loud
        float hue = 0.14f + Mathf.Sin(time * 0.5f + i * 0.02f) * 0.03f;
        float saturation = 0.85f + AudioSpectrum.audioAmp * 0.15f;
        rend.material.color = Color.HSVToRGB(hue, saturation, 1f);
    }
    
    void UpdateWaves()
    {
        for (int i = 0; i < numWaveParticles; i++)
        {
            float x = wavePositions[i].x;
            float waveOffset = Mathf.Sin(time * 2f + x * 0.3f) * (0.5f + AudioSpectrum.audioAmp);
            float y = wavePositions[i].y + waveOffset;
            
            waveParticles[i].transform.position = new Vector3(x, y, 1f);
            
            // Wave color pulse
            Renderer rend = waveParticles[i].GetComponent<Renderer>();
            float brightness = 0.8f + AudioSpectrum.audioAmp * 0.2f;
            rend.material.color = Color.HSVToRGB(0.55f, 0.6f, brightness);
        }
    }
    
    void UpdateBubbles()
    {
        for (int i = 0; i < numBubbles; i++)
        {
            // Bubbles float up and respawn
            Vector3 pos = bubbleSpheres[i].transform.position;
            pos.y += Time.deltaTime * (1f + AudioSpectrum.audioAmp * 2f);
            pos.x += Mathf.Sin(time + i) * Time.deltaTime * 0.5f;
            
            // Respawn at bottom when reaching top
            if (pos.y > 12f)
            {
                pos.y = -10f;
                pos.x = Random.Range(-15f, 15f);
            }
            
            bubbleSpheres[i].transform.position = pos;
            
            // Size pulse with audio
            float scale = 0.2f + AudioSpectrum.audioAmp * 0.4f + Mathf.Sin(time * 3f + i) * 0.1f;
            bubbleSpheres[i].transform.localScale = new Vector3(scale, scale, scale);
        }
    }
        
    float EaseOutBounce(float t)
    {
        if (t < 1f / 2.75f)
            return 7.5625f * t * t;
        else if (t < 2f / 2.75f)
            return 7.5625f * (t -= 1.5f / 2.75f) * t + 0.75f;
        else if (t < 2.5f / 2.75f)
            return 7.5625f * (t -= 2.25f / 2.75f) * t + 0.9375f;
        else
            return 7.5625f * (t -= 2.625f / 2.75f) * t + 0.984375f;
    }
    
    float EaseOutElastic(float t)
    {
        if (t == 0f || t == 1f) return t;
        return Mathf.Pow(2f, -10f * t) * Mathf.Sin((t - 0.075f) * (2f * Mathf.PI) / 0.3f) + 1f;
    }
    
    float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }
    
    float EaseInOutCubic(float t)
    {
        return t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
    }
}