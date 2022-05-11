using System;
using UnityEngine;

public class DragRotator : MonoBehaviour
{
    [SerializeField] private DragRotatorSettings settings = new DragRotatorSettings();
    private const float EPSILON = 0.0001f;
    private const float SMOOTH_DAMP_SEC_FUDGE = 0.1f;
    
    private Vector3 m_prevPos;
    private Vector3 m_originalAngles;
    private float m_pitchDeg;
    private float m_rollDeg;
    private float m_pitchVel;
    private float m_rollVel;

    private void Awake()
    {
        m_originalAngles = transform.localRotation.eulerAngles;
        Reset();
    }

    private void Update()
    {
        try
        {
            var position = transform.position;
            var vector3 = position - m_prevPos;
            if ((double) vector3.sqrMagnitude > 9.99999974737875E-05)
            {
                m_pitchDeg += vector3.y * settings.m_PitchState.m_ForceMultiplier;
                m_pitchDeg = Mathf.Clamp(m_pitchDeg, settings.m_PitchState.m_MinDegrees, settings.m_PitchState.m_MaxDegrees);
                m_rollDeg -= vector3.x * settings.m_RollState.m_ForceMultiplier;
                m_rollDeg = Mathf.Clamp(m_rollDeg, settings.m_RollState.m_MinDegrees, settings.m_RollState.m_MaxDegrees);
            }

            m_pitchDeg = Mathf.SmoothDamp(m_pitchDeg, 0.0f, ref m_pitchVel, settings.m_PitchState.m_RestSeconds * SMOOTH_DAMP_SEC_FUDGE);
            m_rollDeg = Mathf.SmoothDamp(m_rollDeg, 0.0f, ref m_rollVel, settings.m_RollState.m_RestSeconds * SMOOTH_DAMP_SEC_FUDGE);
            transform.localRotation = Quaternion.Euler(m_originalAngles.x + m_pitchDeg, m_originalAngles.y + m_rollDeg, m_originalAngles.z);
            m_prevPos = position;
        }
        catch (Exception e)
        {
            Log.Warn($"Drag Rotator Exception - {e}");
        }
    }

    public void Reset()
    {
        m_prevPos = transform.position;
        transform.localRotation = Quaternion.Euler(m_originalAngles);
        m_rollDeg = 0.0f;
        m_rollVel = 0.0f;
        m_pitchDeg = 0.0f;
        m_pitchVel = 0.0f;
    }
}