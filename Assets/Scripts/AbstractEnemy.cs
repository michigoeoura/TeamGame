using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class AbstractEnemy : Unit
{

    [SerializeField]
    protected MoMath.XZDirection nowDirection;

    protected bool isAttacking = false;

    // Start is called before the first frame update
    new protected void Start()
    {
        base.Start();
        isAttacking = false;


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


}