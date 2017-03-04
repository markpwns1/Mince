using Mince.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mince
{
    public static class Cloner
    {
        public static T Clone<T>(T original)
        {
            if (original == null)
            {
                return original;
            }

            Type t = original.GetType();

            if (t == typeof(ExposedVariable))
            {
                return original;
            }

            if (original is MinceClonable)
            {
                T cloned = (T)(object)(original as MinceClonable).clone();
                return cloned;
            }
            /*else
            {
                Console.WriteLine(t);
            }*/

            if (t == typeof(string) ||
                t.IsPrimitive ||
                t.IsValueType ||
                typeof(MulticastDelegate).IsAssignableFrom(t.BaseType) ||
                t.Name == "RuntimePropertyInfo" ||
                t == typeof(MinceFunction))
            {
                //Console.WriteLine(depth + ":" + original.ToString());
                return original;
            }

            if (t.IsArray)
            {
                var newArray = (T)Activator.CreateInstance(t, new object[] { (original as Array).Length });

                for (int i = 0; i < (newArray as Array).Length; i++)
                {
                    (newArray as Array).SetValue(Clone((original as Array).GetValue(i)), i);
                }

                //Console.WriteLine(depth + ":" + original.ToString());
                return newArray;
            }

            if (original is IList && t.IsGenericType)
            {
                IList newList = (IList)typeof(List<>).MakeGenericType(t.GenericTypeArguments[0]).GetConstructor(Type.EmptyTypes).Invoke(null);
                IList originalList = (IList)original;

                for (int i = 0; i < originalList.Count; i++)
                {
                    newList.Add(Clone(originalList[i]));
                }

                return (T)newList;
            }

            T copy;

            try
            {
                copy = (T)Activator.CreateInstance(t);
            }
            catch (MissingMethodException)
            {
                Console.WriteLine("Could not instantiate " + original.GetType().Name + " because it has no parameterless constructors.");
                Console.WriteLine("Contact the creator of " + original.GetType().Namespace + " and tell them to add a parameterless constructor to " + original.GetType().Name);
                return original;
            }

            //Console.WriteLine(t.GetFields().Length);

            foreach (var field in t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (original is MinceObject && field.Name == "members")
                {
                    continue;
                }

                var originalValue = Clone(field.GetValue(original));
                field.SetValue(copy, originalValue);
            }

            //Console.WriteLine(depth + ":" + original.ToString());
            return copy;
        }
    }
}
