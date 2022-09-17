using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using UnityEditor;
using System;
using System.Net.Mail;
using System.Net;


using UnityEngine.Events;
using UnityEngine.UI;

using System.Text;


using UnityEngine.Networking;


namespace Orangedkeys.WallFX
{
    public class WallFX_Welcome : EditorWindow
    {
        //sUBSCRIBE
        private const string kGFormBaseURL = "https://docs.google.com/forms/d/e/1FAIpQLSfC-ISb57A-WfUnZPXx0eSXBHaWPAML4StnOwSGelPuyosMxA/";
        private const string kGFormEntryID = "entry.837402547";

        // links
        public const string WEB_URL = "https://www.orangedkeys.com";
        public const string YOUTUBE_URL = "https://www.youtube.com/channel/UC68I9tTol5hAoyVVAVedPag";

        private string email;
        private EditorGUILayout Label;

        private static readonly int WelcomeWindowWidth = 512;
        private static readonly int WelcomeWindowHeight = 512;

        private static Texture2D tex;

        private static GUIStyle _largeTextStyle;
        public static GUIStyle LargeTextStyle
        {
            get
            {
                if (_largeTextStyle == null)
                {
                    _largeTextStyle = new GUIStyle(UnityEngine.GUI.skin.label)
                    {
                        richText = true,
                        wordWrap = true,
                        fontStyle = FontStyle.Bold,
                        fontSize = 14,
                        alignment = TextAnchor.MiddleLeft,
                        padding = new RectOffset() { left = 0, right = 0, top = 0, bottom = 0 }
                    };
                }
                _largeTextStyle.normal.textColor = new Color32(200, 100, 0, 255);
                return _largeTextStyle;
            }
        }

        private static GUIStyle _regularTextStyle;
        public static GUIStyle RegularTextStyle
        {
            get
            {
                if (_regularTextStyle == null)
                {
                    _regularTextStyle = new GUIStyle(UnityEngine.GUI.skin.label)
                    {
                        richText = true,
                        wordWrap = true,
                        fontStyle = FontStyle.Normal,
                        fontSize = 12,
                        alignment = TextAnchor.MiddleLeft,
                        padding = new RectOffset() { left = 0, right = 0, top = 0, bottom = 0 }
                    };
                }
                return _regularTextStyle;
            }
        }

        private static GUIStyle _footerTextStyle;
        public static GUIStyle FooterTextStyle
        {
            get
            {
                if (_footerTextStyle == null)
                {
                    _footerTextStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                    {
                        alignment = TextAnchor.LowerCenter,
                        wordWrap = true,
                        fontSize = 12
                    };
                }

                return _footerTextStyle;
            }
        }

        [MenuItem("Tools/OrangedKeys/WallFX_Pack/About ", false, 0)]
        public static void ShowWindow()
        {
            EditorWindow editorWindow = GetWindow(typeof(WallFX_Welcome), false, " About ", true);
            editorWindow.autoRepaintOnSceneChange = true;
            editorWindow.titleContent.image = EditorGUIUtility.IconContent("animationdopesheetkeyframe").image;
            editorWindow.maxSize = new Vector2(WelcomeWindowWidth, WelcomeWindowHeight);
            editorWindow.minSize = new Vector2(WelcomeWindowWidth, WelcomeWindowHeight);
            editorWindow.position = new Rect(Screen.width / 2 + WelcomeWindowWidth / 2, Screen.height / 2, WelcomeWindowWidth, WelcomeWindowHeight);
            editorWindow.Show();
        }

        private void OnGUI()
        {
            if (EditorApplication.isCompiling)
            {
                this.ShowNotification(new GUIContent("Compiling Scripts", EditorGUIUtility.IconContent("BuildSettings.Editor").image));
            }
            else
            {
                this.RemoveNotification();
            }



            /*
            tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            tex.SetPixel(0, 0, new Color(0.55f, 0.55f, 0.55f));
            tex.Apply();
            
            GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), tex, ScaleMode.StretchToFill);
            */

            // Add The Banner
            Texture2D welcomeImage = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Orangedkeys/WallFxPack/Resources/WallFX_PACK.png", typeof(Texture2D));
            Rect welcomeImageRect = new Rect(0, 0, 512, 128);
            UnityEngine.GUI.DrawTexture(welcomeImageRect, welcomeImage);

            GUILayout.Space(20);

            GUILayout.BeginArea(new Rect(EditorGUILayout.GetControlRect().x + 10, 200, WelcomeWindowWidth - 20, WelcomeWindowHeight));

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Have fun with ''Wall FX PACK'' !! \n", LargeTextStyle);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Subscribe to OrangedKeys Newsletter to be updated about our Next Assets Releases/Updates. \n", RegularTextStyle);
            EditorGUILayout.Space();




