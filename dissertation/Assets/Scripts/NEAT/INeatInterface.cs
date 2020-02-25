using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INeatInterface  {
    void SetGenome(GameObject agent, Genome g);
    Genome GetGenome(GameObject agent);
    List<GameObject> GetAgents();
    float FitnessFunction(GameObject agent);
    void ResetEnviorment();
    void OnTimeReached();
    void ResetAgent(GameObject agent, int i);
    bool EarlyTermination(List<GameObject> agents,float time);
    bool Condition();
    void SetSpecies( GameObject agent);
    void Stats(int gen,float best, float avg);
    void StatsSpecies(int numSpecies);
    void SetGeneration(int generation);
    void OnSave();
    void OnLoad();
    void Write(int generation, float best, float avg,int cons, int nodes, int species);

}
