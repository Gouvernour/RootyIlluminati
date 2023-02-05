using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    List<GameObject> players;
    List<GameObject> TeamTanooki;
    List<GameObject> TeamRaccon;
    List<c_Tree> RaccoonTrees;
    List<c_Tree> TanookiTrees;
    bool tanookiPlanting;
    static public ScoreManager instance;
    int TanookiScore= 0;
    int RaccoonScore = 0;

    void Start()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
        GameObject[] gameobjects = GameObject.FindGameObjectsWithTag("Respawn");
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddTree(Tree tree)
    {
        if(tanookiPlanting)
        {
            TanookiTrees.Add(new c_Tree());
            c_Tree newTree = RaccoonTrees[RaccoonTrees.Count - 1];
            newTree.tree = tree;
            newTree.team = Team.Tanooki;
            newTree.stage = TreeStage.One;
        }
        else
        {
            RaccoonTrees.Add(new c_Tree());
            c_Tree newTree = RaccoonTrees[RaccoonTrees.Count - 1];
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
        yield return new WaitForSeconds(2f);
        int tempScore = 0;
        foreach(c_Tree t in TanookiTrees)
        {
            int spriteIndex = 0;
            foreach(Sprite m_sprite in t.tree.sprites)
            {
                if(m_sprite == t.tree.sprite_renderer.sprite)
                {

                }
                spriteIndex++;
            }
        }
        TanookiScore = tempScore;
        tempScore = 0;
        foreach (c_Tree t in RaccoonTrees)
        {

        }
        RaccoonScore = tempScore;
        tempScore = 0;
    }
}
