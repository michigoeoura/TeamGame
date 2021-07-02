using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField, ReadOnly]
    List<MapNode> mapNodeList;

    [SerializeField, ReadOnly]
    List<AbstractEnemy> enemyList;

    [SerializeField, ReadOnly]
    Player player;


    bool isPlayerTurn = true;

    // Start is called before the first frame update
    void Start()
    {
        // ヒエラルキーでアクティブな全MapNodeクラス所有オブジェクトをリストに流す
        mapNodeList.AddRange(FindObjectsOfType<MapNode>());

        // 同Enemy
        enemyList.AddRange(FindObjectsOfType<AbstractEnemy>());

        // プレイヤー（これはリストじゃないけど）
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {

        if (isPlayerTurn)
        {
            player.Action();
            if (player.isActEnd)
            {
                ChangeTurn(true);
            }

        }
        else
        {
            bool isEnemyTurnEnd = true;
            foreach (var enemy in enemyList)
            {
                enemy.Action();
                // 敵の内どれか一体でも行動が終わっていなければ敵のターンは終了していない
                if (!enemy.isActEnd) { isEnemyTurnEnd = false; }
            }

            if (isEnemyTurnEnd)
            {
                ChangeTurn(false);
            }
        }
    }

    // ターンを変える、敵のターンに変える時はtoEnemyTurnをtrue
    void ChangeTurn(bool toEnemyTurn)
    {
        player.EndTurn();
        player.WhenEndTurn();
        if (toEnemyTurn)
        {
            // 全敵のターン開始時関数を呼ぶ
            foreach (var enemy in enemyList)
            {
                enemy.StartTurn();
                enemy.WhenStartTurn();
            }

            isPlayerTurn = false;
        }
        else
        {

            player.StartTurn();
            player.WhenStartTurn();

            // 全敵のターン終了時関数を呼ぶ
            foreach (var enemy in enemyList)
            {
                enemy.EndTurn();
                enemy.WhenEndTurn();
            }

            isPlayerTurn = true;
        }
    }

}