            // subscribe

            email = EditorGUILayout.TextField("E-mail : ", email, GUILayout.MaxWidth(480f));
            EditorGUILayout.Space();
            if (GUILayout.Button(new GUIContent(" SUBSCRIBE ", EditorGUIUtility.IconContent("d_orangeLight").image), GUILayout.MaxWidth(480)))
            {

                SendToMailChimp(email);
                SendGFormData(email);
            }

            // WEBSITE

            if (GUILayout.Button(new GUIContent("  www.OrangedKeys.com  ", EditorGUIUtility.IconContent("BuildSettings.Web.Small").image), GUILayout.MaxWidth(480)))
            {
                Application.OpenURL(WEB_URL);

            }

            // YOUTUBE

            if (GUILayout.Button(new GUIContent("  YouTube Channel  ", EditorGUIUtility.IconContent("Animation.Record").image), GUILayout.MaxWidth(480)))
            {
                Application.OpenURL(YOUTUBE_URL);

            }





            GUILayout.EndArea();

            Rect areaRect = new Rect(0, WelcomeWindowHeight - 20, WelcomeWindowWidth, WelcomeWindowHeight - 20);
            GUILayout.BeginArea(areaRect);
            EditorGUILayout.LabelField("Copyright © 2019 OrangedKeys", FooterTextStyle);
            GUILayout.EndArea();

        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }



        /// MAILCHIMP
        /// 



        /// <summary>
        ///  ADD THE SUBSCRIPTION
        /// </summary>


        
                private const string UrlFormat = "https://{0}.api.mailchimp.com/3.0/lists/{1}/members";
                private const string DataFormat = "{{\"email_address\":\"{0}\", \"status\":\"subscribed\"}}";

                [SerializeField]
                public MailChimpEvent SubscribeSuccess;
                [SerializeField]
                public MailChimpEvent SubscribeError;

                [SerializeField]
                private string _apiKey = "a0c93983246f52debda7fda58e9e3c95-us4";
                [SerializeField]
                private string _listId = "4031a38086";





                public void SendToMailChimp(string Useremail)
                {

                    var www = BuildWWW(Useremail);

                    if (www == null)
                    {
                        Debug.Log("Subscribe error: can't build request");
                        SubscribeError.Invoke(Useremail);
                    }
                    else
                    {
                        //yield return www;

                        if (string.IsNullOrEmpty(www.error))
                        {
                            Debug.Log("Subscribe success");
                            //SubscribeSuccess.Invoke(Useremail);

                        }
                        else
                        {
                            Debug.Log("Subscribe error: " + www.error);
                            SubscribeError.Invoke(Useremail);
                        }
                    }
                }

                public WWW BuildWWW(string Useremail)
                {
                    var headers = new Dictionary<string, string>();
                    headers.Add("Authorization", "apikey " + _apiKey);

                    var data = string.Format(DataFormat, Useremail);
                    var dataBytes = Encoding.ASCII.GetBytes(data);

                    var splittedApiKey = _apiKey.Split('-');

                    if (splittedApiKey.Length != 2)
                    {
                        Debug.LogError("Invalid API Key format");
                        return null;
                    }

                    var urlPrefix = splittedApiKey[1];

                    var url = string.Format(UrlFormat, urlPrefix, _listId);
                    var www = new WWW(url, dataBytes, headers);

                    return www;
                }

                [Serializable]
                public class MailChimpEvent : UnityEvent<string>
                {
                }


                




        ////////



        /// SUBSCRIPTION

        public void SendGFormData(string Useremail)
    {


        string example = Useremail;
        WWWForm form = new WWWForm();

        form.AddField(kGFormEntryID, example);
        string urlGFormResponse = kGFormBaseURL + "formResponse";

        

            UnityWebRequest www = UnityWebRequest.Post(urlGFormResponse, form);
            
            www.SendWebRequest();

            if (www == null)
            {
                Debug.Log("Subscribe error: can't build request");
                
            }
            else
            {
                //yield return www;

                if (string.IsNullOrEmpty(www.error))
                {
                    Debug.Log("Subscribe success");
                    

                }
                else
                {
                    Debug.Log("Subscribe error: " + www.error);
                   
                }
            }

            

            
    }

    }
}
 