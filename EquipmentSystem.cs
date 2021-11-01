using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSystem : MonoBehaviour
{
    Unit unit;
    private void Awake()
    {
        unit = GetComponent<Unit>();
    }

}
