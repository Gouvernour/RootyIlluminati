
using System.Collections.Generic;
using UnityEngine.Animations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    Transform spawnPositionsParent;

    [HideInInspector] public List<GameObject> players = new List<GameObject>();

    public GameObject tanookiAnim;
    public GameObject raccoonAnim;

    public string winner;

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
            GameObject g = Instantiate(tanookiAnim, input.transform);
            input.GetComponent<Movement>().anim = g.GetComponent<Animator>();
            input.name = "Tanooki";
        }
        else
        {
            GameObject g = Instantiate(raccoonAnim, input.transform);
            input.GetComponent<Movement>().anim = g.GetComponent<Animator>();
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
                ScoreManager.instance.SetPlayers();
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
        else if (SceneManager.GetActiveScene().name == "EndScreen")
        {
            MenuScript menu = GameObject.Find("Canvas").GetComponent<MenuScript>();

            menu.EndScreen(winner);
        }


        
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
