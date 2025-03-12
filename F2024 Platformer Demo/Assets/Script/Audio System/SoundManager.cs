using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public SoundDataStorage sounds;
    [Header("Sound Controls")]

    [Range(0f, 100f)]
    [SerializeField] float startingMasterVolume = 100f;
    [Range(0f, 100f)]
    [SerializeField] float startingSfxVolume = 100f;
    [Range(0f, 100f)]
    [SerializeField] float startingMusicVolume = 100f;



    [Range(0f, 1f)]
    public static float currentMasterVolume, currentSfxVolume, currentMusicVolume;


    [Header("Audio Source Management")]
    [SerializeField] int sfxPoolSize = 3;
    [SerializeField] AudioSource musicAudioSource;
    [SerializeField] AudioSource audioSourcePrefab;



    private Queue<AudioSource> audioPool = new Queue<AudioSource>();
    List<repeatAudio> repeatingAudioList = new List<repeatAudio>();


    public class repeatAudio
    {
        public GameObject source;
        public string AudioID;
        public AudioSource affectedSource;
        public SoundDataStorage.SoundDataPack audioPack;
        public AudioClip clip;
        public repeatAudio(GameObject source, AudioSource affectedSource, SoundDataStorage.SoundDataPack audioPack, AudioClip clip, string AudioID)
        {
            this.source = source;
            this.affectedSource = affectedSource;
            this.audioPack = audioPack;
            this.clip = clip;
        }
    }


    private void Awake()
    {
        if (!instance) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        InitializeAudioPool();


        currentMasterVolume = startingMasterVolume / 100;
        currentSfxVolume = startingSfxVolume / 100;
        currentMusicVolume = startingMusicVolume / 100;
    }


    private void Update()
    {
        #region Repeating Audio System

        foreach (var item in repeatingAudioList)
        {
            if(item.affectedSource.gameObject.activeSelf && !item.clip && !item.affectedSource.isPlaying)
            {
                item.affectedSource.clip = item.audioPack.audioClips[Random.Range(0, item.audioPack.audioClips.Count)];
                item.affectedSource.Play();
            }
        }
        for(int i = 0; i < repeatingAudioList.Count; i++)
        {
            if (repeatingAudioList[i].source == null)
            {
                repeatingAudioList.RemoveAt(i);
                break;
            }
        }
        #endregion
    }


    /// <summary>
    ///  Used To Play a Specified SFX Sound Once
    /// </summary>
    /// <param name="volume"> float value from 0 - 1 float | volume is still affected by SFX and Master volume Controls</param>
    public void playSound(string categoryName, string soundName, float volume = 1, bool randomSound = true, int soundIndex = 0)
    {
        if(sounds == null)
        {
            Debug.LogWarning("There is no sound Library");
            return;
        }

        AudioClip clip = FindClip(categoryName,soundName,randomSound,soundIndex);

        if (clip == null)
        {
            Debug.LogWarning($"No Audio Clip found in {categoryName}.{soundName}");
            return;
        }

        if (instance.audioPool.Count > 0)
        {
            AudioSource audioSource = instance.audioPool.Dequeue();

            audioSource.gameObject.SetActive(true);
            audioSource.clip = clip;
            audioSource.volume = volume * currentSfxVolume * currentMasterVolume;
            audioSource.Play();

            StartCoroutine(ReturnAudioSourceToPool(audioSource, clip.length));
        }
        else
        { // This will Create a new audio Source and add it back to the Queue to be used again with the rest of the Pool
            Debug.Log("Audio Pool is Empty | Adding new Source");
            AudioSource newAudioSource = Instantiate(audioSourcePrefab, transform);
            newAudioSource.name = "SFX Audio " + sfxPoolSize + 1;
            newAudioSource.gameObject.SetActive(true);
            newAudioSource.clip = clip;
            newAudioSource.volume = volume * currentSfxVolume * currentMasterVolume;
            newAudioSource.Play();
            sfxPoolSize++;

            StartCoroutine(ReturnAudioSourceToPool(newAudioSource, clip.length));
        }
    }



    /// <summary>
    /// Creates a Dedicated AudioSource for Clip Use PlayRepeating() to play the audio
    /// </summary>
    /// <param name="identifier"> Used for PlayRepeating() and StopRepeating()</param>
    public void SetRepeatingAudio(GameObject obj, string categoryName, string soundName,string identifier, bool randomSound = true, int soundIndex = 0)
    {
        if(repeatingAudioList.Any(x => x.audioPack.name == soundName && x.source == obj))
        {
            Debug.LogWarning("Obj Already has a dedicated AudioSource for SoundPack");
            return;
        }

        AudioClip clip = null;
        SoundDataStorage.SoundDataPack soundPack = new SoundDataStorage.SoundDataPack();
        if(!randomSound) 
        {
            clip = FindClip(categoryName, soundName,randomSound,soundIndex);
        }
        else
        {
            try
            {
                soundPack = sounds.AudioList.First(x => x.categoryName == categoryName).sounds.First(y => y.name == soundName);
            }
            catch 
            {
                Debug.LogError($"Could not find Sound Data Pack for {categoryName}.{soundName}");
                return;
            }
        }

        AudioSource newSource = Instantiate(audioSourcePrefab, transform);
        newSource.name = obj.name + " Dedicated Repeating Audio";
        newSource.gameObject.SetActive(false);

        repeatingAudioList.Add(new repeatAudio(obj,newSource,soundPack,clip,identifier));

    }
    /// <summary>
    /// Start Playing the repeated Audio 
    /// </summary>
    /// <param name="identifier"> identifier set in SetRepeatingAudio()</param>
    /// <param name="volume"> float value from 0 - 1 float | volume is still affected by SFX and Master volume Controls</param>
    public static void PlayRepeating(GameObject obj, string identifier, float volume = 1f)
    {
        repeatAudio audioData;
        try
        {
            audioData = instance.repeatingAudioList.First(x => x.source == obj && x.AudioID == identifier);
        }
        catch 
        {
            Debug.LogWarning($"Could not Find Dedicated Audio for {obj.name}.{identifier}");
            return;
        }

        if (audioData.clip)
        {
            audioData.affectedSource.gameObject.SetActive(true);
            audioData.affectedSource.clip = audioData.clip;
            audioData.affectedSource.loop = true;

        }
        else
        {
            audioData.affectedSource.gameObject.SetActive(true);
            audioData.affectedSource.clip = audioData.audioPack.audioClips[Random.Range(0,audioData.audioPack.audioClips.Count)];
            audioData.affectedSource.loop = false;
        }

        audioData.affectedSource.volume = volume * currentSfxVolume * currentMasterVolume;
        audioData.affectedSource.Play();
    }
    /// <summary>
    /// Stop Playing the repeated Audio 
    /// </summary>
    /// <param name="identifier"> identifier set in SetRepeatingAudio()</param>
    public static void StopRepeating(GameObject obj, string identifier)
    {
        repeatAudio audioData;
        try
        {
            audioData = instance.repeatingAudioList.First(x => x.source == obj && x.AudioID == identifier);
        }
        catch
        {
            Debug.LogWarning($"Could not Find Dedicated Audio for {obj.name}.{identifier}");
            return;
        }

        audioData.affectedSource.Stop();
        audioData.affectedSource.gameObject.SetActive(false);

    }

    public static void DeleteRepeating(GameObject obj, string identifier)
    {
        repeatAudio audioData;
        try
        {
            audioData = instance.repeatingAudioList.First(x => x.source == obj && x.AudioID == identifier);
        }
        catch
        {
            Debug.LogWarning($"Could not Find Dedicated Audio for {obj.name}.{identifier}");
            return;
        }

        instance.repeatingAudioList.Remove(audioData);
    }

        IEnumerator ReturnAudioSourceToPool(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        source.gameObject.SetActive(false);
        audioPool.Enqueue(source);
    }

    // grabs either a random clip at specified location or a specific clip from the list
    AudioClip FindClip(string categoryName, string soundName, bool randomSound = true, int soundIndex = default)
    {
        AudioClip clip = null;

        if (randomSound)
        {
            SoundDataStorage.SoundDataPack selectedPack =
                instance.sounds.AudioList.First(x => x.categoryName == categoryName).sounds.FirstOrDefault(y => y.name == soundName);

            clip = selectedPack.audioClips[Random.Range(0, selectedPack.audioClips.Count)];

        }
        else
        {
            try
            {
                clip = instance.sounds.AudioList.First(x => x.categoryName == categoryName).sounds.FirstOrDefault(y => y.name == soundName).audioClips[soundIndex];
            }
            catch
            {
                Debug.LogWarning("sound Index is out of Range of array");
            }
        }
        return clip;
    }



    void InitializeAudioPool()
    {
        for (int i = 0; i < sfxPoolSize; i++)
        {
            AudioSource newAudioSource = Instantiate(audioSourcePrefab, transform);
            newAudioSource.name = "SFX Audio Source " + i+1;
            newAudioSource.gameObject.SetActive(false); // Initially inactive
            audioPool.Enqueue(newAudioSource);
        }
    }

}
