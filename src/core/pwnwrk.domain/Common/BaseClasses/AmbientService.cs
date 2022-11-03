namespace pwnwrk.domain.Common.BaseClasses;

public interface IAmbientService
{

}

public delegate IAmbientService AmbientServiceFactory();

public static class AmbientService<TService>
    where TService: IAmbientService
{
    private static AmbientServiceFactory _factory { get; set; }
    private static TService _instance;

    public static void SetFactory(AmbientServiceFactory factory)
    {
        _factory = factory;
    }

    public static TService Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }

            if (_factory == null)
            {
                throw new Exception($"AmbientService<{typeof(TService).Name}> not configured.");
            }

            _instance = (TService) _factory.Invoke();

            return _instance;
        }
        private set
        {
            if (value == null)
                throw new ArgumentException("Null service instance", nameof(value));

            _instance = value;
        }
    }
}