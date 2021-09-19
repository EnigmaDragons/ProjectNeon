
public class TutorialSlideshowPresenter
{
    private Maybe<TutorialSlideshow> _current = Maybe<TutorialSlideshow>.Missing();
    
    public void Init(TutorialSlideshow slideshow)
    {
        _current = slideshow;
    }
}
