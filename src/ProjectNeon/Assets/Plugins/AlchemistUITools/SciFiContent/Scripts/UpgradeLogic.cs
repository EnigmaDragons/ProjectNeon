using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AlchemistUISciFi
{
	public class UpgradeLogic : MonoBehaviour 
	{
        public float animKoef = 1;
        public RectTransform descriptionRect;
        public RectTransform hexRect;
		public RectTransform itemMenu;
		public RectTransform bigCircleBtnRect;
        public RectTransform smallCircleBtnRect;
        public Button backBtn;
		
		private UpgradeCategory[] _hexes;
		private int _lastCategoryIndex = -1;
        private Vector2 _startDescriptionRect;
        private Vector2 _aimDescriptionRect;
        private Vector2 _startHexRect;
		private Vector2 _aimHexRect;
		private Vector3 _aimItemMenuScale = Vector3.zero;
		private Coroutine _enumShowItem;
		
		private UpgradeSubCategory[] _bigCircleBtns;
		private int _lastSubCategoryIndex = -1;
        private Vector3 _aimSmallCircleSize = Vector3.one;
        private Coroutine _enumShowSubItem;

        void Reset()
		{
			for(int i = 0; i < _hexes.Length; i++)
				_hexes[i].Reset();
			_aimHexRect = _startHexRect;
			_aimItemMenuScale = Vector3.zero;
			if(_enumShowItem != null)
				StopCoroutine(_enumShowItem);
            if (_enumShowSubItem != null)
                StopCoroutine(_enumShowSubItem);
            _lastCategoryIndex = -1;
            _aimSmallCircleSize = Vector3.one;
            _aimDescriptionRect = _startDescriptionRect;
        }
		
		void OnHexClick(int index)
		{
			if(_lastCategoryIndex == index)
				return;
			if(_enumShowItem != null)
				StopCoroutine(_enumShowItem);
            _enumShowItem = StartCoroutine(LateHexClick(index));
			_lastCategoryIndex = index;
		}
		
		IEnumerator LateHexClick(int index)
		{
			if(itemMenu.localScale.magnitude > 0.025f || _aimItemMenuScale.magnitude > 0.025f)
			{
				_aimItemMenuScale = Vector3.zero;
                _aimDescriptionRect = _startDescriptionRect;
                yield return new WaitForSeconds(0.7f / animKoef);
			}
			_hexes[index].aimPos = Vector2.zero;
			int j = -2;
			for(int i = 0; i < _hexes.Length; i++)
			{
				if(i != index)
				{
					_hexes[i].aimPos = new Vector2(220 * j + 120 * Mathf.Sign(j), 0);
					j++;
					if(j == 0)
						j++;
				}
				_hexes[i].SetLittle();
			}
			yield return new WaitForSeconds(0.35f / animKoef);
            _aimHexRect = new Vector2((0.365f - 0.5f)* 1080 * Screen.width / Screen.height, 0);
            yield return new WaitForSeconds(0.8f / animKoef);
            _aimDescriptionRect = Vector2.zero;
            _aimDescriptionRect = Vector2.zero;
            _aimItemMenuScale = Vector3.one;
		}
		
		void OnBigCicleClick(int index)
		{
            if (_lastSubCategoryIndex == index)
                return;
            if (_enumShowSubItem != null)
                StopCoroutine(_enumShowSubItem);
            _enumShowSubItem = StartCoroutine(LateBigCircleClick(index));
		}

        IEnumerator LateBigCircleClick(int index)
        {
            float[] angles = new float[_bigCircleBtns.Length];
            for (int i = 0; i < _bigCircleBtns.Length; i++)
                angles[i] = _bigCircleBtns[(i - index + 6) % 6].startAngle;
            for (int i = 0; i < _bigCircleBtns.Length; i++)
                _bigCircleBtns[i].aimAngle = angles[i];
            _lastSubCategoryIndex = index;
            _aimSmallCircleSize = Vector3.zero;
            _aimDescriptionRect = _startDescriptionRect;
            yield return new WaitForSeconds(1f / animKoef);
            _aimSmallCircleSize = Vector3.one;
            _aimDescriptionRect = Vector3.zero;
        }

        // Use this for initialization
        void Start () {
			itemMenu.localScale = Vector2.zero;
            _aimDescriptionRect = _startDescriptionRect = new Vector2(1.5f * descriptionRect.rect.width, 0);
			descriptionRect.anchoredPosition = _startDescriptionRect;
            _startHexRect = Vector2.zero;
			_aimHexRect = _startHexRect;
			_hexes = new UpgradeCategory[hexRect.childCount];
			for(int i = 0; i < _hexes.Length; i++)
			{
				int _i = i;
				_hexes[i] = new UpgradeCategory(hexRect.GetChild(i), () => OnHexClick(_i));
			}
			
			_bigCircleBtns = new UpgradeSubCategory[bigCircleBtnRect.childCount];
			for(int i = 0; i < _bigCircleBtns.Length; i++)
			{
				int _i = i;
				_bigCircleBtns[i] = new UpgradeSubCategory(bigCircleBtnRect.GetChild(i), () => OnBigCicleClick(_i));
			}
			backBtn.onClick.AddListener(() => Reset());
		}
		
		// Update is called once per frame
		void Update () {
            descriptionRect.anchoredPosition = Vector2.Lerp(descriptionRect.anchoredPosition, _aimDescriptionRect, Time.deltaTime * 3 * animKoef);
            for (int i = 0; i < _hexes.Length; i++)
				_hexes[i].Update(Time.deltaTime * animKoef);
			hexRect.anchoredPosition = Vector2.Lerp(hexRect.anchoredPosition, _aimHexRect, Time.deltaTime * 3 * animKoef);
			itemMenu.localScale = Vector3.Lerp(itemMenu.localScale, _aimItemMenuScale, Time.deltaTime * 5 * animKoef);
			for(int i = 0; i < _bigCircleBtns.Length; i++)
				_bigCircleBtns[i].Update(Time.deltaTime * animKoef);
            smallCircleBtnRect.localScale = Vector3.Lerp(smallCircleBtnRect.localScale, _aimSmallCircleSize, Time.deltaTime * 5 * animKoef);
        }
	}
}
