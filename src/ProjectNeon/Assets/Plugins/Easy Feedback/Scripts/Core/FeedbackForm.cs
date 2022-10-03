using System;
using System.Collections;
using System.IO;
using System.Linq;
using AeLa.EasyFeedback.APIs;
using AeLa.EasyFeedback.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AeLa.EasyFeedback
{
    public class FeedbackForm : MonoBehaviour
    {
	    private const int BIG_TEX = 4082;
	    private const float TEX_DIMENSION_MAX = 1920;
	    
        [Tooltip("Easy Feedback configuration file")]
        public EFConfig Config;

        [Tooltip("Key to toggle feedback window")]
        public KeyCode FeedbackKey = KeyCode.F12;

        [Tooltip("Include screenshot with reports?")]
        public bool IncludeScreenshot = true;

        /// <summary>
        /// Resizes screenshots larger than 1080p to help with Trello's filesize limit.
        /// </summary>
        [Tooltip("Resizes screenshots larger than 1080p to help with Trello's filesize limit.")]
        public bool ResizeLargeScreenshots = true;

        public Transform Form;

        /// <summary name="OnFormOpened">
        /// Called when the form is first opened, right before it is shown on screen
        /// </summary>
        [Tooltip("Functions to be called when the form is first opened")]
        public UnityEvent OnFormOpened;

        /// <summary name="OnFormSubmitted">
        /// Called right before the report is sent to Trello,
        /// so additional information may be added.
        /// </summary>
        [Tooltip("Functions to be called when the form is submitted")]
        public UnityEvent OnFormSubmitted;

        /// <summary name="OnFormClosed">
        /// Called when the form is closed, whether or not it was submitted
        /// </summary>
        [Tooltip("Functions to be called when the form is closed")]
        public UnityEvent OnFormClosed;

        /// <summary>
        /// Called to notify of any errors during submission
        /// </summary>
        [Tooltip("Called to notify of any errors during submission")]
        public SubmissionMessageEvent OnSubmissionError;

        /// <summary>
        /// Called when the submission has successfully completed
        /// </summary>
        [Tooltip("Called when the submission has successfully completed")]
        public UnityEvent OnSubmissionSucceeded;

        /// <summary>
        /// Called if the submission fails
        /// </summary>
        [Tooltip("Called if the submission fails")]
        public UnityEvent OnSubmissionFailed;

        /// <summary>
        /// A submission event including a message
        /// </summary>
        [Serializable]
        public class SubmissionMessageEvent : UnityEvent<string>
        {
        }

        /// <summary>
        /// The current report being built.
        /// Will be sent as next report
        /// </summary>
        public Report CurrentReport;

        private CursorLockMode initCursorLockMode;

        private bool initCursorVisible;

        // form metadata
        private string screenshotPath;

        private Coroutine ssCoroutine;

        private bool submitting;

        // api handler
        private Trello trello;

        /// <summary>
        /// Whether or not the form is currently being displayed
        /// </summary>
        public bool IsOpen => Form.gameObject.activeSelf;

        private bool _wasOpen;

        public void Awake()
        {
            if (!Config.StoreLocal)
                InitTrelloAPI();

            // initialize current report
            InitCurrentReport();
        }

        // Update is called once per frame
        private void Update()
        {
            // show form when player hits F12
            if (Input.GetKeyDown(FeedbackKey)
                && !IsOpen
                && ssCoroutine == null)
                Show();
            else if ((Input.GetKeyDown(FeedbackKey) ||
                      Input.GetKeyDown(KeyCode.Escape)
                     ) // close form if f12 is hit again,  or escape is hit
                     && IsOpen
                     && !submitting)
                Hide();
            if (!_wasOpen && IsOpen && ssCoroutine == null)
            {
                InitCurrentReport();
                ssCoroutine = StartCoroutine(ScreenshotAndOpenForm());
            }
            _wasOpen = IsOpen;
        }

        public void InitTrelloAPI()
        {
            // initialize api handler
            trello = new Trello(Config.Token);
        }

        /// <summary>
        /// Replaces currentReport with a new instance of Report
        /// </summary>
        private void InitCurrentReport()
        {
            ssCoroutine = null;
            CurrentReport = new Report();
        }

        /// <summary>
        /// Takes a screenshot, then opens the form
        /// </summary>
        public void Show()
        {
            if (!IsOpen && ssCoroutine == null)
                ssCoroutine = StartCoroutine(ScreenshotAndOpenForm());
        }

        /// <summary>
        /// Called by the submit button, submits the form.
        /// </summary>
        public void Submit()
        {
            StartCoroutine(SubmitAsync());
        }

        private IEnumerator SubmitAsync()
        {
            // disable form
            DisableForm();

            submitting = true;

            // call OnFormSubmitted
            OnFormSubmitted.Invoke();

            // close form
            Hide();

            if (!Config.StoreLocal)
            {
                // add card to board
                yield return trello.AddCard(
                    CurrentReport.Title ?? "[no summary]",
                    CurrentReport.ToString() ?? "[no detail]",
                    CurrentReport.Labels,
                    CurrentReport.List.id
                );
                
                // send up attachments 
                if (trello.LastAddCardResponse != null && !trello.UploadError)
                    yield return AttachFilesAsync(
                        trello.LastAddCardResponse.id
                    );
            }
            else
            {
                // store entire report locally, then return
                var localPath = WriteLocal(CurrentReport);
                Debug.Log(localPath);
            }

            if (!Config.StoreLocal && trello.UploadError)
            {
                // preserve report locally if there's an issue during upload
                Debug.Log(WriteLocal(CurrentReport));

                // notify failure
                OnSubmissionError.Invoke(
                    "Trello upload failed.\n" +
                    "Reason: " + trello.ErrorMessage
                );

                if (trello.UploadException != null)
                    Debug.LogException(trello.UploadException);
                else
                    Debug.LogError(trello.ErrorMessage);

                OnSubmissionFailed.Invoke();
            }
            else
            {
                // report success
                OnSubmissionSucceeded.Invoke();
            }

            submitting = false;
            InitCurrentReport();
        }

        /// <summary>
        /// Attaches files on current report to card
        /// </summary>
        /// <param name="cardID"></param>
        /// <returns></returns>
        private IEnumerator AttachFilesAsync(string cardID)
        {
            for (var i = 0; i < CurrentReport.Attachments.Count; i++)
            {
                var attachment = CurrentReport.Attachments[i];
                yield return trello.AddAttachmentAsync(
                    cardID, attachment.Data, null, attachment.Name
                );

                if (trello.UploadError) // failed to add attachment
                    OnSubmissionError.Invoke(
                        "Failed to attach file to report.\n" +
                        "Reason: " + trello.ErrorMessage
                    );
            }
        }

        /// <summary>
        /// Saves the report in a local directory
        /// </summary>
        private string WriteLocal(Report report)
        {
            // create the report directory
            var feedbackDirectory = Application.persistentDataPath +
                                    "/feedback-" +
                                    DateTime.Now.ToString("MMddyyyy-HHmmss");
            Directory.CreateDirectory(feedbackDirectory);

            // save the report
            File.WriteAllText(
                feedbackDirectory + "/report.txt", report.GetLocalFileText()
            );

            // save attachments
            for (var i = 0; i < report.Attachments.Count; i++)
            {
                var attachment = report.Attachments[i];
                File.WriteAllBytes(
                    feedbackDirectory + "/" + attachment.Name, attachment.Data
                );
            }

            return feedbackDirectory;
        }

        /// <summary>
        /// Disables all the Selectable elements on the form.
        /// </summary>
        public void DisableForm()
        {
            foreach (Transform child in Form)
            {
                var selectable = child.GetComponent<Selectable>();
                if (selectable != null) selectable.interactable = false;
            }
        }

        /// <summary>
        /// Enables all the Selectable elements on the form.
        /// </summary>
        public void EnableForm()
        {
            foreach (Transform child in Form)
            {
                var selectable = child.GetComponent<Selectable>();
                if (selectable != null) selectable.interactable = true;
            }
        }

        /// <summary>
        /// Hides the form, called by the Close button.
        /// </summary>
        public void Hide()
        {
            // don't do anything if the form is already hidden
            if (!Form.gameObject.activeInHierarchy)
                return;

            // hide form
            Form.gameObject.SetActive(false);

            // delete temporary screenshot
            if (!Config.StoreLocal && IncludeScreenshot
                                   && File.Exists(screenshotPath))
            {
                if (ssCoroutine != null) StopCoroutine(ssCoroutine);

                File.Delete(screenshotPath);
            }

            screenshotPath = string.Empty;

            // clear screenshot coroutine
            ssCoroutine = null;

            // call OnFormClosed
            OnFormClosed.Invoke();
        }

        private void ReleaseMouse()
        {
            // show mouse
            initCursorVisible = Cursor.visible;
            initCursorLockMode = Cursor.lockState;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void HideMouse()
        {
            Cursor.visible = initCursorVisible;
            Cursor.lockState = initCursorLockMode;
        }
        
        private IEnumerator ScreenshotAndOpenForm()
        {
            if (IncludeScreenshot && !CurrentReport.Attachments.Any(a => a.Name.Equals("screenshot.png")))
            {
                Form.gameObject.SetActive(false);
	            // ScreenCapture.CaptureScreenshot doesn't seem to work properly
	            // see: https://answers.unity.com/questions/1655518/screencapturecapturescreenshotastexture-is-making.html
	            yield return new WaitForEndOfFrame();
	            
                var tex = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
                tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
	            tex.Apply();
                
                if (ResizeLargeScreenshots && (tex.width ^ 2 * tex.height ^ 2) > BIG_TEX)
                {
	                // resize so largest dimension is <= 1080p
	                tex.Scale(TEX_DIMENSION_MAX / Mathf.Max(tex.width, tex.height));
                }
                
                CurrentReport.AttachFile("screenshot.png", tex.EncodeToPNG());
            }

            ReleaseMouse();

            // show form
            EnableForm();
            Form.gameObject.SetActive(true);

            // call OnFormOpened
            OnFormOpened.Invoke();
        }
    }
}