using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSentryEnemy : AbstractEnemy
{



    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
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



#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // �m�[�h�Ԃ̐ڑ���\��
        Vector3 startPos = gameObject.transform.position; // �n�_
        Vector3 endPos = startNode.transform.position;               // �I�_
        var connetionLineThicness = 10;                                             // ����(Gizmos.DrawLine�ł͑�����ݒ�ł��Ȃ�)
        UnityEditor.Handles.DrawBezier(startPos, endPos, startPos, endPos, Color.black, null, connetionLineThicness);

    }

#endif

}