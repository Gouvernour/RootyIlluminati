using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class c_Tree
{
    public TreeStage stage;
    public Team team;
    public Tree tree;
}
public enum TreeStage
{
    Five,
    Four,
    Three,
    Two,
    One

}
public enum Team
{
    Raccoon,
    Tanooki
}

public class ScoreManager : MonoBehaviour
{
    List<GameObject> players = new List<GameObject>();
    List<GameObject> TeamTanooki = new List<GameObject>();
    List<GameObject> TeamRaccon = new List<GameObject>();
    List<c_Tree> RaccoonTrees = new List<c_Tree>();
    List<c_Tree> TanookiTrees = new List<c_Tree>();
    public bool tanookiPlanting;
    static public ScoreManager instance;
    int TanookiScore= 0;
    int RaccoonScore = 0;
    int[] scoreValue = new int[5] {0, 1, 3, 5, 9};

    public float roundTime = 120;
    public TextMeshProUGUI Timer;
    float timeLeft;
    public TextMeshProUGUI TanookiScoreText;
    public TextMeshProUGUI RaccoonScoreText;


    void Start()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        GameObject[] gameobjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject Object in gameobjects)
        {
            players.Add(Object);
            if(Object.name == "Raccoon")
            {
                TeamRaccon.Add(Object);
            }else if(Object.name == "Tanooki")
            {
                TeamTanooki.Add(Object);
            }
        }
        StartCoroutine(ScoreUpdates());
        timeLeft = roundTime;
    }

    // Update is called once per frame
    void Update()
    {
        timeLeft -= Time.deltaTime;
        if(timeLeft <= 0)
        {
            if(TanookiScore == RaccoonScore)
            {
                Timer.text = "Sudden Death";
            }else
            {

                timeLeft = 0;
                SceneManager.LoadScene("EndScreen");

                SceneManager.sceneLoaded += GameManager.instance.OnSceneLoaded;
                GameObject obj = GameObject.Find("EndCanvas");
                if (TanookiScore > RaccoonScore)
                    obj.GetComponent<MenuScript>().text.text = "Tanookis!";
                else
                    obj.GetComponent<MenuScript>().text.text = "Racoons!";

            }
        }else
        {
            TanookiScoreText.text = "Tanooki score: " + TanookiScore;
            RaccoonScoreText.text = "Tanooki score: " + RaccoonScore;
            Timer.text = timeLeft.ToString();

        }
    }

    public void AddTree(Tree tree)
    {
        if(tanookiPlanting)
        {
            TanookiTrees.Add(new c_Tree());
            c_Tree newTree = TanookiTrees[RaccoonTrees.Count - 1];
            newTree.tree = tree;
            newTree.team = Team.Tanooki;
            newTree.stage = TreeStage.One;
        }
        else
        {
            RaccoonTrees.Add(new c_Tree());
            c_Tree newTree = RaccoonTrees[RaccoonTrees.Count - 1];
            newTree.tree = tree;
            newTree.team = Team.Tanooki;
            newTree.stage = TreeStage.One;
        }
    }

    public void TryPlant(GameObject player)
    {
        foreach (GameObject p in TeamTanooki)
        {
            if(player == p)
            {
                tanookiPlanting = true;
            }else
            {
                tanookiPlanting = false;
            }
        }
    }

    public void RemoveTree(Tree tree)
    {
        foreach(c_Tree t in TanookiTrees)
        {
            if(t.tree == tree)
            {
                TanookiTrees.Remove(t);
                return;
            }
        }
        foreach(c_Tree t in RaccoonTrees)
        {
            if(t.tree == tree)
            {
                RaccoonTrees.Remove(t);
                return;
            }
        }
    }


    IEnumerator ScoreUpdates()
    {
        yield return new WaitForSeconds(.5f);
        int tempScore = 0;
        foreach (c_Tree t in TanookiTrees)
        {
            int spriteIndex = 0;
            foreach (Sprite m_sprite in t.tree.sprites)
            {
                if (m_sprite == t.tree.sprite_renderer.sprite)
                {
                    tempScore += scoreValue[spriteIndex];
                    break;
                }
                spriteIndex++;
            }
        }
        TanookiScore = tempScore;
        tempScore = 0;
        foreach (c_Tree t in RaccoonTrees)
        {
            int spriteIndex = 0;
            foreach (Sprite m_sprite in t.tree.sprites)
            {
                if (m_sprite == t.tree.sprite_renderer.sprite)
                {
                    tempScore += scoreValue[spriteIndex];
                    break;
                }
                spriteIndex++;
            }
        }
        RaccoonScore = tempScore;
        print("Tanooki score = " + TanookiScore);
        print("Racoon score = " + RaccoonScore);
        tempScore = 0;
        StartCoroutine(ScoreUpdates());
    }
}
