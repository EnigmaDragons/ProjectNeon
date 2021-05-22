using UnityEngine;

public class TabSelectionUI : OnMessage<CustomizationTabSwitched>
{
    [SerializeField] private SelectTabButton equipmentTab;
    [SerializeField] private SelectTabButton heroTab;
    [SerializeField] private SelectTabButton enemyTab;
    [SerializeField] private GameObject[] equipmentObjects;
    [SerializeField] private GameObject[] heroObjects;
    [SerializeField] private GameObject[] enemyObjects;
    [SerializeField] private GameObject[] notEquipmentObjects;
    [SerializeField] private GameObject[] notHeroObjects;
    [SerializeField] private GameObject[] notEnemyObjects;

    public void Awake()
    {
        equipmentTab.Init(() => Message.Publish(new CustomizationTabSwitched { TabName = "equipment" }), true);
        heroTab.Init(() => Message.Publish(new CustomizationTabSwitched { TabName = "hero" }), false);
        enemyTab.Init(() => Message.Publish(new CustomizationTabSwitched { TabName = "enemy" }), false);
    }

    protected override void Execute(CustomizationTabSwitched msg)
    {
        var equipmentTabActive = msg.TabName == "equipment";
        var heroTabActive = msg.TabName == "hero";
        var enemyTabActive = msg.TabName == "enemy";
        equipmentTab.SetSelected(equipmentTabActive);
        equipmentObjects?.ForEach(x => x.SetActive(equipmentTabActive));
        notEquipmentObjects?.ForEach(x => x.SetActive(!equipmentTabActive));
        heroTab.SetSelected(heroTabActive);
        heroObjects?.ForEach(x => x.SetActive(heroTabActive));
        notHeroObjects?.ForEach(x => x.SetActive(!heroTabActive));
        enemyTab.SetSelected(enemyTabActive);
        enemyObjects?.ForEach(x => x.SetActive(enemyTabActive));
        notEnemyObjects?.ForEach(x => x.SetActive(!enemyTabActive));
    }
}