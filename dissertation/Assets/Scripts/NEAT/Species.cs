using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Species {
    [SerializeField]
    private List<Genome> genomes;
    [SerializeField]
    private List<Genome> childGenomes;
    [SerializeField]
    private List<Genome> selectedGenomes;
    [SerializeField]
    private int id;
    [SerializeField]
    private float avgFitness;
    [SerializeField]
    private float bestFitness;


    public int GetId()
    {

        return id;
    }

    public List<Genome> GetGenomes() {

        return genomes;
    }

    public void AddGenome(Genome genome)
    {
       // Debug.Log(id);
        genome.SetSpecies(id);
        genomes.Add(genome);
    }


    public List<Genome> GetSelectedGenomes()
    {

        return selectedGenomes;
    }

    public void AddSelectedGenome(Genome genome)
    {
        // Debug.Log(id);
        if (genome.GetSpeciesId() == id) {
            genomes.Add(genome);
        }
        Debug.Log("selected should be same species");
       
    }


    public List<Genome> GetChildren()
    {

        return childGenomes;
    }
    public void AddChildGenome(Genome genome)
    {
        //Debug.Log(id);
        genome.SetSpecies(id);
        childGenomes.Add(genome);
    }
    public Species(Genome g,int id,bool child) {
        this.id = id;
        genomes = new List<Genome>();
        childGenomes = new List<Genome>();
        selectedGenomes = new List<Genome>();
        g.SetSpecies(id);
        if (!child)
        {
           
            genomes.Add(g);
            
        }
        else {
            childGenomes.Add(g);
        }
       
    }
   

    public float CalcSharedFitness(Genome g) {
        return g.GetFitness() / (genomes.Count);

    }
    public bool CheckExtinct() {
        if (genomes.Count == 0) {
            return true;
        }
        return false;
    }
    public void RemoveGen(int generation)
    {
        for (int i = 0; i < genomes.Count;i++) {
            if (genomes[i].GetGeneration() != generation) {
                genomes.RemoveAt(i);
                i--;
            }

        }


    }
    //public void Empty() {

       // genomes.Clear();
   // }

    public void SetChildrenToSpecies() {
       
        genomes.Clear();
        //genomes = childGenomes;
        for (int i = 0; i < childGenomes.Count; i++) {
            genomes.Add( childGenomes[i]);

        }
        childGenomes.Clear();
      
        //Debug.Log("new size "+genomes.Count);
        

    }
	
	
}
