using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour
{

    /// <summary>
    /// unit type (SO with data)
    /// </summary>
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
                HandleAttack();
                break;
        }
    }

    protected virtual void HandleAttack()
    {
        // Base does nothing
    }
}
