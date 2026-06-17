using UnityEngine;

#nullable enable

public class AudioManager : MonoBehaviour
{
    private static AudioSource? _source;
    private static AudioClip? _smack = null;
    private static AudioClip? _spawn = null;
    private static AudioClip? _explode = null;
    private static AudioClip? _levelUp = null;
    private static AudioClip? _win = null;

    void Awake()
    {
        _source = gameObject.GetComponent<AudioSource>();
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

    public static void LevelUp(Vector3 pos)
    {
        _levelUp ??= Resources.Load<AudioClip>("Sounds/Levelup");
        AudioSource.PlayClipAtPoint(_levelUp, pos);
    }

    public static void StopAll()
    {
        AudioSource[] list = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);

        foreach (AudioSource i in list)
        {
            if (i != _source)
            {
                i.Stop();
            }
        }
    }

    public static void Win()
    {
        _win ??= Resources.Load<AudioClip>("Sounds/Win");
        AudioSource.PlayClipAtPoint(_win, new Vector3(0, 0, 0));
    }
}
