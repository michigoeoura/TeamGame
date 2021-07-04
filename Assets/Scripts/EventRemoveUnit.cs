using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventRemoveUnit : MoGameEvent.GameEventBase
{
    // 外部からセットしないといけないのでPublic、しかしインスペクターから触られる訳にはいかないのでReadOnly
    [SerializeField, ReadOnly]
    public Unit removeObject;

    [Tooltip("ベジェ曲線の制御点（線はこの点に近づく）")]
    [SerializeField]
    Vector3 bezierControlPoint;

    private Vector3 removeStartPos;
    public Vector3 removeEndPos;

    private Timer timer;

    [SerializeField]
    private float removeTime;   // どかすのに掛かる時間

    public void Initialize(Unit targetUnit)
    {
        removeObject = targetUnit;
        removeStartPos = removeObject.transform.position;
        // 始点と終点の中点を足す
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
