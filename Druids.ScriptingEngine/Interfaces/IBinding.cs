using System;
using Druids.ScriptingEngine.Enums;

namespace Druids.ScriptingEngine.Interfaces
{
    public interface IBinding
    {
        GossipType GossipType { get; }

        Object Invoke(Object obj);

        T Invoke<T>(Object obj);

        Object Invoke(Object obj, object[] parameters);

        T Invoke<T>(Object obj, object[] parameters);
    }
}