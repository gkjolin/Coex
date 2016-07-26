using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace IFGame.Lix
{
    public class CoexEngine : MonoBehaviour
    {
        static CoexEngine sInstance;
        internal static CoexEngine instance
        {
            get
            {
                if (null == sInstance)
                    new GameObject("Lix.CoexEngine", typeof(CoexEngine));
                return sInstance;
            }
        }

        internal List<CoexBehaviour> behaviours = new List<CoexBehaviour>(1024);

        #if UNITY_EDITOR
        public List<CoexBehaviour> behavioursI { get { return behaviours; } }
        #endif // UNITY_EDITOR

        internal void AddBehaviour(CoexBehaviour b)
        {
            if (!behaviours.Contains(b))
                behaviours.Add(b);
        }

        #region process
        enum Process
        {
            Update,
            FixedUpdate,
            EndOfFrames,
        }

        void Loop(Process process)
        {
            for (int i = 0; i < behaviours.Count; )
            {
                CoexBehaviour b = behaviours[i];
                if (null == b)
                {
                    // NOTICE: this is a interesting part, b is not really null
                    // regarding to .NET, we're making use of unity's overload version
                    // of null condition test.
                    //
                    // and StopAllCoroutines must be called on b at this point, or else
                    // dangling Coex which is started by b will probably has a state
                    // other than Interrupt, could be Running or something else.
                    b.StopAllCoroutines();
                    behaviours.RemoveAt(i);
                    continue;
                }

                if (!b.gameObject.activeInHierarchy || !b.enabled || b.coexs.Count == 0)
                {
                    b.StopAllCoroutines();
                    behaviours.RemoveAt(i);
                    continue;
                }

                ++i;
                for (int j = 0; j < b.coexs.Count; )
                {
                    Coex coex = b.coexs[j];
                    if (coex.state != Coex.State.Running)
                    {
                        b.coexs.RemoveAt(j);
                        continue;
                    }
                    else
                    {
                        ++j;
                    }

                    bool pred = false;
                    switch (process)
                    {
                    case Process.Update:
                        pred = ProcessUpdate(coex);
                        break;
                    case Process.FixedUpdate:
                        pred = ProcessFixedUpdate(coex);
                        break;
                    case Process.EndOfFrames:
                        pred = ProcessEndOfFrames(coex);
                        break;
                    }

                    if (pred) coex.MoveNext();
                }
            }
        }
        
        bool ProcessUpdate(Coex coex)
        {
            object rv = coex.returnValue;
            if (null != rv)
            {
                if (rv is WaitForEndOfFrame || rv is WaitForFixedUpdate)
                    return false;
                else if (rv is WaitForSeconds && !((WaitForSeconds)rv).timeout)
                    return false;
                else if (rv is WWW && !((WWW)rv).isDone)
                    return false;
                else if (rv is Coex && ((Coex)rv).state == Coex.State.Running)
                    return false;
            }
            return true;
        }

        bool ProcessFixedUpdate(Coex coex)
        {
            return coex.returnValue is WaitForFixedUpdate;
        }

        bool ProcessEndOfFrames(Coex coex)
        {
            return coex.returnValue is WaitForEndOfFrame;
        }
        #endregion process

        #region unity message
        void Awake()
        {
            if (null != sInstance)
            {
                GameObject.Destroy(this);
            }
            else
            {
                sInstance = this;
                DontDestroyOnLoad(this);
                StartCoroutine(EndOfFrames());
            }
        }

        void Update()
        {
            Loop(Process.Update);
        }

        void FixedUpdate()
        {
            Loop(Process.FixedUpdate);
        }

        IEnumerator EndOfFrames()
        {
            UnityEngine.WaitForEndOfFrame wait = new UnityEngine.WaitForEndOfFrame();
            while (true)
            {
                yield return wait;
                Loop(Process.EndOfFrames);
            }
        }        
        #endregion unity message
    }
}