using UnityEngine;

[CreateAssetMenu(fileName = "BaseUnitConfig", menuName = "Scriptable Objects/BaseUnitConfig")]
public class BaseUnitConfig : ScriptableObject
{
    [System.Serializable]
    public class Stats
    {
        public int BaseHealth;
        public int BaseDamage;
        public float BaseSpeed;
        public string Name;
    }

    [SerializeField]
    private Stats stats;

    public int BaseHealth => stats.BaseHealth;
    public int BaseDamage => stats.BaseDamage;
    public float BaseSpeed => stats.BaseSpeed;
    public string Name => stats.Name;

    public Stats Get()
    {
        return stats;
    }

    public void Initialize(int baseHealth, int baseDamage, float baseSpeed, string name)
    {
        stats ??= new Stats();

        stats.BaseHealth = baseHealth;
        stats.BaseDamage = baseDamage;
        stats.BaseSpeed = baseSpeed;
        stats.Name = name;
    }
}
