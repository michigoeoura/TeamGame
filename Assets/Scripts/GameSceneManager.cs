using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// todo �����Ɉ�a��
// Unit�ɑ΂��郊�X�i�[�Ƃ��ẴQ�[���}�l�[�W���[
// C#�̃C���^�[�t�F�[�X:https://ufcpp.net/study/csharp/oo_interface.html
public interface UnitListner
{
    // ���������Ȃ��Ă�public virtual�ɂȂ�񂾂��ǎ�ȂƋC�����̖��ł������܂�
    public void AddEvent(MoGameEvent.GameEventBase gameEvent);

    public List<AbstractEnemy> GetEnemies();

    public GameObject GetEventTemplate(MoGameEvent.eGameEvent name);
}

// GameEvent�ɑ΂��郊�X�i�[�Ƃ��ẴQ�[���}�l�[�W���[
public interface EventListner
{
    public void AddEvent(MoGameEvent.GameEventBase gameEvent);
    public void EraseUnit(Unit removeUnit);
    public GameObject GetEventTemplate(MoGameEvent.eGameEvent name);
}

public class GameSceneManager : MonoBehaviour, UnitListner, EventListner
{
    [SerializeField, ReadOnly]
    List<MapNode> mapNodeList;

    [SerializeField, ReadOnly]
    List<AbstractEnemy> enemyList;

    [SerializeField, ReadOnly]
    Player player;

    [SerializeField, ReadOnly]
    List<MoGameEvent.GameEventBase> eventList;  // �C�x���g���s�������t���[���Ɍׂ�̂ŃX�^�b�N�ł͂Ȃ����X�g��

    [SerializeField, ReadOnly]
    Dictionary<MoGameEvent.eGameEvent, GameObject> eventTemplates;

    SceneStateBase state = null;


    // Start is called before the first frame update
    void Start()
    {
        // �q�G�����L�[�ŃA�N�e�B�u�ȑSMapNode�N���X���L�I�u�W�F�N�g�����X�g�ɗ���
        mapNodeList.AddRange(FindObjectsOfType<MapNode>());

        // ��Enemy
        enemyList.AddRange(FindObjectsOfType<AbstractEnemy>());

        // �v���C���[�i����̓��X�g����Ȃ����ǁj
        player = FindObjectOfType<Player>();

        // Unit�N���X�̃}�l�[�W���[�i�N���X�ϐ��j�Ɏ������Z�b�g
        Unit.unitListner = this;

        // ���Q�[���C�x���g
        MoGameEvent.GameEventBase.eventListner = this;

        eventTemplates = new Dictionary<MoGameEvent.eGameEvent, GameObject>();
        setEventTemplates();

        state = new PlayerTurn(this);
    }

    // Update is called once per frame
    void Update()
    {
    }


    void FixedUpdate()
    {
        state.Update();


    }

    void setEventTemplates()
    {
        List<MoGameEvent.GameEventBase> events = new List<MoGameEvent.GameEventBase>();
        events.AddRange(FindObjectsOfType<MoGameEvent.GameEventBase>());
        foreach (var gameEvent in events)
        {
            GameObject eventObject = gameEvent.gameObject;

            MoGameEvent.eGameEvent result;
            if (System.Enum.TryParse(eventObject.name, out result))
            {
                eventTemplates.Add(result, eventObject);
            }

        }
    }

    public void AddEvent(MoGameEvent.GameEventBase gameEvent)
    {
        eventList.Add(gameEvent);
    }

    public void EraseUnit(Unit removeUnit)
    {
        if (removeUnit == player)
        {
            player.gameObject.SetActive(false);
            return;
        }

        // as���Z�q�A�L���X�g�����݂ă_���Ȃ�null��Ԃ�
        AbstractEnemy enemy = removeUnit as AbstractEnemy;
        if (enemy)
        {
            enemy.gameObject.SetActive(false);
            enemyList.Remove(enemy);
        }
    }

