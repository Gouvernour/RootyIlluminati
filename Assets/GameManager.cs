using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

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
            Destroy(this);
            return;
        }
    }

    public void OnPlayerJoined(PlayerInput input)
    {
        players.Add(input.gameObject);
        DontDestroyOnLoad(input.gameObject);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("RasmusCorneer");
    }
}
