using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Druids.ScriptingEngine.Compiler;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Interfaces;
using Druids.ScriptingEngine;
using Druids.ScriptingEngine.Host;
using Druids.ScriptingEngine.Interfaces;

namespace Druids.ScriptingEngine
{
    public class GossipVM : IScriptEngine
    {
        internal readonly GossipCompiler Compiler;

        internal readonly List<CompiledScript> CurrentActiveScripts;
        
        internal readonly Dictionary<String, ActionInfo> GlobalActions = new Dictionary<string, ActionInfo>();

        internal readonly Dictionary<String, MethodInfo> GlobalFunctions = new Dictionary<string, MethodInfo>();

        public readonly GlobalState GlobalState;
        internal readonly GossipLexer Lexer;

        private readonly HostBridge m_HostBridge;
        private readonly Random m_Random = new Random(42);

        public StringTable StringTable = new StringTable();

        public GossipVM()
        {
            GlobalState = new GlobalState();

            Lexer = new GossipLexer();
            m_HostBridge = new HostBridge(Lexer);
            Compiler = new GossipCompiler(this, Lexer, new ScriptNodeParser());

            CurrentActiveScripts = new List<CompiledScript>(16);

            // Math
            RegisterGlobalFunction("cos", new Func<double, double>(Math.Cos));
            RegisterGlobalFunction("sin", new Func<double, double>(Math.Sin));
            RegisterGlobalFunction("exp", new Func<double, double>(Math.Exp));
            RegisterGlobalFunction("max", new Func<double, double, double>(Math.Max));
            RegisterGlobalFunction("min", new Func<double, double, double>(Math.Min));

            // System
            RegisterGlobalAction("system", new ActionInfo("Print", Print, null, null));
            RegisterGlobalAction("system", new ActionInfo("Goto", Goto));
        
            RegisterGlobalFunction("system.rand", new Func<double, double, double>((a, b) => m_Random.Next((Int32) a, (Int32) (b) + 1)));
        }

        public HostBridge HostBridge
        {
            get { return m_HostBridge; }
        }

        public Boolean RunScript(string text)
        {
            // Step 1 compile the script
            CompiledScript scriptProgram = Compiler.CompileScript(text, StringTable);

            RunScript(scriptProgram);
            return true;
        }

        public CompiledScript CompileScript(string text, Type self = null)
        {
            var program = Compiler.CompileScript(text, StringTable, self);
            return program;
        }


        public void RegisterGlobalAction(String namespaceName, ActionInfo functionInfo)
        {
            var fullToken = namespaceName + "." + functionInfo.TokenName;
            functionInfo.TokenName = fullToken;

            if (GlobalActions.ContainsKey(fullToken))
                GlobalActions.Remove(fullToken);

            GlobalActions.Add(fullToken, functionInfo);

            Compiler.Lexer.AddDefinition(new TokenDefinition(TokenType.HostFunctionCall, OperationType.FunctionCall, new Regex(Regex.Escape(fullToken))));
        }

        public void Update(Int32 deltaMs)
        {
            for (int i = 0; i < CurrentActiveScripts.Count; i++)
            {
                CompiledScript script = CurrentActiveScripts[i];
                if (script.Update(this, deltaMs) == ScriptResult.Ok)
                {
                    CurrentActiveScripts.Remove(script);
                    i--;
                }
            }
        }

        public Expression ParseExpression(String input, bool normalize = true)
        {
            return Compiler.CompileExpression(input, normalize);
        }

        private ScriptResult Print(GossipActionParameter actionParameter)
        {
            Console.WriteLine(actionParameter.Parameters[0]);
            return ScriptResult.Continue;
        }

        private ScriptResult Goto(GossipActionParameter actionParameter)
        {
            actionParameter.Script.SetActiveNode(actionParameter.Parameters[0] as string);
            return ScriptResult.Yield;
        }

        public Boolean RunScript(CompiledScript compiledScript)
        {
            if (compiledScript == null)
                return false;

            // Step 3 Run the script
            ScriptResult result = compiledScript.Update(this);

            // Step 4 If the script has not finished queue it
            if (result != ScriptResult.Ok &&
                result != ScriptResult.EndProcess &&
                result != ScriptResult.ChangedNode)
            {
                CurrentActiveScripts.Add(compiledScript);
            }
            return true;
        }

        public Expression CompileExpression(String text, Type self = null)
        {
            return Compiler.CompileExpression(text, true);
        }

        public ScriptNode CompileNode(string text, Type self = null)
        {
            var scriptNode = Compiler.CompileScriptNode("", text, StringTable, self);
            return scriptNode;
        }

        public void RegisterGlobalFunction(string name, Delegate function)
        {
            HostBridge.HostCallTable.RegisterFunction(name, function);
            Compiler.Lexer.AddDefinition(new TokenDefinition(TokenType.HostFunctionCall, OperationType.FunctionCall,new Regex(name)));
        }

        public bool HasRunningScripts()
        {
            return CurrentActiveScripts.Count > 0;
        }

        public void RegisterEnum(string name, int value)
        {
            HostBridge.EnumDefineTable.Add(name, value);
            Compiler.Lexer.AddDefinition(new TokenDefinition(TokenType.Enum, OperationType.Operand, new Regex(name)));
        }

        public void RegisterGlobalFunction(string nameSpaceName, string name, Delegate function)
        {
            throw new NotImplementedException();
        }

        public void RegisterEnums(Dictionary<String, Int32> enumsByName)
        {
            foreach (var i in enumsByName)
            {
                HostBridge.EnumDefineTable.Add(i.Key, i.Value);
            }
        }

        public void RegisterEnums(Dictionary<Int32, String> enumsByValue)
        {
            foreach (var i in enumsByValue)
            {
                HostBridge.EnumDefineTable.Add(i.Value, i.Key);
            }
        }



        /// <summary>
        ///     Registers all methods and properties for the type with the scripting engine
        /// </summary>
        public void RegisterType(Type type)
        {
            HostBridge.TypeBindingTable.RegisterType(type);
        }

        public void RegisterCustomEvent(String name, Type eventType = null)
        {
            Compiler.ScriptNodeParser.RegisterCustomEvent(name);

            if (eventType != null)
                HostBridge.TypeBindingTable.RegisterType(eventType);

            HostBridge.EventBindingTable.Register(name, eventType);
        }
    }
}