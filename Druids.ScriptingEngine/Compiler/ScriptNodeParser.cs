using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Exceptions;


namespace Druids.ScriptingEngine.Compiler
{
    public class ScriptNodeParser
    {
        private readonly List<TokenDefinition> m_EventDefinitions = new List<TokenDefinition>();

        public ScriptNodeParser()
        {
            m_EventDefinitions.Add(new TokenDefinition(TokenType.EventOnRun, OperationType.None, new Regex("OnUpdate()")));
            m_EventDefinitions.Add(new TokenDefinition(TokenType.EventOnStart, OperationType.None,new Regex("OnStart()")));
            m_EventDefinitions.Add(new TokenDefinition(TokenType.EventOnEnd, OperationType.None, new Regex("OnEnd()")));
            m_EventDefinitions.Add(new TokenDefinition(TokenType.EventOnInterrupt, OperationType.None,new Regex("OnInterrupt()")));
        }

        public void RegisterCustomEvent(String name)
        {
            m_EventDefinitions.Add(new TokenDefinition(TokenType.EventCustom, OperationType.None, new Regex(Regex.Escape(name))));
        }

        public ScriptNodeInfo ParseScriptNode(String script)
        {
            var rv = new ScriptNodeInfo();
            var tokens = new List<Token>();
            foreach (TokenDefinition definition in m_EventDefinitions)
            {
                Match match = definition.Regex.Match(script);
                if (match.Success)
                {
                    tokens.Add(new Token(definition.Type, OperationType.None, match.Value,new TokenPosition(match.Index, 0, 0)));
                }
            }

            // Default
            if (tokens.Count == 0)
                tokens.Add(new Token(TokenType.EventDefault, OperationType.None, script, new TokenPosition(0, 0, 0)));

            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].Type == TokenType.EventOnStart)
                {
                    rv.OnEnterScript = new ScriptNodeEventInfo
                        {
                            EventScript = MatchCurlyBraces(tokens[i].Position.Index, script),
                            Token = tokens[i]
                        };
                    continue;
                }

                if (tokens[i].Type == TokenType.EventOnEnd)
                {
                    rv.OnExitScript = new ScriptNodeEventInfo
                        {
                            EventScript = MatchCurlyBraces(tokens[i].Position.Index, script),
                            Token = tokens[i]
                        };
                    continue;
                }

                if (tokens[i].Type == TokenType.EventOnRun)
                {
                    rv.OnUpdateScript = new ScriptNodeEventInfo
                        {
                            EventScript = MatchCurlyBraces(tokens[i].Position.Index, script),
                            Token = tokens[i]
                        };
                    continue;
                }

                if (tokens[i].Type == TokenType.EventOnInterrupt)
                {
                    rv.OnInterruptScript = new ScriptNodeEventInfo
                        {
                            EventScript = MatchCurlyBraces(tokens[i].Position.Index, script),
                            Token = tokens[i]
                        };
                    continue;
                }

                if (tokens[i].Type == TokenType.EventCustom)
                {
                    rv.CustomEventScripts.Add(new ScriptNodeEventInfo
                        {
                            EventScript = MatchCurlyBraces(tokens[i].Position.Index, script),
                            Token = tokens[i]
                        });
                    continue;
                }

                // Treat default as OnUpdate
                if (tokens[i].Type == TokenType.EventDefault)
                {
                    rv.OnUpdateScript = new ScriptNodeEventInfo
                        {
                            EventScript = script,
                            Token = tokens[i]
                        };
                }
            }

            return rv;
        }


        private String MatchCurlyBraces(Int32 startIndex, String source)
        {
            // Find end
            Int32 braceCount = 0;
            Int32 endIndex = -1;
            Int32 firstBraceIndex = -1;
            for (int i = startIndex; i < source.Length; i++)
            {
                if (source[i] == '{')
                {
                    braceCount++;
                    firstBraceIndex = i;
                }

                if (source[i] == '}')
                    braceCount--;

                if (braceCount == 0 && firstBraceIndex >= 0)
                {
                    endIndex = i;
                    break;
                }
            }

            if (endIndex == -1)
                throw new GossipScriptException("Mismatched curly braces");


            return source.Substring(firstBraceIndex + 1, endIndex - firstBraceIndex - 2).Trim();
        }
    }
}