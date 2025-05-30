using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitType", menuName = "Game/UnitType")]
public class UnitType : ScriptableObject
{
    [SerializeField] private int _health;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private int _range;
    [SerializeField] private int _damage;
    [SerializeField] private AttackType _attackType = AttackType.Melee;

    [SerializeField] private GameObject _unitPrefab;

    public GameObject UnitPrefab => _unitPrefab;
}