    public GameObject GetEventTemplate(MoGameEvent.eGameEvent name)
    {
        GameObject result;

        // �L�[����l���擾�ł��邩�����ėL����bool�ŕԂ��A�l��value�ɕԂ��Ă���
        if (eventTemplates.TryGetValue(name, out result))
        {
            return result;
        }

        return null;
    }

    public List<AbstractEnemy> GetEnemies()
    {
        return enemyList;
    }


    // �����N���X��private�A�N�Z�X�\
    abstract class SceneStateBase
    {
        public SceneStateBase(GameSceneManager gameSceneManager)
        {
            manager = gameSceneManager;
        }
        protected GameSceneManager manager;
        abstract public void Update();

    }

    class PlayerTurn : SceneStateBase
    {
        public PlayerTurn(GameSceneManager gameSceneManager) : base(gameSceneManager)
        { }

        public override void Update()
        {
            bool endTurn = false;

            // �V�[���C�x���g���񂷊֌W�Ńv���C���[�s����ɍs�����I������(�^�[�����I��������)�����������āA����ꏏ�ɂ����1�t���[���x�����Ăł���Ȏ���(�L��֥`)
            if (manager.player.isAlive)
            {
                manager.player.Action();
            }

            if (manager.player.isActEnd)
            {
                endTurn = true;
            }

            if (endTurn) { ChangeTurnToEventProc(); }
        }

        void ChangeTurnToEventProc()
        {

            manager.player.EndTurn();
            manager.player.WhenEndTurn();

            manager.state = new EventProcTurn(manager, false);
        }
    }

    class EnemyTurn : SceneStateBase
    {
        public EnemyTurn(GameSceneManager gameSceneManager) : base(gameSceneManager)
        { }

        public override void Update()
        {
            bool isEnemyTurnEnd = true;

            foreach (var enemy in manager.enemyList)
            {
                if (enemy.isAlive) { enemy.Action(); }
                // �G�̓��ǂꂩ��̂ł��s�����I����Ă��Ȃ���ΓG�̃^�[���͏I�����Ă��Ȃ�
                if (!enemy.isActEnd) { isEnemyTurnEnd = false; }
            }


            if (isEnemyTurnEnd)
            {
                ChangeToEventProc();
            }

        }

        void ChangeToEventProc()
        {

            // �S�G�̃^�[���I�����֐����Ă�
            foreach (var enemy in manager.enemyList)
            {
                enemy.EndTurn();
                enemy.WhenEndTurn();
            }

            manager.state = new EventProcTurn(manager, true);
        }
    }

    class EventProcTurn : SceneStateBase
    {
        bool toPlayerTurn = false;
        public EventProcTurn(GameSceneManager gameSceneManager, bool bePlayerTurn) : base(gameSceneManager)
        {
            toPlayerTurn = true;
        }

        public override void Update()
        {
            bool endTurn = true;

            foreach (var gameEvent in manager.eventList)
            {
                // �C�x���g���s
                gameEvent.EventUpdate();
                if (gameEvent.isEnd)
                {
                    gameEvent.gameObject.SetActive(false);
                }

            }

            // ����ς�foreach���ŗv�f�폜�͏o���Ȃ��̂�RemoveAll�őΉ�(�����_��)
            manager.eventList.RemoveAll(gameEvent => gameEvent.isEnd);

            // ���̃^�C�~���O�ŃC�x���g���X�g�ɉ��������Ă���Ȃ�C�x���g�I�ɂ̓^�[�����I���������Ȃ�
            if (manager.eventList.Count > 0)
            {
                endTurn = false;
            }

            if (endTurn)
            {
                ChangeTurn();
            }
        }


        void ChangeTurn()
        {
            if (toPlayerTurn)
            {
                manager.player.StartTurn();
                manager.player.WhenStartTurn();
                manager.state = new PlayerTurn(manager);
            }
            else
            {
                // �S�G�̃^�[���J�n���֐����Ă�
                foreach (var enemy in manager.enemyList)
                {
                    enemy.StartTurn();
                    enemy.WhenStartTurn();
                }
                manager.state = new EnemyTurn(manager);
            }


        }

    }
}

