using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

  public static AudioManager instance;

  private AudioSource musicSource;
  private AudioSource crossfadeSource;

  public AudioClip menuTheme;
  public AudioClip dayTheme;
  public AudioClip nightTheme;

  private void Awake() {

    instance = this;

    GameObject newMusicSource = new GameObject("Music");
    musicSource = newMusicSource.AddComponent<AudioSource>();
    newMusicSource.transform.parent = this.transform;

    GameObject newCrossfadeSource = new GameObject("Crossfade");
    crossfadeSource = newCrossfadeSource.AddComponent<AudioSource>();
    newCrossfadeSource.transform.parent = this.transform;    

    PlayMusic(menuTheme, 5);
  }

  public void PlayMusic(AudioClip musicClip, float fadeDuration = 1) {
    
    musicSource.clip = musicClip;
    musicSource.Play();

    StartCoroutine(Crossfade(fadeDuration));
  }

  private IEnumerator Crossfade(float duration) {

    float percent = 0;

    while(percent < 1) {
      percent += Time.deltaTime * 1 / duration;
      musicSource.volume = Mathf.Lerp(0, 1, percent);

      yield return null;
    }
  }
}
