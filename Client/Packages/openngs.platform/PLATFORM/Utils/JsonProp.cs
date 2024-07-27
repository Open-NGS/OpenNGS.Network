using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace OpenNGS.Platform
{
    [AttributeUsage(AttributeTargets.Property)]
    public class JsonProp : Attribute
    {
        public readonly string Name;

        public JsonProp(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class JsonListProp : Attribute
    {
        public readonly Type ElementType;

        public JsonListProp(Type elementType)
        {
            ElementType = elementType;
        }
    }

    public class JsonSerializable
    {
        protected JsonSerializable()
        {
        }

        protected JsonSerializable(object json)
        {
            Fill(json);
        }

        protected JsonSerializable(string json) : this(MiniJSON.Json.Deserialize(json))
        {
        }

        public Dictionary<string, object> JsonDict
        {
            get
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();
                foreach (var property in JsonInfo.GetInfo(this))
                {
                    object[] props = property.GetCustomAttributes(typeof(JsonProp), true);
                    if (props == null || props.Length < 1) continue;
                    object[] listProps = property.GetCustomAttributes(typeof(JsonListProp), true);
                    JsonProp prop = props[0] as JsonProp;
                    JsonListProp list = null;
                    if (listProps != null && listProps.Length > 0) list = (JsonListProp) listProps[0];
                    if (prop == null) continue;
                    object fieldValue = null;
                    try
                    {
                        fieldValue = property.GetValue(this, null);
                    }
                    catch (TargetException e)
                    {
                        PlatformLog.Log(e.Message);
                    }
                    catch (NotSupportedException e)
                    {
                        PlatformLog.Log(e.Message);
                    }
                    catch (FieldAccessException e)
                    {
                        PlatformLog.Log(e.Message);
                    }
                    catch (ArgumentException e)
                    {
                        PlatformLog.Log(e.Message);
                    }

                    if (fieldValue == null) continue;
                    if (list != null)
                    {
                        IList listValue = fieldValue as IList;
                        if (listValue == null) continue;
                        dict[property.Name] = listValue;
                    }
                    else if (typeof(JsonSerializable).IsAssignableFrom(property.PropertyType))
                    {
                        dict[property.Name] = (fieldValue as JsonSerializable).JsonDict;
                    }
                    else
                    {
                        dict[property.Name] = fieldValue;
                    }
                }

                return dict;
            }
        }

        public Dictionary<string, object> UnityJsonDict
        {
            get
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();
                foreach (var field in JsonInfo.GetInfo(this))
                {
                    object[] props = field.GetCustomAttributes(typeof(JsonProp), true);
                    if (props == null || props.Length < 1) continue;
                    object[] listProps = field.GetCustomAttributes(typeof(JsonListProp), true);
                    JsonProp prop = props[0] as JsonProp;
                    JsonListProp list = null;
                    if (listProps != null && listProps.Length > 0) list = (JsonListProp) listProps[0];
                    if (prop == null) continue;
                    object fieldValue = null;
                    try
                    {
                        fieldValue = field.GetValue(this, null);
                    }
                    catch (TargetException e)
                    {
                        PlatformLog.Log(e.Message);
                    }
                    catch (NotSupportedException e)
                    {
                        PlatformLog.Log(e.Message);
                    }
                    catch (FieldAccessException e)
                    {
                        PlatformLog.Log(e.Message);
                    }
                    catch (ArgumentException e)
                    {
                        PlatformLog.Log(e.Message);
                    }

                    if (fieldValue == null) continue;
                    if (list != null)
                    {
                        IList listValue = fieldValue as IList;
                        if (listValue == null) continue;
                        dict[prop.Name] = listValue;
                    }
                    else if (typeof(JsonSerializable).IsAssignableFrom(field.PropertyType))
                    {
                        dict[prop.Name] = (fieldValue as JsonSerializable).UnityJsonDict;
                    }
                    else
                    {
                        dict[prop.Name] = fieldValue;
                    }
                }

                return dict;
            }
        }

        public override string ToString()
        {
            return MiniJSON.Json.Serialize(UnityJsonDict);
        }

        public string ToJsonString()
        {
            return MiniJSON.Json.Serialize(JsonDict);
        }

        public void Fill(string json)
        {
            Fill(MiniJSON.Json.Deserialize(json));
        }

        public void Fill(object json)
        {
            if (json == null)
                throw new ArgumentNullException("json cannot be null");
            var dict = json as Dictionary<string, object>;
            if (dict == null)
                throw new ArgumentException("we take Dictionary<string,object> only");
            foreach (var property in JsonInfo.GetInfo(this))
            {
                object[] props = property.GetCustomAttributes(typeof(JsonProp), true);
                if (props == null || props.Length < 1) continue;
                object[] listProps = property.GetCustomAttributes(typeof(JsonListProp), true);
                JsonProp prop = props[0] as JsonProp;
                JsonListProp list = null;
                if (listProps != null && listProps.Length > 0) list = (JsonListProp) listProps[0];
                if (prop == null) continue;
                try
                {
                    object value;
                    if (dict.TryGetValue(prop.Name, out value))
                    {
                        if (list != null)
                        {
                            var iList = value as IList;
                            if (iList == null)
                                continue; // iList = (IList)Activator.CreateInstance(typeof(List<object>));
                            object currentList = property.GetValue(this, null);//property.GetValue(this);
                            IList castedList = null;
                            if (currentList == null) castedList = (IList) Activator.CreateInstance(property.PropertyType);
                            if (castedList == null) continue;
                            foreach (object item in iList)
                            {
                                //object tmpJson = MiniJSON.Json.Deserialize((string)item);
                                object tmpJson = item;
                                //Debug.Log("type : " + tmpJson.GetType() + ", " + tmpJson.ToString());
                                if (tmpJson != null &&
                                    typeof(Dictionary<string, object>).IsAssignableFrom(tmpJson.GetType()))
                                {
                                    //Debug.Log("item : " + item);
                                    object convt =
                                        Activator.CreateInstance(list.ElementType, new System.Object[] {item});
                                    castedList.Add(convt);
                                }
                                else
                                {
                                    castedList.Add(Convert.ChangeType(item, list.ElementType));
                                }
                            }

                            property.SetValue(this, castedList, null);
                        }
                        else if (typeof(JsonSerializable).IsAssignableFrom(property.PropertyType))
                        {
                            object convertedValue = JsonInfo.InstantiateType(property.PropertyType, value);
                            if (convertedValue == null) continue;
                            property.SetValue(this, convertedValue, null);
                        }
                        else
                        {
                            property.SetValue(this, Convert.ChangeType(value, property.PropertyType), null);
                        }
                    }
                }
                catch (FieldAccessException e)
                {
                    PlatformLog.Log(e.Message);
                }
                catch (TargetException e)
                {
                    PlatformLog.Log(e.Message);
                }
                catch (ArgumentException e)
                {
                    PlatformLog.Log(e.Message);
                }
            }
        }
    }

    public class JsonInfo
    {
        private static readonly Dictionary<Type, LinkedList<PropertyInfo>> JsonFields = new Dictionary<Type, LinkedList<PropertyInfo>>();
        private static readonly Dictionary<Type, ConstructorInfo> Constructors = new Dictionary<Type, ConstructorInfo>();

        public static LinkedList<PropertyInfo> GetInfo(object target)
        {
            var targetType = target.GetType();
            if (!typeof(JsonSerializable).IsAssignableFrom(targetType)) return new LinkedList<PropertyInfo>();
            if (JsonFields.ContainsKey(targetType))
            {
                return JsonFields[targetType];
            }

            var list = new LinkedList<PropertyInfo>();
            foreach (var property in targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!property.CanRead || !property.CanWrite) continue;
                var attrs = property.GetCustomAttributes(typeof(JsonProp), true);
                if (attrs != null && attrs.Length >= 1) list.AddLast(new LinkedListNode<PropertyInfo>(property));
            }

            JsonFields[targetType] = list;
            return list;
        }

        public static object InstantiateType(Type type, object value)
        {
            if (!typeof(JsonSerializable).IsAssignableFrom(type))
                return null;
            if (Constructors.ContainsKey(type))
            {
                return Constructors[type].Invoke(new object[] {value});
            }

            ConstructorInfo cons = type.GetConstructor(new Type[] {typeof(object)});
            if (cons == null)
                return null;
            Constructors[type] = cons;
            return cons.Invoke(new object[] {value});
        }
    }
}
