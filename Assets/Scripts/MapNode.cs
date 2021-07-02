using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MapNode : MonoBehaviour
{

    [Tooltip("リストだけど上下左右4方向接続だから5個以上接続しちゃダメよ")]
    [SerializeField]
    List<MapNode> connectedNodes;

    [SerializeField, ReadOnly]
    MapNode[] nodesOnDirection;

    // Start is called before the first frame update
    void Start()
    {
        // リサイズ
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

            // ノード間の接続を表示
            Vector3 startPos = gameObject.transform.position; // 始点
            Vector3 endPos = node.transform.position;               // 終点
            var connetionLineThicness = 3;                                             // 太さ(Gizmos.DrawLineでは太さを設定できない)
            UnityEditor.Handles.DrawBezier(startPos, endPos, startPos, endPos, Color.black, null, connetionLineThicness);

            Vector3 center = (startPos + endPos) * 0.5f;
            Vector3 to = endPos - startPos;
            // 方向を表示(第二引数はDirectionだがFrom+Directionに飛ばすのでこうすると短くできる)
            GizmoArrow(center, to.normalized * 0.3f, Color.black, 0.3f, 45.0f);
        }
    }

    private void OnDrawGizmosSelected()
    {

        DrawGizmoSolidArc();

        foreach (var node in connectedNodes)
        {
            //Gizmos.DrawLine(gameObject.transform.position, node.transform.position);

            // ノード間の接続を表示
            Vector3 startPos = gameObject.transform.position; // 始点
            Vector3 endPos = node.transform.position;               // 終点
            var connetionLineThicness = 10;                                             // 太さ(Gizmos.DrawLineでは太さを設定できない)
            UnityEditor.Handles.DrawBezier(startPos, endPos, startPos, endPos, Color.black, null, connetionLineThicness);
            DrawGizmoConnectedNodeDirection(node);

        }

    }

    private void DrawGizmoSolidArc()
    {
        Color solidArcColor = Color.magenta;
        solidArcColor.a = 0.5f;

        // UnityEditor.Handlesの描画色を設定
        UnityEditor.Handles.color = solidArcColor;
        // 上下左右(というより東西南北?)の扇形を表示
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

        // 色を戻す
        UnityEditor.Handles.color = Color.white;
    }

    private void DrawGizmoConnectedNodeDirection(MapNode node)
    {
        // ノード間の接続を表示
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
    /// XZ平面(床面)での方向ベクトル同士の角度を符号付きで返す（度）
    ///・XZ平面(床面)に射影された from から to への角度を返す。
    ///・transform.forward を基準とすると、右向き（時計回り）が正、左向き（反時計回り）が負となる。
    /// </summary>
    /// <param name="from">基準の方向ベクトル</param>
    /// <param name="to">対象の方向ベクトル</param>
    /// <returns>-180 <= t <= 180 [degree]</returns>
    private static float AngleXZWithSign(Vector3 from, Vector3 to)
    {
        Vector3 projFrom = from;
        Vector3 projTo = to;
        projFrom.y = projTo.y = 0;  //y軸を無視する（XZ平面に射影する）
        float angle = Vector3.Angle(projFrom, projTo);
        float cross = CrossXZ(projFrom, projTo);
        return (cross != 0) ? angle * -Mathf.Sign(cross) : angle; //2D外積の符号を反転する
    }

    /// <summary>
    /// XZ 平面(床面)での2D外積を求める
    ///・a×b = a1b3 - a3b1
    ///・2D 的に計算する。y軸を無視したものと考える。
    ///・外積 = 0 のとき、両ベクトルは平行（0または180度）。
    ///・外積 > 0 のとき、transform.forward を基準にすると左側。
    ///・外積 < 0 のとき、transform.forward を基準にすると右側。
    ///※Y軸での回転とは正負が逆になるので注意。
    /// </summary>
    /// <param name="a">基準の方向ベクトル</param>
    /// <param name="b">対象の方向ベクトル</param>
    /// <returns>2Dの外積</returns>
    private static float CrossXZ(Vector3 a, Vector3 b)
    {
        return a.x * b.z - a.z * b.x;
    }

}
