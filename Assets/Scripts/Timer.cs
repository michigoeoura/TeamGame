using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    // 見て欲しいけど触られたくない（誤解を招きそうな文章だな）のでSerialzeField,ReadOnlyなPrivate
    [SerializeField, ReadOnly]
    private float time = 0;

    // こっちは普通
    [SerializeField]
    private float limit = 0;

    public Timer(float timeLimit)
    {
        limit = timeLimit;
    }

    public void ChangeLimit(float newTimeLimit)
    {
        limit = newTimeLimit;
    }

    public void TimerUpdate()
    {
        time += UnityEngine.Time.deltaTime;
    }

    public void Reset()
    {
        time = 0.0f;
    }

    // 0から1までの範囲で(クランプして)進捗割合を返す
    public float GetProgressZeroToOne()
    {
        return Mathf.Clamp01(time / limit);
    }

    // 進捗範囲を返す（1より上も直接返る）
    public float GetProgress()
    {
        return time / limit;
    }
}
