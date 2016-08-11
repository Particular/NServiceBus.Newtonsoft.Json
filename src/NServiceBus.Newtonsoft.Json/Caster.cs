namespace NServiceBus.Newtonsoft.Json
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq.Expressions;
    using ObjectFunc = System.Func<object, object>;

    static class Caster
    {
        static ConcurrentDictionary<RuntimeTypeHandle, ObjectFunc> funcs = new ConcurrentDictionary<RuntimeTypeHandle, Func<object, object>>();

        public static object Cast(this object data, Type targetType)
        {
            var run = funcs.GetOrAdd(targetType.TypeHandle, BuildFunc(targetType));
            return run(data);
        }

        static ObjectFunc BuildFunc(Type targetType)
        {
            var dataParam = Expression.Parameter(typeof(object));
            var body = Expression.Block(Expression.Convert(dataParam, targetType));
            return Expression.Lambda<ObjectFunc>(body, dataParam).Compile();
        }
    }
}