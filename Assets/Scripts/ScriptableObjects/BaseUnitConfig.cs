using UnityEngine;

[CreateAssetMenu(fileName = "BaseUnitConfig", menuName = "Scriptable Objects/BaseUnitConfig")]
public class BaseUnitConfig : ScriptableObject
{
    [SerializeField] private int _baseHealth;
    public int BaseHealth => _baseHealth;

    [SerializeField] private int _baseDamage;
    public int BaseDamage => _baseDamage;

    [SerializeField] private float _baseSpeed;
    public float BaseSpeed => _baseSpeed;

    [SerializeField] private string _name;
    public string Name => _name;

    public void Initialize(int baseHealth, int baseDamage, float baseSpeed, string name)
    {
        _baseHealth = baseHealth;
        _baseDamage = baseDamage;
        _baseSpeed = baseSpeed;
        _name = name;
    }
}
