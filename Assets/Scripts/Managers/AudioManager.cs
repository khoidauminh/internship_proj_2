using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource _source;
    private static AudioClip? _smack = null;
    private static AudioClip? _spawn = null;
    private static AudioClip? _explode = null;

    void Awake()
    {
        _source = FindAnyObjectByType<AudioSource>();
    }

    public static void Smack(Vector3 pos)
    {
        _smack ??= Resources.Load<AudioClip>("Sounds/Smack");
        AudioSource.PlayClipAtPoint(_smack, pos);
    }

    public static void Explode(Vector3 pos)
    {
        _explode ??= Resources.Load<AudioClip>("Sounds/Explode");
        AudioSource.PlayClipAtPoint(_explode, pos);
    }

    public static void Spawn(Vector3 pos)
    {
        _spawn ??= Resources.Load<AudioClip>("Sounds/Spawn");
        AudioSource.PlayClipAtPoint(_spawn, pos);
    }
}
