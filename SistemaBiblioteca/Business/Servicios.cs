using System.Data;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using SistemaBiblioteca.Data;
using SistemaBiblioteca.Models;

namespace SistemaBiblioteca.Business
{
    /// <summary>Excepción para reglas de negocio y validaciones (mensajes aptos para el usuario).</summary>
    public class ReglaNegocioException : Exception
    {
        public ReglaNegocioException(string mensaje) : base(mensaje) { }
    }

    internal static class Validador
    {
        private static readonly Regex CorreoRegex =
            new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public static void Requerido(string valor, string campo, int maxLongitud)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ReglaNegocioException($"El campo '{campo}' es obligatorio.");
            if (valor.Trim().Length > maxLongitud)
                throw new ReglaNegocioException($"El campo '{campo}' no puede superar {maxLongitud} caracteres.");
        }

        public static void Opcional(string valor, string campo, int maxLongitud)
        {
            if (!string.IsNullOrWhiteSpace(valor) && valor.Trim().Length > maxLongitud)
                throw new ReglaNegocioException($"El campo '{campo}' no puede superar {maxLongitud} caracteres.");
        }

        public static bool CorreoValido(string correo) => CorreoRegex.IsMatch(correo);

        public static string LimpiarONulo(string valor) =>
            string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();

        /// <summary>Traduce errores de SQL Server a mensajes claros.</summary>
        public static ReglaNegocioException TraducirSql(SqlException ex, string entidad)
        {
            return ex.Number switch
            {
                547 => new ReglaNegocioException(
                    $"No se puede completar la operación sobre {entidad}: existen registros relacionados."),
                2627 or 2601 => new ReglaNegocioException(
                    $"Ya existe un registro de {entidad} con esos datos (valor duplicado)."),
                _ => new ReglaNegocioException("Error de base de datos: " + ex.Message)
            };
        }
    }

    public class AutorService
    {
        private readonly AutorDAO _dao = new();

        public List<Autor> Listar() => _dao.Listar();

        private void Validar(Autor a)
        {
            Validador.Requerido(a.Nombre, "Nombre", 60);
            Validador.Requerido(a.Apellido, "Apellido", 60);
            Validador.Opcional(a.Nacionalidad, "Nacionalidad", 60);
            a.Nacionalidad = Validador.LimpiarONulo(a.Nacionalidad);
            if (_dao.ExisteNombreCompleto(a.Nombre, a.Apellido, a.Id))
                throw new ReglaNegocioException("Ya existe un autor con ese nombre y apellido.");
        }

        public int Registrar(Autor a)
        {
            Validar(a);
            try { return _dao.Insertar(a); }
            catch (SqlException ex) { throw Validador.TraducirSql(ex, "autor"); }
        }

        public void Actualizar(Autor a)
        {
            if (a.Id <= 0) throw new ReglaNegocioException("Seleccione un autor para modificar.");
            Validar(a);
            try { _dao.Actualizar(a); }
            catch (SqlException ex) { throw Validador.TraducirSql(ex, "autor"); }
        }

        public void Eliminar(int id)
        {
            if (id <= 0) throw new ReglaNegocioException("Seleccione un autor para eliminar.");
            if (_dao.TieneLibros(id))
                throw new ReglaNegocioException("No se puede eliminar el autor porque tiene libros registrados.");
            try { _dao.Eliminar(id); }
            catch (SqlException ex) { throw Validador.TraducirSql(ex, "autor"); }
        }
    }

    public class EditorialService
    {
        private readonly EditorialDAO _dao = new();

        public List<Editorial> Listar() => _dao.Listar();

        private void Validar(Editorial e)
        {
            Validador.Requerido(e.Nombre, "Nombre", 100);
            Validador.Opcional(e.Pais, "País", 60);
            e.Nombre = e.Nombre.Trim();
            e.Pais = Validador.LimpiarONulo(e.Pais);
            if (_dao.ExisteNombre(e.Nombre, e.Id))
                throw new ReglaNegocioException("Ya existe una editorial con ese nombre.");
        }

        public int Registrar(Editorial e)
        {
            Validar(e);
            try { return _dao.Insertar(e); }
            catch (SqlException ex) { throw Validador.TraducirSql(ex, "editorial"); }
        }

        public void Actualizar(Editorial e)
        {
            if (e.Id <= 0) throw new ReglaNegocioException("Seleccione una editorial para modificar.");
            Validar(e);
            try { _dao.Actualizar(e); }
            catch (SqlException ex) { throw Validador.TraducirSql(ex, "editorial"); }
        }

        public void Eliminar(int id)
        {
            if (id <= 0) throw new ReglaNegocioException("Seleccione una editorial para eliminar.");
            if (_dao.TieneLibros(id))
                throw new ReglaNegocioException("No se puede eliminar la editorial porque tiene libros registrados.");
            try { _dao.Eliminar(id); }
            catch (SqlException ex) { throw Validador.TraducirSql(ex, "editorial"); }
        }
    }

    public class UsuarioService
    {
        private readonly UsuarioDAO _dao = new();

        public List<Usuario> Listar() => _dao.Listar();

        private void Validar(Usuario u)
        {
            Validador.Requerido(u.Nombre, "Nombre", 60);
            Validador.Requerido(u.Apellido, "Apellido", 60);
            Validador.Requerido(u.Documento, "Documento", 20);
            Validador.Opcional(u.Telefono, "Teléfono", 20);
            Validador.Opcional(u.Correo, "Correo electrónico", 100);

            u.Documento = u.Documento.Trim();
            if (!u.Documento.All(char.IsLetterOrDigit))
                throw new ReglaNegocioException("El documento solo puede contener letras y números.");

            u.Telefono = Validador.LimpiarONulo(u.Telefono);
            if (u.Telefono != null && !u.Telefono.All(c => char.IsDigit(c) || c == '+' || c == ' ' || c == '-'))
                throw new ReglaNegocioException("El teléfono solo puede contener números, espacios, '+' o '-'.");

            u.Correo = Validador.LimpiarONulo(u.Correo);
            if (u.Correo != null && !Validador.CorreoValido(u.Correo))
                throw new ReglaNegocioException("El formato del correo electrónico no es válido.");

            if (_dao.ExisteDocumento(u.Documento, u.Id))
                throw new ReglaNegocioException("Ya existe un usuario con ese documento.");
        }

        public int Registrar(Usuario u)
        {
            Validar(u);
            try { return _dao.Insertar(u); }
            catch (SqlException ex) { throw Validador.TraducirSql(ex, "usuario"); }
        }

        public void Actualizar(Usuario u)
        {
            if (u.Id <= 0) throw new ReglaNegocioException("Seleccione un usuario para modificar.");
            Validar(u);
            try { _dao.Actualizar(u); }
            catch (SqlException ex) { throw Validador.TraducirSql(ex, "usuario"); }
        }

        public void Eliminar(int id)
        {
            if (id <= 0) throw new ReglaNegocioException("Seleccione un usuario para eliminar.");
            if (_dao.TienePrestamos(id))
                throw new ReglaNegocioException("No se puede eliminar el usuario porque tiene préstamos registrados.");
            try { _dao.Eliminar(id); }
            catch (SqlException ex) { throw Validador.TraducirSql(ex, "usuario"); }
        }
    }

    public class LibroService
    {
        private readonly LibroDAO _dao = new();
        private readonly AutorDAO _autores = new();
        private readonly EditorialDAO _editoriales = new();

        public List<Libro> Listar() => _dao.Listar();

        public List<Libro> Buscar(string texto) =>
            string.IsNullOrWhiteSpace(texto) ? _dao.Listar() : _dao.Buscar(texto.Trim());

        private void Validar(Libro l)
        {
            Validador.Requerido(l.ISBN, "ISBN / Código", 20);
            Validador.Requerido(l.Titulo, "Título", 200);
            l.ISBN = l.ISBN.Trim();
            l.Titulo = l.Titulo.Trim();

            if (l.IdAutor <= 0 || !_autores.Listar().Any(a => a.Id == l.IdAutor))
                throw new ReglaNegocioException("Seleccione un autor existente.");
            if (l.IdEditorial <= 0 || !_editoriales.Listar().Any(e => e.Id == l.IdEditorial))
                throw new ReglaNegocioException("Seleccione una editorial existente.");

            int anioActual = DateTime.Today.Year;
            if (l.AnioPublicacion < 1000 || l.AnioPublicacion > anioActual)
                throw new ReglaNegocioException($"El año de publicación debe estar entre 1000 y {anioActual}.");
            if (l.Existencias < 0)
                throw new ReglaNegocioException("Las existencias no pueden ser negativas.");

            if (_dao.ExisteISBN(l.ISBN, l.Id))
                throw new ReglaNegocioException("Ya existe un libro con ese ISBN / código.");
        }

        public int Registrar(Libro l)
        {
            Validar(l);
            try { return _dao.Insertar(l); }
            catch (SqlException ex) { throw Validador.TraducirSql(ex, "libro"); }
        }

        public void Actualizar(Libro l)
        {
            if (l.Id <= 0) throw new ReglaNegocioException("Seleccione un libro para modificar.");
            Validar(l);
            try { _dao.Actualizar(l); }
            catch (SqlException ex) { throw Validador.TraducirSql(ex, "libro"); }
        }

        public void Eliminar(int id)
        {
            if (id <= 0) throw new ReglaNegocioException("Seleccione un libro para eliminar.");
            if (_dao.TienePrestamos(id))
                throw new ReglaNegocioException("No se puede eliminar el libro porque tiene préstamos asociados.");
            try { _dao.Eliminar(id); }
            catch (SqlException ex) { throw Validador.TraducirSql(ex, "libro"); }
        }
    }

    public class PrestamoService
    {
        private readonly PrestamoDAO _dao = new();
        private readonly UsuarioDAO _usuarios = new();
        private readonly LibroDAO _libros = new();

        public List<Prestamo> Listar(string estado = null) => _dao.Listar(estado);

        public int Registrar(int idUsuario, int idLibro, DateTime fechaPrestamo)
        {
            if (idUsuario <= 0 || !_usuarios.Existe(idUsuario))
                throw new ReglaNegocioException("Seleccione un usuario existente.");

            var libro = idLibro > 0 ? _libros.ObtenerPorId(idLibro) : null;
            if (libro == null)
                throw new ReglaNegocioException("Seleccione un libro existente.");

            if (fechaPrestamo.Date > DateTime.Today)
                throw new ReglaNegocioException("La fecha del préstamo no puede ser futura.");
            if (libro.Existencias <= 0)
                throw new ReglaNegocioException("El libro no tiene existencias disponibles.");
            if (_dao.UsuarioTienePrestamoActivoDelLibro(idUsuario, idLibro))
                throw new ReglaNegocioException("El usuario ya tiene un préstamo activo de este libro.");

            try
            {
                return _dao.Registrar(new Prestamo
                {
                    IdUsuario = idUsuario,
                    IdLibro = idLibro,
                    FechaPrestamo = fechaPrestamo.Date
                });
            }
            catch (InvalidOperationException ex) { throw new ReglaNegocioException(ex.Message); }
            catch (SqlException ex) { throw Validador.TraducirSql(ex, "préstamo"); }
        }

        public void Devolver(int idPrestamo, DateTime fechaDevolucion)
        {
            var p = idPrestamo > 0 ? _dao.ObtenerPorId(idPrestamo) : null;
            if (p == null) throw new ReglaNegocioException("Seleccione un préstamo.");
            if (p.Estado == Prestamo.EstadoDevuelto)
                throw new ReglaNegocioException("Este préstamo ya fue devuelto.");
            if (fechaDevolucion.Date < p.FechaPrestamo.Date)
                throw new ReglaNegocioException("La fecha de devolución no puede ser anterior a la del préstamo.");
            if (fechaDevolucion.Date > DateTime.Today)
                throw new ReglaNegocioException("La fecha de devolución no puede ser futura.");

            try { _dao.Devolver(idPrestamo, fechaDevolucion); }
            catch (InvalidOperationException ex) { throw new ReglaNegocioException(ex.Message); }
            catch (SqlException ex) { throw Validador.TraducirSql(ex, "préstamo"); }
        }

        public void EditarFecha(int idPrestamo, DateTime nuevaFecha)
        {
            var p = idPrestamo > 0 ? _dao.ObtenerPorId(idPrestamo) : null;
            if (p == null) throw new ReglaNegocioException("Seleccione un préstamo.");
            if (nuevaFecha.Date > DateTime.Today)
                throw new ReglaNegocioException("La fecha del préstamo no puede ser futura.");
            if (p.FechaDevolucion.HasValue && nuevaFecha.Date > p.FechaDevolucion.Value.Date)
                throw new ReglaNegocioException("La fecha del préstamo no puede ser posterior a la de devolución.");

            try { _dao.ActualizarFechaPrestamo(idPrestamo, nuevaFecha); }
            catch (SqlException ex) { throw Validador.TraducirSql(ex, "préstamo"); }
        }

        public void Eliminar(int idPrestamo)
        {
            if (idPrestamo <= 0) throw new ReglaNegocioException("Seleccione un préstamo para eliminar.");
            try { _dao.Eliminar(idPrestamo); }
            catch (SqlException ex) { throw Validador.TraducirSql(ex, "préstamo"); }
        }
    }

    public class ReporteService
    {
        private readonly ReporteDAO _dao = new();

        /// <summary>Catálogo de reportes disponibles: nombre → consulta SQL.</summary>
        public static readonly Dictionary<string, string> Reportes = new()
        {
            ["Listado de libros"] =
                @"SELECT l.ISBN, l.Titulo, a.Nombre + N' ' + a.Apellido AS Autor, e.Nombre AS Editorial,
                         l.AnioPublicacion AS Año, l.Existencias
                  FROM Libros l
                  INNER JOIN Autores a ON a.IdAutor = l.IdAutor
                  INNER JOIN Editoriales e ON e.IdEditorial = l.IdEditorial
                  ORDER BY l.Titulo",
            ["Listado de autores"] =
                @"SELECT a.IdAutor AS Id, a.Nombre, a.Apellido, a.Nacionalidad, COUNT(l.IdLibro) AS Libros
                  FROM Autores a LEFT JOIN Libros l ON l.IdAutor = a.IdAutor
                  GROUP BY a.IdAutor, a.Nombre, a.Apellido, a.Nacionalidad
                  ORDER BY a.Apellido, a.Nombre",
            ["Listado de editoriales"] =
                @"SELECT e.IdEditorial AS Id, e.Nombre, e.Pais AS País, COUNT(l.IdLibro) AS Libros
                  FROM Editoriales e LEFT JOIN Libros l ON l.IdEditorial = e.IdEditorial
                  GROUP BY e.IdEditorial, e.Nombre, e.Pais
                  ORDER BY e.Nombre",
            ["Listado de usuarios"] =
                @"SELECT IdUsuario AS Id, Nombre, Apellido, Documento, Telefono AS Teléfono, Correo
                  FROM Usuarios ORDER BY Apellido, Nombre",
            ["Préstamos activos"] =
                @"SELECT IdPrestamo AS Id, Usuario, Documento, Libro, FechaPrestamo AS [Fecha préstamo],
                         DATEDIFF(DAY, FechaPrestamo, CAST(GETDATE() AS DATE)) AS [Días prestado]
                  FROM vw_PrestamosDetalle WHERE Estado = N'Prestado'
                  ORDER BY FechaPrestamo",
            ["Préstamos devueltos"] =
                @"SELECT IdPrestamo AS Id, Usuario, Libro, FechaPrestamo AS [Fecha préstamo],
                         FechaDevolucion AS [Fecha devolución]
                  FROM vw_PrestamosDetalle WHERE Estado = N'Devuelto'
                  ORDER BY FechaDevolucion DESC",
            ["Historial de préstamos"] =
                @"SELECT IdPrestamo AS Id, Usuario, Libro, FechaPrestamo AS [Fecha préstamo],
                         FechaDevolucion AS [Fecha devolución], Estado
                  FROM vw_PrestamosDetalle ORDER BY FechaPrestamo DESC, IdPrestamo DESC",
            ["Resumen consolidado"] =
                @"SELECT N'Libros (títulos)' AS Concepto, COUNT(*) AS Total FROM Libros
                  UNION ALL SELECT N'Existencias disponibles', ISNULL(SUM(Existencias),0) FROM Libros
                  UNION ALL SELECT N'Autores', COUNT(*) FROM Autores
                  UNION ALL SELECT N'Editoriales', COUNT(*) FROM Editoriales
                  UNION ALL SELECT N'Usuarios', COUNT(*) FROM Usuarios
                  UNION ALL SELECT N'Préstamos activos', COUNT(*) FROM Prestamos WHERE Estado = N'Prestado'
                  UNION ALL SELECT N'Préstamos devueltos', COUNT(*) FROM Prestamos WHERE Estado = N'Devuelto'"
        };

        public DataTable Generar(string nombreReporte)
        {
            if (!Reportes.TryGetValue(nombreReporte, out var sql))
                throw new ReglaNegocioException("Seleccione un reporte válido.");
            try { return _dao.Ejecutar(sql); }
            catch (SqlException ex) { throw new ReglaNegocioException("Error al generar el reporte: " + ex.Message); }
        }
    }
}
