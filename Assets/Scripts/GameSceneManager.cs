using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// todo 命名に違和感
// Unitに対するリスナーとしてのゲームマネージャー
// C#のインターフェース:https://ufcpp.net/study/csharp/oo_interface.html
public interface UnitListner
{
    // 何も書かなくてもpublic virtualになるんだけど手癖と気持ちの問題でこうします
    public void AddEvent(MoGameEvent.GameEventBase gameEvent);

    public List<AbstractEnemy> GetEnemies();

    public GameObject GetEventTemplate(MoGameEvent.eGameEvent name);
}

// GameEventに対するリスナーとしてのゲームマネージャー
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
    List<MoGameEvent.GameEventBase> eventList;  // イベント実行が複数フレームに跨るのでスタックではなくリストに

    [SerializeField, ReadOnly]
    Dictionary<MoGameEvent.eGameEvent, GameObject> eventTemplates;

    SceneStateBase state = null;


    // Start is called before the first frame update
    void Start()
    {
        // ヒエラルキーでアクティブな全MapNodeクラス所有オブジェクトをリストに流す
        mapNodeList.AddRange(FindObjectsOfType<MapNode>());

        // 同Enemy
        enemyList.AddRange(FindObjectsOfType<AbstractEnemy>());

        // プレイヤー（これはリストじゃないけど）
        player = FindObjectOfType<Player>();

        // Unitクラスのマネージャー（クラス変数）に自分をセット
        Unit.unitListner = this;

        // 同ゲームイベント
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

        // as演算子、キャストを試みてダメならnullを返す
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

        // キーから値を取得できるか試して有無をboolで返す、値はvalueに返ってくる
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


    // 内部クラスはprivateアクセス可能
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

            // シーンイベントを回す関係でプレイヤー行動後に行動が終了した(ターンを終了させる)か聞きたくて、二つを一緒にすると1フレーム遅延してでこんな事に(´･ω･`)
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
                // 敵の内どれか一体でも行動が終わっていなければ敵のターンは終了していない
                if (!enemy.isActEnd) { isEnemyTurnEnd = false; }
            }


            if (isEnemyTurnEnd)
            {
                ChangeToEventProc();
            }

        }

        void ChangeToEventProc()
        {

            // 全敵のターン終了時関数を呼ぶ
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
                // イベント実行
                gameEvent.EventUpdate();
                if (gameEvent.isEnd)
                {
                    gameEvent.gameObject.SetActive(false);
                }

            }

            // やっぱりforeach内で要素削除は出来ないのでRemoveAllで対応(ラムダ式)
            manager.eventList.RemoveAll(gameEvent => gameEvent.isEnd);

            // このタイミングでイベントリストに何か入っているならイベント的にはターンを終了したくない
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
                // 全敵のターン開始時関数を呼ぶ
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

