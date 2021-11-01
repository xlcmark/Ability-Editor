using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierBehaviour : UnitBehaviour
{
    protected override Unit SetPriority(List<Unit> units)
    {
        List<Unit> condition1;
        condition1 = Condition_HelpFriend(units,UnitType.Hero);
        if (condition1.Count != 0)
        {
            List<Unit> condition2;
            condition2 = Condition_EnemyType(condition1, UnitType.Hero);
            if (condition2.Count != 0) return condition2[0];//敵方英雄攻擊我方英雄(1
            condition2 = Condition_EnemyType(condition1, UnitType.Solider);
            if (condition2.Count != 0) return condition2[0];//敵方小兵攻擊我方英雄(2
        }

        condition1= Condition_HelpFriend(units, UnitType.Solider);
        if (condition1.Count != 0)
        {
            List<Unit> condition2;
            condition2 = Condition_EnemyType(condition1, UnitType.Solider);
            if (condition2.Count != 0) return condition2[0];//敵方小兵攻擊我方小兵(3
            condition2 = Condition_EnemyType(condition1, UnitType.Hero);
            if (condition2.Count != 0) return condition2[0];//敵方小兵攻擊我方英雄(4
        }

        condition1 = Condition_EnemyType(units, UnitType.Solider);
        if (condition1.Count != 0) return condition1[0];//最近的敵方小兵(5

        condition1 = Condition_EnemyType(units, UnitType.Hero);
        if (condition1.Count != 0) return condition1[0];//最近的敵方英雄(6

        return units[0];//最近的單位(7
    }
}
