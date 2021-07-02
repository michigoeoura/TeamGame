using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnEnemy : AbstractEnemy
{
    [SerializeField]
    private float turnSpeed = 7;

    MoMath.XZDirection targetDirection;


    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        targetDirection = nowDirection;

        transform.rotation = Quaternion.LookRotation(MoMath.DirectionMath.FromDirection(targetDirection), Vector3.up);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Action()
    {
        Rotate();
        if (IsEndRotate()) { isActEnd = true; }
    }

    public override void WhenEndTurn()
    {
        //throw new System.NotImplementedException();
    }

    public override void WhenStartTurn()
    {
        targetDirection = MoMath.DirectionMath.Inverse(targetDirection);
    }

    private void Rotate()
    {
        Vector3 targetEuler = MoMath.DirectionMath.EulerFromDirection(targetDirection);
        Quaternion targetQuaternion = Quaternion.Euler(targetEuler);
        // Lerp�œ�����
        transform.rotation = Quaternion.Slerp(transform.rotation, targetQuaternion, turnSpeed * Time.deltaTime);
    }

    private bool IsEndRotate()
    {
        float angle = Vector3.Angle(transform.forward, MoMath.DirectionMath.FromDirection(targetDirection));

        if (angle < 1.0f) { return true; }
        return false;
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
