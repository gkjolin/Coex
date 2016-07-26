using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace IFGame.Lix
{
    public class CoexBehaviour : MonoBehaviour
    {
        #region coex api
        public new Coex StartCoroutine(IEnumerator routine)
        {
            return StartCoroutine(routine, null);
        }

        public Coex StartCoroutine<T>(IEnumerator routine)
        {
            return StartCoroutine(routine, typeof(T));
        }

        public void StopCoroutine(Coex routine)
        {
            for (int i = 0; i < coexs.Count; ++i)
            {
                if (coexs[i] == routine)
                {
                    routine.Interrupt();
                    coexs.RemoveAt(i);
                    break;
                }
            }
        }

        public new void StopAllCoroutines()
        {
            if (coexs.Count > 0)
            {
                for (int i = 0; i < coexs.Count; ++i)
                    coexs[i].Interrupt();
                coexs.Clear();
            }
        }
        #endregion coex api

        #region disable original api
        public new Coroutine StartCoroutine(string methodName)
        {
            throw new CoroutineApiDeprecated("StartCoroutine(string)");
        }

        public new Coroutine StartCoroutine(string methodName, object value)
        {
            throw new CoroutineApiDeprecated("StartCoroutine(string, object)");
        }

        public new Coroutine StartCoroutine_Auto(IEnumerator routine)
        { 
            throw new CoroutineApiDeprecated("StartCoroutine_Auto(IEnumerator)");
        }

        public new void StopCoroutine(Coroutine routine)
        {
            throw new CoroutineApiDeprecated("StopCoroutine(Coroutine)");
        }

        public new void StopCoroutine(IEnumerator routine)
        { 
            throw new CoroutineApiDeprecated("StopCoroutine(IEnumerator)");
        }

        public new void StopCoroutine(string methodName)
        {
            throw new CoroutineApiDeprecated("StopCoroutine(string)");
        }
        #endregion disable original api

        #region coex internal
        internal bool addToEngine;
        internal List<Coex> coexs = new List<Coex>(32);

        #if UNITY_EDITOR
        public bool addToEngineI { get { return addToEngine; } }
        public List<Coex> coexsI { get { return coexs; } }
        #endif // UNITY_EDITOR

        internal Coex StartCoroutine(IEnumerator routine, Type rtType)
        {
            if (!addToEngine)
            {
                CoexEngine.instance.AddBehaviour(this);
                addToEngine = true;
            }

            CheckReturnType(rtType);

            Coex c = new Coex(routine, rtType);
            c.MoveNext();
            if (c.state == Coex.State.Running)
                coexs.Add(c);
            return c;
        }

        internal void CheckReturnType(Type t)
        {
            if (t == typeof(WWW) || 
                t == typeof(Coex) || 
                t == typeof(YieldInstruction))
            {
                throw new CoroutineForbiddenReturnTypeException(t); 
            }
        }
        #endregion coex internal
    }
}