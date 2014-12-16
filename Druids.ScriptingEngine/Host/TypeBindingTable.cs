using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Druids.ScriptingEngine.Attributes;
using Druids.ScriptingEngine.Compiler;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Exceptions;
using Druids.ScriptingEngine.Exceptions;
using Druids.ScriptingEngine.Host;
using Druids.ScriptingEngine.Interfaces;

namespace Druids.ScriptingEngine.Host
{
    public class TypeBindingTable
    {
        private readonly ActionBindingTable m_ActionBindingTable = new ActionBindingTable();
        private readonly GossipLexer m_Lexer;

        private readonly MethodBindingTable m_MethodBindingTable = new MethodBindingTable();
        private readonly PropertyBindingTable m_PropertyBindingTable = new PropertyBindingTable();

        public TypeBindingTable(GossipLexer lexer)
        {
            m_Lexer = lexer;
        }

        public PropertyGetBinding GetPropertyGetById(int actionId, Type type)
        {
            return m_PropertyBindingTable.GetPropertyGetById(actionId, type);
        }

        public PropertyGetBinding GetPropertyGetByName(string name, Type type)
        {
            return m_PropertyBindingTable.GetPropertyGetByName(name, type);
        }

        public PropertySetBinding GetPropertySetByName(string name, Type type)
        {
            return m_PropertyBindingTable.GetPropertySetByName(name, type);
        }

        public PropertySetBinding GetPropertySetById(int id, Type type)
        {
            return m_PropertyBindingTable.GetPropertySetById(id, type);
        }

        public bool GetPropertyExists(string name, Type type)
        {
            return m_PropertyBindingTable.GetPropertyExists(name, type);
        }

        public bool SetPropertyExists(string name, Type type)
        {
            return m_PropertyBindingTable.SetPropertyExists(name, type);
        }

        public MethodBinding GetMethodByName(string name, Type type)
        {
            return m_MethodBindingTable.GetMethodByName(name, type);
        }

        public MethodBinding GetMethodById(int methodId)
        {
            return m_MethodBindingTable.GetMethodById(methodId);
        }

        public ActionBinding GetActionByName(string name, Type objType)
        {
            return m_ActionBindingTable.GetActionByName(name, objType);
        }

        public ActionBinding GetActionById(Int32 id)
        {
            return m_ActionBindingTable.GetActionById(id);
        }

        protected void RegisterActions(Type type, TokenType tokenType = TokenType.ReferenceAction)
        {
            MethodInfo[] methods = type.GetMethods();
            foreach (MethodInfo methodInfo in methods)
            {
                object[] attributes = methodInfo.GetCustomAttributes(typeof (GossipScriptAction), false);
                if (attributes.Any())
                {
                    GossipScriptAction gossipScriptActionAttribute = attributes.Cast<GossipScriptAction>().Single();

                    if (!methodInfo.IsStatic)
                        throw new GossipScriptException("Method to create an Action must be static");

                    if (methodInfo.ReturnType != typeof (void))
                        throw new GossipScriptException("Method to create an Action must return void");

                    Type[] interfaces = gossipScriptActionAttribute.ActionType.GetInterfaces();
                    if (!interfaces.Contains(typeof (ICustomActionNode)))
                        throw new GossipScriptException("Action must inherit from ICustomActionNode");

                    // Register with the lexer
                    string key = "." + gossipScriptActionAttribute.Name;
                    var token = new TokenDefinition(TokenType.ReferenceAction, OperationType.MethodCall, new Regex(key));
                    m_Lexer.AddDefinition(token);

                    if (methodInfo.ReturnType != typeof (void))
                        throw new GossipScriptException("Expected return type for Action is Void");

                    m_ActionBindingTable.Register(gossipScriptActionAttribute.Name, type,
                                                  gossipScriptActionAttribute.ActionType, methodInfo);
                }
            }
        }

