using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventRemoveUnit : MoGameEvent.GameEventBase
{
    // �O������Z�b�g���Ȃ��Ƃ����Ȃ��̂�Public�A�������C���X�y�N�^�[����G�����ɂ͂����Ȃ��̂�ReadOnly
    [SerializeField, ReadOnly]
    public Unit removeObject;

    [Tooltip("�x�W�F�Ȑ��̐���_�i���͂��̓_�ɋ߂Â��j")]
    [SerializeField]
    Vector3 bezierControlPoint;

    private Vector3 removeStartPos;
    public Vector3 removeEndPos;

    private Timer timer;

    [SerializeField]
    private float removeTime;   // �ǂ����̂Ɋ|���鎞��

    public void Initialize(Unit targetUnit)
    {
        removeObject = targetUnit;
        removeStartPos = removeObject.transform.position;
        // �n�_�ƏI�_�̒��_�𑫂�
        bezierControlPoint += Vector3.Lerp(removeStartPos, removeEndPos, 0.5f);
    }

    // Start is called before the first frame update
    void Start()
    {
        timer = new Timer(removeTime);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public override void EventUpdate()
    {
        timer.TimerUpdate();
        float progress = timer.GetProgressZeroToOne();
        removeObject.transform.position = MoMath.LerpMath.QuadraticBeziercurve(removeStartPos, bezierControlPoint, removeEndPos, progress);
        if(progress>=1.0f)
        {
            eventListner.EraseUnit(removeObject);
            isEnd = true;
        }

    }
}
