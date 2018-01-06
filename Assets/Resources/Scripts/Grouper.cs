using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grouper : MonoBehaviour {
	private Object herdPrefab;

	public List<GameObject> herds;

	private Animal playerAnimal;

	void Start () {
		herdPrefab = Resources.Load ("Prefabs/AnimalGroup");
		for (int i = 0; i < 30; i++) {
			MakeHerd();
		}
		Util.PoissonPlace(herds, transform.position, 200f);
		playerAnimal = GameObject.FindGameObjectWithTag("Player").GetComponent<Animal>();
	}

	// Update is called once per frame
	void Update () {
		GroupBehavior.AssignNearbyGroupsToPlayer(playerAnimal, herds);
		GroupBehavior.FindParentsForIdleGroups(herds);
	}

	void MakeHerd () {
		GameObject newHerd = (GameObject)Instantiate (herdPrefab);
		herds.Add(newHerd);
	}

}
