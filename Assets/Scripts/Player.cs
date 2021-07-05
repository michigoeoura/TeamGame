using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{


    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        targetPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        // ボタンを押したとき
        if (Input.GetMouseButtonDown(0))
        {
            // マウス座標からメインカメラのスクリーンの奥方向に飛ぶレイを作成
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


            foreach (RaycastHit hit in Physics.RaycastAll(ray))
            {

                MapNode to = hit.collider.gameObject.GetComponent<MapNode>();
                // レイに当ったコライダーの元オブジェクトがMapNode(とその派生クラス)コンポーネントを持っていれば
                if (to)
                {
                    if (CanMove(nowNode, to))
                    {
                        SetMoveTarget(to);
                    }
                }
            }

        }


    }


    private void DecideMove()
    {
        if (!targetNode) { return; }

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

    public override void Action()
    {

        // 移動する、移動するかの判断を移動中はしないかつ移動判断がコード上で先に来て欲しいのでこんな書き方に
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
            // 敵リストのうちターン終了時に0.2m以内に居る奴
            // todo:そんな判定で良いんですか？コライダー使えない？
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
