using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
    bool IsDead { get; }
    Vector3 Position { get; }
    UnitState State { get; }
    Transform transform { get; }

    void TakeDamage(int amount);
}
