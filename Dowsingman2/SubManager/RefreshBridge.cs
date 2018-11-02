using Dowsingman2.BaseClass;
using System;

namespace Dowsingman2.SubManager
{
    public class RefreshBridge
    {
        public static event Action<AbstractManager> RefreshEvent = delegate { };

        public static void Refresh(AbstractManager manager)
        {
            OnRefreshEvent(manager);
        }

        private static void OnRefreshEvent(AbstractManager manager)
        {
            RefreshEvent?.Invoke(manager);
        }
    }
}
