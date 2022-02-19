﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MemberSimplifiedVisualStatPanel : MemberUiBase
{
    [SerializeField] private UIHPBarController hpBar;
    [SerializeField] private GameObject cardPlaysItem;
    [SerializeField] private TextMeshProUGUI cardPlaysLabel;
    [SerializeField] private GameObject resourceGainItem;
    [SerializeField] private TextMeshProUGUI resourceGainLabel;
    [SerializeField] private Image resourceGainIcon;
    [SerializeField] private GameObject leadershipItem;
    [SerializeField] private TextMeshProUGUI leadershipLabel;
    [SerializeField] private GameObject atkItem;
    [SerializeField] private TextMeshProUGUI atkLabel;
    [SerializeField] private GameObject magicItem;
    [SerializeField] private TextMeshProUGUI magicLabel;
    [SerializeField] private TextMeshProUGUI armorLabel;
    [SerializeField] private TextMeshProUGUI resistLabel;
    [SerializeField] private GameObject dodgeItem;
    [SerializeField] private TextMeshProUGUI dodgeLabel;
    [SerializeField] private GameObject aegisItem;
    [SerializeField] private TextMeshProUGUI aegisLabel;
    [SerializeField] private GameObject stealthItem;
    [SerializeField] private GameObject tauntItem;
    [SerializeField] private TextMeshProUGUI tauntLabel;

    public override void Init(Member m)
    {
        hpBar.Init(m);
        Set(cardPlaysItem, cardPlaysLabel, m.ExtraCardPlays());
        resourceGainItem.SetActive(false);
        Set(leadershipItem, leadershipLabel, m.Leadership());
        Set(atkItem, atkLabel, m.Attack());
        Set(magicItem, magicLabel, m.Magic());
        armorLabel.text = m.Armor().ToString();
        resistLabel.text = m.Resistance().ToString();
        Set(dodgeItem, dodgeLabel, m.Dodge());
        Set(aegisItem, aegisLabel, m.Aegis());
        stealthItem.SetActive(m.Stealth() > 0);
        Set(tauntItem, tauntLabel, m.Taunt());
    }

    public void SetPrimaryResourceGain(Sprite icon, int amount)
    {
        resourceGainIcon.sprite = icon;
        resourceGainLabel.text = amount.ToString();
        resourceGainItem.SetActive(true);
    }
    
    private void Set(GameObject g, TextMeshProUGUI t, int value)
    {
        g.SetActive(value > 0);
        t.text = value.ToString();
    }
}
