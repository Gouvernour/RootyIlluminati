using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    Transform spawnPositionsParent;

    List<GameObject> players = new List<GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Main")
        {
            spawnPositionsParent = GameObject.Find("SpawnPositions").transform;

        }
    }

    public void OnPlayerJoined(PlayerInput input)
    {
        players.Add(input.gameObject);
        DontDestroyOnLoad(input.gameObject);
        
        
        if (SceneManager.GetActiveScene().name == "Main")
        {
            spawnPositionsParent = GameObject.Find("SpawnPositions").transform;

            input.transform.position = spawnPositionsParent.GetChild(players.Count-1).position;
            input.GetComponent<Movement>().spawnPoint = spawnPositionsParent.GetChild(players.Count - 1);
        }
    }

    public void StartGame(string name)
    {
        SceneManager.LoadScene(name);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //GetComponent<PlayerInputManager>().enabled = false;

        spawnPositionsParent = GameObject.Find("SpawnPositions").transform;

        for (int i = 0; i < players.Count; i++)
        {
            players[i].transform.position = spawnPositionsParent.GetChild(i).position;
            players[i].GetComponent<Movement>().spawnPoint = spawnPositionsParent.GetChild(i);
        }
    }

    private void OnDisable()
    {

        SceneManager.sceneLoaded -= OnSceneLoaded;

    }
}
