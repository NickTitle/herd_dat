using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimalGroup : MonoBehaviour
{

	private int maxAnimals;
	private Object animalPrefab;
	private Vector3 _pos;

	public int depth = 0;
	public Animal leader;
	public List<Animal> herdAnimals;

	public AnimalGroup parentGroup;
	public List<AnimalGroup> childGroups;

	public Vector3 groupVel;
	public int stateTimer = 0;

	public enum GroupState
	{
		Idle,
		Stampeding,
		Disenfranchised,

	};
	public GroupState currentState = GroupState.Idle;

	void Start ()
	{
		childGroups = new List<AnimalGroup>();
		_pos = transform.position;
		animalPrefab = Resources.Load ("Prefabs/Gazelle");
		maxAnimals = Random.Range (2, 9);
		MakeGroup();
	}

	// Update is called once per frame
	void Update ()
	{
		GetGroupVel ();
		CheckShouldChangeState();

	}

	void MakeGroup() {
		GameObject leaderObj = (GameObject)Instantiate (animalPrefab);
		leader = leaderObj.GetComponent<Animal> ();
		leader.depth = depth;
		leader.transform.parent = transform;
		leader.name = "Leader";
		for (int i = 0; i < maxAnimals; i++) {
			MakeAnimal();
		}
		List<GameObject> animalstoPlace = herdAnimals.ConvertAll(x => x.gameObject);
		animalstoPlace.Add(leaderObj);
		Util.PoissonPlace(animalstoPlace, transform.position, 20f);
	}

	void MakeAnimal () {
		GameObject newAnimal = (GameObject)Instantiate (animalPrefab);
		Animal animal = newAnimal.GetComponent<Animal> ();
		animal.parent = leader;
		animal.depth = depth + 1;
		animal.group = this;
		herdAnimals.Add (animal);
		animal.transform.parent = transform;
	}

	void GetGroupVel() {
		int count = 0;
		Vector3 _groupVel = Vector3.zero;
		foreach (Animal animal in herdAnimals){
			_groupVel += animal._vel;
			count += 1;
		}
		groupVel = (count > 0) ? (_groupVel / count) : _groupVel;
	}

	void CheckShouldChangeState () {
		if (stateTimer > 0) {
			stateTimer -= 1;
		}
		else {
			if (currentState == GroupState.Stampeding) {
				EndStampeding();
			}
			else if (currentState == GroupState.Disenfranchised) {
				currentState = GroupState.Idle;
			}
		}
	}

	void EndStampeding() {
		// currentState = GroupState.Disenfranchised;
		stateTimer = 600;
		// leader.parent = null;
		GroupBehavior.UnassignParentGroupForGroup(this);
		// leader.depth = 998;
		// foreach(Animal animal in herdAnimals) {
		// 	animal.depth = 999;
		// }
	}

}

