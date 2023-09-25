namespace ITPortal.Lib.Utility
{
    public static class EventHandlerExtensions
    {
        // Should only be used in cases where the consumer does not have explicit access to event delegates
        // Otherwise, -= is preferred to dispose of delegates
        public static void DisposeSubscriptions<T>(this EventHandler<T>? handler)
        {
            if (handler == null) return;

            foreach (Delegate d in handler.GetInvocationList())
            {
                handler -= (EventHandler<T>)d;
            }
        }
    }
}
