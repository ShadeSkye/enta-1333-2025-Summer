using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour
{
    [SerializeField] protected UnitType _unitType;

    public abstract void MoveTo(GridNode targetNode);

    public UnitState State;

    public virtual void Tick()
    {
        switch (State)
        {
            case UnitState.Moving:

                break;

            case UnitState.Attacking:

                break;
        }
    }

    public virtual void DoMove()
    {

    }
}
