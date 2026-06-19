using UnityEngine;

class Particle: MonoBehaviour
{
    public ParticleManager.ParticleType Type { get; private set; }
    private float _timer = 0f;

    public void Init(ParticleManager.ParticleType type, float lifeTime)
    {
        _timer = lifeTime;
        Type = type;
    }

    public bool Decay()
    {
        _timer -= Time.deltaTime;
        return _timer <= 0f;
    }
}
