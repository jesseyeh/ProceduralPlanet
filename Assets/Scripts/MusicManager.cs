using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {

  public AudioClip menuMusic;
  public AudioClip dayMusic;
  public AudioClip nightMusic;

  private void Start() {

    // AudioManager.instance.PlayMusic(menuMusic, 5);
  }

  void Update () {
	  if(Input.GetKeyDown(KeyCode.Space)) {
      AudioManager.instance.PlayMusic(nightMusic, 5);
    }
	}
}
