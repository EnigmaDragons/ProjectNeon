using AeLa.EasyFeedback.APIs;
using UnityEditor;
using UnityEngine;

namespace AeLa.EasyFeedback.Editor
{
    internal class NewBoardWindow : EditorWindow
    {
        private const string WINDOW_TITLE = "New Feedback Board";
        private const int WIDTH = 312;
        private const int HEIGHT = 46;


        private Trello trello;
        private string boardName = "My Feedback Board";

        public static NewBoardWindow GetWindow()
        {
            NewBoardWindow window =
                GetWindow<NewBoardWindow>(true, WINDOW_TITLE);

            // set window size
            window.minSize = new Vector2(WIDTH, HEIGHT);
            window.maxSize = window.minSize;

            return window;
        }

        private void OnEnable()
        {
            if (trello == null)
            {
                // init trello API handler
                EFConfig config = AssetDatabase.LoadAssetAtPath<EFConfig>(
                    Settings.GetAssetRootPath() +
                    Constants.CONFIG_ASSET_NAME
                );
                trello = new Trello(config.Token);
            }
        }

        private void OnGUI()
        {
            if (trello == null) return;

            boardName = EditorGUILayout.TextField("Board Name", boardName);

            if (GUILayout.Button("Create Board"))
            {
                // add board
                SetupBoard(boardName);

                if (EditorUtility.DisplayDialog(
                    "Board created!",
                    "The board " + boardName +
                    " has been successfully created!", "Ok"
                ))
                {
                    // refresh boards in configuration
                    Settings.ScheduleRefresh();

                    // close self
                    Close();
                }
            }
        }

        /// <summary>
        /// Clones the default feedback board
        /// </summary>
        /// <param name="boardName"></param>
        /// <returns></returns>
        public void SetupBoard(string boardName)
        {
            trello.AddBoard(
                boardName, true, true, null, null, Trello.TEMPLATE_BOARD_ID
            );
        }
    }
}