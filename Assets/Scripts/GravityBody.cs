using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class GravityBody : MonoBehaviour {

  private GravityAttractor planet;
  private Rigidbody myRigidbody;
   
  void Awake() {

    planet = GameObject.FindGameObjectWithTag("Planet").GetComponent<GravityAttractor>();
    myRigidbody = this.GetComponent<Rigidbody>();
    // disable the rigidbody's native gravity and rotation
    myRigidbody.useGravity = false;
    myRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
  }

  void FixedUpdate() {
    planet.Attract(myRigidbody);
  }
}
