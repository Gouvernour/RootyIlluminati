using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    Transform spawnPositionsParent;

    [HideInInspector] public List<GameObject> players = new List<GameObject>();

    public AnimatorController tanookiAnim;
    public AnimatorController raccoonAnim;

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
        if (SceneManager.GetActiveScene().name != "Menu")
        {
            spawnPositionsParent = GameObject.Find("SpawnPositions").transform;
        }
        else
        {
            GetComponent<PlayerInputManager>().DisableJoining();
        }
    }

    public void OnPlayerJoined(PlayerInput input)
    {
        players.Add(input.gameObject);
        DontDestroyOnLoad(input.gameObject);

        input.transform.position = spawnPositionsParent.GetChild(players.Count - 1).position;
        input.GetComponent<Movement>().spawnPoint = spawnPositionsParent.GetChild(players.Count - 1);

        if (players.Count % 2 == 0)
        {
            input.transform.GetChild(0).GetComponent<Animator>().runtimeAnimatorController = tanookiAnim;
            input.name = "Tanooki";
        }
        else
        {
            input.transform.GetChild(0).GetComponent<Animator>().runtimeAnimatorController = raccoonAnim;
            input.name = "Raccoon";
        }

        if (SceneManager.GetActiveScene().name == "PlayerSpawningTest")
        {
            input.GetComponent<Movement>().enabled = false;

        }
    }

    


    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "PlayerSpawningTest" || SceneManager.GetActiveScene().name == "Main")
        {
            if (SceneManager.GetActiveScene().name == "Main")
            {
                GetComponent<PlayerInputManager>().DisableJoining();
            }
            else
            {
                GetComponent<PlayerInputManager>().EnableJoining();
            }

            spawnPositionsParent = GameObject.Find("SpawnPositions").transform;
            for (int i = 0; i < players.Count; i++)
            {
                players[i].transform.position = spawnPositionsParent.GetChild(i).position;
                players[i].GetComponent<Movement>().spawnPoint = spawnPositionsParent.GetChild(i);

                players[i].GetComponent<Movement>().enabled = true;

            }
        }


        
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
