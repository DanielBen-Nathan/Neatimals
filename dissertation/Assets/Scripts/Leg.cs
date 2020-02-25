using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg : MonoBehaviour {
    private float floorSensor = 0.0f;
    public bool checkNonFeetFloor = true;
    public bool foot = false;
    public float force;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /*void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Agent" || collision.gameObject.tag == "leg")
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
        }


    }*/

    public float ReadSensor() {

        return floorSensor;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "floor" || collision.gameObject.tag == "wall")
        {

            floorSensor = 1.0f;

            //if (GetComponentInParent<Brain>().disableOnLimbFall && (transform.childCount > 0 )) {
            if (GetComponentInParent<Brain>().disableOnLimbFall && !foot)
            {
                GetComponentInParent<Brain>().SetFallen(true);
                GetComponentInParent<Brain>().gameObject.GetComponent<Rigidbody>().isKinematic = true;
            }

        }


    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "floor" || collision.gameObject.tag == "wall")
        {
            floorSensor = 0.0f;
        }
    }

}
