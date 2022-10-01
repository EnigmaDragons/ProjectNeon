using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleGridGenerator : MonoBehaviour {

    public bool rewriteVertexStreams = true;
    public float particleSize = 1f;
    public Color particleColor = Color.white;
    public Vector3 particleRotation3D;
    public bool randomColorAlpha = true; // For MetallicSmoothness random offset
    public float xDistance = 0.25f;
    public float yDistance = 0.25f;
    public float zDistance = 0.25f;
    public int xSize = 10;
    public int ySize = 10;
    public int zSize = 10;
    public float OffsetEven = 0.125f;
    public bool updateEveryFrame = false;

    private float even;
    private Vector3[] positions;
    private ParticleSystem ps;
    private ParticleSystem.Particle[] particles;
    private List<Vector4> customData = new List<Vector4>();
    private List<Vector4> customData2 = new List<Vector4>();

    void Start () {
        ps = GetComponent<ParticleSystem>();
        UpdateGrid();
    }

    private void OnEnable()
    {
        ps = GetComponent<ParticleSystem>();
        UpdateGrid();
    }

    public void UpdateGrid()
    {
        GenerateGrid();
        GenerateParticles();
        CreateOffsetVector();

        ParticleSystemRenderer psrend = GetComponent<ParticleSystemRenderer>();
        if (rewriteVertexStreams == true)
        {
            psrend.SetActiveVertexStreams(new List<ParticleSystemVertexStream>(new ParticleSystemVertexStream[] { ParticleSystemVertexStream.Position, ParticleSystemVertexStream.Normal, ParticleSystemVertexStream.Color, ParticleSystemVertexStream.UV, ParticleSystemVertexStream.Center, ParticleSystemVertexStream.Tangent, ParticleSystemVertexStream.Custom1XYZ }));

        }
        psrend.alignment = ParticleSystemRenderSpace.Local;
    }

    // Generating array of positions
    private void GenerateGrid()
    {
        positions = new Vector3[xSize * ySize * zSize];
        for (int z = 0, i = 0; z < zSize; z++)
        {
            even = 0f;
            if (z % 2 == 0)
            {
                even = OffsetEven;
            }
            for (int y = 0; y < ySize; y++)
            {                
                for (int x = 0; x < xSize; x++, i++)
                {                    
                    positions[i] = new Vector3(x * xDistance + even, y * yDistance, z * zDistance);
                }
            }
        }        
    }

    // Generating particles with grid based positions
    private void GenerateParticles()
    {
        particles = new ParticleSystem.Particle[xSize * ySize * zSize];
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].position = positions[i];
            if (randomColorAlpha == true)
            particleColor.a = Random.Range(0f, 1f);
            particles[i].startColor = particleColor;
            particles[i].startSize = particleSize;
            particles[i].rotation3D = particleRotation3D;
        }
        ps.SetParticles(particles, particles.Length);
    }

    // Creating Vector for Offset
    private void CreateOffsetVector()
    {
        ps.GetCustomParticleData(customData, ParticleSystemCustomData.Custom1);        

        for (int i = 0; i < particles.Length; i++)
        {
            customData[i] = this.gameObject.transform.up;
        }

        ps.SetCustomParticleData(customData, ParticleSystemCustomData.Custom1);        
    }

    private void FixedUpdate()
    {
        if (updateEveryFrame == true)
        {
            UpdateGrid();
        }
    }
}
