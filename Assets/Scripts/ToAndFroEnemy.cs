using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToAndFroEnemy : AbstractTurnEnemy
{
    [SerializeField, ReadOnly]
    bool IsRotating = false;

    public override void Action()
    {
        if (isAttacking)
        {
            Move();
            if (IsMoveEnd()) { isActEnd = true; }
            return;
        }

        if (!IsRotating)
        {
            Move();
            if (IsMoveEnd())
            {
                if (nowNode.GetConnectedNode(nowDirection) == false)
                {
                    // 行き止まりなら反転開始してターン続行
                    targetDirection = MoMath.DirectionMath.Inverse(nowDirection);
                    IsRotating = true;
                }
                else
                {
                    // 行き止まりでなければターン終了
                    isActEnd = true;
                }
            }
        }
        else
        {
            Rotate();
            if (IsEndRotate())
            {
                IsRotating = false;
                isActEnd = true;
            }
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
        if (!TryStartAttack())
        {
            SetMoveTarget(nowNode.GetConnectedNode(nowDirection));
        }

    }

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
