using System;
using System.Collections.Generic;
using System.IO;

namespace NewMediaPlayer.Generic
{

    public static class Extended
    {
        public static string FormateEx(this string target,params object[] s)
        {
            return string.Format(target,s);
        }

        public static bool AvailableEx(this string target)
        {
            return !(String.IsNullOrEmpty(target) || String.IsNullOrWhiteSpace(target));
        }

        public static bool DExist(this string path,FType ft)
        {
            if (ft == FType.FILE)
                return File.Exists(path);
            else
                return Directory.Exists(path);
        }

        /// <summary>
        /// 对Dictinary字典类进行遍历操作
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="d"></param>
        /// <param name="Act"></param>
        public static void ForEach<TKey,TValue>(this IDictionary<TKey, TValue> d,Action<TKey, TValue> Act)
        {
            foreach(var i in d)
            {
                Act(i.Key, i.Value);
            }
        }

        public static bool True4All<TKey,TValue>(this IDictionary<TKey,TValue> d,Func<TKey,TValue,bool> proc)
        {
            foreach(var i in d)
            {
                if (!proc(i.Key, i.Value)) return false;
            }
            return true;
        }

        public static bool Add4nRep<TKey,TValue>(this IDictionary<TKey,TValue> d,TKey key,TValue val)
        {
            if (!d.ContainsKey(key))
            {
                d.Add(key, val);
                return true;
            }
            return false;
        }

        public static T Possible<T>(this T[] a,Func<T,bool> condition)
        {
            foreach(T t in a)
            {
                if (condition(t))
                    return t;
            }
            return default(T);
        }

        public static bool Contains<T>(this T[] a, Func<T, bool> condition)
        {
            foreach (T t in a)
            {
                if (condition(t))
                    return true;
            }
            return false;
        }
    }

    public enum FType
    {
        FILE,
        DICT
    }
}
