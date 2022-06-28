using System.Collections.Generic;

public class SimpleAI : PlayerBase
{
    private List<Unit> Enemys;
    
    public SimpleAI(List<Unit> enemys)
    {
        PlayerInit();
        Enemys = enemys;
    }

    public override void TakeTurn(Unit u)
    {
        var target = FindTarget();
        var curAtack = target ? Animation.Action1 : Animation.Idle;
        EndTurn(target, curAtack);
    }

    // поиск самой подходящей цели
    protected virtual Unit FindTarget()
    {
        var result = Enemys[0];
        for (var i = 1; i < Enemys.Count; i++)
        {
            // проверка на сколько потенциальная цель слабее текущей цели
            var block = result.parameters.Block >= Enemys[i].parameters.Block ? 1 : 0;
            var defence = result.parameters.Defence >= Enemys[i].parameters.Defence ? 1 : 0;
            var health = result.parameters.HP >= Enemys[i].parameters.HP ? 1 : 0;
            var coef = block + defence + health;
            
            // c 10% вероятностью сменит цель
            var missHit = (UnityEngine.Random.Range(0f,100f) / 90) < 1;
            
            // если хотя бы по двум параметрам юнит слабее, то он выберается текущей целью
            // выбор самой слабой цели объясняется тем, что устранение даже слабого атакующего
            // уменьшит общий урон. Чем меньше урон, тем больше выжевет
            if (coef > 1 && missHit)
                result = Enemys[i];
        }
        return result;
    }
    
}