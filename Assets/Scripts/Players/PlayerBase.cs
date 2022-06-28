using UnityEngine;

public class PlayerBase : Object
{
    public SelectTarget EndTurn;

    public virtual void TakeTurn(Unit u){}

    public virtual void PlayerInit(){}
}