using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Util : MonoBehaviour
{

    public static float AngleDiff (Transform a, Transform b) {
        Vector3 flatObjForward = new Vector3 (a.transform.forward.x, 0, a.transform.forward.z).normalized;
        Vector3 posDiff = b.transform.position-a.transform.position;
        Vector3 flatPosDiff = new Vector3(posDiff.x, 0, posDiff.z);
		return Vector3.Angle(flatObjForward, flatPosDiff);
    }

    public static bool CheckFacing (Transform obj, Transform target, float cutoff_angle=30f) {
        float angle = AngleDiff(obj, target);
		bool angleCorrect = angle < cutoff_angle || angle > 360f-cutoff_angle;
		return angleCorrect;
    }

    public static void PoissonPlace (List<GameObject> arr, Vector3 center, float radius) {
        List<GameObject> placed = new List<GameObject>();
        foreach(GameObject _object in arr) {
            for (int j = 0; j < 30; ++j) {

                float angle = 2 * Mathf.PI * Random.value;
                float r = Random.value * radius; // See: http://stackoverflow.com/questions/9048095/create-random-number-within-an-annulus/9048443#9048443
                Vector3 ray = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * r;
                Vector3 testPos = center + ray;

                // Accept candidates if it's inside the rect and farther than 2 * radius to any existing sample.
                if (IsFarEnough(testPos, placed, radius)) {
                    _object.transform.position = testPos;
                    break;
                }
            }
        }
    }

    private static bool IsFarEnough(Vector3 position, List<GameObject> arr, float radius) {
        int i = 0;
		while (i<arr.Count) {
			if (Vector3.Distance(position, arr[i].transform.position) < radius) {
                return false;
            }
            i++;
		}
        return true;
    }

}

