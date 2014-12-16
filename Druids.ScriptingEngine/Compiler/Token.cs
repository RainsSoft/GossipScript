using System;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Exceptions;
using Druids.ScriptingEngine;
using Druids.ScriptingEngine.Exceptions;
using Druids.ScriptingEngine.Host;

namespace Druids.ScriptingEngine.Compiler
{
    public class Token
    {
        public Token(TokenType type, OperationType operationType, string value, TokenPosition position)
        {
            OperationType = operationType;
            Type = type;
            Value = value;
            Position = position;
        }

        public OperationType OperationType { get; set; }
        public TokenPosition Position { get; set; }
        public TokenType Type { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return
                string.Format(
                    "Token: {{ Type: \"{0}\", Value: \"{1}\", Position: {{ Index: \"{2}\", Line: \"{3}\", Column: \"{4}\" }} }}",
                    Type, Value, Position.Index, Position.Line, Position.Column);
        }

        public Int32 GetAccessIndex(EnumDefineTable enumDefineTable)
        {
            string replace = Value.Replace('[', ' ').Replace(']', ' ');
            string[] strSplit = replace.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
            if (Type == TokenType.LocalFlagAccessEnum ||
                Type == TokenType.LocalStringAccessEnum ||
                Type == TokenType.LocalVariableAccessEnum ||
                Type == TokenType.GlobalFlagAccessEnum ||
                Type == TokenType.GlobalStringAccessEnum ||
                Type == TokenType.GlobalVariableAccessEnum ||
                Type == TokenType.ReferenceObjectAccessEnum)
            {
                if (enumDefineTable.HasEnum(strSplit[1]))
                {
                    return enumDefineTable.DefinesByString[strSplit[1]];
                }
                else
                {
                    throw new GossipScriptException("Invalid index");
                }
            }
            else
            {
                int varIndex = Int32.Parse(strSplit[1]);

                if (Type == TokenType.GlobalVariableAccess || Type == TokenType.GlobalFlagAccess ||
                    Type == TokenType.GlobalStringAccess)
                {
                    if (varIndex < 0 || varIndex >= 1024)
                        throw new GossipScriptException("Index out of bounds");
                }
                if (Type == TokenType.LocalFlagAccess || Type == TokenType.LocalVariableAccess ||
                    Type == TokenType.LocalStringAccess)
                {
                    if (varIndex < 0 || varIndex >= 64)
                        throw new GossipScriptException("Index out of bounds");
                }
                return varIndex;
            }
        }
    }
}