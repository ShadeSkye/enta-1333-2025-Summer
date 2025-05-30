using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentArmyManager : MonoBehaviour
{
    public int ArmyID;
    public bool IsPlayer => ArmyID == 0;

    public List<UnitBase> CurrentlyActiveUnits = new List<UnitBase>();

    public void UpdateAllUnits()
    {
        foreach(UnitBase unit in CurrentlyActiveUnits)
        {
            
        }
    }
}
