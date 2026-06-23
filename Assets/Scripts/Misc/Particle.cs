using UnityEngine;

class Particle: MonoBehaviour
{
    public ParticleManager.ParticleType Type { get; private set; }
    private float _timer = 0f;
    private Vector3 _direction;
    private float _decay;

    public void Init(ParticleManager.ParticleType type)
    {
        _timer = 0;
        _direction = Vector3.zero;
        _decay = 0f;
        Type = type;
    }

    public void Init(ParticleManager.ParticleType type, float lifeTime, Vector3 direction, float decay)
    {
        _timer = lifeTime;
        _direction = direction;
        _decay = decay;
        transform.rotation = Quaternion.LookRotation(direction);
        Type = type;
    }

    public bool Decay()
    {
        _timer -= Time.deltaTime;

        _direction *= _decay;
        transform.Translate(_direction * Time.deltaTime);

        return _timer <= 0f;
    }
}
