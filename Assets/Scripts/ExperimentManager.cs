using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExperimentManager : MonoBehaviour
{
    public GameObject[] experiments;

    public List<GameObject> objs;
    private void Start()
    {
        ResetExperiment();
    }

    public void ResetExperiment()
    {
        foreach (var obj in objs)
        {
            var gameObject = obj;
            objs.Remove(gameObject);
            Destroy(gameObject);
        }
        objs.Clear();
        foreach (var obj in experiments)
        {
            GameObject objs = Instantiate(obj);
            objs.SetActive(false);
            this.objs.Add(objs);
        }
        ActiveExperiment(0);
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(0);
    }

    public void ActiveExperiment(int i)
    {
        foreach (var obj in objs)
        {
            obj.gameObject.SetActive(false);
        }
        objs[i].SetActive(true);
    }
}
