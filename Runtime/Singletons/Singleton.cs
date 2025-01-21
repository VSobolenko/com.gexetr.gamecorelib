namespace Game.Singletons
{
    public class Singleton<T> : ISingleton where T : Singleton<T>, new()
    {
        private static T _instance;

        private SingletonInitializationStatus _initializationStatus = SingletonInitializationStatus.None;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (typeof(T))
                    {
                        if (_instance == null)
                        {
                            _instance = new T();
                            _instance.InitializeSingleton();
                        }
                    }
                }

                return _instance;
            }
        }

        public virtual bool IsInitialized => _initializationStatus == SingletonInitializationStatus.Initialized;
        
        protected virtual void OnInitializing() { }

        protected virtual void OnInitialized() { }

        public virtual void InitializeSingleton()
        {
            if (_initializationStatus != SingletonInitializationStatus.None)
            {
                return;
            }

            _initializationStatus = SingletonInitializationStatus.Initializing;
            OnInitializing();
            _initializationStatus = SingletonInitializationStatus.Initialized;
            OnInitialized();
        }

        public virtual void ClearSingleton() { }

        public static void CreateInstance()
        {
            DestroyInstance();
            _instance = Instance;
        }

        public static void DestroyInstance()
        {
            if (_instance == null)
            {
                return;
            }

            _instance.ClearSingleton();
            _instance = default;
        }
    }
}