using System;
using System.Collections.Generic;
using UnityEngine;

namespace Features.BattleV2.Visuals.Enemies
{
    public sealed class VisualResourceCounterPresenter : OnMessage<MemberStateChanged>
    {
        [SerializeField] private SpriteRenderer spritePrototype;
        [SerializeField] private float xSpacingWidth;
        [SerializeField] private float iconWidth;

        private readonly List<SpriteRenderer> _icons = new List<SpriteRenderer>();

        private Member _member;
        
        public VisualResourceCounterPresenter Initialized(Member m)
        {
            _member = m;
            UpdateUi();
            return this;
        }
        
        protected override void Execute(MemberStateChanged msg)
        {
            if (msg.State.MemberId == _member.Id)
                UpdateUi();
        }

        private void UpdateUi()
        {
            var resourceAmount = _member.State.PrimaryResourceAmount;
            var primaryResourceIcon = _member.State.ResourceTypes[0].Icon;
            var pos = transform.position;
            
            for (var i = 0; i < Math.Max(resourceAmount, _icons.Count); i++)
            {
                if (i >= _icons.Count)
                    _icons.Add(Instantiate(spritePrototype, pos + new Vector3((i * iconWidth) + Math.Max(0, i - 1) * xSpacingWidth, 0, 0), Quaternion.identity, transform));
                else
                    _icons[i].gameObject.SetActive(i < resourceAmount);
                
                _icons[i].sprite = primaryResourceIcon;
            }
        }
    }
}
