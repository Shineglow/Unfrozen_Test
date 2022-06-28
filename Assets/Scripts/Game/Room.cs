using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Room : MonoBehaviour
{
    // переменные для размещения юнитов на сцене
    private readonly Vector3 SpaceBetweenUnits = new Vector3(4f, 0, 0);
    private readonly Vector3 startPointAtack = new Vector3(-3.2f, -3f, 0);
    private List<Vector3> unitPositions;
    
    private PlayerBase Atacker; //загружается в комнату (в данном случае генерируется)
    private List<Unit> AtackersSquad; //отряд нападающих

    private PlayerBase Defender; //defender - AI
    private List<Unit> DefendersSquad; //отряд защитников

    [SerializeField] private QueueControl UnitsTurnQueue; //очередь ходов
    private Unit CurrentUnit; // первый в очереди персонаж

    private bool gameEnd;
    private TextMeshProUGUI finalText;
    
    // здесь начинается подготовка всей сцены боя
    private void Start()
    {
        SpawnUnits();
        QueueInitialize();

        UnitsTurnQueue.GetThisTurnUnit += NewTurn;
        UnitsTurnQueue.onUnitDestroy += InitiateSquadSwipe;

        Atacker = new HumanPlayerBase(DefendersSquad);
        Defender = new SimpleAI(AtackersSquad);

        Atacker.EndTurn += EndTurn;
        Defender.EndTurn += EndTurn;

        UnitsTurnQueue.NextTurn();
    }
    
    private void Update()
    {
        // можно выйти нажав escape
        if (Input.GetKeyUp(KeyCode.Escape))
            Application.Quit(0);
        // если игра завершена, можно перезапустить нажав Space
        if (gameEnd && Input.GetKeyUp(KeyCode.Space))
            SceneManager.LoadScene("Figth");
    }

    // убирает юнита из отряда, начинает сдвигать отряд
    // если отряд опустел завершает уровень
    private void InitiateSquadSwipe(Unit u)
    {
        if (u.Side)
        {
            AtackersSquad.Remove(u);
            MoveSquead(AtackersSquad);
            if (AtackersSquad.Count < 1)
            {
                var canvas = FindObjectOfType<Canvas>();
                var orig = Resources.Load<RectTransform>("Prefabs/UI_Prefabs/EndGameMesage");
                finalText = Instantiate(orig, canvas.transform).GetComponent<TextMeshProUGUI>();
                finalText.text = "You Lose";
                finalText.color = new Color(1, 0.49f, 0.49f, 0.7f);
                gameEnd = true;
            }
        }
        else
        {
            DefendersSquad.Remove(u);
            MoveSquead(DefendersSquad);
            if (DefendersSquad.Count < 1)
            {
                var canvas = FindObjectOfType<Canvas>();
                var orig = Resources.Load<RectTransform>("Prefabs/UI_Prefabs/EndGameMesage");
                finalText = Instantiate(orig, canvas.transform).GetComponent<TextMeshProUGUI>();
                finalText.text = "You Win";
                finalText.color = new Color(0.49f, 0.49f, 1f, 0.7f);
                gameEnd = true;
            }
        }
    }

    // обработка результатов хода
    public void EndTurn(Unit u, Animation uAction)
    {
        switch (uAction)
        {
            case Animation.Action1:
                CurrentAtackTarget(u, uAction);
                break;
            case Animation.Action2:
                break;
            case Animation.Action3:
                break;
            case Animation.Action4:
                break;
            case Animation.Action5:
                break;
            case Animation.Action6:
                break;
            case Animation.Idle:
                NextTurn();
                break;
            case Animation.Pull:
                break;
            case Animation.Damage:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(uAction), uAction, null);
        }

        CurrentUnit.transform.GetChild(0).gameObject.SetActive(false);
    }

    // Рассчёт урона текущего юнита по его цели
    // производится в классе комнаты, чтобы избежать прямого доступа юнитов друг к другу
    private void CurrentAtackTarget(Unit Target, Animation targetAction)
    {
        AtackResult result;
        var damage = 0;
        var pA = CurrentUnit.parameters;
        var pD = Target.parameters;

        if (Random.Range(0, 100) - (pA.Acuracy - pD.Evasion) <= 0)
        {
            damage = pA.Atack - pD.Defence;
            if (damage > 0)
                result = AtackResult.Damage;
            else
                result = AtackResult.Block;
        }
        else
        {
            result = AtackResult.Evasion;
        }

        // 
        var a = FightAnimation(CurrentUnit, targetAction, Target, result);
        var atackAnim = StartCoroutine(a);
        var b = FloatDamage(atackAnim, Target, damage*5);
        StartCoroutine(b);
    }

    private IEnumerator FloatDamage(Coroutine needWait, Unit target, int damage)
    {
        yield return needWait;
        var a = target.TakeDamage(damage);
        if (a)
            UnitsTurnQueue.ONUnitDestroy(target);
        else
            NextTurn();
    }

    private void NextTurn() => UnitsTurnQueue.RollQueue();

    private void NewTurn(Unit curentUnit)
    {
        if (gameEnd)
            return;
        
        CurrentUnit = curentUnit;
        // подсвечивает активного юнита
        CurrentUnit.transform.GetChild(0).gameObject.SetActive(true);
        
        if (CurrentUnit.Side)
            Atacker.TakeTurn(curentUnit);
        else
            Defender.TakeTurn(curentUnit);
    }

    // Вспомогательная функция запускает анимации 
    private IEnumerator FightAnimation(Unit Atacker, Animation a, Unit Defender, AtackResult result)
    {
        switch (result)
        {
            case AtackResult.Evasion:
                
                Atacker.PlayAnimation(a);
                break;
            case AtackResult.Block:
                
                Atacker.PlayAnimation(a);
                break;
            case AtackResult.Damage:
                Atacker.PlayAnimation(a);
                
                yield return new WaitForSeconds(0.5f);
                
                var track = Defender.PlayAnimation(Animation.Damage);
                
                yield return new WaitForSpineAnimationComplete(track);
                break;
        }
    }

    // спавн отрядов
    private void SpawnUnits()
    {
        var orig = Resources.Load<Transform>("Prefabs/Miner_Default");//AssetDatabase.LoadAssetAtPath<Transform>("Assets/Prefabs/Miner_Default.prefab");

        DefendersSquad = new List<Unit>(4);
        AtackersSquad = new List<Unit>(4);
        unitPositions = new List<Vector3>(4);

        for (var i = 0; i < 4; i++)
        {
            DefendersSquad.Add(Instantiate(orig,
                new Vector3(-startPointAtack.x, startPointAtack.y, 0) + SpaceBetweenUnits * i, new Quaternion(),
                GetComponent<Transform>()).GetComponent<Unit>());
            DefendersSquad[i].Initialization(false, "AiUnit ");

            unitPositions.Add(startPointAtack + SpaceBetweenUnits * -i);

            AtackersSquad.Add(Instantiate(orig, unitPositions[i], new Quaternion(),
                GetComponent<Transform>()).GetComponent<Unit>());
            AtackersSquad[i].Initialization(true, "HumanUnit ");
        }
    }

    // инициализация очереди
    private void QueueInitialize()
    {
        //общий список юнитов
        var queue = new Unit[AtackersSquad.Count + DefendersSquad.Count];

        int maxSquad = AtackersSquad.Count < DefendersSquad.Count ? DefendersSquad.Count : AtackersSquad.Count;
        
        for (var i = 0; i < maxSquad; i++)
        {
            if(DefendersSquad.Count > i)
                queue[i] = DefendersSquad[i];
            if(AtackersSquad.Count > i)
                queue[i + 4] = AtackersSquad[i];
        }

        // перемешивание списка юнитов
        for (var i = 0; i < 8; i++)
        {
            var a = queue[i];
            var b = Random.Range(0, 8);
            queue[i] = queue[b];
            queue[b] = a;
        }

        if (!UnitsTurnQueue)
            UnitsTurnQueue = FindObjectOfType<QueueControl>();
        UnitsTurnQueue.LoadQueue(queue);
    }

    // смещение юнитов к центру
    private void MoveSquead(List<Unit> uList)
    {
        for (var y = 0; y < uList.Count; y++)
        {
            var a = IMooveUnit(uList[y], y);
            StartCoroutine(a);
        }
    }

    // корутина анимации движения юнитов
    private IEnumerator IMooveUnit(Unit u, int position)
    {
        var a = false;
        var newPos = u.Side
            ? new Vector3(unitPositions[position].x, unitPositions[position].y, unitPositions[position].z)
            : new Vector3(-unitPositions[position].x, unitPositions[position].y, unitPositions[position].z);
        while (!a)
        {
            u.transform.position = Vector3.MoveTowards(u.transform.position, newPos, 10 * Time.deltaTime);
            a = u.transform.position == newPos;
            yield return new WaitForEndOfFrame();
        }
    }
}