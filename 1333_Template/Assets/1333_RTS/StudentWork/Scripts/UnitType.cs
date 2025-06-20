using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "UnitType", menuName = "Game/UnitType")]
public class UnitType : ScriptableObject
{
    [SerializeField] private int _health;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private int _range;
    [SerializeField] private int _damage;
    [SerializeField] private AttackType _attackType = AttackType.Melee;
    [SerializeField] private string _name;
    [SerializeField] private Sprite _icon;

    [SerializeField] private GameObject _unitPrefab;

    public GameObject UnitPrefab => _unitPrefab;
    public int Health => _health;
    public float MovementSpeed => _movementSpeed;
    public int Range => _range;
    public int Damage => _damage;
    public AttackType AttackType => _attackType;
    public string Name => _name;
    public Sprite Icon => _icon;
}
