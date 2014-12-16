using System;

namespace Druids.ScriptingEngine.Compiler
{
    public class StateMachineParser
    {
        public ScriptStateMachineInfo ParseStateMachineInfo(String text)
        {
            var nodes = Parse(text);
            if (nodes.Nodes.Count == 0)
            {
                nodes.Nodes.Add(new ScriptStateNodeRecord("",text));
            }
            return nodes;
        }

        public static ScriptStateMachineInfo Parse(String text)
        {
            text = text.Replace("’", "'");

            var rv = new ScriptStateMachineInfo();

            var len = text.Length;
            var labelStart = 0;
            var numBrackets = 0;
            var scriptStart = 0;
            var currentState = new ScriptStateNodeRecord();
            var foundName = false;

            for (int i = 0; i < len; i++)
            {
                if (text[i] == '@')                    // Found start of label
                {
                    foundName = true;
                    labelStart = i;
                }
                if (text[i] == '{' && foundName)                     // Found end of label
                {
                    if (numBrackets == 0)
                    {
                        scriptStart = i;
                        currentState.Name = text.Substring(labelStart + 1, i - labelStart - 1).Trim();
                    }

                    numBrackets++;
                }
                if (text[i] == '}')
                {
                    numBrackets--;
                    if (numBrackets == 0)
                    {
                        var str = text.Substring(scriptStart + 1, i - scriptStart - 1).Trim();
                        str += Environment.NewLine;
                        currentState.Text = str;
                        rv.Nodes.Add(new ScriptStateNodeRecord(currentState.Name, currentState.Text));

                        currentState = new ScriptStateNodeRecord();
                        foundName = false;
                    }
                }
            }
            return rv;
        }
     
    }
}
