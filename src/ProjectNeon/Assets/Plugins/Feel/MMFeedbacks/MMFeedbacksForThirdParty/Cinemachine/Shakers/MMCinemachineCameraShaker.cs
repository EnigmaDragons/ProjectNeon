using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using MoreMountains.Feedbacks;

namespace MoreMountains.FeedbacksForThirdParty
{
    /// <summary>
    /// Add this component to your Cinemachine Virtual Camera to have it shake when calling its ShakeCamera methods.
    /// </summary>
    [AddComponentMenu("More Mountains/Feedbacks/Shakers/Cinemachine/MMCinemachineCameraShaker")]
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class MMCinemachineCameraShaker : MonoBehaviour
    {
        [Header("Settings")]
        /// the channel to receive events on
        [Tooltip("the channel to receive events on")]
        public int Channel = 0;
        /// The default amplitude that will be applied to your shakes if you don't specify one
        [Tooltip("The default amplitude that will be applied to your shakes if you don't specify one")]
        public float DefaultShakeAmplitude = .5f;
        /// The default frequency that will be applied to your shakes if you don't specify one
        [Tooltip("The default frequency that will be applied to your shakes if you don't specify one")]
        public float DefaultShakeFrequency = 10f;
        /// the amplitude of the camera's noise when it's idle
        [Tooltip("the amplitude of the camera's noise when it's idle")]
        [MMFReadOnly]
        public float IdleAmplitude;
        /// the frequency of the camera's noise when it's idle
        [Tooltip("the frequency of the camera's noise when it's idle")]
        [MMFReadOnly]
        public float IdleFrequency = 1f;
        /// the speed at which to interpolate the shake
        [Tooltip("the speed at which to interpolate the shake")]
        public float LerpSpeed = 5f;

        [Header("Test")]
        /// a duration (in seconds) to apply when testing this shake via the TestShake button
        [Tooltip("a duration (in seconds) to apply when testing this shake via the TestShake button")]
        public float TestDuration = 0.3f;
        /// the amplitude to apply when testing this shake via the TestShake button
        [Tooltip("the amplitude to apply when testing this shake via the TestShake button")]
        public float TestAmplitude = 2f;
        /// the frequency to apply when testing this shake via the TestShake button
        [Tooltip("the frequency to apply when testing this shake via the TestShake button")]
        public float TestFrequency = 20f;

        [MMFInspectorButton("TestShake")]
        public bool TestShakeButton;
        
        public virtual float GetTime() { return (_timescaleMode == TimescaleModes.Scaled) ? Time.time : Time.unscaledTime; }
        public virtual float GetDeltaTime() { return (_timescaleMode == TimescaleModes.Scaled) ? Time.deltaTime : Time.unscaledDeltaTime; }

        protected TimescaleModes _timescaleMode;
        protected Vector3 _initialPosition;
        protected Quaternion _initialRotation;
        protected Cinemachine.CinemachineBasicMultiChannelPerlin _perlin;
        protected Cinemachine.CinemachineVirtualCamera _virtualCamera;
        protected float _targetAmplitude;
        protected float _targetFrequency;
        private Coroutine _shakeCoroutine;

        /// <summary>
        /// On awake we grab our components
        /// </summary>
        protected virtual void Awake()
        {
            _virtualCamera = this.gameObject.GetComponent<CinemachineVirtualCamera>();
            _perlin = _virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
        }

        /// <summary>
        /// On Start we reset our camera to apply our base amplitude and frequency
        /// </summary>
        protected virtual void Start()
        {
            if (_perlin != null)
            {
                IdleAmplitude = _perlin.m_AmplitudeGain;
                IdleFrequency = _perlin.m_FrequencyGain;
            }            

            _targetAmplitude = IdleAmplitude;
            _targetFrequency = IdleFrequency;
        }

        protected virtual void Update()
        {
            if (_perlin != null)
            {
                _perlin.m_AmplitudeGain = _targetAmplitude;
                _perlin.m_FrequencyGain = Mathf.Lerp(_perlin.m_FrequencyGain, _targetFrequency, GetDeltaTime() * LerpSpeed);
            }
        }

        /// <summary>
        /// Use this method to shake the camera for the specified duration (in seconds) with the default amplitude and frequency
        /// </summary>
        /// <param name="duration">Duration.</param>
        public virtual void ShakeCamera(float duration, bool infinite, bool useUnscaledTime = false)
        {
            StartCoroutine(ShakeCameraCo(duration, DefaultShakeAmplitude, DefaultShakeFrequency, infinite, useUnscaledTime));
        }

        /// <summary>
        /// Use this method to shake the camera for the specified duration (in seconds), amplitude and frequency
        /// </summary>
        /// <param name="duration">Duration.</param>
        /// <param name="amplitude">Amplitude.</param>
        /// <param name="frequency">Frequency.</param>
        public virtual void ShakeCamera(float duration, float amplitude, float frequency, bool infinite, bool useUnscaledTime = false)
        {
            if (_shakeCoroutine != null)
            {
                StopCoroutine(_shakeCoroutine);
            }
            _shakeCoroutine = StartCoroutine(ShakeCameraCo(duration, amplitude, frequency, infinite, useUnscaledTime));
        }

        /// <summary>
        /// This coroutine will shake the 
        /// </summary>
        /// <returns>The camera co.</returns>
        /// <param name="duration">Duration.</param>
        /// <param name="amplitude">Amplitude.</param>
        /// <param name="frequency">Frequency.</param>
        protected virtual IEnumerator ShakeCameraCo(float duration, float amplitude, float frequency, bool infinite, bool useUnscaledTime)
        {
            _targetAmplitude  = amplitude;
            _targetFrequency = frequency;
            _timescaleMode = useUnscaledTime ? TimescaleModes.Unscaled : TimescaleModes.Scaled;
            if (!infinite)
            {
                yield return new WaitForSeconds(duration);
                CameraReset();
            }                        
        }

        /// <summary>
        /// Resets the camera's noise values to their idle values
        /// </summary>
        public virtual void CameraReset()
        {
            _targetAmplitude = IdleAmplitude;
            _targetFrequency = IdleFrequency;
        }

        public virtual void OnCameraShakeEvent(float duration, float amplitude, float frequency, float amplitudeX, float amplitudeY, float amplitudeZ, bool infinite, int channel, bool useUnscaledTime)
        {
            if ((channel != Channel) && (channel != -1) && (Channel != -1))
            {
                return;
            }
            this.ShakeCamera(duration, amplitude, frequency, infinite, useUnscaledTime);
        }

        public virtual void OnCameraShakeStopEvent(int channel)
        {
            if ((channel != Channel) && (channel != -1) && (Channel != -1))
            {
                return;
            }
            if (_shakeCoroutine != null)
            {
                StopCoroutine(_shakeCoroutine);
            }            
            CameraReset();
        }

        protected virtual void OnEnable()
        {
            MMCameraShakeEvent.Register(OnCameraShakeEvent);
            MMCameraShakeStopEvent.Register(OnCameraShakeStopEvent);
        }

        protected virtual void OnDisable()
        {
            MMCameraShakeEvent.Unregister(OnCameraShakeEvent);
            MMCameraShakeStopEvent.Unregister(OnCameraShakeStopEvent);
        }

        protected virtual void TestShake()
        {
            MMCameraShakeEvent.Trigger(TestDuration, TestAmplitude, TestFrequency, 0f, 0f, 0f, false, 0);
        }
    }
}