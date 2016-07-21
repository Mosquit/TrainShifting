using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class InitialCreations : MonoBehaviour {

  GameObject napr;
  List<Vector3> track;
  int currentTrackPosition;
  int nextTrackPosition;
  bool at_destination = false;

  float speed = 0;
  float absSpeed = 0;
  const float speedInc = 0.005f;
  int direction;

  void determineDirection() {
    if (speed < 0) {
      direction = -1;
    } else  {
      direction = 1;
    }
    absSpeed = Math.Abs(speed);

  }

  void determineNextTrackPosition() {
    int expectedNext;
    if (direction == 1) {
      expectedNext = currentTrackPosition + direction;
    } else {
      expectedNext = currentTrackPosition;
      // currentTrackPosition = currentTrackPosition + 1;
    }

    if (expectedNext < 0) expectedNext = 0;
    if (expectedNext > track.Count - 1) expectedNext = track.Count -1;

    nextTrackPosition = expectedNext;
    if (currentTrackPosition == nextTrackPosition) {
      at_destination = true;
      Debug.Log("In target");
    } else {
      at_destination = false;
    }
  }

	// Use this for initialization
	void Start () {

       track = new List<Vector3>();
       currentTrackPosition = 0;
       nextTrackPosition = 0;
       speed = 0f;
       direction = 1;
        /*
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(0, 0.05F, 0);
        cube.transform.localScale = new Vector3(0.2F, 0.1F, 10);
        */
        var waypoints = GameObject.Find("Waypoint");
        Debug.Log(waypoints.transform.childCount);



        for(int i=0; i < waypoints.transform.childCount; i++) {

            //create sphere at object position, for debug purposes
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = waypoints.transform.GetChild(i).position;
            sphere.transform.localScale = new Vector3(0.2F, 0.2F, 0.2F);


            Debug.Log(waypoints.transform.GetChild(i));
            track.Add(waypoints.transform.GetChild(i).position);
            if (i == 0) continue; // skip first one as it would be accessed in next loop to create a pair

            //create a new track segment from previous and current position
            createTrackFrom2Waypoints(waypoints.transform.GetChild(i - 1).gameObject, waypoints.transform.GetChild(i).gameObject);


        }

        //OK, now let's create a testing Sphere in the first point
        napr = GameObject.Find("Naprava");

        // find nearest track point
        float minDist = float.MaxValue;
        // Vector3 cloasest = new Vector3(0,0,0);
        for (int i = 0; i < track.Count; i++) {

          float dist = Vector3.Distance(track[i], napr.transform.position);
          if(dist < minDist) {
            minDist = dist;
            // cloasest = track[i];
            currentTrackPosition = i;
          }
        }

        napr.transform.position = track[currentTrackPosition];
    }

    float calculateFullfillment(float exp, float real) {
      if (exp == 0f) return 0f;
      else return (real / exp);
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown("up"))
            speed += speedInc;
        if (Input.GetKeyDown("down")) {
            speed -= speedInc;
        }
        determineDirection();
        determineNextTrackPosition();

        Vector3 oldPos = napr.transform.position;
        Debug.Log("Current = " +currentTrackPosition+  ", Heading towards index " + nextTrackPosition);
        Vector3 headingTowards = track[nextTrackPosition];
        Vector3 newPos = Vector3.MoveTowards(oldPos, headingTowards, absSpeed);

        float movedDistance = Vector3.Distance(oldPos, newPos);
        Debug.Log("Moved distance = " +  movedDistance);

        float fullfill = calculateFullfillment(absSpeed, movedDistance);
        if(fullfill < 0.98f && fullfill > 0) {
          // we did not reached full step, close to next track point. Have to move more...
          currentTrackPosition += direction;
          determineNextTrackPosition();
          newPos = Vector3.MoveTowards(newPos, headingTowards, absSpeed - movedDistance);
          //
          // if(nextTrackPosition < track.Count - 1) {
          //   nextTrackPosition++;
          //   //TODO: this shall be in while cycle actually
          //    headingTowards = track[nextTrackPosition];
          //
          //
          // } else {
          //   // already in last segment
          //   at_destination = true;
          //   Debug.Log("At destination...");
          // }

        }

        napr.transform.position = newPos;
        //napr.transform.position = newPos;
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
        trk.transform.localScale = new Vector3(0.1F, 0.1F, Vector3.Distance(w1.transform.position, w2.transform.position));

        Vector3 relativePos = trk.transform.position - w2.transform.position;
        Quaternion newRotation = Quaternion.LookRotation(relativePos);

        trk.transform.rotation = newRotation;
        Debug.Log("Rotation is " + trk.transform.rotation);

    }
}
