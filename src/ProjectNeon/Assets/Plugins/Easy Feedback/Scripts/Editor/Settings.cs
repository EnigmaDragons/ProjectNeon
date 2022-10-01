using System;
using System.Linq;
using UnityEditor;
using AeLa.EasyFeedback.APIs;
using AeLa.EasyFeedback.Editor.Utility;
using AeLa.EasyFeedback.Utility;
using UnityEngine;
using UnityEngine.UIElements;
using Label = AeLa.EasyFeedback.APIs.Label;

namespace AeLa.EasyFeedback.Editor
{
    internal class Settings : SettingsProvider
    {
        /// <summary>
        /// The path of the EF asset, relative to the project folder
        /// </summary>
        private static string assetRootPath;

        /// <summary>
        /// Cache of Trello boards for this account
        /// </summary>
        private Board[] boards;

        /// <summary>
        /// Whether a refresh of the boards cache was requested
        /// </summary>
        private static bool boardsRefreshRequested;

        private EFConfig config;
        private string lastValidToken;

        /// <summary>
        /// The index of the currently selected board in the boards list
        /// </summary>
        private int selectedBoard;

        private bool subscribed;
        private string token;
        private Trello trello;

        private Settings() : base(Constants.PROJECT_SETTINGS_PATH, SettingsScope.Project)
        {
            label = "Easy Feedback";
        }

        public override void OnActivate(
            string searchContext, VisualElement rootElement
        )
        {
            // attempt to load config if it hasn't already been loaded
            if (!config) LoadConfigAsset();

            switch (string.IsNullOrEmpty(lastValidToken))
            {
                // check that Trello key is still valid if it has changed
                case false when lastValidToken != config.Token &&
                                IsTokenValid(config.Token):
                    // update lastValidToken, reinit api handler
                    lastValidToken = config.Token;
                    InitAPIHandler();
                    break;
                case false when lastValidToken != config.Token:
                    // log out
                    LogOut();
                    break;
            }

            EditorApplication.update += Update;

            base.OnActivate(searchContext, rootElement);
        }

        public override void OnDeactivate()
        {
            EditorApplication.update -= Update;
            base.OnDeactivate();
        }

        private void Update()
        {
            // wait until authenticated
            if (!config || string.IsNullOrEmpty(config.Token)) return;

            if (!Application.isPlaying)
                UpdateEditorOnly();
        }

        private void UpdateEditorOnly()
        {
            // init Trello API handler if we need to
            if (trello == null && config) InitAPIHandler();

            // block until API handler is initialized
            // it can still be null after initAPIHandler if the token was not valid
            if (trello == null) return;

            // refresh boards if we need to
            if (boardsRefreshRequested || boards == null)
            {
                RefreshBoardsList();
                RefreshBoardInfo();
                boardsRefreshRequested = false;
            }

            // refresh board info if we need to
            if (boards.Length > 0 &&
                config.Board.Id != boards[selectedBoard].id)
                RefreshBoardInfo();
        }

        public override void OnGUI(string searchContext)
        {
            // wait until config is created to display anything
            if (!config) return;

            // request authentication if Trello info is null on config asset
            if (string.IsNullOrEmpty(config.Token))
            {
                ShowAuth();
                ResetButton();
            }

            // wait until API handler has been initalized to show Trello controls
            if (trello != null)
            {
                // show log out option
                LogOutButton();

                // show board controls
                BoardControls();
            }

            var lastSLVal = config.StoreLocal;
            config.StoreLocal = EditorGUILayout.Toggle(
                "Store Locally", config.StoreLocal
            );

            if (lastSLVal != config.StoreLocal)
                EditorUtility.SetDirty(config);

            if (trello != null)
            {
				// show filesize warning
				EditorGUILayout.HelpBox(Constants.FILESIZE_WARNING, MessageType.Warning);
            }
            
            base.OnGUI(searchContext);
        }

        /// <summary>
        /// Initializes a new instance of the Trello API handler
        /// </summary>
        private void InitAPIHandler()
        {
            if (Trello.IsValidToken(config.Token))
            {
                trello = new Trello(config.Token);
            }
            else
            {
                EditorUtility.DisplayDialog(
                    "Invalid Token",
                    "Invalid Trello API credentials. Please re-enter your token.",
                    "OK"
                );
                // remove bad token to force reauth
                LogOut();
            }
        }

