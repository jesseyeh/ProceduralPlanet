using UnityEngine;
using System.Collections;

public class Animal : MonoBehaviour {

  private void Start() {

    StartCoroutine(RandomMovement());
  }

  private IEnumerator RandomMovement() {

    // eight different directions of random movement
    while(true) {
      System.Random prng = new System.Random();
      int direction = prng.Next(0, 8);
      switch(direction) {
        case 0:
          this.transform.rotation = Quaternion.LookRotation(Vector3.back, Vector3.up);
          break;
        case 1:
          this.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
          break;
        case 2:
          this.transform.rotation = Quaternion.LookRotation(Vector3.left, Vector3.up);
          break;
        case 3:
          this.transform.rotation = Quaternion.LookRotation(Vector3.right, Vector3.up);
          break;
        case 4:
          this.transform.rotation = Quaternion.LookRotation(Vector3.back + Vector3.left, Vector3.up);
          break;
        case 5:
          this.transform.rotation = Quaternion.LookRotation(Vector3.back + Vector3.right, Vector3.up);
          break;
        case 6:
          this.transform.rotation = Quaternion.LookRotation(Vector3.forward + Vector3.left, Vector3.up);
          break;
        case 7:
          this.transform.rotation = Quaternion.LookRotation(Vector3.forward + Vector3.right, Vector3.up);
          break;
      }

      float percent = 0;
      while(percent < 1) {

        percent += Time.deltaTime / 0.5f;
        this.transform.position += this.transform.forward * Time.deltaTime * 2;
        yield return null;
      }

      yield return new WaitForSeconds(3.5f);
    }
  }
}
