using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{

    public Sprite tanooki;
    public Sprite raccoon;
    public SpriteRenderer endScreen;
    public TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OpenScene(string name)
    {
        SceneManager.LoadScene(name);

        SceneManager.sceneLoaded += GameManager.instance.OnSceneLoaded;
    }

    public void EndScreen(string winner)
    {
        if (winner == "Tanooki")
        {
            endScreen.sprite = tanooki;
        }
        else
        {
            endScreen.sprite = raccoon;
        }
        text.text = "Congrats " + winner + "!";
    }
}
