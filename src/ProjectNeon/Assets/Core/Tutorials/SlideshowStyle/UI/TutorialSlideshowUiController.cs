using UnityEngine;

public class TutorialSlideshowUiController : OnMessage<ShowTutorialSlideshow, HideTutorial>
{
    [SerializeField] private GameObject view;
    [SerializeField] private TutorialSlideshowPresenter presenter;
    
    protected override void Execute(ShowTutorialSlideshow msg)
    {
        presenter.Init(msg.Slideshow);
        view.SetActive(true);
    }

    protected override void Execute(HideTutorial msg) => view.SetActive(false);
}
