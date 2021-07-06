using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnEnemy : AbstractEnemy
{
    [SerializeField]
    private float turnSpeed = 7;

    Timer turnTimer;

    [SerializeField, ReadOnly]
    MoMath.XZDirection targetDirection;


    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        targetDirection = nowDirection;

        turnTimer = new Timer(turnSpeed);

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Action()
    {
        if (isAttacking)
        {
            Move();
            if (IsMoveEnd()) { isActEnd = true; }
        }
        else
        {
            Rotate();
            if (IsEndRotate()) { isActEnd = true; }
        }



    }

    public override void WhenEndTurn()
    {
        Player player = unitListner.GetPlayer();
        if (Vector3.Distance(transform.position, player.transform.position) <= 0.2f)
        {
            GameObject eventObject = GameObject.Instantiate(unitListner.GetEventTemplate(MoGameEvent.eGameEvent.RemoveObject));
            EventRemoveUnit removeEvent = eventObject.GetComponent<EventRemoveUnit>();
            removeEvent.Initialize(player);
            unitListner.AddEvent(removeEvent);
        }
    }

    public override void WhenStartTurn()
    {
        if (TryStartAttack()) { return; }
        targetDirection = MoMath.DirectionMath.Inverse(targetDirection);
    }

    private void Rotate()
    {
        turnTimer.TimerUpdate();
        Vector3 nowNodeEuler = MoMath.DirectionMath.EulerFromDirection(nowDirection);
        Quaternion nowNodeQuaternion = Quaternion.Euler(nowNodeEuler);
        Vector3 targetEuler = MoMath.DirectionMath.EulerFromDirection(targetDirection);
        Quaternion targetQuaternion = Quaternion.Euler(targetEuler);
        // Lerpで動かす
        transform.rotation = Quaternion.Slerp(nowNodeQuaternion, targetQuaternion, turnTimer.GetProgressZeroToOne());
    }

    private bool IsEndRotate()
    {
        float angle = Vector3.Angle(transform.forward, MoMath.DirectionMath.FromDirection(targetDirection));

        if (angle < 1.0f)
        {
            nowDirection = targetDirection;
            turnTimer.Reset();
            return true;
        }
        return false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // ノード間の接続を表示
        Vector3 startPos = gameObject.transform.position; // 始点
        Vector3 endPos = startNode.transform.position;               // 終点
        var connetionLineThicness = 10;                                             // 太さ(Gizmos.DrawLineでは太さを設定できない)
        UnityEditor.Handles.DrawBezier(startPos, endPos, startPos, endPos, Color.black, null, connetionLineThicness);

    }

#endif

}
