using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Unit : MonoBehaviour
{

    [SerializeField]
    protected MapNode startNode;

    public MapNode nowNode { get; protected set; }

    [SerializeField, ReadOnly]
    protected Vector3 targetPos;
    [SerializeField]
    protected float moveTime;
    [SerializeField]
    private Timer moveTimer;

    protected bool isNowMove = false;

    protected MapNode targetNode;

    public static UnitListner unitListner;

    public bool isActEnd { get; protected set; } = false;

    public bool isAlive { get; protected set; } = true;

    // Start is called before the first frame update
    protected void Start()
    {
        transform.position = startNode.transform.position;
        nowNode = startNode;

        moveTimer = new Timer(moveTime);
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
        moveTimer.Reset();

    }


    protected void Move()
    {
        moveTimer.TimerUpdate();
        // todo 7/2現在思いつかないけどなんかマトモなのに改善のこと
        // Lerpで動かす
        //transform.position = Vector3.Slerp(transform.position, targetPos, speed * Time.deltaTime);
        Vector3 bezierControlPoint = Vector3.Lerp(nowNode.transform.position, targetPos, 0.5f);
        bezierControlPoint.y += 1.0f;
        transform.position = MoMath.LerpMath.QuadraticBeziercurve(nowNode.transform.position, bezierControlPoint, targetPos, moveTimer.GetProgressZeroToOne());

        if (moveTimer.GetProgress() >= 1.0f)
        {
            // todo このままではキャラが上下軸にめり込むのでキャラの分Y座標は上にする必要がある
            transform.position = targetNode.transform.position;
            nowNode = targetNode;
        }

    }

    protected void SetMoveTarget(MapNode to)
    {
        if (to == nowNode) { return; }
        // 現在ノードから対象ノードに移動可能なら移動開始
        targetNode = to;
        targetPos = targetNode.transform.position;
        isNowMove = true;
    }

    protected bool CanMove(MapNode from, MapNode to)
    {
        if (!from.CanMove(to)) { return false; }

        return true;
    }
    protected bool IsMoveEnd()
    {
        if (moveTimer.GetProgress() >= 1.0f)
        {
            return true;
        }
        return false;
    }


}
