using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public AudioClip ambient;
    public AudioClip boss;
    public AudioClip highThreat;
    public AudioClip lowThreat;
    static MusicManager instance = null;
    private static AudioSource _audioSource;
    private static AudioSource _audioSource2;
    private static AudioClip _ambient;
    private static AudioClip _boss;
    private static AudioClip _highThreat;
    private static AudioClip _lowThreat;
    private static int _currentThreatLevel = -1;
    private void Awake(){
        if (instance != null){
            Destroy(this.gameObject);
            UpdateBgm();
        }
        else{
            instance = this;
            DontDestroyOnLoad(transform.gameObject);
            var audioSources = GetComponents<AudioSource>();
            _audioSource = audioSources[0];
            _audioSource2 = audioSources[1];
            _ambient = ambient;
            _boss = boss;
            _highThreat = highThreat;
            _lowThreat = lowThreat;
            UpdateBgm();

        }
    }

    public static void SilenceBgm(){
        Destroy(instance.transform.gameObject);
        instance = null;
    }

    public static void UpdateBgm(){
        var time = _audioSource.time;
        if (SceneManager.GetActiveScene().name != "MainScene"){
            SeamlessTrackSwitch(_ambient);
        }
        else{
            if (Data.board == null){
                return;
            }

            var threatLevel = Data.board.ThreatLevel();
            if (_currentThreatLevel == threatLevel){
                return;
            }

            _currentThreatLevel = threatLevel;
            switch (Data.board.ThreatLevel()){
                case 0:
                    SeamlessTrackSwitch(_lowThreat);
                    break;
                case 1:
                    SeamlessTrackSwitch(_highThreat);
                    break;
                case 2:
                    SeamlessTrackSwitch(_boss);
                    break;
            }
        }

    }

    private static void SeamlessTrackSwitch(AudioClip newClip){
        AudioSource active;
        AudioSource free;
        if (_audioSource.isPlaying){
            active = _audioSource;
            free = _audioSource2;
        }
        else{
            active = _audioSource2;
            free = _audioSource;
        }
        
        free.clip = newClip;
        free.Play();
        free.time = active.time;
        active.Stop();
    }
}