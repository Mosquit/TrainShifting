using UnityEngine;
using System.Collections;

public class InitialCreations : MonoBehaviour {

	// Use this for initialization
	void Start () {
        /*
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(0, 0.05F, 0);
        cube.transform.localScale = new Vector3(0.2F, 0.1F, 10);
        */
        var waypoints = GameObject.Find("Waypoint");
        Debug.Log(waypoints.transform.childCount);
        


        for(int i=0; i < waypoints.transform.childCount; i++) {

            //create sphere at object position, for debug purposes
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            cube.transform.position = waypoints.transform.GetChild(i).position;
            cube.transform.localScale = new Vector3(0.5F, 0.5F, 0.5F);


            Debug.Log(waypoints.transform.GetChild(i));
            if (i == 0) continue; // skip first one as it would be accessed in next loop to create a pair

            //create a new track segment from previous and current position 
            createTrackFrom2Waypoints(waypoints.transform.GetChild(i - 1).gameObject, waypoints.transform.GetChild(i).gameObject);


        }
        
        
        


    }

    // Update is called once per frame
    void Update () {
	
	}

    void createTrackFrom2Waypoints(GameObject w1, GameObject w2) {
        Debug.Log("creating a new track");
        GameObject trk = GameObject.CreatePrimitive(PrimitiveType.Cube);

        Debug.Log("W1 post " + w1.transform.position);
        Debug.Log("W2 post " + w2.transform.position);

        Vector3 newPosition = (w1.transform.position + w2.transform.position) / 2;
        Debug.Log("before correction " + newPosition);
        newPosition += new Vector3(0, 0, 0);
        Debug.Log("new position " + newPosition);
        trk.transform.position = newPosition;
        trk.transform.localScale = new Vector3(0.1F, 0.3F, Vector3.Distance(w1.transform.position, w2.transform.position));

        Vector3 relativePos = trk.transform.position - w2.transform.position;
        Quaternion newRotation = Quaternion.LookRotation(relativePos);
        
        trk.transform.rotation = newRotation;
        Debug.Log("Rotation is " + trk.transform.rotation);

    }
}
