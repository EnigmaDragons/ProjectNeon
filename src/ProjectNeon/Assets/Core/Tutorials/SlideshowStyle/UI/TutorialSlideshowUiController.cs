using UnityEngine;

public class TutorialSlideshowUiController : OnMessage<ShowTutorialSlideshow, HideTutorial>
{
    [SerializeField] private GameObject view;
    [SerializeField] private TutorialSlideshowPresenter presenter;

    private void Start()
    {
        view.SetActive(presenter.IsShowingTutorial);
    }
    
    protected override void Execute(ShowTutorialSlideshow msg)
    {
        presenter.Enqueue(msg.Slideshow);
        view.SetActive(true);
    }

    protected override void Execute(HideTutorial msg) => view.SetActive(false);
}
