using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingParticlesAudioSource : MonoBehaviour {

    [Range(0.1f, 2f)]
    public float bufferInitialDecreaseSpeed = 1f;
    [Range (0f, 10f)]
    public float bufferDecreaseSpeedMultiply = 5f;
    public float freqBandsPower = 10f;
    public float audioProfileInitialValue = 5f;
    public bool audioProfileDecreasing = true;
    public float audioProfileDecreasingSpeed = 0.1f;

    public enum _numberOfBands { Bands8, Bands16 }
    public _numberOfBands numberOfBands = _numberOfBands.Bands8;

    private float[] initialSamplesL = new float[512];
    private float[] initialSamplesR = new float[512];

    private float[] freqBands8 = new float[8];
    private float[] freqBands8Buffer = new float[8];
    private float[] freqBands8BufferDecrease = new float[8];
    private float[] freqBands8Highest = new float[8];

    private float[] freqBands16 = new float[16];
    private float[] freqBands16Buffer = new float[16];
    private float[] freqBands16BufferDecrease = new float[16];
    private float[] freqBands16Highest = new float[16];

    private AudioSource audioSource;


    [HideInInspector] public float[] finalBands8 = new float[8];
    [HideInInspector] public float[] finalBands8Buffer = new float[8];

    [HideInInspector] public float[] finalBands16 = new float[16];
    [HideInInspector] public float[] finalBands16Buffer = new float[16];

    [HideInInspector] public float amplitude8, amplitudeBuffer8;
    [HideInInspector] public float amplitude16, amplitudeBuffer16;
    private float amplitudeHighest;

    

	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
        AudioProfile(audioProfileInitialValue);        
    }
	
	// Update is called once per frame
	void Update () {
        audioSource.GetSpectrumData(initialSamplesL, 0, FFTWindow.Blackman);
        audioSource.GetSpectrumData(initialSamplesR, 1, FFTWindow.Blackman);

        switch (numberOfBands)
        {
            case _numberOfBands.Bands8:
                CreateFreqBands8();
                CreateBandBuffer8();
                CreateFinalBands8();
                CreateAmplitude8();
                break;
            case _numberOfBands.Bands16:
                CreateFreqBands16();
                CreateBandBuffer16();
                CreateFinalBands16();
                CreateAmplitude16();
                break;
            default:
                break;
        }        
    }

    void AudioProfile(float audioProfileValue)
    {
        for (int i = 0; i < 8; i++)
        {
            freqBands8Highest[i] = audioProfileValue;
        }
        for (int i = 0; i < 16; i++)
        {
            freqBands16Highest[i] = audioProfileValue;
        }
    }

    // Creating average amplitude 8
    void CreateAmplitude8()
    {
        float currentAmplitude = 0f;
        float currentAmplitudeBuffer = 0f;
        for (int i = 0; i < 8; i++)
        {
            currentAmplitude += finalBands8[i];
            currentAmplitudeBuffer += finalBands8Buffer[i];
        }
        if (currentAmplitude > amplitudeHighest)
        {
            amplitudeHighest = currentAmplitude;
        }
        amplitude8 = currentAmplitude / amplitudeHighest;
        amplitudeBuffer8 = currentAmplitudeBuffer / amplitudeHighest;
    }

    // Creating average amplitude 16
    void CreateAmplitude16()
    {
        float currentAmplitude = 0f;
        float currentAmplitudeBuffer = 0f;
        for (int i = 0; i < 16; i++)
        {
            currentAmplitude += finalBands16[i];
            currentAmplitudeBuffer += finalBands16Buffer[i];
        }
        if (currentAmplitude > amplitudeHighest)
        {
            amplitudeHighest = currentAmplitude;
        }
        amplitude16 = currentAmplitude / amplitudeHighest;
        amplitudeBuffer16 = currentAmplitudeBuffer / amplitudeHighest;
    }

    // Creating Final Bands 8 to use in the Material
    void CreateFinalBands8()
    {
        for (int i = 0; i < 8; i++)
        {
            if (audioProfileDecreasing == true)
            {
                freqBands8Highest[i] -= audioProfileDecreasingSpeed * Time.deltaTime;
            }

            if (freqBands8[i] > freqBands8Highest[i])
            {
                freqBands8Highest[i] = freqBands8[i];
            }
            finalBands8[i] = freqBands8[i] / freqBands8Highest[i];
            finalBands8Buffer[i] = freqBands8Buffer[i] / freqBands8Highest[i];
        }        
    }

    // Creating Final Bands 16 to use in the Material
    void CreateFinalBands16()
    {
        for (int i = 0; i < 16; i++)
        {
            if (audioProfileDecreasing == true)
            {
                freqBands16Highest[i] -= audioProfileDecreasingSpeed * Time.deltaTime;
            }

            if (freqBands16[i] > freqBands16Highest[i])
            {
                freqBands16Highest[i] = freqBands16[i];
            }
            finalBands16[i] = freqBands16[i] / freqBands16Highest[i];
            finalBands16Buffer[i] = freqBands16Buffer[i] / freqBands16Highest[i];
        }
    }

    // Creating 8 Freq Bands Buffer
    void CreateBandBuffer8()
    {
        for (int y = 0; y < 8; y++)
        {
            if (freqBands8[y] > freqBands8Buffer[y])
            {
                freqBands8Buffer[y] = freqBands8[y];
                freqBands8BufferDecrease[y] = bufferInitialDecreaseSpeed * freqBands8Highest[y] * Time.deltaTime; // You can delete the second part of multiplication
            }

            if (freqBands8[y] < freqBands8Buffer[y])
            {
                freqBands8Buffer[y] -= freqBands8BufferDecrease[y];
                freqBands8BufferDecrease[y] *= 1f + (bufferDecreaseSpeedMultiply * Time.deltaTime);
            }
        }
    }

    // Creating 16 Freq Bands Buffer
    void CreateBandBuffer16()
    {
        for (int y = 0; y < 16; y++)
        {
            if (freqBands16[y] > freqBands16Buffer[y])
            {
                freqBands16Buffer[y] = freqBands16[y];
                freqBands16BufferDecrease[y] = bufferInitialDecreaseSpeed * freqBands16Highest[y] * Time.deltaTime; // You can delete the second part of multiplication
            }

            if (freqBands16[y] < freqBands16Buffer[y])
            {
                freqBands16Buffer[y] -= freqBands16BufferDecrease[y];
                freqBands16BufferDecrease[y] *= 1f + (bufferDecreaseSpeedMultiply * Time.deltaTime);
            }
        }
    }

    // Creating 8 Freq Bands
    void CreateFreqBands8()
    {
        int count = 0;

        for (int i = 0; i < 8; i++)
        {
            float average = 0f;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            if (i == 7)
            {
                sampleCount += 2;
            }
            for (int j = 0; j < sampleCount; j++)
            {
                average += (initialSamplesL[count] + initialSamplesR[count]) * (count + 1);
                count++;
            }

            average /= count;

            freqBands8[i] = average * freqBandsPower;
        }
    }

    // Creating 16 Freq Bands
    void CreateFreqBands16()
    {
        int count = 0;
        int sampleCount = 1;
        int power = 0;

        /*
         * 0 - 1 =   1 samples > 2
         * 2 - 3 =   2 samples > 4
         * 4 - 5 =   4 samples > 8
         * 6 - 7 =  8 samples > 16
         * 8 - 9 = 16 samples > 32
         * 10 - 11 = 32 samples > 64
         * 12 - 13 = 64 samples > 128
         * 14 - 15 = 128 samples > 256
         */

        for (int i = 0; i < 16; i++)
        {
            float average = 0f;

            if (i == 2 || i == 4 || i == 6 || i == 8 || i == 10 || i == 12 || i == 14)
            {
                power++;
                sampleCount = (int)Mathf.Pow(2, power);
                if (power == 7)
                {
                    sampleCount += 1;
                }
            }
            for (int j = 0; j < sampleCount; j++)
            {
                average += (initialSamplesL[count] + initialSamplesR[count]) * (count + 1);
                count++;
            }

            average /= count;

            freqBands16[i] = average * freqBandsPower * 2;
        }
    }
}
