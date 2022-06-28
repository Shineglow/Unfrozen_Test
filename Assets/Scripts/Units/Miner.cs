using System.Collections.Generic;
using Spine.Unity;
using UnityEditor;
using UnityEngine;

public class Miner : Unit
{
    // перегруженная инициализация юнита
    public override void Initialization(bool side, string name = "Default")
    {
        unitSA = GetComponent<SkeletonAnimation>();

        unitType = UnitType.Miner;
        
        parameters = new Parameters();
        parameters.HP = 85;
        parameters.Atack = 10;
        parameters.Defence = 2;
        parameters.Block = 0;
        parameters.Acuracy = 85;
        parameters.Evasion = 2;

        var hit = new UnitAction();
        hit.anim = Animation.Action1;
        hit.name = nameof(hit.anim);

        hit.animation = Resources.Load<AnimationReferenceAsset>("artForTestWork/Miner/ReferenceAssets/PickaxeCharge");

        var pull = new UnitAction();
        pull.anim = Animation.Damage;
        pull.name = nameof(pull.anim);;
        pull.animation = Resources.Load<AnimationReferenceAsset>("artForTestWork/Miner/ReferenceAssets/Pull");

        var idle = new UnitAction();
        idle.anim = Animation.Idle;
        idle.name = nameof(idle.anim);
        idle.animation = Resources.Load<AnimationReferenceAsset>("artForTestWork/Miner/ReferenceAssets/Idle");

        AnimationsDicitonary = new Dictionary<Animation, UnitAction>();
        AnimationsDicitonary.Add(Animation.Action1, hit);
        AnimationsDicitonary.Add(Animation.Damage, pull);
        AnimationsDicitonary.Add(Animation.Idle, idle);

        unitSA.AnimationState.SetAnimation(0, AnimationsDicitonary[Animation.Idle].animation, true);
        
        base.Initialization(side, name);
    }
    
    // обработка нажатия мыши
    private void OnMouseUpAsButton() => onUnitSelect(this);
}