        /// <summary>
        /// Refreshes the boards list
        /// </summary>
        public void RefreshBoardsList()
        {
            // show progress bar
            refreshProgressBar(0f);

            try
            {
                // get all boards
                var allBoards = trello.GetBoards();

                // select only feedback boards
                boards = allBoards.Where(b => IsFeedbackBoard(b)).ToArray();

                // set selected index to current board from config
                var foundCurrentBoard = false;
                for (var i = 0; i < boards.Length; i++)
                    if (boards[i].id == config.Board.Id)
                    {
                        foundCurrentBoard = true;
                        selectedBoard = i;
                    }

                // reset selected board index if it no longer exists on the list
                if (!foundCurrentBoard)
                    selectedBoard = 0;

                // update progress bars
                refreshProgressBar(0.8f);

                // update subscribed check for current board
                if (boards.Length > 0)
                    UpdateSubscribed();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                showErrorDialog(
                    "Loading boards failed!",
                    "Failed to load boards from Trello. See the console for more information."
                );
                boards = new Board[0];
                boardsRefreshRequested = false;
            }

            // clear progress bar
            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// Checks if the user is subscribed to the current board
        /// </summary>
        private void UpdateSubscribed()
        {
            subscribed = trello.GetSubscribed(boards[selectedBoard].id);
        }

        /// <summary>
        /// Checks if a board has all of the required feedback components
        /// </summary>
        /// <returns></returns>
        private bool IsFeedbackBoard(Board board)
        {
            // check if the board is closed
            if (board.closed) return false;

            // check that there are category lists
            if (!BoardHasCategoryLists(board)) return false;

            // passed checks
            return true;
        }

        /// <summary>
        /// Check if a board has category lists (part of isFeedbackBoard check)
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        private bool BoardHasCategoryLists(Board board)
        {
            var lists = trello.GetLists(board.id);
            for (var i = 0; i < lists.Length; i++)
                if (lists[i].name.EndsWith(Trello.CATEGORY_TAG))
                    return true;

            return false;
        }

        private void RefreshBoardInfo()
        {
            // don't refresh current board if there aren't any feedback boards on the account
            if (boards.Length == 0)
                return;

            SetBoardProgressBar(
                boards[selectedBoard].name, "Setting board info", 0f
            );

            config.Board.Id = boards[selectedBoard].id;

            SetBoardProgressBar(
                boards[selectedBoard].name, "Getting lists and categories", 0.3f
            );

            // update lists and categories
            UpdateLists();

            SetBoardProgressBar(
                boards[selectedBoard].name, "Getting labels", 0.6f
            );

            // set board priority labels
            var labels = trello.GetLabels(boards[selectedBoard].id);

            // update labels but keep existing sort order
            config.Board.Labels = labels.Join(
                    config.Board.Labels,
                    label => label.id,
                    label => label.id,
                    (newLabel, oldLabel) =>
                    {
                        newLabel.order = oldLabel.order;
                        return newLabel;
                    }
                )
                .Union(
                    labels,
                    new InlineComparer<Label>(
                        (a, b) => a.id == b.id,
                        l => l.id.GetHashCode()
                    )
                )
                .OrderBy(l => l.order)
                .ToArray();

            SetBoardProgressBar(
                boards[selectedBoard].name, "Checking subscribed state", 0.9f
            );

            // update subscribed checkbox
            UpdateSubscribed();

            EditorUtility.ClearProgressBar();

            // save asset
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Gets lists from the Trello API, then updates categories
        /// </summary>
        private void UpdateLists()
        {
            var lists = trello.GetLists(boards[selectedBoard].id);

            config.Board.ListIds = new string[lists.Length];
            config.Board.ListNames = new string[lists.Length];

            for (var i = 0; i < lists.Length; i++)
            {
                config.Board.ListIds[i] = lists[i].id;
                config.Board.ListNames[i] = lists[i].name;
            }

            EditorUtility.SetDirty(config);

            UpdateCategories(lists);
        }

        /// <summary>
        /// Checks for Easy Feedback (EF) lists on the board
        /// </summary>
        private void UpdateCategories(List[] lists)
        {
            var efLists =
                lists.Where(l => l.name.EndsWith(Trello.CATEGORY_TAG));

            // get shortnames
            config.Board.CategoryNames = efLists.Select(
                l => l.name.Substring(
                    0, l.name.Length - Trello.CATEGORY_TAG.Length
                )
            ).ToArray();

            // get ids
            config.Board.CategoryIds = efLists.Select(l => l.id).ToArray();

            EditorUtility.SetDirty(config);
        }

        private void showErrorDialog(string title, string message)
        {
            EditorUtility.DisplayDialog(title, message, "OK");
        }

        /// <summary>
        /// Displays or updates the progress bar for refreshing the boards list
        /// </summary>
        /// <param name="progress"></param>
        private void refreshProgressBar(float progress)
        {
            EditorUtility.DisplayProgressBar(
                "Loading Boards", "Loading list of boards from Trello", progress
            );
        }

        /// <summary>
        /// Displays or updates the progress bar for setting the feedback board
        /// </summary>
        /// <param name="board"></param>
        /// <param name="message"></param>
        /// <param name="progress"></param>
        private void SetBoardProgressBar(
            string board, string message, float progress
        )
        {
            EditorUtility.DisplayProgressBar(
                "Loading board: " + board, message, progress
            );
        }

        /// <summary>
        /// Attempts to load the TrelloInfo asset from the EasyFeedback directory
        /// </summary>
        /// <returns></returns>
        private void LoadConfigAsset()
        {
            config = AssetDatabase.LoadAssetAtPath<EFConfig>(
                GetAssetRootPath() + Constants.CONFIG_ASSET_NAME
            );

            if (!config) // no config file yet, create one
                CreateConfigAsset();
        }

        /// <summary>
        /// Shows controls for authenticating with Trello
        /// </summary>
        /// <returns></returns>
        private void ShowAuth()
        {
            // show Trello API token field
            token = EditorGUILayout.TextField("Token", token)?.Trim(' ');

            // show authenticate button
            if (GUILayout.Button("Authenticate With Token") &&
                IsTokenValid(token))
            {
                // user has authenticated with Trello, update config
                config.StoreLocal = false;
                config.Token = token;
                lastValidToken = token;

                // save config
                EditorUtility.SetDirty(config);
                AssetDatabase.SaveAssets();

                // request a refresh (first load) of account when the api handler is initialized
                boardsRefreshRequested = true;
            }

            // direct user to Trello API auth page if the window isn't already open
            if (GUILayout.Button("Get Trello API Token"))
            {
                // prompt user to auth via browser
                Application.OpenURL(Trello.AuthURL);
            }
        }

        /// <summary>
        /// Shows the "Reset Config" button, and handles clicks
        /// </summary>
        public void ResetButton()
        {
            if (GUILayout.Button("Reset to Default")
                && EditorUtility.DisplayDialog(
                    "Reset Config",
                    "Are you sure you want to reset the Easy Feedback config file to default values?",
                    "Yes", "Cancel"
                ))
            {
                // replace config with default values
                config.StoreLocal = true;
                config.Token = null;
                config.Board = new FeedbackBoard();

                // save config
                EditorUtility.SetDirty(config);
                AssetDatabase.SaveAssets();

                EditorUtility.DisplayDialog(
                    "Reset Successful",
                    "The Easy Feedback config file has been reset to default values.",
                    "OK"
                );
            }
        }

        public void CreateConfigAsset()
        {
            // create the config asset
            config = new EFConfig();
            AssetDatabase.CreateAsset(
                config, GetAssetRootPath() + Constants.CONFIG_ASSET_NAME
            );
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();

            // update Feedback prefab
            var feedbackPrefab =
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    GetAssetRootPath() + "Feedback.prefab"
                );
            feedbackPrefab.GetComponent<FeedbackForm>().Config = config;
            EditorUtility.SetDirty(feedbackPrefab);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Checks if the current token is valid, shows a message if invalid
        /// </summary>
        /// <returns></returns>
        private bool IsTokenValid(string token)
        {
            var valid = Trello.IsValidToken(token);
            if (!valid)
            {
                // show message
                EditorUtility.DisplayDialog(
                    "Invalid Token",
                    "The provided token is invalid. Please enter a valid Trello API token.",
                    "OK"
                );

                // highlight token field
                EditorGUI.FocusTextInControl("Token");
            }

            return valid;
        }

        /// <summary>
        /// Displays the "Log Out of Trello" button, and handles clicks
        /// </summary>
        private void LogOutButton()
        {
            if (GUILayout.Button("Log Out of Trello")) LogOut();
        }

        /// <summary>
        /// Logs out of Trello
        /// </summary>
        private void LogOut()
        {
            // clear token
            token = string.Empty;
            lastValidToken = null;

            // clear temporary variables
            trello = null;
            boards = null;
            selectedBoard = 0;

            // reset Trello info in config
            // not resetting FeedbackBoard to preserve categories and labels from Trello
            config.Token = null;
            config.StoreLocal =
                true; // default to store local when not logged in

            // save config
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();

            // unfocus token text area so it properly clears
            GUIUtility.hotControl = 0;
            GUIUtility.keyboardControl = 0;
        }

        /// <summary>
        /// Displays the "New Board" button, and handles clicks
        /// </summary>
        private void NewBoardButton()
        {
            if (GUILayout.Button("New Board"))
            {
                // show new board window
                NewBoardWindow.GetWindow();
            }
        }

        /// <summary>
        /// Displays the "Refresh Boards" button, and handles clicks
        /// </summary>
        private void RefreshBoardsButton()
        {
            if (GUILayout.Button("Refresh Boards"))
                boardsRefreshRequested = true;
        }

        /// <summary>
        /// Displays controls for managing boards
        /// </summary>
        private void BoardControls()
        {
            if (boards != null && boards.Length > 0)
            {
                var boardNames = boards
                    .Select(
                        b => b.name.Replace(
                            "/", Constants.UNICODE_FORWARD_SLASH
                        )
                    )
                    .ToArray();

                selectedBoard = EditorGUILayout.Popup(
                    "Feedback Board", selectedBoard, boardNames
                );
                
                NewBoardButton();

                // show refresh boards button
                RefreshBoardsButton();

                if (GUILayout.Button("Open Board in Browser"))
                    Application.OpenURL(boards[selectedBoard].url);

                // show subscribed checkbox underneath boards list
                if (EditorGUILayout.Toggle("Subscribe to board", subscribed) !=
                    subscribed)
                    // switch subscribed state
                    SetSubscribed(
                        !subscribed
                    ); // ef-TODO: this should probably be moved to Update
            }
            else // no boards on this account yet
            {
                NewBoardButton();
            
                GUI.skin.label.wordWrap = true;
                GUILayout.Label(
                    "There are no Easy Feeback boards on this account yet!\nClick \"New Board\" above to create one!"
                );
            }
            
        }

        /// <summary>
        /// Subscribes or unsubscribes a user from the selected board
        /// </summary>
        /// <param name="value">Whether or not the user is subscribed to the board</param>
        private void SetSubscribed(bool value)
        {
            trello.PutSubscribed(boards[selectedBoard].id, value);
            UpdateSubscribed();
        }

        /// <summary>
        /// Gets path of the EF asset, relative to the project folder
        /// </summary>
        public static string GetAssetRootPath()
        {
            if (assetRootPath != null
                && !AssetDatabase.IsValidFolder(assetRootPath))
                assetRootPath = null;

            if (assetRootPath == null)
            {
                // find EF root folder
                var guids = AssetDatabase.FindAssets(
                    "\"" + Constants.ASSET_ROOT_DIRECTORY + "\""
                );

                // convert guids to paths and filter paths without
                // an explicit root directory at the end
                var paths = guids
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Where(
                        s => s.EndsWith(
                            Constants.ASSET_ROOT_DIRECTORY
                        )
                    )
                    .ToArray();

                if (paths.Length == 1)
                    assetRootPath =
                        paths[0] + "/";
                else if (paths.Length > 1)
                    Debug.LogErrorFormat(
                        "[Easy Feedback] Found more than one folder " +
                        "named \"{0}\"! Please remove or rename folders " +
                        "named \"{0}\" in your project that are not the " +
                        "Easy Feedback folder.",
                        Constants.ASSET_ROOT_DIRECTORY
                    );
                else
                    Debug.LogErrorFormat(
                        "[Easy Feedback] Could not find the root asset " +
                        "folder. Please rename the root Easy Feedback " +
                        "folder to its original name (\"{0}\").",
                        Constants.ASSET_ROOT_DIRECTORY
                    );
            }

            return assetRootPath;
        }

        [SettingsProvider]
        private static SettingsProvider RegisterProvider() => new Settings();

        public static void ScheduleRefresh() => boardsRefreshRequested = true;
    }
}