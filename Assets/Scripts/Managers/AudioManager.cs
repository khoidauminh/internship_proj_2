using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    private static float volume = 0.67f;

    private GameObject _prefab;
    private GameObject _holder;

    private AudioClip _smack = null;
    private AudioClip _spawn = null;
    private AudioClip _explode = null;
    private AudioClip _levelUp = null;
    private AudioClip _win = null;

    private readonly Queue<AudioSource> _pool = new();

    private static AudioManager _instance;
    public static AudioManager GetInstance()
    {
        _instance ??= FindAnyObjectByType<AudioManager>();
        _instance ??= new GameObject(nameof(AudioManager)).AddComponent<AudioManager>();
        return _instance;
    }

    void Start()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        _smack ??= Resources.Load<AudioClip>("Sounds/Smack");
        _smack ??= Resources.Load<AudioClip>("Sounds/Smack");
        _explode ??= Resources.Load<AudioClip>("Sounds/Explode");
        _spawn ??= Resources.Load<AudioClip>("Sounds/Spawn");
        _levelUp ??= Resources.Load<AudioClip>("Sounds/Levelup");
        _win ??= Resources.Load<AudioClip>("Sounds/Win");
    }

    void SpawnAudioSource(AudioClip clip, Vector3 pos)
    {
        _prefab ??= Resources.Load<GameObject>("Prefabs/BaseAudio");
        _holder ??= GameObject.Find("Audio Source Holder");

        AudioSource src = (_pool.Count > 0 && !_pool.Peek().isPlaying) ? _pool.Dequeue() : Instantiate(_prefab, pos, Quaternion.identity).GetComponent<AudioSource>();

        src.transform.position = pos;
        _pool.Enqueue(src);
        src.transform.SetParent(_holder.transform);
        src.PlayOneShot(clip, volume);
    }

    void SpawnAudioSource(AudioClip clip)
    {
        SpawnAudioSource(clip, Vector3.zero);
    }

    public void Smack(Vector3 pos)
    {
        SpawnAudioSource(_smack, pos);
    }

    public void Explode(Vector3 pos)
    {
        SpawnAudioSource(_explode, pos);
    }

    public void Spawn(Vector3 pos)
    {
        SpawnAudioSource(_spawn, pos);
    }

    public void LevelUp(Vector3 pos)
    {
        SpawnAudioSource(_levelUp, pos);
    }

    public void ChangeVolume(float val)
    {
        volume = Mathf.Clamp01(val);

        AudioSource[] list = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);

        foreach (AudioSource i in list)
        {
            if (i.gameObject.name == "Bgm")
            {
                i.volume = volume * 0.5f;
                continue;
            }

            i.volume = volume;
        }
    }

    public void StopAll()
    {
        AudioSource[] list = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);

        foreach (AudioSource i in list)
        {
            i.Stop();
        }
    }

    public void Win()
    {
        SpawnAudioSource(_win);
    }
}
