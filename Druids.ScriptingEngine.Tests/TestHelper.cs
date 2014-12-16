using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Druids.ScriptingEngine;
using Druids.ScriptingEngine.Host;
using Druids.ScriptingEngine.Interfaces;

namespace JRPG.ScriptingEngine.Tests
{
    public static class TestHelper
    {
        public static String ReadTextFile(String filename)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = String.Format("JRPG.ScriptingEngine.Tests.Resources.{0}", filename);

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static List<String> SplitScript(String text)
        {
            var rv = new List<string>();

            text = text.Replace("’", "'");
            var len = text.Length;
            var numBrackets = 0;
            var scriptStart = 0;
           
            for (int i = 0; i < len; i++)
            {
                if (text[i] == '{')                     // Found end of label
                {
                    if (numBrackets == 0)
                        scriptStart = i;

                    numBrackets++;
                }
                if (text[i] == '}')
                {
                    numBrackets--;
                    if (numBrackets == 0)
                    {
                        var str = text.Substring(scriptStart + 1, i - scriptStart - 1).Trim();
                        str += Environment.NewLine;
                        rv.Add(str);
                    }
                }
            }
            return rv;
        }

        public static void RunScriptToCompletion(GossipVM engine, CompiledScript compiledScript)
        {
            engine.RunScript(compiledScript);
            while (engine.HasRunningScripts())
            {
                Thread.Sleep(16);
                engine.Update(16);
            }
        }
    }
}
