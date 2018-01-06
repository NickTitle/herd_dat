using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalBehavior : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {

	}

	public static Animal chooseParent (Animal animal) {
		List<Animal> nearby = neighbors (animal, 3.0f);
		Animal deepest = null;
		int i = 0;
		while (i< nearby.Count) {
			Animal _animal = nearby[i];
			if ( (animal == _animal) || (_animal.depth >= animal.depth) ) {
				i++;
				continue;
			}
			if (!deepest || _animal.depth > deepest.depth) {
				deepest = _animal;
			}
			i++;
		}
		return deepest;
	}

	public static bool canSeeParent (Animal animal) {
		Animal parent = animal.parent;
		float angle = Vector3.Angle(animal.transform.forward, parent.transform.position-animal.transform.position);

		bool close = Vector3.Distance (animal.transform.position, parent.transform.position) < 6.0f;
		bool angleCorrect = angle < 30f || angle > 270f;

		return ( close && angleCorrect );
	}

	public static List<Animal> neighbors(Animal animal, float radius) {
		Collider[] herdColliders = Physics.OverlapSphere (animal.transform.position, radius);
		List<Animal> nearby = new List<Animal>();

		int i = 0;
		while (i<herdColliders.Length) {
			Collider found = herdColliders[i];
			if ((found.tag == "aiAnimal" || found.tag == "Player") && (found.gameObject != animal.gameObject)) {
				Animal _animal = found.GetComponent<Animal> ();
				nearby.Add (_animal);
			}
			i++;
		}

		return nearby;
	}

	public static Vector3 parentForce(Animal animal) {
		return animal.parent.transform.position - animal.transform.position;
	}

	public static Vector3 neighborForce(Animal animal, float radius, bool parentsOnly=false, bool sameDepthOrLess=false) {
		Rigidbody rb = animal.rigidBody;
		List<Animal> _neighbors = neighbors (animal, radius);
		int divideBy = 0;

		if (_neighbors.Count == 0) {
			return Vector3.zero;
		}

		Vector3 center = Vector3.zero;
		foreach (Animal neighbor in _neighbors) {
			bool noKids = parentsOnly && neighbor.children.Count == 0;
			bool tooDeep = sameDepthOrLess && neighbor.depth > animal.depth;
			bool parent = neighbor == animal.parent;
			if (tooDeep) {
				continue;
			}
			center += neighbor.transform.position;
			divideBy += 1;
		}
		if (divideBy == 0) {
			return Vector3.zero;
		}
		center *= (1.0f/divideBy);
		return center-rb.position;
	}

	public static Vector3 velocityDirection (Animal animal) {
		if (animal._vel.magnitude < 0.5f) {
			return animal.transform.forward;
		}
		float angle = Mathf.Atan2 (animal._vel.x, animal._vel.z) * Mathf.Rad2Deg;
		return new Vector3 (0, angle, 0);
	}

	public static Vector3 scaleByDistance(Vector3 force, float distance, float maxDistance, float minDistance = 1.0f, bool inverse = false) {
		if (distance > maxDistance || distance < minDistance) {
			return Vector3.zero;
		}

		float effectiveDistance = maxDistance - distance;

		if (inverse) {
			return force * (1- ( distance/maxDistance));
		} else {
			return force * (1 - (effectiveDistance / maxDistance));
		}
	}

	public static Vector3 inverseScaleByDistance(Vector3 force, float distance, float maxDistance, float minDistance = 1.0f) {
		return scaleByDistance (force, distance, maxDistance, minDistance, true);
	}

	public static Vector3 superiorNeighborForce(Animal animal, float radius) {
		return neighborForce (animal, radius, false, true);
	}

	public static float angleBetweenVector3s(Vector3 vec1, Vector3 vec2)
	{
		var angle = Vector3.Angle(vec1, vec2); // calculate angle
		// assume the sign of the cross product's Y component:
		return angle * Mathf.Sign(Vector3.Cross(vec1, vec2).y);
	}
}
