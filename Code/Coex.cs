using UnityEngine;
using System;
using System.Collections;

namespace IFGame.Lix
{
    public class Coex
    {
        public enum State
        {
            Running,
            Interrupted,
            Error,
            Done,
        }

        public bool hasReturnValue { get { return mReturnValueType != null; } }
        public int yieldCount { get { return mYieldCount; } }
        public State state { get { return mState; } }

        public T ReturnValue<T>()
        {
            if (null == mReturnValueType)
                throw new CoroutineNoReturnValueException();
            if (null != mException)
                throw mException;

            return (T)mReturnValue;
        }

        public void CheckError()
        {
            if (null != mException)
                throw mException;
        }

        #region internal
        IEnumerator mRoutine;
        object mReturnValue;
        Type mReturnValueType;
        Exception mException;
        int mYieldCount;
        State mState;

        internal Coex(IEnumerator routine, Type returnValueType)
        {
            mRoutine = routine;
            mReturnValue = null;
            mReturnValueType = returnValueType;
            mException = null;
            mYieldCount = 0;
            mState = State.Running;
        }

        internal object returnValue { get { return mReturnValue; } }

        #if UNITY_EDITOR
        public object returnValueI { get { return mReturnValue; } }
        public string routineNameI { get { return mRoutine.ToString(); } }
        public Type returnValueTypeI { get { return mReturnValueType; } }
        [NonSerialized]
        public bool foldout;
        #endif // UNITY_EDITOR

        internal void Interrupt()
        {
            if (mState == State.Running)
                mState = State.Interrupted;
        }

        internal void MoveNext()
        {
            try
            {
                MoveNextExceptional();
            }
            catch (Exception e)
            {
                mException = e;
                mState = State.Error;
            }
        }

        internal void MoveNextExceptional()
        {
            if (!mRoutine.MoveNext())
            {
                mState = State.Done;
                return;
            }

            ++mYieldCount;
            mReturnValue = mRoutine.Current;
            if (null != mReturnValueType && 
                null != mReturnValue && 
                mReturnValue.GetType() == mReturnValueType)
            {
                mState = State.Done;
            }
            else if (null != mReturnValue)
            {
                if (mReturnValue is WaitForSeconds)
                    ((WaitForSeconds)mReturnValue).Reset();
            }
        }
        #endregion internal
    }
}