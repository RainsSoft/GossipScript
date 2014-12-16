using System;
using Druids.ScriptingEngine;
using Druids.ScriptingEngine.Host;

namespace Druids.ScriptingEngine.Interfaces
{
    public interface IScriptEngine
    {
        Boolean RunScript(String text);

        Boolean RunScript(CompiledScript script);

        CompiledScript CompileScript(String text, Type self = null);

        void RegisterGlobalAction(String nameSpaceName, ActionInfo functionInfo);

        void Update(Int32 deltaMs);

        void RegisterEnum(String name, Int32 value);

        void RegisterGlobalFunction(string nameSpaceName, string name, Delegate function);
    }
}