        protected void RegisterMethods(Type type, TokenType tokenType = TokenType.MethodCall)
        {
            MethodInfo[] methods = type.GetMethods();
            foreach (MethodInfo methodInfo in methods)
            {
                object[] attributes = methodInfo.GetCustomAttributes(typeof (GossipScriptMethod), false);
                if (attributes.Any())
                {
                    GossipScriptMethod gossipScriptMethod = attributes.Cast<GossipScriptMethod>().Single();
                    string key = "." + gossipScriptMethod.MethodName;
                    var token = new TokenDefinition(tokenType, OperationType.PropertyCall, new Regex(key));
                    m_Lexer.AddDefinition(token);

                    if (methodInfo.ReturnType == typeof (void))
                        throw new GossipScriptException(String.Format("Method:{0} cannot return void", key));

                    m_MethodBindingTable.RegisterMethod(gossipScriptMethod.MethodName, type, methodInfo);
                }
            }
        }

        protected void RegisterProperties(Type type, TokenType tokenType = TokenType.ReferencePropertyGet)
        {
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                object[] attributes = propertyInfo.GetCustomAttributes(typeof (GossipScriptProperty), false);
                if (attributes.Any())
                {
                    GossipScriptProperty gossipScriptProperty = attributes.Cast<GossipScriptProperty>().Single();
                    if (gossipScriptProperty.Usage == PropertyUsage.ReadOnly)
                    {
                        RegisterReferencePropertyGet(gossipScriptProperty.Propertyname, propertyInfo, type, tokenType);
                    }

                    if (gossipScriptProperty.Usage == PropertyUsage.ReadWrite)
                    {
                        RegisterReferencePropertyGet(gossipScriptProperty.Propertyname, propertyInfo, type, tokenType);
                        RegisterReferencePropertySet(gossipScriptProperty.Propertyname, propertyInfo, type, tokenType);
                    }
                }
            }
        }

        private void RegisterReferencePropertyGet(string propertyName, PropertyInfo propertyInfo, Type targetType,
                                                  TokenType tokenType)
        {
            // Tell the lexer
            string key = "." + propertyName;
            var token = new TokenDefinition(tokenType, OperationType.PropertyCall, new Regex(key));
            m_Lexer.AddDefinition(token);


            MethodInfo methodInfo = propertyInfo.GetGetMethod();
            if (methodInfo == null)
                throw new GossipScriptException(String.Format("No get method found for property:{0} on type:{1}",
                                                              propertyName, targetType));

            bool ignoreProperty = Attribute.IsDefined(propertyInfo, typeof (IgnoreRegistrationAttribute));
            if (ignoreProperty)
                throw new GossipScriptException("Property has Ignore registration attribute");

            m_PropertyBindingTable.RegisterPropertyGet(key, targetType, methodInfo);
        }

        private void RegisterReferencePropertySet(string propertyName, PropertyInfo propertyInfo, Type targetType,
                                                  TokenType tokenType)
        {
            // The lexer should already know
            bool getExists = m_Lexer.HasDefinition(tokenType, OperationType.PropertyCall);
            if (getExists == false)
                throw new GossipScriptException("Must register get before set");

            MethodInfo setmethod = propertyInfo.GetSetMethod();
            if (setmethod == null)
                throw new GossipScriptException(String.Format("No set method found for property:{0} on type:{1}",
                                                              propertyName, targetType));

            bool ignoreProperty = Attribute.IsDefined(propertyInfo, typeof (IgnoreRegistrationAttribute));
            if (ignoreProperty)
                throw new GossipScriptException("Property has Ignore registration attribute");

            string key = "." + propertyName;
            m_PropertyBindingTable.RegisterPropertySet(key, targetType, setmethod);
        }

        public void RegisterType(Type type)
        {
            bool correctType = type.GetCustomAttributes(typeof (GossipScriptType), false).Any();
            if (correctType == false)
                throw new GossipScriptException(
                    String.Format("Type:{0} requires a GossipScriptTypeAttribute on class definition", type));

            // Register methods
            RegisterMethods(type);
            RegisterActions(type);
            RegisterProperties(type);
        }
    }
}