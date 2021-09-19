using System;
using TMPro;
using UnityEngine;

public class TutorialSlideshowPresenter : OnMessage<TutorialNextRequested, TutorialPreviousRequested>
{
    [SerializeField] private GameObject slideUiParent;
    [SerializeField] private TextMeshProUGUI slideText;
    [SerializeField] private GameObject nextButtonIndicator;
    [SerializeField] private GameObject previousButtonIndicator;
    [SerializeField] private GameObject doneIndicator;
    
    private Maybe<TutorialSlideshow> _current = Maybe<TutorialSlideshow>.Missing();
    private Maybe<IndexSelector<TutorialSlide>> _maybeSlideWalker = Maybe<IndexSelector<TutorialSlide>>.Missing();
    
    public void Init(TutorialSlideshow slideshow)
    {
        _current = slideshow;
        _maybeSlideWalker = new Maybe<IndexSelector<TutorialSlide>>(new IndexSelector<TutorialSlide>(slideshow.Slides));
        Render();
    }

    protected override void Execute(TutorialNextRequested msg) => IfHasSlides(slides =>
    {
        if (slides.Index == slides.Count - 1)
        {
            Message.Publish(new HideTutorial(_current.Value.TutorialName));
            _current = Maybe<TutorialSlideshow>.Missing();
            _maybeSlideWalker = Maybe<IndexSelector<TutorialSlide>>.Missing();
        }
        else
        {
            slides.MoveNextWithoutLooping();
            Render();
        }
    });

    protected override void Execute(TutorialPreviousRequested msg) => IfHasSlides(slides =>
    {
        slides.MovePreviousWithoutLooping();
        Render();
    });

    private void Render()
    {
        slideUiParent.DestroyAllChildren();
        nextButtonIndicator.SetActive(HasSlides && _maybeSlideWalker.Value.Index + 1 < _maybeSlideWalker.Value.Count);
        IfHasSlides(slides =>
        {
            var slide = slides.Current;
            Instantiate(slide.UiElementPrototype, slideUiParent.transform);
            slideText.text = slide.Text;
            previousButtonIndicator.SetActive(!slides.IsFirstItem);
            nextButtonIndicator.SetActive(!slides.IsLastItem);
            doneIndicator.SetActive(slides.IsLastItem);
        });
    }

    private void IfHasSlides(Action<IndexSelector<TutorialSlide>> action)
    {
        if (HasSlides)
            action(_maybeSlideWalker.Value);
    }
    
    private bool HasSlides => _maybeSlideWalker.IsPresentAnd(s => s.Count > 0);
}
