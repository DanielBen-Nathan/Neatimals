using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HUD : MonoBehaviour {

    private List<GameObject> agents;
    private float time = 0.0f;
    private int numSpecies = 0;
    private int lastGenNumSpecies = 0;
    private Neat neat;
    private float deltaTime;


    public Text lastGenText;
    public Text thisGenText;
    public Text generation;
    public Text timeText;
    public Text fpsText;
    public float updateBestTime = 1.0f;
    

    // Use this for initialization
    void Start () {
        agents = new List<GameObject>();
        //agents = GameObject.Find("Scripts").GetComponent<Generate>().GetAgents();
        for (int i = 0; i < GameObject.Find("Scripts").GetComponent<Generate>().GetAgents().Count; i++) {
            agents.Add(GameObject.Find("Scripts").GetComponent<Generate>().GetAgents()[i]);
        }
        neat = GetComponent<Neat>();
    }
	
	// Update is called once per frame
	void Update () {

        //Debug.Log(agents[0].GetComponent<Brain>().GetGenome());

        if (updateBestTime >= time) {
            float sum = 0.0f;
            for (int i = 0; i < agents.Count; i++)
            {
               
                float fitness = GetComponent<ImplementFunctions>().FitnessFunction(agents[i]);
                agents[i].GetComponent<Brain>().GetGenome().SetFitness(fitness);
                sum += fitness;
                

            }
            float avg = sum / agents.Count;
            agents.Sort((x, y) => x.GetComponent<Brain>().GetGenome().GetFitness().CompareTo(y.GetComponent<Brain>().GetGenome().GetFitness()));
            agents.Reverse();
            thisGenText.text = "Current Gen:\nAvg: "+avg.ToString("F2")+"\nBest: " + agents[0].GetComponent<Brain>().GetGenome().GetFitness().ToString("F2")+"\nno. species: "+numSpecies;
            time = 0.0f;

        }
        time += Time.deltaTime;
        SetTime(neat.maxTime - neat.GetTimer());


        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = "fps: "+Mathf.Ceil(fps).ToString();

    }

    public void SetLastGen(int gen,float best, float avg) {
        lastGenText.text= "Last Gen:  \nAvg: "+avg.ToString("F2")+"\nBest: "+best.ToString("F2")+"\nno. species: "+lastGenNumSpecies;
        generation.text = "Generation: " + gen;


    }
    public void SetSpeciesNo(int numSpecies)
    {
        lastGenNumSpecies = this.numSpecies;
        this.numSpecies = numSpecies;


    }
    public void SetGeneration(int gen) {
        generation.text = "Generation: " + gen;

    }
    public void SetTime(float time)
    {
        timeText.text = "Time Remaining: " + time.ToString("F2");

    }
}
