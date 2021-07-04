using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoGameEvent
{
    public enum eGameEvent
    {
        RemoveObject
    }

    abstract public class GameEventBase : MonoBehaviour
    {

        public static EventListner eventListner;

        public bool isEnd { get; protected set; } = false;



        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public abstract void EventUpdate();
    }
}