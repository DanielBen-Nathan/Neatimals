using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossover {

    public Genome CrossoverGenes(Genome genome1, Genome genome2,GeneTracker track) {
        Genome child = new Genome(track);
        List<NodeGene> newNodeGenes = new List<NodeGene>();

        //for (int i = 0; i < genome1.GetNodeGenes().Count; i++)
        //{
        //    commonNodeGenes.Add(genome1.GetNodeGenes()[i].Copy());

        //}
        //for (int i = 0; i < genome2.GetNodeGenes().Count; i++)
        //{
        //    if (!commonNodeGenes.Contains(genome2.GetNodeGenes()[i]))
        //    {
        //        commonNodeGenes.Add(genome2.GetNodeGenes()[i].Copy());

        //    }


        //}

        if (genome1.GetFitness() < genome2.GetFitness())
        {//swap so genome1 has the higher fitness
            Genome temp = genome2;
            genome2 = genome1;
            genome1 = temp;

        }
        //Debug.Log("g1 "+genome1+ "\ng2 " + genome2);
      
        child.SetLayers(genome1.GetLayers());
        /*
        for (int i = 0; i < genome1.GetNodeGenes().Count; i++) {
            for (int i2 = 0; i2 < genome2.GetNodeGenes().Count; i2++)
            {


            }
        }
        */

        for (int i = 0; i < genome1.GetNodeGenes().Count; i++)
        {
           
            newNodeGenes.Add(genome1.GetNodeGenes()[i].Copy());
            
            

        }
        
        for (int i = 0; i < genome2.GetNodeGenes().Count; i++)
        {
            if (newNodeGenes.Contains(genome2.GetNodeGenes()[i]))
            {
               
                if (Random.Range(0, 2) == 1)
                {
                    // newNodeGenes.RemoveAt(newNodeGenes.IndexOf(genome2.GetNodeGenes()[i]));
                    //newNodeGenes.Add(genome2.GetNodeGenes()[i].Copy());
                    newNodeGenes[newNodeGenes.IndexOf(genome2.GetNodeGenes()[i])].SetBias(genome2.GetNodeGenes()[i].GetBias());

                }
            }
            

        }
        for (int i = 0; i < genome1.GetNodeGenes().Count; i++)
        {
            child.AddNodeGene(newNodeGenes[i]);

        }

        List<ConnectionGene> newConnectionGenes=new List<ConnectionGene>();
        
        //if equal choose random
        for (int i = 0; i < genome1.GetConnectionGenes().Count; i++) {//copy all connections
            newConnectionGenes.Add(genome1.GetConnectionGenes()[i].Copy());
            newConnectionGenes[i].SetInNode(child.FindNode(genome1.GetConnectionGenes()[i].GetInNode().GetId()));
            newConnectionGenes[i].SetOutNode(child.FindNode(genome1.GetConnectionGenes()[i].GetOutNode().GetId()));

        }
        for (int i = 0; i < genome2.GetConnectionGenes().Count; i++)
        {
            //Debug.Log(newConnectionGenes.Contains(genome2.GetConnectionGenes()[i]));
            if (newConnectionGenes.Contains(genome2.GetConnectionGenes()[i])) {

                //Debug.Log(Random.Range(0, 2) );
                if (Random.Range(0, 2) == 1) {
                    //newConnectionGenes.RemoveAt(newConnectionGenes.IndexOf(genome2.GetConnectionGenes()[i]));
                    //newConnectionGenes.Add(genome2.GetConnectionGenes()[i].Copy());

                    //newConnectionGenes[i].SetWeight(genome2.GetConnectionGenes()[i].GetWeight());
                    //Debug.Log(newConnectionGenes[newConnectionGenes.IndexOf(genome2.GetConnectionGenes()[i])].GetWeight());
                    newConnectionGenes[newConnectionGenes.IndexOf(genome2.GetConnectionGenes()[i])].SetWeight(genome2.GetConnectionGenes()[i].GetWeight());
                   
                }
               

            }
           
           


        }
        for (int i = 0; i < genome1.GetConnectionGenes().Count; i++)
        {
            child.AddConnectionGene(newConnectionGenes[i]);

        }
        //Debug.Log("child "+child);
        return child;

    }
}
