using UnityEngine;

#nullable enable

public class AudioManager : MonoBehaviour
{
    private static GameObject? _prefab;
    private static AudioClip? _smack = null;
    private static AudioClip? _spawn = null;
    private static AudioClip? _explode = null;
    private static AudioClip? _levelUp = null;
    private static AudioClip? _win = null;

    static void SpawnAudioSource(AudioClip clip, Vector3 pos)
    {
        _prefab ??= Resources.Load<GameObject>("Prefabs/BaseAudio");
        AudioSource src = Instantiate(_prefab, pos, Quaternion.identity).GetComponent<AudioSource>();
        src.PlayOneShot(clip);
        Destroy(src.gameObject, clip.length);
    }

    static void SpawnAudioSource(AudioClip clip)
    {
        SpawnAudioSource(clip, Vector3.zero);
    }

    public static void Smack(Vector3 pos)
    {
        _smack ??= Resources.Load<AudioClip>("Sounds/Smack");
        SpawnAudioSource(_smack, pos);
    }

    public static void Explode(Vector3 pos)
    {
        _explode ??= Resources.Load<AudioClip>("Sounds/Explode");
        SpawnAudioSource(_explode, pos);
    }

    public static void Spawn(Vector3 pos)
    {
        _spawn ??= Resources.Load<AudioClip>("Sounds/Spawn");
        SpawnAudioSource(_spawn, pos);
    }

    public static void LevelUp(Vector3 pos)
    {
        _levelUp ??= Resources.Load<AudioClip>("Sounds/Levelup");
        SpawnAudioSource(_levelUp, pos);
    }

    public static void StopAll()
    {
        AudioSource[] list = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);

        foreach (AudioSource i in list)
        {
            i.Stop();
        }
    }

    public static void Win()
    {
        _win ??= Resources.Load<AudioClip>("Sounds/Win");
        SpawnAudioSource(_win);
    }
}
