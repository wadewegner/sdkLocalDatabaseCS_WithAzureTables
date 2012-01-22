using Microsoft.Phone.Shell;

namespace sdkLocalDatabaseCS
{
    public static class ApplicationStateHelpers
    {
        public static T Get<T>(string key)
        {
            if (!PhoneApplicationService.Current.State.ContainsKey(key))
                return default(T);

            return (T)PhoneApplicationService.Current.State[key];
        }

        public static void Set(string key, object value)
        {
            if (PhoneApplicationService.Current.State.ContainsKey(key))
                PhoneApplicationService.Current.State.Remove(key);

            PhoneApplicationService.Current.State.Add(key, value);
        }

        public static void Remove(string key)
        {
            if (PhoneApplicationService.Current.State.ContainsKey(key))
                PhoneApplicationService.Current.State.Remove(key);
        }

        public static bool Contains(string key)
        {
            return PhoneApplicationService.Current.State.ContainsKey(key);
        }
    }
}
