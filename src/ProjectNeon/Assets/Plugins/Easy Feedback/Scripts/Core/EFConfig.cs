using System;
using AeLa.EasyFeedback.APIs;
using UnityEngine;

namespace AeLa.EasyFeedback
{
    /// <summary>
    /// Configuration information for Easy Feedback
    /// </summary>
    public class EFConfig : ScriptableObject
    {
        [Tooltip("Save feedback locally, instead of sending it to Trello")]
        public bool StoreLocal;
        public string Token;
        public FeedbackBoard Board;

        public EFConfig()
        {
            StoreLocal = true; // default to store locally when not logged in to Trello

            // default board setup (for local storage without Trello authentication)
            Board = new FeedbackBoard();
        }
    }

    [Serializable]
    public class FeedbackBoard
    {
        public string Id;

        public string[] ListNames;
        public string[] ListIds;

        public string[] CategoryNames = new string[] { "Feedback", "Bug" };
        public string[] CategoryIds = new string[] { null, null };

        public Label[] Labels = new Label[]
        {
            new Label("1", null, "Low Priority"),
            new Label("2", null, "Medium Priority"),
            new Label("3", null, "High Priority")
        };

    }
}