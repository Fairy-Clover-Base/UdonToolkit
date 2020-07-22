﻿#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UdonSharp;
using UnityEditor;
using UnityEngine;
using VRC.Udon;
using VRC.Udon.Serialization.OdinSerializer.Utilities;

namespace UdonToolkit {
  public static class UTUtils {
    public static GameObject CreateObjectWithComponents(GameObject target, string name, Type[] components) {
      // check if object exists
      var obj = target.transform.Find(name).gameObject;
      if (obj != null) {
        // check if component exists on it
        foreach (var component in components) {
          if (obj.GetComponent(component) == null) {
            obj.AddComponent(component);
          }
        }

        return obj;
      }
      obj = new GameObject(name, components);
      obj.transform.SetParent(target.transform);
      obj.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
      return obj;
    }
    
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)       
      => self.Select((item, index) => (item, index));
    
    public static IList<T> Swap<T>(this IList<T> list, int indexA, int indexB)
    {
      T tmp = list[indexA];
      list[indexA] = list[indexB];
      list[indexB] = tmp;
      return list;
    }

    public static object GetValueThroughAttribute(SerializedProperty property, string methodName, out Type type) {
      if (methodName.StartsWith("@")) {
        var startIndex = 1;
        if (methodName.IndexOf("!") > -1) startIndex = 2;
        var methodActual = methodName.Substring(startIndex);
        var val = property.serializedObject.targetObject.GetType().GetField(methodActual);
        type = val.FieldType.GetElementType();
        return val.GetValue(property.serializedObject.targetObject);
      }

      var method = property.serializedObject.targetObject.GetType().GetMethod(methodName);
      type = method.GetReturnType().GetElementType();
      return method.Invoke(property.serializedObject.targetObject, null);
    }

    public static bool GetVisibleThroughAttribute(SerializedProperty property, string methodName, bool flipValue) {
      var isVisible = true;
      var value = GetValueThroughAttribute(property, methodName, out _);
      isVisible = (bool) value;
      if (methodName.StartsWith("@") && methodName.IndexOf("!") > -1) {
        isVisible = !isVisible;
      }
      isVisible = flipValue ? !isVisible : isVisible;
      return isVisible;
    }

    public static string[] GetAnimatorTriggers(Animator animator) {
      if (animator == null) return new[] {"no triggers found"};
      if (animator.runtimeAnimatorController != null) {
        if (animator.GetCurrentAnimatorStateInfo(0).length == 0) {
          animator.enabled = false;
          animator.enabled = true;
          animator.gameObject.SetActive(true);
        }
        var found = animator.parameters.Where(p => p.type == AnimatorControllerParameterType.Trigger)
          .Select(x => x.name).ToArray();
        if (found.Length > 0) {
          return found;
        }
      }
      return new[] {"no triggers found"};
    }

    public static string[] GetUdonEvents(UdonBehaviour source) {
      var events = new[] {"no events found "};
      if (source != null) {
        var uPa =source.programSource as UdonSharpProgramAsset;
        if (uPa != null) {
          var methods = uPa.sourceCsScript.GetClass().GetMethods();
          var mapped = methods.Where(m => m.Module.Name == "Assembly-CSharp.dll").Select(m => m.Name).ToArray();
          if (mapped.Length > 0) {
            events = mapped;
          }
        }
      }
      return events;
    }
    
    public static T GetPropertyAttribute<T>(SerializedProperty prop) where T : Attribute {
      var attrs = GetPropertyAttributes<T>(prop);
      if (attrs.Length == 0) return null;
      return (T) attrs[0];
    }
    
    public static object[] GetPropertyAttributes(SerializedProperty prop) {
      return GetPropertyAttributes<PropertyAttribute>(prop);
    }

    public static object[] GetPropertyAttributes<T>(SerializedProperty prop) where T : Attribute {
      const BindingFlags flags = BindingFlags.GetField
                                 | BindingFlags.GetProperty
                                 | BindingFlags.IgnoreCase
                                 | BindingFlags.Instance
                                 | BindingFlags.NonPublic
                                 | BindingFlags.Public;
      if (prop.serializedObject.targetObject == null) return null;
      var tType = prop.serializedObject.targetObject.GetType();
      var field = tType.GetField(prop.name, flags);
      return field != null ? field.GetCustomAttributes(typeof(T), true) : null;
    }
  }
}

#endif