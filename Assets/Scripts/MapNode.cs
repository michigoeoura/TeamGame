using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MapNode : MonoBehaviour
{

    [Tooltip("���X�g�����Ǐ㉺���E4�����ڑ�������5�ȏ�ڑ�������_����")]
    [SerializeField]
    List<MapNode> connectedNodes;

    [SerializeField, ReadOnly]
    MapNode[] nodesOnDirection;

    // Start is called before the first frame update
    void Start()
    {
        // ���T�C�Y
        System.Array.Resize(ref nodesOnDirection, 4);
        foreach (var node in connectedNodes)
        {
            MoMath.XZDirection direction;
            float angle = AngleXZWithSign(Vector3.forward, node.transform.position - transform.position);
            direction = MoMath.DirectionMath.FromDegree(angle);
            switch (direction)
            {
                case MoMath.XZDirection.UP_ZPlus:
                    nodesOnDirection[(int)MoMath.XZDirection.UP_ZPlus] = node;
                    break;
                case MoMath.XZDirection.DOWN_ZMinus:
                    nodesOnDirection[(int)MoMath.XZDirection.DOWN_ZMinus] = node;
                    break;
                case MoMath.XZDirection.RIGHT_XPlus:
                    nodesOnDirection[(int)MoMath.XZDirection.RIGHT_XPlus] = node;
                    break;
                case MoMath.XZDirection.LEFT_XMinus:
                    nodesOnDirection[(int)MoMath.XZDirection.LEFT_XMinus] = node;
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool CanMove(MapNode to)
    {
        if (!connectedNodes.Contains(to)) { return false; }

        return true;
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {

        foreach (var node in connectedNodes)
        {
            //Gizmos.DrawLine(gameObject.transform.position, node.transform.position);

            // �m�[�h�Ԃ̐ڑ���\��
            Vector3 startPos = gameObject.transform.position; // �n�_
            Vector3 endPos = node.transform.position;               // �I�_
            var connetionLineThicness = 3;                                             // ����(Gizmos.DrawLine�ł͑�����ݒ�ł��Ȃ�)
            UnityEditor.Handles.DrawBezier(startPos, endPos, startPos, endPos, Color.black, null, connetionLineThicness);

            Vector3 center = (startPos + endPos) * 0.5f;
            Vector3 to = endPos - startPos;
            // ������\��(��������Direction����From+Direction�ɔ�΂��̂ł�������ƒZ���ł���)
            GizmoArrow(center, to.normalized * 0.3f, Color.black, 0.3f, 45.0f);
        }
    }

    private void OnDrawGizmosSelected()
    {

        DrawGizmoSolidArc();

        foreach (var node in connectedNodes)
        {
            //Gizmos.DrawLine(gameObject.transform.position, node.transform.position);

            // �m�[�h�Ԃ̐ڑ���\��
            Vector3 startPos = gameObject.transform.position; // �n�_
            Vector3 endPos = node.transform.position;               // �I�_
            var connetionLineThicness = 10;                                             // ����(Gizmos.DrawLine�ł͑�����ݒ�ł��Ȃ�)
            UnityEditor.Handles.DrawBezier(startPos, endPos, startPos, endPos, Color.black, null, connetionLineThicness);
            DrawGizmoConnectedNodeDirection(node);

        }

    }

    private void DrawGizmoSolidArc()
    {
        Color solidArcColor = Color.magenta;
        solidArcColor.a = 0.5f;

        // UnityEditor.Handles�̕`��F��ݒ�
        UnityEditor.Handles.color = solidArcColor;
        // �㉺���E(�Ƃ�����蓌����k?)�̐�`��\��
        UnityEditor.Handles.DrawSolidArc(transform.position, Vector3.up, (Vector3.forward + Vector3.left).normalized, 90.0f, 0.5f);

        solidArcColor = Color.green;
        solidArcColor.a = 0.5f;
        UnityEditor.Handles.color = solidArcColor;
        UnityEditor.Handles.DrawSolidArc(transform.position, Vector3.up, (Vector3.forward + Vector3.right).normalized, 90.0f, 0.5f);

        solidArcColor = Color.yellow;
        solidArcColor.a = 0.5f;
        UnityEditor.Handles.color = solidArcColor;
        UnityEditor.Handles.DrawSolidArc(transform.position, Vector3.up, (Vector3.back + Vector3.left).normalized, 90.0f, 0.5f);

        solidArcColor = Color.cyan;
        solidArcColor.a = 0.5f;
        UnityEditor.Handles.color = solidArcColor;
        UnityEditor.Handles.DrawSolidArc(transform.position, Vector3.up, (Vector3.back + Vector3.right).normalized, 90.0f, 0.5f);

        // �F��߂�
        UnityEditor.Handles.color = Color.white;
    }

    private void DrawGizmoConnectedNodeDirection(MapNode node)
    {
        // �m�[�h�Ԃ̐ڑ���\��
        float angle = AngleXZWithSign(Vector3.forward, node.transform.position - transform.position);
        if (angle < 0.0f) { angle += 360.0f; }


        MoMath.XZDirection direction;
        direction = MoMath.DirectionMath.FromDegree(angle);
        Color solidArcColor = Color.magenta;
        switch (direction)
        {
            case MoMath.XZDirection.UP_ZPlus:
                solidArcColor = Color.magenta;
                break;
            case MoMath.XZDirection.DOWN_ZMinus:
                solidArcColor = Color.cyan;
                break;
            case MoMath.XZDirection.RIGHT_XPlus:
                solidArcColor = Color.green;
                break;
            case MoMath.XZDirection.LEFT_XMinus:
                solidArcColor = Color.yellow;
                break;
        }
        solidArcColor.a = 0.5f;

        UnityEditor.Handles.color = solidArcColor;
        UnityEditor.Handles.DrawSolidDisc(node.transform.position, Vector3.up, 0.5f);
    }


    private static void GizmoArrow(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Gizmos.DrawRay(pos, direction);

        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
        Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
    }

    private static void GizmoArrow(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Gizmos.color = color;
        Gizmos.DrawRay(pos, direction);

        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
        Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
    }
#endif


    /// <summary>
    /// XZ����(����)�ł̕����x�N�g�����m�̊p�x�𕄍��t���ŕԂ��i�x�j
    ///�EXZ����(����)�Ɏˉe���ꂽ from ���� to �ւ̊p�x��Ԃ��B
    ///�Etransform.forward ����Ƃ���ƁA�E�����i���v���j�����A�������i�����v���j�����ƂȂ�B
    /// </summary>
    /// <param name="from">��̕����x�N�g��</param>
    /// <param name="to">�Ώۂ̕����x�N�g��</param>
    /// <returns>-180 <= t <= 180 [degree]</returns>
    private static float AngleXZWithSign(Vector3 from, Vector3 to)
    {
        Vector3 projFrom = from;
        Vector3 projTo = to;
        projFrom.y = projTo.y = 0;  //y���𖳎�����iXZ���ʂɎˉe����j
        float angle = Vector3.Angle(projFrom, projTo);
        float cross = CrossXZ(projFrom, projTo);
        return (cross != 0) ? angle * -Mathf.Sign(cross) : angle; //2D�O�ς̕����𔽓]����
    }

    /// <summary>
    /// XZ ����(����)�ł�2D�O�ς����߂�
    ///�Ea�~b = a1b3 - a3b1
    ///�E2D �I�Ɍv�Z����By���𖳎��������̂ƍl����B
    ///�E�O�� = 0 �̂Ƃ��A���x�N�g���͕��s�i0�܂���180�x�j�B
    ///�E�O�� > 0 �̂Ƃ��Atransform.forward ����ɂ���ƍ����B
    ///�E�O�� < 0 �̂Ƃ��Atransform.forward ����ɂ���ƉE���B
    ///��Y���ł̉�]�Ƃ͐������t�ɂȂ�̂Œ��ӁB
    /// </summary>
    /// <param name="a">��̕����x�N�g��</param>
    /// <param name="b">�Ώۂ̕����x�N�g��</param>
    /// <returns>2D�̊O��</returns>
    private static float CrossXZ(Vector3 a, Vector3 b)
    {
        return a.x * b.z - a.z * b.x;
    }

}
