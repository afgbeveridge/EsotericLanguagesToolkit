using System;
using System.Linq;

namespace Interpreter.Abstractions {
        public static class TypeExtender {
                private const string GenericSeparator = "`";

                /// <summary>
                ///         Given a type name, an assembly name and some type parameters, attempt to instantiate a generic type
                /// </summary>
                public static Type FormGenericType(this Type pType, string pName, Type[] pTypeArguments,
                        string pAssName = null) {
                        var unboundType = LocateGenericType(pType, pName, pAssName, pTypeArguments);
                        ExecutionSupport.AssertNotNull(unboundType, string.Format("Cannot find base {0}", pName));
                        pTypeArguments.ToList().ForEach(arg => ExecutionSupport.AssertNotNull(arg,
                                string.Format("Null type arg passed for unboundType {0}", unboundType)));
                        return unboundType.MakeGenericType(pTypeArguments);
                }

                public static Type LocateGenericType(this Type pType, string pName, string pAssName,
                        Type[] pTypeArguments) {
                        ExecutionSupport.AssertNotNull(pName, "Cannot form a generic type from nothing");
                        ExecutionSupport.Assert(pTypeArguments != null && pTypeArguments.Length > 0,
                                "No or invalid type arguments supplied");
                        var mangledTypeName =
                                string.Format("{0}{1}{2}", pName, GenericSeparator, pTypeArguments.Length);
                        if (!string.IsNullOrEmpty(pAssName))
                                mangledTypeName = string.Format("{0},{1}", mangledTypeName, pAssName);

                        var unboundType = Type.GetType(mangledTypeName);
                        return unboundType;
                }

                // TODO: Fold into previous method
                public static string CreateMangledTypeName(this Type pType, string pName, int pNumArguments) {
                        ExecutionSupport.AssertNotNull(pName, "Cannot form a generic type from nothing");
                        ExecutionSupport.Assert(pNumArguments > 0, "No or invalid type arguments supplied");
                        return string.Format("{0}{1}{2}", pName, GenericSeparator, pNumArguments);
                }

                /// <summary>
                ///         Instantiate an object of type T given arguments that describe a generic type
                /// </summary>
                public static T InstantiateGenericType<T>(this Type pType, string pName, Type[] pTypeArguments,
                        string pAssName = null) {
                        var boundType = FormGenericType(pType, pName, pTypeArguments, pAssName);
                        ExecutionSupport.AssertNotNull(boundType,
                                string.Format("Cannot form generic type from base {0}", pName));
                        return (T) Activator.CreateInstance(boundType);
                }

                /// <summary>
                ///         Instantiate an object given arguments that describe a generic type (including assembly name)
                /// </summary>
                public static object InstantiateGenericType(this Type pType, string pName, string pAssName,
                        Type[] pTypeArguments) {
                        var boundType = FormGenericType(pType, pName, pTypeArguments, pAssName);
                        ExecutionSupport.AssertNotNull(boundType,
                                string.Format("Cannot form generic type from base {0}", pName));
                        return Activator.CreateInstance(boundType);
                }
        }
}