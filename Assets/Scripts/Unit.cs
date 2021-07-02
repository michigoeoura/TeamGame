using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Unit : MonoBehaviour
{

    [SerializeField]
    protected MapNode startNode;

    protected MapNode nowNode;

    [SerializeField]
    protected static GameSceneManager manager;

    public bool isActEnd { get; protected set; } = false;

    // Start is called before the first frame update
    protected void Start()
    {
        transform.position = startNode.transform.position;
        nowNode = startNode;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public abstract void Action();
    public abstract void WhenStartTurn();
    public abstract void WhenEndTurn();

    public void StartTurn()
    {
        isActEnd = false;
    }
    public void EndTurn()
    {

    }
}
