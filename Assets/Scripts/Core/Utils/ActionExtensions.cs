using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Runner.Core.Utils
{
    public static class ActionExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SafeInvoke(this Action action) 
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SafeInvoke<T>(this Action<T> action, T arg) 
        {
            try
            {
                action?.Invoke(arg);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SafeInvoke<T, T1>(this Action<T, T1> action, T arg, T1 arg1) 
        {
            try
            {
                action?.Invoke(arg, arg1);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SafeInvoke<T, T1, T2>(this Action<T, T1, T2> action, T arg, T1 arg1, T2 arg2) 
        {
            try
            {
                action?.Invoke(arg, arg1, arg2);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SafeInvoke<T, T1, T2, T3>(this Action<T, T1, T2, T3> action, T arg, T1 arg1, T2 arg2, T3 arg3) 
        {
            try
            {
                action?.Invoke(arg, arg1, arg2, arg3);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}
