using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreator2D.UI
{
    public class BodySegmentSlider : MonoBehaviour
    {
        public SegmentType segmentType;
        public BodySliderType sliderType;

        private UICreator _uicreator;
        private InputFieldSlider input;

        void Awake()
        {
            _uicreator = this.GetComponentInParent<UICreator>();
            input = GetComponent<InputFieldSlider>();
            input.onValueChanged.AddListener(UpdateScale);
        }

        void OnEnable()
        {
            input = this.GetComponent<InputFieldSlider>();
            if (_uicreator == null || input == null)
                return;

            switch (sliderType)
            {
                case BodySliderType.X:
                    input.slider.value = _uicreator.character.GetBodySlider(segmentType).x;
                    break;
                case BodySliderType.Y:
                    input.slider.value = _uicreator.character.GetBodySlider(segmentType).y;
                    break;
                case BodySliderType.Symmetrical:
                    input.slider.value = _uicreator.character.GetBodySlider(segmentType).x;
                    break;
                default:
                    break;
            }
        }

        void UpdateScale (float value)
        {            
            if (_uicreator == null)
                _uicreator = this.GetComponentInParent<UICreator>();

            switch (sliderType)
            {
                case BodySliderType.X:
                    _uicreator.character.SetBodySlider(segmentType, new Vector2(value, _uicreator.character.GetBodySlider(segmentType).y));
                    break;
                case BodySliderType.Y:
                    _uicreator.character.SetBodySlider(segmentType, new Vector2(_uicreator.character.GetBodySlider(segmentType).x, value));
                    break;
                case BodySliderType.Symmetrical:
                    _uicreator.character.SetBodySlider(segmentType, new Vector2(value, value));
                    break;
                default:
                    break;
            }
        }

        public void ResetScale()
        {
            if (_uicreator == null)
                _uicreator = this.GetComponentInParent<UICreator>();

            switch (sliderType)
            {
                case BodySliderType.X:
                    _uicreator.character.SetBodySlider(segmentType, new Vector2(0.5f, _uicreator.character.GetBodySlider(segmentType).y));
                    break;
                case BodySliderType.Y:
                    _uicreator.character.SetBodySlider(segmentType, new Vector2(_uicreator.character.GetBodySlider(segmentType).x, 0.5f));
                    break;
                case BodySliderType.Symmetrical:
                    _uicreator.character.SetBodySlider(segmentType, new Vector2(0.5f, 0.5f));
                    break;
                default:
                    break;
            }
        }

        public void RandomizeScale()
        {
            if (_uicreator == null)
                _uicreator = this.GetComponentInParent<UICreator>();
                
            float extremeDice = Random.Range(0f, 1f);
            float value = 0.5f;
            if (extremeDice > 0.85f)
                value = Random.Range(0.0f, 1.0f);
            else if (extremeDice > 0.5f)
                value = Random.Range(0.3f, 0.7f);
            else if (extremeDice > 0.15f)
                value = Random.Range(0.4f, 0.6f);
            
            switch (sliderType)
            {
                case BodySliderType.X:
                    _uicreator.character.SetBodySlider(segmentType, new Vector2(value, _uicreator.character.GetBodySlider(segmentType).y));
                    break;
                case BodySliderType.Y:
                    _uicreator.character.SetBodySlider(segmentType, new Vector2(_uicreator.character.GetBodySlider(segmentType).x, value));
                    break;
                case BodySliderType.Symmetrical:
                    _uicreator.character.SetBodySlider(segmentType, new Vector2(value, value));
                    break;
                default:
                    break;
            }
        }
    }
}