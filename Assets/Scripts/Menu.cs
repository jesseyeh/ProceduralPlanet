using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

  [Header("Holders")]
  public GameObject mainMenuHolder;
  public GameObject creditsHolder;
  public GameObject newGameMenu;

  [Header("UI")]
  public Image transition;
  public Camera menuCamera;

  [Header("References")]
  public Transform player;
  public Transform animalCompanion;
  public IcoMesh icoMesh;

  public void MainMenu() {

    mainMenuHolder.SetActive(true);
    newGameMenu.SetActive(false);
    creditsHolder.SetActive(false);
  }

  public void NewGame() {

    mainMenuHolder.SetActive(false);
    newGameMenu.SetActive(true);
  }

  public void Embark() {

    StartCoroutine(Transition(Color.clear, Color.white, 3));
    newGameMenu.SetActive(false);
    Cursor.visible = false;
    Instantiate(animalCompanion, player.position + Vector3.forward * 2, Quaternion.identity);
    AudioManager.instance.ChangeMusic(AudioManager.instance.nightTheme);
  }

  public void Credits() {

    mainMenuHolder.SetActive(false);
    creditsHolder.SetActive(true);
  }

  public void Quit() {

    Application.Quit();
  }

  private IEnumerator Transition(Color from, Color to, float duration) {
    
    float percent = 0;

    // fade from clear to white
    while(percent < 1) {
      percent += Time.deltaTime / duration;
      transition.color = Color.Lerp(from, to, percent);
      yield return null;
    }

    menuCamera.gameObject.SetActive(false);
    player.gameObject.SetActive(true);

    yield return new WaitForSeconds(1f);

    percent = 0;
    // fade from white to clear
    while(percent < 1) {
      percent += Time.deltaTime / duration;
      transition.color = Color.Lerp(to, from, percent);
      yield return null;
    }
  }

  public void SetSubdivisions(float value) {

    IcoMesh.instance.subdivisions = (int)value;
  }

  public void SetSmoothness(float value) {

    IcoMesh.instance.smoothness = (int)value;
  }
}
