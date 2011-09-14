using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;
using System.Collections.ObjectModel;

namespace MayhemCore
{
    public class ModuleTypeResolver : DataContractResolver
    {
        ICollection<Type> types;

        public ModuleTypeResolver()
        {
        }
        public ModuleTypeResolver(ICollection<Type> types)
        {
            this.types = types;
        }

        // Used at deserialization
        public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver)
        {
            Type t = knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType, null);
            if (t != null)
                return t;

            foreach (Type type in types)
            {
                if (type.FullName == typeName && type.Assembly.FullName == typeNamespace)
                    return type;
            }
            return null;
        }

        // Used at serialization
        public override bool TryResolveType(Type type, Type declaredType, DataContractResolver knownTypeResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (declaredType == null)
            {
                throw new ArgumentNullException("declaredType");
            }

            string name = type.FullName;
            string namesp = type.Assembly.FullName;
            typeName = new XmlDictionaryString(XmlDictionary.Empty, name, 0);
            typeNamespace = new XmlDictionaryString(XmlDictionary.Empty, namesp, 0);
            return true;
        }
    }
}
