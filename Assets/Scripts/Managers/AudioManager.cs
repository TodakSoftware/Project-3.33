using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
    public SO_AudioPreset sounds; // store all our sounds
    public SO_AudioPreset playlist; // store all our music

    private int currentPlayingIndex = 999; // set high to signify no song playing

    // a play music flag so we can stop playing music during cutscenes etc
    private bool shouldPlayMusic = false;

    public static AudioManager instance; // will hold a reference to the first AudioManager created

    private float mvol; // Global music volume
    private float evol; // Global effects volume
    AudioSource bgmMusic;

    private void Awake()
    {
        if (instance == null){     // if the instance var is null this is first AudioManager
            instance = this;        //save this AudioManager in instance 
        }
        else{
            Destroy(gameObject);    // this isnt the first so destroy it
            return;                 // since this isn't the first return so no other code is run
        }

        DontDestroyOnLoad(gameObject); // do not destroy me when a new scene loads

        // get preferences
        mvol = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        evol = PlayerPrefs.GetFloat("EffectsVolume", 0.75f);
        
        if(sounds != null)
        createAudioSources(sounds.sounds, evol);     // create sources for effects

        if(playlist != null)
        createAudioSources(playlist.sounds, mvol);   // create sources for music

        bgmMusic = GetComponent<AudioSource>();

    }

    // create sources
    private void createAudioSources(C_Sound[] snd, float volume)
    {
        foreach (C_Sound s in snd)
        {   // loop through each music/effect
            s.source = gameObject.AddComponent<AudioSource>(); // create anew audio source(where the sound splays from in the world)
            s.source.clip = s.clip;     // the actual music/effect clip
            s.source.volume = s.volume * volume; // set volume based on parameter
            s.source.pitch = s.pitch;   // set the pitch
            s.source.loop = s.loop;     // should it loop
            s.source.outputAudioMixerGroup = s.audioMixerGroup;   // controlling volume of our sound using audio mixer
        }
    }

    // play sfx sound
    public void PlaySound(string name)
    {
        // here we get the Sound from our array with the name passed in the methods parameters
        C_Sound s = Array.Find(sounds.sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("Unable to play sound " + name);
            return;
        }
        //s.source.Play(); // play the sound
        s.source.PlayOneShot(s.clip); // use PlayOneShot so that audio play fully without interupt/replaced by next audio
    }

    public void StopSound(string name)
    {
        // here we get the Sound from our array with the name passed in the methods parameters
        C_Sound s = Array.Find(sounds.sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("Unable to play sound " + name);
            return;
        }
        //s.source.Play(); // play the sound
        s.source.Stop(); // use PlayOneShot so that audio play fully without interupt/replaced by next audio
    }

    void Update(){
        
    }

    // play music (randomly choose)
    public void PlayMusic()
    {
        if (shouldPlayMusic == false)
        {
            shouldPlayMusic = true;
            // pick a random song from our playlist
            currentPlayingIndex = UnityEngine.Random.Range(0, playlist.sounds.Length - 1);
            playlist.sounds[currentPlayingIndex].source.volume = playlist.sounds[0].volume * mvol; // set the volume
            playlist.sounds[currentPlayingIndex].source.Play(); // play it
        }
    }

    // play specific music
    public void PlayMusic2(string name, bool isNew){  // Parameters : name = BGM name to be played | isNew = TRUE (Will Play Audio From Start) FALSE (Will pause current audio and resumed when trigger again)
        C_Sound s = Array.Find(playlist.sounds, sound => sound.name == name);
        int i = Array.FindIndex(playlist.sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("Unable to play music " + name);
            return;
        }else{
            bgmMusic = s.source;
        }
        

        if (shouldPlayMusic == false)
        {
            shouldPlayMusic = true;
            currentPlayingIndex = i;
            s.source.Play(); // play it
        }

        if(name != playlist.sounds[currentPlayingIndex].name){
            StartCoroutine(FadeOutMusic(playlist.sounds[currentPlayingIndex].source, 0f, isNew));
            currentPlayingIndex = i;
            StartCoroutine(FadeInMusic(s.source, playlist.sounds[currentPlayingIndex].volume));
            s.source.Play(); // play it
        }else{ // if same, but vol 0, turn it up
            if(playlist.sounds[currentPlayingIndex].source.volume <= 0f){
                StartCoroutine(FadeInMusic(s.source, playlist.sounds[currentPlayingIndex].volume));
            }
        }
    }

    // stop music
    public void StopMusic()
    {
        if (shouldPlayMusic == true)
        {
            StopCoroutine("PlayMusic2Delay");
            StartCoroutine(FadeOutMusic(playlist.sounds[currentPlayingIndex].source, 0f, true));
            shouldPlayMusic = false;
            //currentPlayingIndex = 999; // reset playlist counter
        }
    }

   /*  NOTE: NOT NEED FOR NOW. WILL USE LATER ONCE FARIS APPROVED.
   void Update()
    {
        // if we are playing a track from the playlist && it has stopped playing
        if (currentPlayingIndex != 999 && !playlist[currentPlayingIndex].source.isPlaying)
        {
            currentPlayingIndex++; // set next index
            if (currentPlayingIndex >= playlist.Length)
            { //have we went too high
                currentPlayingIndex = 0; // reset list when max reached
            }
            playlist[currentPlayingIndex].source.Play(); // play that funky music
        }
    }
    */

    // get the song name
    public String getSongName()
    {
        return playlist.sounds[currentPlayingIndex].name;
    }

    // if the music volume change update all the audio sources
    public void musicVolumeChanged()
    {
        foreach (C_Sound m in playlist.sounds)
        {
            mvol = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
            m.source.volume = playlist.sounds[0].volume * mvol;
        }
    }

    //if the effects volume changed update the audio sources
    public void effectVolumeChanged()
    {
        evol = PlayerPrefs.GetFloat("EffectsVolume", 0.75f);
        foreach (C_Sound s in sounds.sounds)
        {
            s.source.volume = s.volume * evol;
        }
        sounds.sounds[0].source.Play(); // play an effect so user can her effect volume
    }

    /* ----------------------- Fade in and Fade Out functions when switching music -----------------------------------*/
    IEnumerator FadeOutMusic(AudioSource audioSource, float targetVolume, bool isNewBool){
        float currentTime = 0;
        float start = audioSource.volume;

        while(currentTime < .75f){
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / .75f);
            yield return null;
        }
        if(isNewBool){
            audioSource.Stop();
        }else{
            audioSource.Pause();
        }
        yield break;
    }

    IEnumerator FadeInMusic(AudioSource audioSource, float targetVolume){
        float currentTime = 0;
        float start = 0;

        while(currentTime < 1f){
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / .75f);
            yield return null;
        }
        yield break;
    }
    
}
