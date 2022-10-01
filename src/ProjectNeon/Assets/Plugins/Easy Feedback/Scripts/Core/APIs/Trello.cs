using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AeLa.EasyFeedback.Utility;
using AeLa.EasyFeedback.Web;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace AeLa.EasyFeedback.APIs
{
    public class Trello
    {
        public const int MAX_CHAR_LENGTH = 16384;
        public const string CATEGORY_TAG = "(EF)";
        public string ErrorMessage;
        public bool IsDoneUploading;
        public AddCardResponse LastAddCardResponse;

        public UnityWebRequest LastRequest;

        private readonly string token;

        public bool UploadError;
        public Exception UploadException;

        public static string AuthURL
        {
            get
            {
                return string.Format(
                    "{0}/authorize?expiration=never&scope=read,write,account&response_type=token&name=Easy%20Feedback&key={1}",
                    API_URI, APP_KEY
                );
            }
        }

        public Trello(string token)
        {
            this.token = token;
        }

        /// <summary>
        /// Returns a fully formed and authenticated request URI for the Trello API path
        /// provided
        /// </summary>
        /// <param name="apiPath">The Trello API endpoint path (starting with /)</param>
        /// <returns></returns>
        public string GetURI(string apiPath)
        {
            var ext = "?";
            if (apiPath.Contains("?")) // already has a query string
                ext = "&";
            return $"{API_URI}{apiPath}{ext}key={APP_KEY}&token={token}";
        }

        /// <summary>
        /// Checks if a token is valid
        /// </summary>
        public static bool IsValidToken(string token, bool silent = false)
        {
            // make a dummy request to Trello
            // (GET /1/members/me)
            var requestUrl =
                $"{API_URI}/members/me?key={APP_KEY}&token={token}";

            // make request
#pragma warning disable CS0618 // Type or member is obsolete
            var request = new WWW(requestUrl);
#pragma warning restore CS0618 // Type or member is obsolete
            while (!request.isDone)
            {
                // wait until request is finished
#if UNITY_EDITOR
                if (!silent)
                    EditorUtility.DisplayProgressBar(
                        "Testing token", "", request.progress
                    );
#endif
            }

#if UNITY_EDITOR
            if (!silent)
                EditorUtility.ClearProgressBar();
#endif

            return string.IsNullOrEmpty(request.error);
        }

        /// <summary>
        /// Adds a card to a board
        /// </summary>
        /// <param name="name">Title of the card</param>
        /// <param name="description">Description of the card</param>
        /// <param name="labels">Any labels on the card</param>
        /// <param name="list">The list the card belongs to</param>
        /// <param name="fileSource">File data to attach to the card</param>
        public IEnumerator AddCard(
            string name, string description, IEnumerable<Label> labels,
            string list, byte[] fileSource = null
        )
        {
            IsDoneUploading = false;
            UploadError = false;
            ErrorMessage = string.Empty;
            UploadException = null;

            var form = new WWWForm();
            form.AddField("key", APP_KEY);
            form.AddField("token", token);
            form.AddField("name", name);

            if (description.Length > MAX_CHAR_LENGTH)
            {
                Debug.LogError(
                    "Card description length is higher than maximum length of " +
                    MAX_CHAR_LENGTH + ". Truncating..."
                );
                description = description.Remove(MAX_CHAR_LENGTH - 1);
            }

            form.AddField("desc", description);
            var idLabels = string.Join(",", labels.Select(l => l.id).ToArray());
            form.AddField("idLabels", idLabels);
            form.AddField("idList", list);
            if (fileSource != null)
                form.AddBinaryData("fileSource", fileSource);

            yield return WebInterface.PostCoroutine(
                "https://api.trello.com/1/cards", form, resp =>
                {
                    UploadError = resp.IsError;

                    if (!resp.IsError)
                        LastAddCardResponse =
                            JsonUtility.FromJson<AddCardResponse>(resp.Text);
                    else
                        ErrorMessage = resp.Text;
                }
            );
        }

        public IEnumerator AddAttachmentAsync(
            string cardID, byte[] file = null, string url = null,
            string name = null, string mimeType = null
        )
        {
            IsDoneUploading = false;
            UploadError = false;
            ErrorMessage = string.Empty;
            UploadException = null;

            var form = new WWWForm();

            if (file != null)
                form.AddBinaryData("file", file, name ?? "file.dat");

            if (url != null)
                form.AddField("url", url);

            if (name != null)
                form.AddField("name", name);

            if (mimeType != null)
                form.AddField("mimeType", mimeType);

            var uri = GetURI("/cards/" + cardID + "/attachments");

            yield return WebInterface.PostCoroutine(
                uri, form, resp =>
                {
                    // this will replace the value set in AddCard but that's ok because AddAttachmentAsync is only called if AddCard succeeds
                    UploadError = resp.IsError;

                    if (resp.IsError) ErrorMessage = resp.Text;
                }
            );
        }

        public IEnumerator GetLabelsAsync(
            string boardID, Action<Label[]> onFinished
        )
        {
            var uri = GetURI("/boards/" + boardID + "/labels");

            // make the request
            yield return WebInterface.GetCoroutine(
                uri, response =>
                {
                    // ef-TODO: check for errors
                    var resp = response.Text;

                    resp = resp.WrapToClass("labels");

                    // create labels from json
                    var labels = JsonUtility.FromJson<LabelCollection>(resp)
                        .labels;

                    // call onFinished
                    onFinished(labels);
                }
            );
        }

        public IEnumerator GetListsAsync(
            string boardID, Action<List[]> onFinished
        )
        {
            var uri = GetURI("/boards/" + boardID + "/lists");

            // make the request
            yield return WebInterface.GetCoroutine(
                uri, response =>
                {
                    // ef-TODO: check for errors
                    // get json
                    var resp = response.Text;
                    resp = resp.WrapToClass("lists");

                    //Debug.Log(respString);

                    // get lists from json
                    var lists = JsonUtility.FromJson<ListCollection>(resp)
                        .lists;

                    // call onFinished
                    onFinished(lists);
                }
            );
        }

        /// <summary>
        /// Editor-safe method for getting the lists on a board
        /// </summary>
        /// <param name="boardID"></param>
        /// <returns></returns>
        public List[] GetLists(string boardID)
        {
            // get the uri
            var uri = GetURI("/boards/" + boardID + "/lists");

            // make the request
            var resp = WebInterface.Get(uri);

            // ef-TODO: check for errors

            // get json
            var json = resp.Text.WrapToClass("lists");

            // get board array
            var lists = JsonUtility.FromJson<ListCollection>(json).lists;

            return lists;
        }

        /// <summary>
        /// Editor-safe method for adding a board
        /// </summary>
        public Board AddBoard(
            string name,
            bool defaultLabels = true,
            bool defaultLists = true,
            string desc = null,
            string idOrganization = null,
            string idBoardSource = null,
            string keepFromSource = "all",
            string powerUps = "all",
            Prefs? prefs = null
        )
        {
            // prepare web request
            var uri = "https://api.trello.com/1/boards";
            var form = new WWWForm();

            // authentication
            form.AddField("key", APP_KEY);
            form.AddField("token", token);

            // card info
            form.AddField("name", name);
            form.AddField("defaultLabels", defaultLabels.ToString().ToLower());
            form.AddField("defaultLists", defaultLists.ToString().ToLower());

            if (desc != null)
                form.AddField("desc", desc);

            if (idOrganization != null)
                form.AddField("idOrganization", idOrganization);

            if (idBoardSource != null)
                form.AddField("idBoardSource", idBoardSource);

            form.AddField("keepFromSource", keepFromSource);
            form.AddField("powerUps", powerUps);

            if (prefs.HasValue)
            {
                var p = prefs.Value;
                if (p.permissionLevel.HasValue)
                    form.AddField(
                        "prefs_permissionLevel",
                        p.permissionLevel.Value.ToString()
                    );

                if (p.voting.HasValue)
                    form.AddField("prefs_voting", p.voting.Value.ToString());

                if (p.comments.HasValue)
                    form.AddField(
                        "prefs_comments", p.comments.Value.ToString()
                    );

                if (p.invitations.HasValue)
                    form.AddField(
                        "prefs_invitations", p.invitations.Value.ToString()
                    );

                if (p.selfJoin.HasValue)
                    form.AddField(
                        "prefs_selfJoin", p.selfJoin.Value.ToString().ToLower()
                    );

                if (p.cardCovers.HasValue)
                    form.AddField(
                        "prefs_cardCovers",
                        p.cardCovers.Value.ToString().ToLower()
                    );

                if (p.background != null)
                    form.AddField("prefs_background", p.background);

                if (p.cardAging.HasValue)
                    form.AddField(
                        "prefs_cardAging", p.cardAging.Value.ToString()
                    );
            }

            // make the request
            var resp = WebInterface.Post(uri, form);

            // get board from response
            var board = JsonUtility.FromJson<Board>(resp.Text);
            return board;
        }

        public IEnumerator GetBoardsAsync(Action<Board[]> onFinished)
        {
            var uri = GetURI("/members/me/boards");

            // make the request
            yield return WebInterface.GetCoroutine(
                uri, resp =>
                {
                    // ef-TODO: check for errors

                    // get json
                    var json = resp.Text.WrapToClass("boards");

                    // get board array
                    var boards = JsonUtility.FromJson<BoardCollection>(json)
                        .boards;

                    // call onfinished
                    onFinished(boards);
                }
            );
        }

        /// <summary>
        /// Editor-safe method for getting the boards on the authorized Trello account
        /// </summary>
        public Board[] GetBoards()
        {
            // get the uri
            var uri = GetURI("/members/me/boards");

            // make the request
            var resp =
                WebInterface.Get(
                    uri
                ); //ef-TODO: add some kind of update function to all these

            // ef-TODO: check for errors function

            // get json
            var respString = resp.Text.WrapToClass("boards");

            // get board array
            var boards = JsonUtility.FromJson<BoardCollection>(respString)
                .boards;

            return boards;
        }

        /// <summary>
        /// Editor-safe method for getting labels from a board
        /// </summary>
        public Label[] GetLabels(string boardID)
        {
            // get the uri
            var uri = GetURI("/boards/" + boardID + "/labels");

            // make the request
            var resp = WebInterface.Get(uri);

            // ef-TODO: check for errors function

            // get json
            var json = resp.Text.WrapToClass("labels");

            // get board array
            var labels = JsonUtility.FromJson<LabelCollection>(json).labels;

            return labels;
        }

        /// <summary>
        /// Returns whether or not the authenticated user is subscribed to a board
        /// </summary>
        /// <param name="boardID">The board</param>
        /// <returns>Whether or not the authenticated user is subscribed to the board</returns>
        public bool GetSubscribed(string boardID)
        {
            // construct the URI
            var uri = GetURI("/boards/" + boardID + "/subscribed");

            // make the request
            var resp = WebInterface.Get(uri);

            // ef-TODO: check for errors function

            // get response object
            var sub = JsonUtility.FromJson<Subscribed>(resp.Text);

            // return value
            return sub._value;
        }

        /// <summary>
        /// Sets a user's subscribed state for a board
        /// </summary>
        /// <param name="boardID">The board</param>
        /// <param name="value">The subscribed state</param>
        public void PutSubscribed(string boardID, bool value)
        {
            // construct the URI
            var uri = GetURI(
                "/boards/" + boardID + "?subscribed=" +
                value.ToString().ToLower()
            );

            // send the request
            WebInterface.Put(uri); //ef-TODO: update function here
        }

        #region keys

        public const string TEMPLATE_BOARD_ID = "589d1b02a4856195b7cc31c9";
        public const string APP_KEY = "9babe077311b8a24fddaebb73de1df6a";
        public const string API_URI = "https://trello.com/1";

        #endregion
    }
}