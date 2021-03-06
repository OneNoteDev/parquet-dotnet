﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using Parquet.Data;

namespace Parquet
{
   static class TypeExtensions
   {
      public static bool TryExtractEnumerableType(this Type t, out Type baseType)
      {
         if(typeof(byte[]) == t)
         {
            //it's a special case to avoid confustion between byte arrays and repeatable bytes
            baseType = null;
            return false;
         }

         TypeInfo ti = t.GetTypeInfo();
         Type[] args = ti.GenericTypeArguments;

         if (args.Length == 1)
         {
            //check derived interfaces
            foreach (Type interfaceType in ti.ImplementedInterfaces)
            {
               TypeInfo iti = interfaceType.GetTypeInfo();
               if (iti.IsGenericType && iti.GetGenericTypeDefinition() == typeof(IEnumerable<>))
               {

                  baseType = ti.GenericTypeArguments[0];
                  return true;
               }
            }

            //check if this is an IEnumerable<>
            if (ti.IsGenericType && ti.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
               baseType = ti.GenericTypeArguments[0];
               return true;
            }
         }

         if(ti.IsArray)
         {
            baseType = ti.GetElementType();
            return true;
         }

         baseType = null;
         return false;
      }

      public static bool TryExtractDictionaryType(this Type t, out Type keyType, out Type valueType)
      {
         TypeInfo ti = t.GetTypeInfo();

         if(ti.IsGenericType && ti.GetGenericTypeDefinition().GetTypeInfo().IsAssignableFrom(typeof(Dictionary<,>).GetTypeInfo()))
         {
            keyType = ti.GenericTypeArguments[0];
            valueType = ti.GenericTypeArguments[1];
            return true;
         }

         keyType = valueType = null;
         return false;
      }

      public static bool IsNullable(this IList list)
      {
         TypeInfo ti = list.GetType().GetTypeInfo();

         Type t = ti.GenericTypeArguments[0];
         Type gt = t.GetTypeInfo().IsGenericType ? t.GetTypeInfo().GetGenericTypeDefinition() : null;

         return gt == typeof(Nullable<>) || t.GetTypeInfo().IsClass;
      }

      public static bool IsNullable(this Type t)
      {
         TypeInfo ti = t.GetTypeInfo();

         return
            ti.IsClass ||
            (ti.IsGenericType && ti.GetGenericTypeDefinition() == typeof(Nullable<>));
      }

      public static Type GetNonNullable(this Type t)
      {
         TypeInfo ti = t.GetTypeInfo();

         if(ti.IsClass)
         {
            return t;
         }
         else
         {
            return ti.GenericTypeArguments[0];
         }
      }

      public static bool IsSimple(this Type t)
      {
         if (t == null) return true;

         return
            t == typeof(bool) ||
            t == typeof(byte) ||
            t == typeof(sbyte) ||
            t == typeof(char) ||
            t == typeof(decimal) ||
            t == typeof(double) ||
            t == typeof(float) ||
            t == typeof(int) ||
            t == typeof(uint) ||
            t == typeof(long) ||
            t == typeof(ulong) ||
            t == typeof(short) ||
            t == typeof(ushort) ||
            t == typeof(TimeSpan) ||
            t == typeof(DateTime) ||
            t == typeof(Guid) ||
            t == typeof(string);
      }

   }
}