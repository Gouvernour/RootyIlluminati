using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    List<GameObject> players;
    List<GameObject> TeamTanooki;
    List<GameObject> TeamRaccon;

    void Start()
    {
        GameObject[] gameobjects = GameObject.FindGameObjectsWithTag("Respawn");
        foreach (GameObject Object in gameobjects)
        {
            players.Add(Object);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
