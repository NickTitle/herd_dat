using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour {

	public static float neighborDistance = 8.0f;
	public static float crowderDistance = 4.0f;

	public Animal 		parent;
	public List<Animal> children;
	public int depth = 0;

	public Rigidbody rigidBody;

	public Vector3 _previousVel;
	public Vector3 _vel;
	public bool canSeeParent;

	// Use this for initialization
	void Start () {
		rigidBody = GetComponent<Rigidbody> ();
		canSeeParent = false;
	}
	
	// Update is called once per frame
	void Update () {

		if (parent) {
			ParentedUpdate ();
		} else {
			OrphanUpdate ();
		}
		UniversalUpdate ();
	}

	void ParentedUpdate (){
		checkCanSeeParent ();
		moveTowardParent ();
		avoidCrowdByDepth ();
	}

	void OrphanUpdate() {
		moveWithHerd ();
		Animal found = AnimalBehavior.chooseParent(this);
		if (found) {
			SetParent (found);
		}
	}

	void UniversalUpdate(){
		faceForward ();
		addVel ();
	}

	void SetParent(Animal parentAnimal) {

		parent = parentAnimal;
		parentAnimal.children.Add (this);

		depth = parentAnimal.depth + 1;
		// steal a child if too many
		List<Animal> parentChildren = parentAnimal.children;
		if (parentChildren.Count > 3) {
			Animal stolen = parentChildren [Random.Range (0, parentChildren.Count - 1)];
			parentChildren.Remove (stolen);
			children.Add (stolen);
		}
	}

	void moveTowardParent() {
		_vel += scaledParentForce();
	}

	void moveWithHerd() {
		_vel += scaledHerdForce();
	}

	void avoidCrowdByDepth() {
		_vel += scaledCrowdForce() * 4;
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
		if (_vel.magnitude < 1)
			return;
		float diff = AnimalBehavior.angleBetweenVector3s(transform.forward, _vel);
//		float divisor = canSeeParent ? 50f : 25f;
		float divisor = 15f;
		if (Mathf.Abs(diff) > 2) {
			transform.eulerAngles = new Vector3 (0, (transform.eulerAngles.y + diff/divisor), 0);
		}
	}

	void addVel() {
		float velMag = _vel.magnitude;
		float maxVelMag = 75.0f;
		if (velMag > maxVelMag) {
			_vel *= (maxVelMag / velMag);
		}
			
		_vel *= 0.99f;

		transform.position += (_vel.magnitude / 250f * transform.forward);
	}

	void OnDrawGizmos() {
//		Gizmos.DrawWireSphere (transform.position, 3.0f);
		DrawConeThing ();
		DrawOrphanForces ();
	}

	Vector3 scaledParentForce() {
		Vector3 parentForce = AnimalBehavior.parentForce (this);
		return 10 * AnimalBehavior.scaleByDistance (parentForce, parentForce.magnitude, 100.0f, neighborDistance/2.0f);
	}

	Vector3 scaledHerdForce() {
		Vector3 herdForce = AnimalBehavior.superiorNeighborForce (this, neighborDistance);
		return AnimalBehavior.scaleByDistance (herdForce, herdForce.magnitude, neighborDistance, neighborDistance/2.0f);
	}

	Vector3 scaledCrowdForce() {
		Vector3 crowdForce = AnimalBehavior.superiorNeighborForce (this, crowderDistance);
		return -2 * AnimalBehavior.inverseScaleByDistance (crowdForce, crowdForce.magnitude, crowderDistance, 0f);
	}



	void DrawOrphanForces() {
//		Debug.DrawRay (transform.position + transform.right * -1, scaledParentForce(), Color.blue);
//		Debug.DrawRay (transform.position + transform.right, scaledHerdForce(), Color.green);
		Debug.DrawRay (transform.position, _vel/20.0f, Color.magenta);
		Debug.DrawRay (transform.position, transform.forward, Color.cyan);
		Debug.DrawRay (transform.position, scaledCrowdForce(), Color.red);

	}

	void DrawConeThing() {
		Vector3 forward = transform.TransformDirection (Vector3.forward)*3;
		Vector3 left = transform.TransformDirection (Vector3.left).normalized;
		Gizmos.DrawRay (transform.position, forward + left);
		Gizmos.DrawRay (transform.position, forward - left);
	}
}
