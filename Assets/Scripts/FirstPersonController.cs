using UnityEngine;
using System.Collections;

public class FirstPersonController : MonoBehaviour {

  public float mouseSensitivityX = 1f;
  public float mouseSensitivityY = 1f;
  public float playerMoveSpeed = 3f;

  private Transform myCamera;
  private float verticalLookRotation;
  private Vector3 movement;

  private Rigidbody myRigidBody;

	void Start () {

    myCamera = Camera.main.transform;
    myRigidBody = this.GetComponent<Rigidbody>();
	}

	void Update () {

    // player rotation from looking around
    this.transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivityX);
    verticalLookRotation += (Input.GetAxis("Mouse Y") * mouseSensitivityY);
    // constrain the verticalLookRotation between -60 and 60
    verticalLookRotation = Mathf.Clamp(verticalLookRotation, -60, 60);
    // orient the camera to rotate around its local x-axis
    myCamera.localEulerAngles = Vector3.left * verticalLookRotation;

    // calculate movement vector
    Vector3 moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0,
                                        Input.GetAxisRaw("Vertical")).normalized;
    movement = moveDirection * playerMoveSpeed;
	}

  void FixedUpdate() {

    // spherical world, so move player in its local space
    Vector3 localMovement = this.transform.TransformDirection(movement) * Time.fixedDeltaTime;
    myRigidBody.MovePosition(myRigidBody.position + localMovement);
  }
}
