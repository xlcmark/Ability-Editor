using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Action/CreateUnit")]
public class CreateUnit : BaseAction
{
    public GameObject prefab;
    [ValueAuto]
    public value lifeTime;
    public override void DoAction(Vector3 point, Ability ability, Modifier modifier)
    {
        Create(ability, point);
    }
    public override void DoAction(Transform target, Ability ability, Modifier modifier)
    {
        Create(ability, target.position);
    }
    private void Create(Ability ability,Vector3 SpwanPos)
    {
        GameObject clone =Instantiate(prefab, SpwanPos, ability.Owner.rotation);
        Unit masterUnit = ability.Owner.GetComponent<Unit>();
        if (masterUnit == null) Debug.Log("master hadn't <Unit>");

        SummonedUnit summoned = clone.GetComponent<SummonedUnit>();
        if(summoned==null) Debug.Log("summoned hadn't <SummonedUnit>");

        int i = Mathf.Clamp(ability.Level - 1, 0, lifeTime.values.Length - 1);
        summoned.SetSummonedUnit(lifeTime.values[i], masterUnit);

        //若是玩家要給召喚物操控權
        if (masterUnit == HeroManager.instance.heroes[0].hero)
        {
            clone.AddComponent<SummonedControler>();
        }

    }
}
