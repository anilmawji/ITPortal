namespace ITPortal.Lib.Utilities.Extensions
{
    public static class EventHandlerExtensions
    {
        // Should only be used in cases where the consumer does not have explicit access to event delegates
        // Otherwise, -= is preferred to dispose of delegates
        public static bool DisposeSubscriptions<T>(this EventHandler<T>? handler)
        {
            if (handler != null)
            {
                bool unsubscribed = false;

                foreach (Delegate d in handler.GetInvocationList())
                {
                    handler -= (EventHandler<T>)d;
                    unsubscribed = true;
                }
                return unsubscribed;
            }
            return false;
        }
    }
}
