using System;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Exceptions;
using Druids.ScriptingEngine.Exceptions;

namespace Druids.ScriptingEngine.Helper
{
    public static class TypeConverter
    {
        public static GossipType ConvertHostType(Type type)
        {
            var rv = GossipType.None;

            if (type == null)
                throw new GossipScriptException("Void is not a supported return type");

            if (type == typeof (byte))
                rv = GossipType.Number;

            if (type == typeof (Int16))
                rv = GossipType.Number;

            if (type == typeof (Int32))
                rv = GossipType.Number;

            if (type == typeof (Int64))
                rv = GossipType.Number;

            if (type == typeof (float))
                rv = GossipType.Number;

            if (type == typeof (double))
                rv = GossipType.Number;

            if (type == typeof (String))
                rv = GossipType.String;

            if (rv == GossipType.None) // Still not assigned
            {
                if (type.BaseType == typeof (Object))
                    rv = GossipType.ReferenceObject;
            }

            if (rv == GossipType.None)
                throw new GossipScriptException("Unsupported return type: {0}");

            return rv;
        }

        internal static object ConvertNumber(double item, Type hostType)
        {
            if (hostType == typeof (Int32))
                return (Int32) item;

            if (hostType == typeof (Byte))
                return (Int32) item;

            if (hostType == typeof (Int16))
                return (Int16) item;

            if (hostType == typeof (Int64))
                return (Int64) item;

            return item;
        }
    }
}