using UnityEngine.Audio;
using UnityEngine;
using System;

public class SoundManager : MonoBehaviour
{
    public Sound[] Sounds;
    private static SoundManager instance;

    public static SoundManager GetInstance() 
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;

        foreach(Sound s in Sounds) 
        {
            /*Inicializar o AudioSource dos nossos elementos da array com os atributos da classe Sound*/
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.volume = s.volume;
            s.source.name = s.name;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.clip = s.clip;
            s.source.mute = s.mute;
        }
    }
    /*Função que toca um som*/
    public void Play(string name)
    {
        Sound s =  Array.Find(Sounds, Sounds => Sounds.name == name);
        if(s == null) 
        {
            Debug.LogError("Som: " + name + " nao existe");
            return;
        }
        s.source.Play();

    /*Função que pará um Som*/
    }public void Stop(string name)
    {
        Sound s =  Array.Find(Sounds, Sounds => Sounds.name == name);
        if(s == null) 
        {
            Debug.LogError("Som: " + name + " nao existe");
            return;
        }
        s.source.Stop();
    }
}
[System.Serializable]
public class Sound 
{
    /*Definição dos atribuitos da classe*/
    public string name;

    [Range(0.1f, 3f)]
    public float pitch;

    [Range(0f, 1f)]
    public float volume;

    public bool loop;

    [HideInInspector]
    public AudioSource source; /*Referencia*/

    public AudioClip clip;
    public bool mute;
}
