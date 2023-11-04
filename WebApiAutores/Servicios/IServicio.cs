namespace WebApiAutores.Servicios
{
    public interface IServicio
    {
        void RealizarTarea();
        Guid ObtenerTransient();
        Guid ObtenerScoped();
        Guid ObtenerSingleton();
    }

    public class ServicioA : IServicio
    {
        public ServicioA(
            ILogger<ServicioA> logger,
            ServicioTransient servicioTransient,
            ServicioScoped servicioScoped,
            ServicioSingleton servicioSingleton 
            ) 
        {
            Logger = logger;
            ServicioTransient = servicioTransient;
            ServicioScoped = servicioScoped;
            ServicioSingleton = servicioSingleton;
        }

        public Guid ObtenerTransient() { return ServicioTransient.Guid; }
        public Guid ObtenerScoped() { return ServicioScoped.Guid; }
        public Guid ObtenerSingleton() { return ServicioSingleton.Guid; }


        public ILogger<ServicioA> Logger { get; }
        public ServicioTransient ServicioTransient { get; }
        public ServicioScoped ServicioScoped { get; }
        public ServicioSingleton ServicioSingleton { get; }

        public void RealizarTarea()
        { 
        }
    }

    public class ServicioB : IServicio
    {
        public Guid ObtenerTransient() 
        {
            throw new NotImplementedException();
        }
        public Guid ObtenerScoped() 
        {
            throw new NotImplementedException();
        }
        public Guid ObtenerSingleton() 
        {
            throw new NotImplementedException();
        }
        public void RealizarTarea()
        {
        }
    }

    public class ServicioTransient
    {
        public Guid Guid = Guid.NewGuid();
    }

    public class ServicioScoped
    {
        public Guid Guid = Guid.NewGuid();
    }

    public class ServicioSingleton
    {
        public Guid Guid = Guid.NewGuid();
    }
}
