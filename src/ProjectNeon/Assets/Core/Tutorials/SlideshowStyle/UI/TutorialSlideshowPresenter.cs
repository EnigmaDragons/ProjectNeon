using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TutorialSlideshowPresenter : OnMessage<TutorialNextRequested, TutorialPreviousRequested>
{
    [SerializeField] private GameObject backgroundUiParent;
    [SerializeField] private TextMeshProUGUI titleLabel;
    [SerializeField] private GameObject slideUiParent;
    [SerializeField] private SlideTextPresenter defaultTextPresenter;
    [SerializeField] private GameObject nextButtonIndicator;

    private readonly Queue<TutorialSlideshow> _queue = new Queue<TutorialSlideshow>();
    private Maybe<TutorialSlideshow> _current = Maybe<TutorialSlideshow>.Missing();
    private SlideTextPresenter _currentSlideTextPresenter;
    private Maybe<IndexSelector<TutorialSlide>> _maybeSlideWalker = Maybe<IndexSelector<TutorialSlide>>.Missing();

    public bool IsShowingTutorial => HasSlides; 
    
    public void Enqueue(TutorialSlideshow slideshow)
    {
        if (_current.IsPresent)
        {
            _queue.Enqueue(slideshow);
            return;
        }

        StartSlideshow(slideshow);
    }

    private void StartSlideshow(TutorialSlideshow slideshow)
    {
        _current = slideshow;
        if (_currentSlideTextPresenter != defaultTextPresenter)
            Destroy(_currentSlideTextPresenter);
        _currentSlideTextPresenter = defaultTextPresenter;
        _currentSlideTextPresenter.gameObject.SetActive(true);

        _maybeSlideWalker = new IndexSelector<TutorialSlide>(slideshow.Slides);
        titleLabel.text = slideshow.DisplayName;
        backgroundUiParent.DestroyAllChildren();
        if (slideshow.BackgroundPrototype != null)
            Instantiate(slideshow.BackgroundPrototype, backgroundUiParent.transform);
        if (slideshow.SlideTextPresenterPrototype != null)
        {
            _currentSlideTextPresenter.gameObject.SetActive(false);
            _currentSlideTextPresenter = Instantiate(slideshow.SlideTextPresenterPrototype, backgroundUiParent.transform);
        }

        Render();
    }

    protected override void Execute(TutorialNextRequested msg) => IfHasSlides(slides =>
    {
        if (slides.IsLastItem)
        {
            var completedTutorialName = _current.Value.TutorialName;
            _current = Maybe<TutorialSlideshow>.Missing();
            _maybeSlideWalker = Maybe<IndexSelector<TutorialSlide>>.Missing();
            MoveNext(completedTutorialName);
        }
        else
        {
            slides.MoveNextWithoutLooping();
            Render();
        }
    });

    private void MoveNext(string completedTutorialName)
    {
        if (_queue.Any())
            StartSlideshow(_queue.Dequeue());
        else
            Message.Publish(new HideTutorial(completedTutorialName));
    }
    
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
            if (slide.UiElementPrototype != null)
                Instantiate(slide.UiElementPrototype, slideUiParent.transform);
            _currentSlideTextPresenter.Init(slide.Text, slides);
        });
    }

    private void IfHasSlides(Action<IndexSelector<TutorialSlide>> action)
    {
        if (HasSlides)
            action(_maybeSlideWalker.Value);
    }
    
    private bool HasSlides => _maybeSlideWalker.IsPresentAnd(s => s.Count > 0);
}
