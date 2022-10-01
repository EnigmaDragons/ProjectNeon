using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingParticlesAudioModule : MonoBehaviour {

    public Transform audioPosition;
    public LivingParticlesAudioSource LPaSourse;
    public bool useBuffer;
    public bool firstAndLastPixelBlack = false;

    private Texture2D t2d;
    private float[] finalSpectrum;
    private Color col = Color.black;
    private ParticleSystemRenderer psr;

    // Use this for initialization
    void Start () {
        psr = GetComponent<ParticleSystemRenderer>();
        switch (LPaSourse.numberOfBands)
        {
            case LivingParticlesAudioSource._numberOfBands.Bands8:
                if (firstAndLastPixelBlack == true)
                {
                    t2d = new Texture2D(10, 1);
                }
                else
                {
                    t2d = new Texture2D(8, 1);
                }                
                break;
            case LivingParticlesAudioSource._numberOfBands.Bands16:
                if(firstAndLastPixelBlack == true)
                {
                    t2d = new Texture2D(18, 1);
                }
                else
                {
                    t2d = new Texture2D(16, 1);
                }
                break;
            default:
                break;
        }        
        t2d.wrapMode = TextureWrapMode.Repeat;
    }
	
	// Update is called once per frame
	void Update () {
        if (useBuffer == true)
        {
            switch (LPaSourse.numberOfBands)
            {
                case LivingParticlesAudioSource._numberOfBands.Bands8:
                    finalSpectrum = LPaSourse.finalBands8Buffer;
                    break;
                case LivingParticlesAudioSource._numberOfBands.Bands16:
                    finalSpectrum = LPaSourse.finalBands16Buffer;
                    break;
                default:
                    break;
            }            
        }
        else
        {
            switch (LPaSourse.numberOfBands)
            {
                case LivingParticlesAudioSource._numberOfBands.Bands8:
                    finalSpectrum = LPaSourse.finalBands8;
                    break;
                case LivingParticlesAudioSource._numberOfBands.Bands16:
                    finalSpectrum = LPaSourse.finalBands16;
                    break;
                default:
                    break;
            }
        }
        
        for (int i = 0; i < finalSpectrum.Length; i++)
        {
            col.r = finalSpectrum[i];
            if (firstAndLastPixelBlack == true)
            {
                t2d.SetPixel(i+1, 0, col);
            }
            else
            {
                t2d.SetPixel(i, 0, col);
            }            
        }
        if (firstAndLastPixelBlack == true)
        {
            t2d.SetPixel(0, 0, Color.black);
            switch (LPaSourse.numberOfBands)
            {
                case LivingParticlesAudioSource._numberOfBands.Bands8:
                    t2d.SetPixel(9, 0, Color.black);
                    break;
                case LivingParticlesAudioSource._numberOfBands.Bands16:
                    t2d.SetPixel(17, 0, Color.black);
                    break;
                default:
                    break;
            }
        }
        t2d.Apply();

        psr.material.SetTexture("_AudioSpectrum", t2d);
        psr.material.SetVector("_AudioPosition", audioPosition.position);
        switch (LPaSourse.numberOfBands)
        {
            case LivingParticlesAudioSource._numberOfBands.Bands8:
                psr.material.SetFloat("_AudioAverageAmplitude", LPaSourse.amplitudeBuffer8);
                break;
            case LivingParticlesAudioSource._numberOfBands.Bands16:
                psr.material.SetFloat("_AudioAverageAmplitude", LPaSourse.amplitudeBuffer16);
                break;
            default:
                break;
        }
    }
}
