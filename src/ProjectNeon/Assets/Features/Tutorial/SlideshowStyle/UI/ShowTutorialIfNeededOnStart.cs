using UnityEngine;

public class ShowTutorialIfNeededOnStart : MonoBehaviour
{
    [SerializeField] private TutorialSlideshow tutorial;

    private void Start() => Message.Publish(new ShowTutorialSlideshowIfNeeded(tutorial));
}
