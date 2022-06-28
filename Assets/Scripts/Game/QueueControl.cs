using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class QueueControl : MonoBehaviour
{
    // класс содержит ссылки на пары юнит\иконка
    // класс не вынесен в публичный доступ, потому что используется только здесь
    class UnitIconPare
    {
        public Unit unit;
        public RectTransform icon;

        public UnitIconPare(Unit unit, RectTransform icon)
        {
            this.unit = unit;
            this.icon = icon;
        }
    }

    // массив реализующий очередь пар
    private UnitIconPare[] queue;

    // переменные положения иконок очереди
    private Vector2 UiStartPos;
    private Vector2 UiStep;
    private Vector2[] queuePositions;
    
    public event UnitDelegate GetThisTurnUnit;
    public event UnitDelegate onUnitDestroy;

    // инициализация очереди списком юнитов
    public void LoadQueue(Unit[] units)
    {
        // Инициализация очередей
        queue = new UnitIconPare[units.Length];
        queuePositions = new Vector2[units.Length];

        // Поиск префаба UI элемента очереди
        var orig = Resources.Load<RectTransform>("Prefabs/UI_Prefabs/Item");
            // AssetDatabase.LoadAssetAtPath<RectTransform>("Assets/Prefabs/UI_Prefabs/Item.prefab");

        // Рассчёт шага и стартовой позиции исходя из размера объекта
        UiStep = new Vector2(orig.sizeDelta.x, 0) + new Vector2(10, 0);
        UiStartPos = -UiStep * 3.5f + new Vector2(5, 0);
        GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, UiStep.x) / 2;
        
        // спавн и настройка иконок
        for (var i = 0; i < 8; i++)
        {
            var next = Instantiate(orig, GetComponent<Transform>());
            next.anchoredPosition = queuePositions[i] = UiStartPos + UiStep * i;

            if (units[i].Side)
            {
                next.localScale = new Vector3(-1f, 1f, 1f);
                next.GetComponent<Image>().color = new Color(0.9f, 0.49f, 0.49f, 0.7f);
            }
            else
            {
                next.localScale = new Vector3(1f, 1f, 1f);
                next.GetComponent<Image>().color = new Color(0.49f, 0.49f, 0.9f, 0.7f);
            }

            next.gameObject.name = "UnitIcon " + i;
            // подписка на ивенты мыши, для взаимной подсветки
            next.GetComponent<Icon>().onMouseOverCustom += ONMouseOverIcon;
            units[i].onMouseOverUnit += ONMouseOverUnit;
            
            var item = new UnitIconPare(units[i], next);
            queue[i] = item;
        }
    }

    // подсвечивает или отключает подсветку связанных иконки и юнита
    private void ONMouseOverIcon(RectTransform rt, bool isSwitchOn)
    {
        foreach (var a in queue)
            if (a.icon == rt)
            {
                a.unit.transform.GetChild(1).gameObject.SetActive(isSwitchOn);
                a.icon.GetComponent<Image>().color +=
                    isSwitchOn ? new Color(0.1f, 0.1f, 0.1f, 1f) : new Color(-0.1f, -0.1f, -0.1f, -1f);
                return;
            }
    }
    
    // подсвечивает или отключает подсветку связанных иконки и юнита
    private void ONMouseOverUnit(Unit u, bool isSwitchOn)
    {
        foreach (var a in queue)
            if (a.unit == u)
            {
                a.unit.transform.GetChild(1).gameObject.SetActive(isSwitchOn);
                a.icon.GetComponent<Image>().color +=
                    isSwitchOn ? new Color(0.1f, 0.1f, 0.1f, 1f) : new Color(-0.1f, -0.1f, -0.1f, -1f);
                return;
            }
    }

    // при убийстве юнита уничтожает объекты: Unit и Icon
    // перестраивает очередь и выравнивает объекты в ней
    public void ONUnitDestroy(Unit u)
    {
        var newArr = new UnitIconPare[queue.Length - 1];

        for (int i = 0, a = 0; i < queue.Length; i++)
            if (queue[i].unit == u)
            {
                DestroyImmediate(queue[i].unit.gameObject);
                DestroyImmediate(queue[i].icon.gameObject);
            }
            else
            {
                newArr[a++] = queue[i];
            }
        queue = newArr;

        onUnitDestroy(u);
        RollQueue();
    }

    // извлечение-добавление элементов в очередь, на основе позиции в очереди
    // осуществляется движене иконок
    public void RollQueue()
    {
        var temp = queue[0];
        for (int i = 1; i < queue.Length; i++)
            queue[i - 1] = queue[i];
        queue[^1] = temp;

        var toLast = queuePositions[queue.Length - 1];

        var iAnim = ReenqueueAnimation(toLast, queue[^1].icon, 400);
        StartCoroutine(iAnim);
    }

    // корутина анимирующая отдельную иконку
    private IEnumerator IconSwapAnimation(Vector2 target, RectTransform obj, float speed = 150)
    {
        var a = false;
        while (!a)
        {
            a = MoveIcon(target, obj);
            yield return new WaitForEndOfFrame();
        }
    }

    private bool MoveIcon(Vector2 target, RectTransform obj, float speed = 150)
    {
        obj.anchoredPosition = Vector3.MoveTowards(obj.anchoredPosition3D, target, speed * Time.deltaTime);
        return obj.anchoredPosition3D == new Vector3(target.x, target.y, 0);
    }

    // Корутина для анимации извлечения-добавления элемента в очередь иконок
    private IEnumerator ReenqueueAnimation(Vector3 target, RectTransform obj, float speed = 150)
    {
        var s = new Vector3(UiStep.y, UiStep.x + 10, 0);
        var down = obj.anchoredPosition3D - s;
        var up = target;
        target = target - s;

        var b = false;
        while (!b)
        {
            b = MoveIcon(down, obj, speed);
            yield return new WaitForEndOfFrame();
        }

        SwipeQueue(obj);

        var a = false;
        while (!a)
        {
            a = MoveIcon(target, obj, speed);
            yield return new WaitForEndOfFrame();
        }

        var c = false;
        while (!c)
        {
            c = MoveIcon(up, obj, speed);
            yield return new WaitForEndOfFrame();
        }

        NextTurn();
    }

    // функция смещающая ряд иконок
    private void SwipeQueue(RectTransform obj)
    {
        for (int i = 0; i < queue.Length - 1; i++)
        {
            var aAnim = IconSwapAnimation(queuePositions[i], queue[i].icon);
            StartCoroutine(aAnim);
        }
    }

    public void NextTurn() => GetThisTurnUnit(queue[0].unit);
}