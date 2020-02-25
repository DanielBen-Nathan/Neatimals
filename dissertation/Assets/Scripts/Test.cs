using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    // Use this for initialization
    void Start() { } }
        /*
        GeneTracker gt = new GeneTracker();
        Genome g1 = new Genome(gt);
        
        NodeGene n1 = new NodeGene(NodeGene.NODETYPE.INPUT, gt.NewId(),0,false);
        g1.AddNodeGene(n1);
       
        NodeGene n2 = new NodeGene(NodeGene.NODETYPE.INPUT, gt.NewId(),0, false);
        g1.AddNodeGene(n2);

        NodeGene n3 = new NodeGene(NodeGene.NODETYPE.HIDDEN, gt.NewId(),1, false);
        g1.AddNodeGene(n3);

        NodeGene n4 = new NodeGene(NodeGene.NODETYPE.HIDDEN, gt.NewId(),1, false);
        g1.AddNodeGene(n4);

        NodeGene n5 = new NodeGene(NodeGene.NODETYPE.OUTPUT, gt.NewId(),2, false);
        g1.AddNodeGene(n5);

        for (int i = 0; i < g1.GetNodeGenes().Count; i++)
        {
            //Debug.Log(g1.GetNodeGenes()[i].GetId());

        }


        ConnectionGene c1 = new ConnectionGene(n1, n3, 0.5f, true, gt.Innovate());
        g1.AddConnectionGene(c1);

        ConnectionGene c2 = new ConnectionGene(n2, n4, 0.5f, true, gt.Innovate());
        g1.AddConnectionGene(c2);

        ConnectionGene c3 = new ConnectionGene(n3, n5, 0.5f, true, gt.Innovate());
        g1.AddConnectionGene(c3);

        ConnectionGene c4 = new ConnectionGene(n4, n5, 0.5f, true, gt.Innovate());
        g1.AddConnectionGene(c4);

        ConnectionGene c5 = new ConnectionGene(n1, n5, 0.5f, true, gt.Innovate());
        g1.AddConnectionGene(c5);

        for (int i = 0; i < g1.GetConnectionGenes().Count; i++)
        {
           // Debug.Log(g1.GetConnectionGenes()[i].GetInnovation());

        }
        Debug.Log(g1.ToString());

        Genome g2 = new Genome(gt);
        g2.AddNodeGene(n1);
        g2.AddNodeGene(n2);
        g2.AddNodeGene(n3);
        g2.AddNodeGene(n4);
        g2.AddNodeGene(n5);
       // g2.AddNodeGene(new NodeGene(NodeGene.NODETYPE.HIDDEN,1, gt.NewId(), 1));

        g2.AddConnectionGene(c1);
        g2.AddConnectionGene(c2);
        g2.AddConnectionGene(c3);
        g2.AddConnectionGene(c4);
        g2.AddConnectionGene(new ConnectionGene(n1, n5, 0.5f, true, gt.Innovate()));



        g2.SetFitness(0.1f);
        g1.SetFitness(0.5f);

        Crossover crossover = new Crossover();
        Genome g3 =crossover.CrossoverGenes(g1, g2,gt);
        for (int i = 0; i < g3.GetNodeGenes().Count; i++) {
            //Debug.Log(g3.GetNodeGenes()[i].GetId());

        }
        // g3.AddConnectionMutation();
        g3.AddNodeMutation();
        //g3.Mutate();
        for (int i = 0; i < g3.GetConnectionGenes().Count; i++)
        {
            Debug.Log(g3.GetConnectionGenes()[i].GetInnovation());

        }

        Debug.Log(g3.ToString());
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
*/