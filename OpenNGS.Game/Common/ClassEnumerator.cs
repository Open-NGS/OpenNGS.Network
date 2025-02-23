using System;
using System.Collections.Generic;
using System.Reflection;

namespace InnovFramework
{
    public class ClassEnumerator
    {
        List<Type> _Groups = new List<Type>();
        public List<Type> Groups
        {
            get { return _Groups; }
        }

        Type AttributeType;
        Type InterfaceType;

        public ClassEnumerator(Type attributetype, Type interfacetype,
            Assembly assembly, bool bInheritAttribute = false, bool bSearchMultiAssembly = false)
        {
            AttributeType = attributetype;
            InterfaceType = interfacetype;

            try
            {
                if (bSearchMultiAssembly)
                {
                    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    if (assemblies != null)
                    {
                        for (int i = 0; i < assemblies.Length; i++)
                        {
                            ParseAssembly(assemblies[i], bInheritAttribute);

                        }
                    }
                }
                else
                    ParseAssembly(assembly, bInheritAttribute);
            }
            catch (Exception e)
            {
                NgDebug.LogError("Parse Assembly error:" + e.Message);

            }
        }

        void ParseAssembly(Assembly assembly, bool bInheritAttribute)
        {
            Type[] types = assembly.GetTypes();
            if (types != null)
            {
                for (int i = 0; i < types.Length; i++)
                {
                    var type = types[i];
                    if (InterfaceType == null || InterfaceType.IsAssignableFrom(type))
                    {
                        if (!type.IsAbstract)
                        {
                            if (type.GetCustomAttributes(AttributeType, bInheritAttribute).Length > 0)
                                _Groups.Add(type);
                        }
                    }
                }
            }
        }

    }
}
