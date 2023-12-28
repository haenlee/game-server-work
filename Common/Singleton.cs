namespace Common
{
    public class Singleton<T> where T : new()
    {
        private static T instance = default!;

        public static T Instance
        {
            get
            { 
                return Equals(instance, default(T)) ? (instance = new T()) : instance;
            }
        }

        public static void Destroy()
        {
            instance = default!;
        }
    }
}
