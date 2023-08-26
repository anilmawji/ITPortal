namespace ITPortal.Lib.Services.Event
{
    public static class EventHandlerExtensions
    {
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
