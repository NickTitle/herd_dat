using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour {

	public static float neighborDistance = 7.0f;
	public static float crowderDistance = 4.0f;

	public Animal parent;
	public AnimalGroup group;
	public List<Animal> children;
	public int depth = 0;

	public Rigidbody rigidBody;

	public Vector3 _previousVel;
	public Vector3 _vel;
	public float velMag;
	public bool canSeeParent;

	public Animator animator;
	public bool isPlayer = false;

	// Use this for initialization
	void Start () {
		animator 	= GetComponent<Animator>();
		rigidBody = GetComponent<Rigidbody> ();
		canSeeParent = false;
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (parent) {
			ParentedUpdate ();
		} else {
			OrphanUpdate ();
		}
		UniversalUpdate ();
		animator.SetFloat("velMag", velMag);
	}

	void ParentedUpdate (){
		checkCanSeeParent ();
		moveTowardParent ();
		avoidCrowdByDepth ();
	}

	void OrphanUpdate() {
		moveWithHerd ();
	}

	void UniversalUpdate(){
		faceForward ();
		addVel ();
	}

	void moveTowardParent() {
		_vel += scaledParentForce()*4;
	}

	void moveWithHerd() {
		_vel += scaledHerdForce()*2;
	}

	void avoidCrowdByDepth() {
		_vel += scaledCrowdForce() * -3.5f;
	}

//	void avoidCrowd() {
//		Vector3 crowdForce = AnimalBehavior.neighborForce (this, crowderDistance);
//		Vector3 scaled = AnimalBehavior.inverseScaleByDistance (crowdForce, crowdForce.magnitude, crowderDistance, 0f);
//		_vel -= scaled*3;
//	}

	void checkCanSeeParent() {
		if (!this.parent)
			return;
		Vector3 _diff = this.parent.transform.position - this.transform.position;
		float dot = Vector3.Dot (_diff, this.transform.forward);
		canSeeParent = (dot >= 0);
	}

	void faceForward() {
		if (_vel.magnitude < 1 || isPlayer)
			return;
		float diff = AnimalBehavior.angleBetweenVector3s(transform.forward, _vel);
		float divisor = canSeeParent ? 15f : 10f;
		if (Mathf.Abs(diff) > 2) {
			transform.eulerAngles = new Vector3 (0, (transform.eulerAngles.y + diff/divisor), 0);
		}
	}

	void addVel() {
		if (isPlayer) {
			_vel = Vector3.zero;
			velMag = GetComponent<AnimalMover>().momentum.magnitude;
			return;
		}
		velMag = _vel.magnitude;
		float maxVelMag = 30.0f;
		if (velMag > maxVelMag) {
			_vel *= (maxVelMag / velMag);
		}

		_vel *= this.parent ? 0.975f : 0.99f;
		velMag = _vel.magnitude;

		Vector3 flatVel = new Vector3 (_vel.x, 0, _vel.z);

		transform.position += (flatVel / 100f);
	}

	Vector3 scaledParentForce() {
		Vector3 parentForce = AnimalBehavior.parentForce (this);
		return AnimalBehavior.scaleByDistance (parentForce, parentForce.magnitude, 100.0f, neighborDistance/2.0f);
	}

	Vector3 scaledHerdForce() {
		// if (group) {
		// 	return group.groupVel / 10f;
		// }
		Vector3 herdForce = AnimalBehavior.superiorNeighborForce (this, neighborDistance);
		return AnimalBehavior.scaleByDistance (herdForce, herdForce.magnitude, neighborDistance, neighborDistance);
	}

	Vector3 scaledCrowdForce() {
		Vector3 crowdForce = AnimalBehavior.superiorNeighborForce (this, crowderDistance);
		return AnimalBehavior.inverseScaleByDistance (crowdForce, crowdForce.magnitude, crowderDistance, 0f);
	}


	void OnDrawGizmos() {
		//		Gizmos.DrawWireSphere (transform.position, 3.0f);
		// DrawConeThing ();
		DrawForces ();
	}

	void DrawForces() {
		if (parent) {

			switch(depth) {
				case 1:
					Debug.DrawLine(transform.position, parent.transform.position, Color.red);
					break;
				case 2:
					Debug.DrawLine(transform.position, parent.transform.position, Color.yellow);
					break;
				case 3:
					Debug.DrawLine(transform.position, parent.transform.position, Color.green);
					break;
				case 4:
					Debug.DrawLine(transform.position, parent.transform.position, Color.blue);
					break;
				case 5:
					Debug.DrawLine(transform.position, parent.transform.position, Color.magenta);
					break;
				default:
					Debug.DrawLine(transform.position, parent.transform.position, Color.black);
					break;
			}
		}
		if (group) {
			// Debug.DrawRay (transform.position, group.groupVel, Color.green);
		}

		// Debug.DrawRay (transform.position, scaledCrowdForce(), Color.red);
		// Debug.DrawRay (transform.position, _vel/20.0f, Color.magenta);
		// Debug.DrawRay (transform.position, transform.forward, Color.cyan);


	}

	void DrawConeThing() {
		Vector3 forward = transform.TransformDirection (Vector3.forward)*3;
		Vector3 left = transform.TransformDirection (Vector3.left).normalized;
		Gizmos.DrawRay (transform.position, forward + left);
		Gizmos.DrawRay (transform.position, forward - left);
	}
}
