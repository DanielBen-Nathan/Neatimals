using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {
    private List<GameObject> agents;
    private float maxDistance = 0.0f;
    private int pop;
    private float time;
    private Vector3 originalPos;
    private GameObject following;
    private float gap;
    private bool moveBack = false;
    public float checkTime;
    public float speed;
    public int differencePow = 3;
    public bool tilted;
    public bool viewAll = true;

    // Use this for initialization
    void Start () {
        gap = GameObject.Find("Scripts").GetComponent<Generate>().GetGap();
        Neat neat = GameObject.Find("Scripts").GetComponent<Neat>();
        pop = neat.population;
        Orth();
        agents = GameObject.Find("Scripts").GetComponent<Generate>().GetAgents();
       
       
        following = agents[0];
    }
	
	// Update is called once per frame
	void Update () {
        if (viewAll)
        {

            //GetComponent<Rigidbody>().velocity = new Vector3(0,0, -maxVelocity*0.1f);
            if ((maxDistance > (pop * gap) / 200) && !moveBack)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(gap, (pop * gap) / 450.0f, -maxDistance + pop / 55), Mathf.Pow(Mathf.Abs(transform.position.z - following.transform.position.z), differencePow) * speed * Time.deltaTime);
            }

            if (moveBack)
            {
                transform.position = Vector3.MoveTowards(transform.position, originalPos, (Mathf.Pow(Mathf.Abs(transform.position.z - originalPos.z), 2.5f) + 2500) * speed * Time.deltaTime);
                if (transform.position.z >= originalPos.z)
                {
                    moveBack = false;
                }
            }



        }
        else {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(following.transform.position.x+7, 1.5f, following.transform.position.z-2.5f), Mathf.Pow(Mathf.Abs(transform.position.z - following.transform.position.z), differencePow*8) * (speed) *Time.deltaTime);
            moveBack = false;
        }
       
       
        if (time >= checkTime && !moveBack) {
            maxDistance = 0.0f;
            //agents.Sort((x, y) => x.GetComponent<Brain>().GetGenome().GetFitness().CompareTo(y.GetComponent<Brain>().GetGenome().GetFitness()));
            //agents.Reverse();

            for (int i = 0; i < agents.Count; i++) {

                // if (maxVelocity < Mathf.Abs(agents[i].GetComponent<Rigidbody>().velocity.z)) {
                // maxVelocity = Mathf.Abs(agents[i].GetComponent<Rigidbody>().velocity.z);
                // }
                if (maxDistance < Mathf.Abs(agents[i].transform.position.z) && !agents[i].GetComponent<Brain>().GetFallen()) {
                    maxDistance = Mathf.Abs(agents[i].transform.position.z);
                    following = agents[i];
                }
            }
            time = 0.0f;

        }
        time += Time.deltaTime;
		//if(Input.GetKeyDown())
	}

    public void Orth() {
       // Debug.Log((pop * gap));
        transform.position = new Vector3(gap, (pop*gap) / 450.0f,  -(pop * gap) / 400);
        originalPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        //transform.position = new Vector3(5, pop / 50,0);
        //originalPos = new Vector3(5, pop / 50, 0);

        GetComponent<Camera>().orthographicSize =  (pop*gap) / 325.0f;
        //transform.eulerAngles = new Vector3(0.25f + ((1.0f / (pop*gap)) * 50.0f), -90, 0);
        if (tilted)
        {
            transform.eulerAngles = new Vector3(0.27f, -90.08f, 0);
        }
        else {
            transform.eulerAngles = new Vector3(0.27f, -90.0f, 0);
        }
    }

    public void ResetPos() {
        //maxDistance = 0.0f;
        //transform.position = originalPos;
        moveBack = true;

    }
    public void SnapReset()
    {
        maxDistance = 0f;
        transform.position = new Vector3(originalPos.x, originalPos.y, originalPos.z);
       
    }
    public bool GetMoveBack() {
        if (!viewAll) {
            return false;
        }
        return moveBack;
    }
    public void ChangeView() {
        if (viewAll)
        {
            GetComponent<Camera>().orthographic = false;
            viewAll = false;
            checkTime += 2;
            return;
        }
        else {
            transform.position = new Vector3(originalPos.x, originalPos.y, originalPos.z);
            GetComponent<Camera>().orthographic = true;
            viewAll = true;
            checkTime -= 2;
        }
       

    }
}
