using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour {

    public Text viewText;

    private SaveLoad saveLoad;

	// Use this for initialization
	void Start () {
        saveLoad = new SaveLoad();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Save() {
        saveLoad.Save();

    }

    public void Load()
    {
        saveLoad.Load();

    }
    public void ViewBest() {
        if (viewText.text == "View Best")
        {
            viewText.text = "View All";

        }
        else {
            viewText.text = "View Best";
        }
       
        GameObject.Find("Main Camera").GetComponent<CameraScript>().ChangeView();


    }
    public void Restart()
    {

        SceneManager.LoadScene("Training");

    }
    public void Quit()
    {

        Application.Quit();

    }
}
