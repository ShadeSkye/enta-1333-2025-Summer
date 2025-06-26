using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArmyManager : MonoBehaviour
{
    public ArmyManager ArmyManagerRef;

    private void Awake()
    {
        ArmyManagerRef = new ArmyManager();

        ArmyManagerRef.ArmyID = 0;
    }
}
