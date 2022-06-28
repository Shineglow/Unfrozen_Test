using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// класс обрабатывает взаимодействия с пользовательским интерфейсом
public class GuiListner : MonoBehaviour
{
    public bool active;

    // ссылки на кнопки атаки и пропуска хода
    [SerializeField] private Button Atack;
    [SerializeField] private Button Wait;

    // ссылки на элементы GUI отражающие параметры юнитов
    [SerializeField] private UnitInfoUI AtackerStats;
    [SerializeField] private UnitInfoUI DefenderStats;
    
    private List<Unit> Enemys;
    public SelectTarget GuiCallback;

    private Animation unitAction = Animation.Idle;

    
    public void ShowStats(Unit u)
    {
        if (u.Side)
            AtackerStats.LoadStats(new[]
            {
                u.parameters.HP, u.parameters.Atack,
                u.parameters.Defence, u.parameters.Block,
                u.parameters.Evasion, u.parameters.Acuracy
            });
        else
            DefenderStats.LoadStats(new[]
            {
                u.parameters.HP, u.parameters.Atack,
                u.parameters.Defence, u.parameters.Block,
                u.parameters.Evasion, u.parameters.Acuracy
            });
    }

    // инициализация класса
    public void GuiInit(List<Unit> enemys)
    {
        Atack.onClick.AddListener(AtackListner);
        Wait.onClick.AddListener(WaitListner);

        Enemys = new List<Unit>(enemys);
        foreach (var u in Enemys)
        {
            u.onUnitDestroy += RemoveUnit;
            u.onUnitSelect += DeactivateGUI;
            u.onUnitHower += ShowStats;
        }
    }
    
    // удаляет юнит из списка противников
    private void RemoveUnit(Unit u)
    {
        Enemys.Remove(u);
        u.onUnitHower -= ShowStats;
        u.onUnitSelect -= DeactivateGUI;
        u.onUnitDestroy -= RemoveUnit;
    }
    
    // прекращает обработку ввода если выбранно какое-либо действие
    private void DeactivateGUI(Unit u)
    {
        if (unitAction == Animation.Idle)
            return;

        GuiCallback(u, unitAction);
        active = false;
        DehighlightAllUnitButtons();
        unitAction = Animation.Idle;
    }

    // подсвечивает юнитов доступных для атаки
    private void HighlightUnitButtons()
    {
        foreach (var u in Enemys)
            u.transform.GetChild(0).gameObject.SetActive(true);
    }

    // отключает подсветку юнитов
    private void DehighlightAllUnitButtons()
    {
        foreach (var u in Enemys)
            u.transform.GetChild(0).gameObject.SetActive(false);
    }

    // обработка нажатия на кнопку атаки
    public void AtackListner()
    {
        if (!active)
            return;

        unitAction = Animation.Action1;
        HighlightUnitButtons();
    }

    // обработка ажатия н кнопку ожидания
    public void WaitListner()
    {
        if (!active)
            return;

        DehighlightAllUnitButtons();
        GuiCallback(null, Animation.Idle);
    }
}