using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class AbstractTurnEnemy : AbstractEnemy
{


    [SerializeField]
    private float turnTime = 1;

    Timer turnTimer;


    [SerializeField, ReadOnly]
    protected MoMath.XZDirection targetDirection;

    // Start is called before the first frame update
    protected new void Start()
    {
        base.Start();

        targetDirection = nowDirection;

        turnTimer = new Timer(turnTime);

    }

    // Update is called once per frame
    void Update()
    {

    }

    protected void Rotate()
    {
        turnTimer.TimerUpdate();
        Vector3 nowNodeEuler = MoMath.DirectionMath.EulerFromDirection(nowDirection);
        Quaternion nowNodeQuaternion = Quaternion.Euler(nowNodeEuler);
        Vector3 targetEuler = MoMath.DirectionMath.EulerFromDirection(targetDirection);
        Quaternion targetQuaternion = Quaternion.Euler(targetEuler);
        // Lerpで動かす
        transform.rotation = Quaternion.Slerp(nowNodeQuaternion, targetQuaternion, turnTimer.GetProgressZeroToOne());
    }

    protected bool IsEndRotate()
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
