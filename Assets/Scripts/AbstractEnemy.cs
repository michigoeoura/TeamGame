using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class AbstractEnemy : Unit
{
    protected bool isAttacking = false;

    [SerializeField]
    protected MoMath.XZDirection nowDirection;

    [SerializeField]
    private float turnTime = 1;
    Timer turnTimer;
    [SerializeField, ReadOnly]
    protected MoMath.XZDirection targetDirection;


    // Start is called before the first frame update
    new protected void Start()
    {
        base.Start();
        isAttacking = false;

        targetDirection = nowDirection;

        turnTimer = new Timer(turnTime);

        transform.rotation = Quaternion.LookRotation(MoMath.DirectionMath.FromDirection(nowDirection), Vector3.up);

    }

    protected bool TryStartAttack()
    {
        MapNode forwardAdjecentNode = nowNode.GetConnectedNode(nowDirection);
        if (unitListner.GetPlayer().nowNode == forwardAdjecentNode)
        {
            if (CanMove(nowNode, forwardAdjecentNode))
            {
                isAttacking = true;
                SetMoveTarget(forwardAdjecentNode);
                return true;
            }
        }
        isAttacking = false;

        return false;
    }



    protected void Rotate()
    {
        turnTimer.TimerUpdate();
        Vector3 nowNodeEuler = MoMath.DirectionMath.EulerFromDirection(nowDirection);
        Quaternion nowNodeQuaternion = Quaternion.Euler(nowNodeEuler);
        Vector3 targetEuler = MoMath.DirectionMath.EulerFromDirection(targetDirection);
        Quaternion targetQuaternion = Quaternion.Euler(targetEuler);
        // Lerp‚Å“®‚©‚·
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
}