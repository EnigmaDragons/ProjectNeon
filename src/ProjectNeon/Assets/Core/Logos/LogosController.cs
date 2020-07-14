using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogosController : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite secondSprite;
    [SerializeField] private float showDuration = 2f;
    [SerializeField] private float transitionDuration = 0.75f;

    private Color targetColor;
    private Color targetTransparent;

    private bool _isOnSecondSprite;
    private float _fadingInFinishedInSeconds;
    private float _startFadingOutInSeconds;
    private float _finishInSeconds;
    private bool _finishedCurrent;
    private bool _startedLoading;

    private void Awake()
    {
        targetColor = image.color;
        targetTransparent = new Color(targetColor.r, targetColor.g, targetColor.b, 0f);
        image.color = targetTransparent;
        BeginAnim();
    }

    private bool AnyRelevantButtonPress() => Input.GetButton("Fire1") || Input.GetButton("Cancel") ||
                                             Input.GetButton("Submit") || Input.GetButton("Jump") ||
                                             Input.GetButton("Fire2");
    private bool AnyMouseButtonDown() => Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1);
    
    private void FixedUpdate()
    {
        if (_startedLoading)
            return;
        
        UpdateCounters();
        UpdatePresentation();
        if (AnyMouseButtonDown() || AnyRelevantButtonPress())
        {
            StartLoad();
        }
        else if (_finishedCurrent && !_isOnSecondSprite)
        {
            if (secondSprite == null)
            {
                StartLoad();
            }
            else
            {
                _isOnSecondSprite = true;
                image.sprite = secondSprite;
                BeginAnim();
            }
        }
        else if (_finishedCurrent && _isOnSecondSprite)
        {
            StartLoad();
        }
    }

    private void StartLoad()
    {
        _startedLoading = true;
        image.color = targetTransparent;
        Invoke("NavigateToNextScene", 0.2f);
    }

    private void BeginAnim()
    {
        _finishedCurrent = false;
        _finishInSeconds = showDuration + transitionDuration * 2;
        _fadingInFinishedInSeconds = transitionDuration;
        _startFadingOutInSeconds = transitionDuration + showDuration;
    }

    private void UpdatePresentation()
    {
        if (_finishedCurrent)
            return;

        if (_fadingInFinishedInSeconds > 0.01f)
            image.color = Color.Lerp(targetTransparent, targetColor, (transitionDuration - _fadingInFinishedInSeconds) / transitionDuration);
        if (_startFadingOutInSeconds < 0.1f)
            image.color = Color.Lerp(targetColor, targetTransparent, (transitionDuration - _finishInSeconds) / transitionDuration);
        if (_finishInSeconds < 0.01f)
            _finishedCurrent = true;
    }

    private void UpdateCounters()
    {
        _fadingInFinishedInSeconds = Mathf.Max(0, _fadingInFinishedInSeconds - Time.deltaTime);
        _startFadingOutInSeconds = Mathf.Max(0, _startFadingOutInSeconds - Time.deltaTime);
        _finishInSeconds = Mathf.Max(0, _finishInSeconds - Time.deltaTime);
    }

    private void NavigateToNextScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
}
