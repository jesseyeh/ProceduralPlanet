using UnityEngine;
using System.Collections;

public class GravityAttractor : MonoBehaviour {

  public float gravityModifier = -9.81f;

  // attact GravityBody entities
  public void Attract(Rigidbody body) {

    // get the direction
    Vector3 gravityUp = (body.position - this.transform.position).normalized;
    // orient the body
    body.rotation = Quaternion.FromToRotation(body.transform.up, gravityUp) * body.rotation;
    // apply downward force
    body.AddForce(gravityUp * gravityModifier);
  }
}
