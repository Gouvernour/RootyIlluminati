using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;
    [Range(0f, 3f)]
    public float pitch;
    public bool loop;
    
    [HideInInspector] public AudioSource sauce;
}


public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public Sound[] sounds;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (Sound s in sounds)
        {
            AddSource(s);
        }

        Play("Song");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AddSource(Sound s)
    {
        s.sauce = gameObject.AddComponent<AudioSource>();
        s.sauce.clip = s.clip;
        s.sauce.volume = s.volume;
        s.sauce.pitch = s.pitch;
        s.sauce.loop = s.loop;


    }

    public void Play(string name)
    {
        foreach (Sound s in sounds)
        {
            if (s.name == name)
            {
                s.sauce.Play();
                print(name);
                return;
            }
        }
        print(name + " doesnt exist yo");
    }
    public void Stop(string name)
    {
        foreach (Sound s in sounds)
        {
            if (s.name == name)
            {
                s.sauce.Stop();
                return;
            }
        }
        print(name + " doesnt exist yo");
    }


}
