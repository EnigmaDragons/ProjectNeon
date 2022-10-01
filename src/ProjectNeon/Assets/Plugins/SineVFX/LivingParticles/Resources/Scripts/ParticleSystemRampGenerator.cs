using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemRampGenerator : MonoBehaviour {

    public Gradient procedrualGradientRamp;
    public bool procedrualGradientEnabled = false;
    public bool updateEveryFrame = false;

    private ParticleSystemRenderer psr;
    private Texture2D rampTexture;
    private Texture2D tempTexture;
    private float width = 256;
    private float height = 1;

    void Start () {
        psr = GetComponent<ParticleSystemRenderer>();

        if (procedrualGradientEnabled == true)
        {
            UpdateRampTexture();
        }
    }

    void Update () {
        if (procedrualGradientEnabled == true)
        {
            if (updateEveryFrame == true)
            {
                UpdateRampTexture();
            }
        }
    }

    // Generating a texture from gradient variable
    Texture2D GenerateTextureFromGradient(Gradient grad)
    {        
        if (tempTexture == null)
        {
            tempTexture = new Texture2D((int)width, (int)height);
        }        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color col = grad.Evaluate(0 + (x / width));
                tempTexture.SetPixel(x, y, col);
            }
        }
        tempTexture.wrapMode = TextureWrapMode.Clamp;
        tempTexture.Apply();
        return tempTexture;
    }

    // Update procedural ramp textures and applying them to the shaders
    public void UpdateRampTexture()
    {
        rampTexture = GenerateTextureFromGradient(procedrualGradientRamp);
        psr.material.SetTexture("_Ramp", rampTexture);
    }
}
