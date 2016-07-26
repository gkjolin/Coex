using System;

namespace IFGame.Lix
{
    public class CoroutineException : ApplicationException
    {
        public CoroutineException(string format, params object[] args) 
            : base(string.Format(format, args)) 
        { }
    }

    public class CoroutineForbiddenReturnTypeException : CoroutineException
    {
        public CoroutineForbiddenReturnTypeException(Type t)
            : base(
                "specify {0} as return value type is forbidden, it's been used by CoexEngine",
                t)
        { }
    }

    public class CoroutineNoReturnValueException : CoroutineException
    {
        public CoroutineNoReturnValueException()
            : base("trying to access the return value of a coroutine which hasn't yield one")
        { }
    }

    public class CoroutineApiDeprecated : CoroutineException
    { 
        public CoroutineApiDeprecated(string signature)
            : base(
                "[{0}] is deprecated, use new API provided by CoexBehaviour, any question plz @llisper",
                signature)
        { }
    }
}