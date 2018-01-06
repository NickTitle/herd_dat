using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupBehavior : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {

	}

    public static void AssignParentGroupToGroup(AnimalGroup parentGroup, AnimalGroup group) {
        Animal newParent = parentGroup.herdAnimals[Random.Range(0, parentGroup.herdAnimals.Count)];
        AssignParentToGroup(newParent, group);
    }

    public static void AssignParentToGroup(Animal parent, AnimalGroup group) {
        group.currentState = AnimalGroup.GroupState.Stampeding;
        group.stateTimer = 600;
        group.parentGroup = parent.group;

        if (group.parentGroup){
            group.parentGroup.childGroups.Add(group);
        }
        group.leader.parent = parent;
        group.leader.depth = parent.depth + 1;
        foreach(Animal animal in group.herdAnimals) {
            animal.depth = group.leader.depth +1 ;
        }
    }

    public static void UnassignParentGroupForGroup(AnimalGroup group) {
        if (group.leader.parent.isPlayer) {
            foreach(AnimalGroup childGroup in group.childGroups) {
                AssignParentToGroup(group.leader.parent, childGroup);
            }
            group.childGroups.Clear();
        }
        else {
            group.parentGroup.childGroups.Remove(group);
            foreach(AnimalGroup childGroup in group.childGroups) {
                AssignParentToGroup(group.leader.parent, childGroup);
            }
        }

        group.leader.depth = 998;
        foreach(Animal animal in group.herdAnimals) {
            animal.depth = 999;
        }
        group.leader.parent = null;
        group.currentState = AnimalGroup.GroupState.Disenfranchised;
    }

    public static List<GameObject> NearbyGroups(Vector3 position, float radius, List<GameObject> groups, AnimalGroup.GroupState groupState) {
        List<GameObject> nearby = new List<GameObject>();

        foreach(GameObject groupObj in groups) {
            AnimalGroup group = groupObj.GetComponent<AnimalGroup>();
            if (group.currentState != groupState){
                continue;
            }
            float distance = Vector3.Distance(position, group.leader.transform.position);
            if (distance < radius) {
                nearby.Add(groupObj);
            }
        }
        return nearby;
    }

    public static void AssignNearbyGroupsToPlayer(Animal player, List<GameObject> groups) {
        List<GameObject> nearbyIdle = NearbyGroups(player.transform.position, 7f, groups, AnimalGroup.GroupState.Idle);
        foreach(GameObject groupObj in nearbyIdle){
            AnimalGroup group = groupObj.GetComponent<AnimalGroup>();
            AssignParentToGroup(player, group);
        }
    }

    public static void FindAFriend(AnimalGroup group, List<GameObject> groups) {
        List<GameObject> nearbyStampeding = NearbyGroups(group.leader.transform.position, 10f, groups, AnimalGroup.GroupState.Stampeding);
        List<Animal> animals = new List<Animal>();
        foreach(GameObject groupObj in nearbyStampeding) {
            AnimalGroup _group = groupObj.GetComponent<AnimalGroup>();
            if (group == _group) {
                continue;
            }
            animals.AddRange(_group.herdAnimals);
        }
        if (animals.Count == 0) {
            return;
        }
        AssignParentToGroup(animals[Random.Range(0, animals.Count)], group);
    }

    public static void FindParentsForIdleGroups(List<GameObject> groups) {
        foreach(GameObject groupObj in groups) {
            AnimalGroup group = groupObj.GetComponent<AnimalGroup>();
            if (group.currentState == AnimalGroup.GroupState.Idle) {
                FindAFriend(group, groups);
            }
        }
    }
}
