using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class PocoListenerUtils
{
    public static void SubscribePocoListeners(RPCParser rpc, PocoListenersBase listeners)
    {
        var methods = listeners.GetType()
            .GetMethods(BindingFlags.Public | BindingFlags.Instance);

        var uniqueListeners = new HashSet<string>();

        foreach (var method in methods)
        {
            var attribute = method.GetCustomAttribute<PocoMethodAttribute>();

            if (attribute != null)
            {
                rpc.addListener(listeners, attribute.Name, method);

                if (uniqueListeners.Add(attribute.Name) == false)
                {
                    Debug.LogError($"Attempt to add non-unique Poco listener: `{attribute.Name}`, " +
                                   $"please check attributes of listeners at `{listeners.GetType().Name}`");
                }
            }
        }
    }

    public static object HandleInvocation(
        Dictionary<string, (object instance, MethodInfo method)> listeners,
        Dictionary<string, object> data)
    {
        if (listeners == null)
        {
            throw new ArgumentNullException(
                nameof(listeners),
                "To use `poco.invoke()`, please assign object " +
                $"of class derived from {nameof(PocoListenersBase)} " +
                $"to field at `{nameof(PocoManager)}`");
        }

        Dictionary<string, object> paramsObject = (Dictionary<string, object>)data["params"];

        var listener = (string)paramsObject["listener"];

        if (listeners.TryGetValue(listener, out var listenerPair) == false)
        {
            throw new NotImplementedException(
                $"Listener method for `{listener}` " +
                $"marked with `{nameof(PocoMethodAttribute)}` was not found " +
                $"at `{listeners.GetType().Name}`");
        }

        var (instance, method) = listenerPair;

        var args = GetInvocationArgs(paramsObject, method);

        var result = method.Invoke(instance, args);

        return result;
    }

    private static object[] GetInvocationArgs(Dictionary<string, object> paramsObject, MethodInfo method)
    {
        var parameters = method.GetParameters();

        if(paramsObject.ContainsKey("parameters") == false)
        {
            if (parameters.Length > 0)
            {
                throw new ArgumentException(
                    $"Signature mismatch of method `{method}`: " +
                    "expected 0 arguments in listener, " +
                    $"received {parameters.Length} arguments");
            }

            return Array.Empty<object>();
        }

        Dictionary<string, object> data = (Dictionary<string, object>)paramsObject["data"];

        var args = new List<object>();

        var remainingArgNames = new HashSet<string>(data.Keys);



        foreach (var parameter in parameters)
        {
            var parameterName = parameter.Name;

            var argToken = data[parameterName];

            if (argToken == null)
            {
                throw new ArgumentException(
                    $"Signature mismatch of method `{method}`: " +
                    $"excess parameter `{parameterName}` in listener");
            }

            try
            {

                args.Add( argToken );
                remainingArgNames.Remove(parameterName);
            }
            catch (Exception exception)
            {
                throw new ArgumentException(
                    $"Signature mismatch of method `{method}`: " +
                    $"parameter `{parameterName}` type mismatch: " +
                    $"tried to parse received value `{argToken}`, " +
                    $"with type `{parameter.ParameterType.Name}` at listener",
                    exception);
            }
        }

        if (remainingArgNames.Count > 0)
        {
            throw new ArgumentException(
                $"Signature mismatch of method `{method}`: " +
                $"missing parameters in listener: `{string.Join(", ", remainingArgNames)}`");
        }

        return args.ToArray();
    }
}
