using Spine.Unity;
using UnityEngine;

// файл с различными структурами данных использующимися в различных участках кода

public delegate void UnitDelegate(Unit u);
public delegate void UnitMouseDelegate(Unit rt, bool isSwitchOn);
public delegate void IconDelegate(RectTransform rt, bool isSwitchOn);
public delegate void SelectTarget(Unit u, Animation uAction);

//перечисление юнитов
public enum UnitType
{
    Miner
}

//структура параметров каждого юнита
public class Parameters
{
    public int Acuracy;
    public int Atack;
    public int Block;
    public int Defence;
    public int Evasion;
    public int HP;
}

//структура хранит название действия, анимацию действия и тип действия юнита
public struct UnitAction
{
    public Animation anim;
    public string name;
    public AnimationReferenceAsset animation;
}

public enum AtackResult
{
    Evasion,
    Block,
    Damage
}

// перечисление обозначений анимаций, действия с запасом
// по идее определение действий, в том числе урона, должно зависеть
// от конкретного юнита
public enum Animation
{
    Action1,
    Action2,
    Action3,
    Action4,
    Action5,
    Action6,
    Idle,
    Pull,
    Damage
}