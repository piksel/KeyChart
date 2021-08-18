using System;

namespace KeyChart.GUI.Util
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Calls the specified function block with this value as its argument and returns its result.
        /// </summary>
        /// <remarks>Shameless ripoff of Kotlin's `.let()`</remarks>
        /// <param name="target"></param>
        /// <param name="block"></param>
        /// <typeparam name="TTarget"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        public static TResult Let<TTarget, TResult>(this TTarget target, Func<TTarget, TResult> block)
            => block(target);
        
        /// <summary>
        /// Calls the specified function block with this value as its argument.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="block"></param>
        /// <typeparam name="TTarget"></typeparam>
        public static void Let<TTarget>(this TTarget target, Action<TTarget> block)
            => block(target);
        
        public static void MutateIf<T>(this T o, Func<T, bool> condition, Action<T> mutateFunc)
            => o.MutateIf(condition(o), mutateFunc);
        
        public static void MutateIf<T>(this T o, bool condition, Action<T> mutateFunc)
        {
            if (condition) return;
            mutateFunc(o);
        }
    }

    public static class LogicStatic
    {
        static BoolState TrueState = new(State: true);
        static BoolState FalseState = new(State: false);
        
        public record BoolState(bool State)
        {
            public bool Do(Action action)
            {
                if(State) action();
                return State;
            }
        }

        public static BoolState Given(bool state) => state ? TrueState : FalseState;
        
        
    }
}