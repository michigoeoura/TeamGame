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
        // �q�G�����L�[�ŃA�N�e�B�u�ȑSMapNode�N���X���L�I�u�W�F�N�g�����X�g�ɗ���
        mapNodeList.AddRange(FindObjectsOfType<MapNode>());

        // ��Enemy
        enemyList.AddRange(FindObjectsOfType<AbstractEnemy>());

        // �v���C���[�i����̓��X�g����Ȃ����ǁj
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
                // �G�̓��ǂꂩ��̂ł��s�����I����Ă��Ȃ���ΓG�̃^�[���͏I�����Ă��Ȃ�
                if (!enemy.isActEnd) { isEnemyTurnEnd = false; }
            }

            if (isEnemyTurnEnd)
            {
                ChangeTurn(false);
            }
        }
    }

    // �^�[����ς���A�G�̃^�[���ɕς��鎞��toEnemyTurn��true
    void ChangeTurn(bool toEnemyTurn)
    {
        player.EndTurn();
        player.WhenEndTurn();
        if (toEnemyTurn)
        {
            // �S�G�̃^�[���J�n���֐����Ă�
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

            // �S�G�̃^�[���I�����֐����Ă�
            foreach (var enemy in enemyList)
            {
                enemy.EndTurn();
                enemy.WhenEndTurn();
            }

            isPlayerTurn = true;
        }
    }

}
