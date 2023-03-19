using UnityEngine;

public class ShowTutorialSlideshowOnStart : MonoBehaviour
{
    [SerializeField] private TutorialSlideshow tutorial;

    private void Start() => Message.Publish(new ShowTutorialSlideshow(tutorial));
}
