using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;

[RequireComponent(typeof(Transform), typeof(SkeletonAnimation))]
public class Unit : MonoBehaviour
{
    public UnitType unitType { get; protected set; }
    public bool Side { get; protected set; } // true - left, false - right
    public bool isDead = false;

    protected SkeletonAnimation unitSA;
    protected Dictionary<Animation, UnitAction> AnimationsDicitonary;
    
    public UnitDelegate onUnitDestroy;
    public UnitDelegate onUnitSelect;
    public UnitDelegate onUnitHower;

    public UnitMouseDelegate onMouseOverUnit;

    // класс поля которого определяют параметры юнита
    public Parameters parameters { get; protected set; } // значение на первом уровне равно значению базы

    // базовая инициализация
    public virtual void Initialization(bool side, string name = "Default")
    {
        Side = side;
        gameObject.name = name;
        
        if(!Side)
            GetComponent<SkeletonAnimation>().Skeleton.ScaleX = -1f;
    }
    
    // получение урона
    public bool TakeDamage(int damage)
    {
        parameters.HP -= damage;
        isDead = parameters.HP <= 0;
        
        if (isDead && !Side)
            onUnitDestroy(this);

        return isDead;
    }
    
    // воспроизведение анимации
    public TrackEntry PlayAnimation(Animation anim)
    {
        var i = SetAnimation(AnimationsDicitonary[anim].animation);
        StartCoroutine(i);
        return unitSA.AnimationState.GetCurrent(0);
    }

    // воспроизводит необходимую анимацию, после её завершения, воспроизводит предыдущую (обычно Idle)
    IEnumerator SetAnimation(AnimationReferenceAsset spineAnimation)
    {
        var a = unitSA.AnimationState.GetCurrent(0).animation;
        unitSA.AnimationState.SetAnimation(0, spineAnimation, false);
        yield return new WaitForSpineAnimationComplete(unitSA.AnimationState.Tracks.Items[0]);
        unitSA.AnimationState.SetAnimation(0, a, true);
    }

    // обработка событий мыши
    private void OnMouseOver()
    {
        if(!Side && !isDead)
            onUnitHower(this);
    }

    private void OnMouseEnter()
    {
        onMouseOverUnit(this, true);
    }

    private void OnMouseExit()
    {
        onMouseOverUnit(this, false);
    }

    
}