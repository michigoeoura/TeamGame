using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    public Vector3 targetPos;
    private float speed = 7;

    private bool isNowMove = false;

    private MapNode targetNode;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        targetPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        // �{�^�����������Ƃ�
        if (Input.GetMouseButtonDown(0))
        {
            // �}�E�X���W���烁�C���J�����̃X�N���[���̉������ɔ�ԃ��C���쐬
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


            foreach (RaycastHit hit in Physics.RaycastAll(ray))
            {

                MapNode to = hit.collider.gameObject.GetComponent<MapNode>();
                // ���C�ɓ������R���C�_�[�̌��I�u�W�F�N�g��MapNode(�Ƃ��̔h���N���X)�R���|�[�l���g�������Ă����
                if (to)
                {
                    if (CanMove(nowNode, to))
                    {
                        targetNode = to;
                    }
                }
            }

        }


    }

    private void Move()
    {
        // todo 7/2���ݎv�����Ȃ����ǂȂ񂩃}�g���Ȃ̂ɉ��P�̂���
        // Lerp�œ�����
        transform.position = Vector3.Slerp(transform.position, targetPos, speed * Time.deltaTime);
    }

    private void DecideMove()
    {
        if (!targetNode) { return; }

        if (targetNode != nowNode)
        {
            // ���݃m�[�h����Ώۃm�[�h�Ɉړ��\�Ȃ�ړ��J�n
            targetPos = targetNode.transform.position;
            nowNode = targetNode;
            isNowMove = true;
        }
    }

    private bool CanMove(MapNode from, MapNode to)
    {
        if (!from.CanMove(to)) { return false; }

        return true;
    }

    private bool IsMoveEnd()
    {
        if (Vector3.Distance(transform.position, nowNode.transform.position) < 0.1f)
        {
            // todo ���̂܂܂ł̓L�������㉺���ɂ߂荞�ނ̂ŃL�����̕�Y���W�͏�ɂ���K�v������
            transform.position = nowNode.transform.position;
            return true;
        }
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

    public override void Action()
    {

        // �ړ�����A�ړ����邩�̔��f���ړ����͂��Ȃ����ړ����f���R�[�h��Ő�ɗ��ė~�����̂ł���ȏ�������
        if (!isNowMove)
        {
            DecideMove();
        }
        else
        {
            Move();

            if (IsMoveEnd())
            {
                isNowMove = false;
                isActEnd = true;



            }
        }

    }

    public override void WhenStartTurn()
    {
        isActEnd = false;
    }

    public override void WhenEndTurn()
    {
        List<AbstractEnemy> enemies = unitListner.GetEnemies();
        foreach (var enemy in enemies)
        {
            // �G���X�g�̂����^�[���I������0.2m�ȓ��ɋ���z
            // todo:����Ȕ���ŗǂ���ł����H�R���C�_�[�g���Ȃ��H
            if (Vector3.Distance(transform.position, enemy.transform.position) <= 0.2f)
            {
                GameObject eventObject = GameObject.Instantiate(unitListner.GetEventTemplate(MoGameEvent.eGameEvent.RemoveObject));
                EventRemoveUnit removeEvent = eventObject.GetComponent<EventRemoveUnit>();
                removeEvent.Initialize(enemy);
                unitListner.AddEvent(removeEvent);
            }
        }
    }
}
