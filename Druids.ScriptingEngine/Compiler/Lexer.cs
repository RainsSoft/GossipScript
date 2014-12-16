using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Exceptions;
using Druids.ScriptingEngine;
using Druids.ScriptingEngine.Exceptions;

namespace Druids.ScriptingEngine.Compiler
{
    public class Lexer
    {
        protected readonly IList<TokenDefinition> m_CustomTokenDefinitions = new List<TokenDefinition>();
        protected readonly Regex m_EndOfLineRegex = new Regex(@"\r\n|\r|\n", RegexOptions.Compiled);
        protected readonly IList<TokenDefinition> m_TokenDefinitions = new List<TokenDefinition>();

        public void AddDefinition(TokenDefinition tokenDefinition)
        {
            m_CustomTokenDefinitions.Add(tokenDefinition);
        }

        public IEnumerable<Token> Tokenize(string source)
        {
            int currentIndex = 0;
            int currentLine = 1;
            int currentColumn = 0;

            IOrderedEnumerable<TokenDefinition> sortedTokens = m_CustomTokenDefinitions.OrderByDescending(o => o.Regex.ToString());

            while (currentIndex < source.Length)
            {
                TokenDefinition matchedDefinition = null;
                int matchLength = 0;

                foreach (TokenDefinition rule in m_TokenDefinitions)
                {
                    Match match = rule.Regex.Match(source, currentIndex);

                    if (match.Success && (match.Index - currentIndex) == 0)
                    {
                        matchedDefinition = rule;
                        matchLength = match.Length;
                        break;
                    }
                }

                if (matchedDefinition == null)
                {
                    foreach (TokenDefinition rule in sortedTokens)
                    {
                        Match match = rule.Regex.Match(source, currentIndex);

                        if (match.Success && (match.Index - currentIndex) == 0)
                        {
                            matchedDefinition = rule;
                            matchLength = match.Length;
                            break;
                        }
                    }
                }

                if (matchedDefinition == null)
                {
                    // Read statement until ( or . or end of line
                    var n = currentIndex;
                    while (n <source.Length-1)
                    {
                        if (source[n] == '(')
                            break;

                        if (source[n] == ';')
                            break;


                        n++;
                    }

                    var guessToken = source.Substring(currentIndex, n - currentIndex);

                    var errorString = string.Format("Unrecognized symbol '{0}' at index {1} (line {2}, column {3}).", guessToken, currentIndex, currentLine, currentColumn);

                    throw new GossipScriptException(errorString);
                }
                else
                {
                    string value = source.Substring(currentIndex, matchLength);

                    if (matchedDefinition.OperationType == OperationType.Error)
                    {
                        // This should not be matched in the source
                        throw new GossipScriptException(string.Format("Invalid Token '{0}' at index {1} (line {2}, column {3}).", value, currentIndex, currentLine, currentColumn));
                    }


                    if (matchedDefinition.DiscardPolicy == TokenDiscardPolicy.Keep)
                        yield return new Token(matchedDefinition.Type, matchedDefinition.OperationType, value, new TokenPosition(currentIndex, currentLine, currentColumn));

                    Match endOfLineMatch = m_EndOfLineRegex.Match(value);
                    if (endOfLineMatch.Success)
                    {
                        currentLine += 1;
                        currentColumn = value.Length - (endOfLineMatch.Index + endOfLineMatch.Length);
                    }
                    else
                    {
                        currentColumn += matchLength;
                    }

                    currentIndex += matchLength;
                }
            }

            yield return
                new Token(TokenType.EndOfProgram, OperationType.None, null,
                          new TokenPosition(currentIndex, currentLine, currentColumn));
        }

        public bool HasDefinition(TokenType referencePropertyGet, OperationType functionCall)
        {
            bool found = m_TokenDefinitions.Any(o => o.Type == referencePropertyGet && o.OperationType == functionCall);
            if (found == false)
            {
                found =
                    m_CustomTokenDefinitions.Any(o => o.Type == referencePropertyGet && o.OperationType == functionCall);
            }
            return found;
        }
    }
}