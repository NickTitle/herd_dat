using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
public class AnimalMover : MonoBehaviour {

	private Joystick joystick;
	private Vector3 originalJoystickPosition;

	public Vector3 camJoyVec;
	public float   camJoyVecMag;

	public Vector3 momentum;
	public float maxMomentum = 30f;

	public Animal animal;

	// Use this for initialization
	void Start () {
		GameObject joystickObj  = GameObject.FindGameObjectWithTag("joystick") as GameObject;
		joystick = joystickObj.GetComponent<Joystick> ();
		originalJoystickPosition = joystick.transform.position;
		animal = transform.GetComponent<Animal> ();
	}

	void FixedUpdate () {
		ProcessInput();
	}

	void ProcessInput(){
		Vector3 joyPos = joystick.transform.position - originalJoystickPosition;

		Vector3 camForward = Camera.main.transform.TransformDirection(Vector3.forward);
		Vector3 flatCamForward = new Vector3(camForward.x, 0, camForward.z).normalized;

		Vector3 camRight = Camera.main.transform.TransformDirection(Vector3.right);
		Vector3 flatCamRight = new Vector3(camRight.x, 0, camRight.z).normalized;

		camJoyVec = (flatCamRight*joyPos.x + flatCamForward*joyPos.y)/5f;

		float camJoyVecMag = camJoyVec.magnitude;

		if (camJoyVecMag > 0.5f) {
			momentum += camJoyVec;
			transform.rotation = Quaternion.LookRotation(momentum, Vector3.up);
		} else {
			momentum *= 0.975f;
		}

		scaleMomentum();

		transform.position = transform.position + momentum/100;
		Vector3 forward = transform.TransformDirection (Vector3.forward)*3;
		// Debug.DrawLine (transform.position, transform.position+camJoyVec, Color.green);
	}

	void scaleMomentum() {
		if (momentum.magnitude > maxMomentum) {
			momentum = (maxMomentum/momentum.magnitude) * momentum;
		}
	}
}
