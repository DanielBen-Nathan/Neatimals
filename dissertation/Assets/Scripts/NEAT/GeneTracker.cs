using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GeneTracker{
    [SerializeField]
    private  int id = 0;
    [SerializeField]
    private  int innovation = 0;
    [SerializeField]
    private int speciesId = 0;

    public int getInnovation() {

        return innovation;
    }

    public int Innovate()//connections ids
    {


        return innovation++;
    }
    public int NewId()//node ids
    {
        
        return id++;

    }
    public int NewSpeciesId()//node ids
    {

        return ++speciesId;

    }

    public void SetId(int id) {
        this.id = id;

    }

}
