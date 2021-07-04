using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    // ���ė~�������ǐG��ꂽ���Ȃ��i��������������ȕ��͂��ȁj�̂�SerialzeField,ReadOnly��Private
    [SerializeField, ReadOnly]
    private float time = 0;

    // �������͕���
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

    // 0����1�܂ł͈̔͂�(�N�����v����)�i��������Ԃ�
    public float GetProgressZeroToOne()
    {
        return Mathf.Clamp01(time / limit);
    }

    // �i���͈͂�Ԃ��i1��������ڕԂ�j
    public float GetProgress()
    {
        return time / limit;
    }
}
