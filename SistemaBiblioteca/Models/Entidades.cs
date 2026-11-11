namespace SistemaBiblioteca.Models
{
    /// <summary>
    /// Clase base abstracta (herencia): toda entidad persistente tiene un identificador.
    /// </summary>
    public abstract class Entidad
    {
        public int Id { get; set; }
    }

    /// <summary>
    /// Clase base para personas (Autor y Usuario). Aplica herencia y polimorfismo
    /// mediante el método virtual NombreCompleto().
    /// </summary>
    public abstract class Persona : Entidad
    {
        private string _nombre = string.Empty;
        private string _apellido = string.Empty;

        public string Nombre
        {
            get => _nombre;
            set => _nombre = (value ?? string.Empty).Trim();
        }

        public string Apellido
        {
            get => _apellido;
            set => _apellido = (value ?? string.Empty).Trim();
        }

        public virtual string NombreCompleto() => $"{Nombre} {Apellido}".Trim();

        public override string ToString() => NombreCompleto();
    }

    public class Autor : Persona
    {
        public string Nacionalidad { get; set; }

        public Autor() { }

        public Autor(int id, string nombre, string apellido, string nacionalidad)
        {
            Id = id;
            Nombre = nombre;
            Apellido = apellido;
            Nacionalidad = nacionalidad;
        }
    }

    public class Usuario : Persona
    {
        public string Documento { get; set; } = string.Empty;
        public string Telefono { get; set; }
        public string Correo { get; set; }

        public Usuario() { }

        public Usuario(int id, string nombre, string apellido, string documento, string telefono, string correo)
        {
            Id = id;
            Nombre = nombre;
            Apellido = apellido;
            Documento = documento;
            Telefono = telefono;
            Correo = correo;
        }

        // Polimorfismo: el usuario se muestra con su documento en los ComboBox.
        public override string NombreCompleto() => $"{base.NombreCompleto()} ({Documento})";
    }

    public class Editorial : Entidad
    {
        public string Nombre { get; set; } = string.Empty;
        public string Pais { get; set; }

        public Editorial() { }

        public Editorial(int id, string nombre, string pais)
        {
            Id = id;
            Nombre = nombre;
            Pais = pais;
        }

        public override string ToString() => Nombre;
    }

    public class Libro : Entidad
    {
        public string ISBN { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public int IdAutor { get; set; }
        public int IdEditorial { get; set; }
        public int AnioPublicacion { get; set; }
        public int Existencias { get; set; }

        // Propiedades de solo lectura cargadas por JOIN (para mostrar en grillas)
        public string AutorNombre { get; set; }
        public string EditorialNombre { get; set; }

        public bool Disponible => Existencias > 0;

        public override string ToString() => $"{Titulo} [{ISBN}]";
    }

    public class Prestamo : Entidad
    {
        public const string EstadoPrestado = "Prestado";
        public const string EstadoDevuelto = "Devuelto";

        public int IdUsuario { get; set; }
        public int IdLibro { get; set; }
        public DateTime FechaPrestamo { get; set; } = DateTime.Today;
        public DateTime? FechaDevolucion { get; set; }
        public string Estado { get; set; } = EstadoPrestado;

        // Datos de visualización (vista vw_PrestamosDetalle)
        public string UsuarioNombre { get; set; }
        public string LibroTitulo { get; set; }
    }
}
