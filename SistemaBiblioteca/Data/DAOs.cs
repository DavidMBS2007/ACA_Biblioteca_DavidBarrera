using System.Data;
using Microsoft.Data.SqlClient;
using SistemaBiblioteca.Models;

namespace SistemaBiblioteca.Data
{
    // Todas las consultas son parametrizadas (prevención de inyección SQL).

    public class AutorDAO
    {
        public List<Autor> Listar()
        {
            var lista = new List<Autor>();
            using var cn = ConexionBD.CrearConexion();
            using var cmd = new SqlCommand(
                "SELECT IdAutor, Nombre, Apellido, Nacionalidad FROM Autores ORDER BY Apellido, Nombre", cn);
            cn.Open();
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new Autor(rd.GetInt32(0), rd.GetString(1), rd.GetString(2),
                                    rd.IsDBNull(3) ? null : rd.GetString(3)));
            }
            return lista;
        }

        public int Insertar(Autor a)
        {
            using var cn = ConexionBD.CrearConexion();
            using var cmd = new SqlCommand(
                @"INSERT INTO Autores (Nombre, Apellido, Nacionalidad)
                  VALUES (@Nombre, @Apellido, @Nacionalidad);
                  SELECT CAST(SCOPE_IDENTITY() AS INT);", cn);
            cmd.Parameters.AddWithValue("@Nombre", a.Nombre);
            cmd.Parameters.AddWithValue("@Apellido", a.Apellido);
            cmd.Parameters.AddWithValue("@Nacionalidad", (object)a.Nacionalidad ?? DBNull.Value);
            cn.Open();
            return (int)cmd.ExecuteScalar();
        }

        public bool Actualizar(Autor a)
        {
            using var cn = ConexionBD.CrearConexion();
            using var cmd = new SqlCommand(
                @"UPDATE Autores SET Nombre=@Nombre, Apellido=@Apellido, Nacionalidad=@Nacionalidad
                  WHERE IdAutor=@Id", cn);
            cmd.Parameters.AddWithValue("@Id", a.Id);
            cmd.Parameters.AddWithValue("@Nombre", a.Nombre);
            cmd.Parameters.AddWithValue("@Apellido", a.Apellido);
            cmd.Parameters.AddWithValue("@Nacionalidad", (object)a.Nacionalidad ?? DBNull.Value);
            cn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Eliminar(int id)
        {
            using var cn = ConexionBD.CrearConexion();
            using var cmd = new SqlCommand("DELETE FROM Autores WHERE IdAutor=@Id", cn);
            cmd.Parameters.AddWithValue("@Id", id);
            cn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool ExisteNombreCompleto(string nombre, string apellido, int idExcluir = 0)
        {
            using var cn = ConexionBD.CrearConexion();
            using var cmd = new SqlCommand(
                @"SELECT COUNT(1) FROM Autores
                  WHERE Nombre=@N AND Apellido=@A AND IdAutor<>@Id", cn);
            cmd.Parameters.AddWithValue("@N", nombre);
            cmd.Parameters.AddWithValue("@A", apellido);
            cmd.Parameters.AddWithValue("@Id", idExcluir);
            cn.Open();
            return (int)cmd.ExecuteScalar() > 0;
        }

        public bool TieneLibros(int idAutor)
        {
            using var cn = ConexionBD.CrearConexion();
            using var cmd = new SqlCommand("SELECT COUNT(1) FROM Libros WHERE IdAutor=@Id", cn);
            cmd.Parameters.AddWithValue("@Id", idAutor);
            cn.Open();
            return (int)cmd.ExecuteScalar() > 0;
        }
    }

    public class EditorialDAO
    {
        public List<Editorial> Listar()
        {
            var lista = new List<Editorial>();
            using var cn = ConexionBD.CrearConexion();
            using var cmd = new SqlCommand(
                "SELECT IdEditorial, Nombre, Pais FROM Editoriales ORDER BY Nombre", cn);
            cn.Open();
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new Editorial(rd.GetInt32(0), rd.GetString(1),
                                        rd.IsDBNull(2) ? null : rd.GetString(2)));
            }
            return lista;
        }

        public int Insertar(Editorial e)
        {
            using var cn = ConexionBD.CrearConexion();
            using var cmd = new SqlCommand(
                @"INSERT INTO Editoriales (Nombre, Pais) VALUES (@Nombre, @Pais);
                  SELECT CAST(SCOPE_IDENTITY() AS INT);", cn);
            cmd.Parameters.AddWithValue("@Nombre", e.Nombre);
            cmd.Parameters.AddWithValue("@Pais", (object)e.Pais ?? DBNull.Value);
            cn.Open();
            return (int)cmd.ExecuteScalar();
        }

        public bool Actualizar(Editorial e)
        {
            using var cn = ConexionBD.CrearConexion();
            using var cmd = new SqlCommand(
                "UPDATE Editoriales SET Nombre=@Nombre, Pais=@Pais WHERE IdEditorial=@Id", cn);
            cmd.Parameters.AddWithValue("@Id", e.Id);
            cmd.Parameters.AddWithValue("@Nombre", e.Nombre);
            cmd.Parameters.AddWithValue("@Pais", (object)e.Pais ?? DBNull.Value);
            cn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Eliminar(int id)
        {
            using var cn = ConexionBD.CrearConexion();
            using var cmd = new SqlCommand("DELETE FROM Editoriales WHERE IdEditorial=@Id", cn);
            cmd.Parameters.AddWithValue("@Id", id);
            cn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool ExisteNombre(string nombre, int idExcluir = 0)
        {
            using var cn = ConexionBD.CrearConexion();
            using var cmd = new SqlCommand(
                "SELECT COUNT(1) FROM Editoriales WHERE Nombre=@N AND IdEditorial<>@Id", cn);
            cmd.Parameters.AddWithValue("@N", nombre);
            cmd.Parameters.AddWithValue("@Id", idExcluir);
            cn.Open();
            return (int)cmd.ExecuteScalar() > 0;
        }

        public bool TieneLibros(int idEditorial)
        {
            using var cn = ConexionBD.CrearConexion();
            using var cmd = new SqlCommand("SELECT COUNT(1) FROM Libros WHERE IdEditorial=@Id", cn);
            cmd.Parameters.AddWithValue("@Id", idEditorial);
            cn.Open();
            return (int)cmd.ExecuteScalar() > 0;
        }
    }

    public class UsuarioDAO
    {
        public List<Usuario> Listar()
        {
            var lista = new List<Usuario>();
            using var cn = ConexionBD.CrearConexion();
            using var cmd = new SqlCommand(
                @"SELECT IdUsuario, Nombre, Apellido, Documento, Telefono, Correo
                  FROM Usuarios ORDER BY Apellido, Nombre", cn);
            cn.Open();
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new Usuario(rd.GetInt32(0), rd.GetString(1), rd.GetString(2), rd.GetString(3),
                                      rd.IsDBNull(4) ? null : rd.GetString(4),
                                      rd.IsDBNull(5) ? null : rd.GetString(5)));
            }
            return lista;
        }

        public int Insertar(Usuario u)
        {
            using var cn = ConexionBD.CrearConexion();
            using var cmd = new SqlCommand(
                @"INSERT INTO Usuarios (Nombre, Apellido, Documento, Telefono, Correo)
                  VALUES (@Nombre, @Apellido, @Documento, @Telefono, @Correo);
                  SELECT CAST(SCOPE_IDENTITY() AS INT);", cn);
            AgregarParametros(cmd, u);
            cn.Open();
            return (int)cmd.ExecuteScalar();
        }

        public bool Actualizar(Usuario u)
        {
            using var cn = ConexionBD.CrearConexion();
            using var cmd = new SqlCommand(
                @"UPDATE Usuarios SET Nombre=@Nombre, Apellido=@Apellido, Documento=@Documento,
                         Telefono=@Telefono, Correo=@Correo
                  WHERE IdUsuario=@Id", cn);
            AgregarParametros(cmd, u);
            cmd.Parameters.AddWithValue("@Id", u.Id);
            cn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Eliminar(int id)
        {
            using var cn = ConexionBD.CrearConexion();
            using var cmd = new SqlCommand("DELETE FROM Usuarios WHERE IdUsuario=@Id", cn);
            cmd.Parameters.AddWithValue("@Id", id);
            cn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool ExisteDocumento(string documento, int idExcluir = 0)
        {
            using var cn = ConexionBD.CrearConexion();
            using var cmd = new SqlCommand(
                "SELECT COUNT(1) FROM Usuarios WHERE Documento=@D AND IdUsuario<>@Id", cn);
            cmd.Parameters.AddWithValue("@D", documento);
            cmd.Parameters.AddWithValue("@Id", idExcluir);
            cn.Open();
            return (int)cmd.ExecuteScalar() > 0;
        }

        public bool Existe(int id)
        {
            using var cn = ConexionBD.CrearConexion();
            using var cmd = new SqlCommand("SELECT COUNT(1) FROM Usuarios WHERE IdUsuario=@Id", cn);
            cmd.Parameters.AddWithValue("@Id", id);
            cn.Open();
            return (int)cmd.ExecuteScalar() > 0;
        }

        public bool TienePrestamos(int idUsuario)
        {
            using var cn = ConexionBD.CrearConexion();
            using var cmd = new SqlCommand("SELECT COUNT(1) FROM Prestamos WHERE IdUsuario=@Id", cn);
            cmd.Parameters.AddWithValue("@Id", idUsuario);
            cn.Open();
            return (int)cmd.ExecuteScalar() > 0;
        }

        private static void AgregarParametros(SqlCommand cmd, Usuario u)
        {
            cmd.Parameters.AddWithValue("@Nombre", u.Nombre);
            cmd.Parameters.AddWithValue("@Apellido", u.Apellido);
            cmd.Parameters.AddWithValue("@Documento", u.Documento);
            cmd.Parameters.AddWithValue("@Telefono", (object)u.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Correo", (object)u.Correo ?? DBNull.Value);
        }
    }

    public class LibroDAO
    {
        private const string SelectBase =
            @"SELECT l.IdLibro, l.ISBN, l.Titulo, l.IdAutor, l.IdEditorial, l.AnioPublicacion,
                     l.Existencias, a.Nombre + N' ' + a.Apellido AS Autor, e.Nombre AS Editorial
              FROM Libros l
              INNER JOIN Autores a     ON a.IdAutor = l.IdAutor
              INNER JOIN Editoriales e ON e.IdEditorial = l.IdEditorial ";

        private static Libro Mapear(SqlDataReader rd) => new Libro
        {
            Id = rd.GetInt32(0),
            ISBN = rd.GetString(1),
            Titulo = rd.GetString(2),
            IdAutor = rd.GetInt32(3),
            IdEditorial = rd.GetInt32(4),
            AnioPublicacion = rd.GetInt16(5),
            Existencias = rd.GetInt32(6),
            AutorNombre = rd.GetString(7),
            EditorialNombre = rd.GetString(8)
        };

        public List<Libro> Listar() => Consultar(SelectBase + "ORDER BY l.Titulo", null);

        /// <summary>Busca por ISBN (código), título o autor.</summary>
        public List<Libro> Buscar(string texto)
        {
            return Consultar(
                SelectBase + @"WHERE l.ISBN LIKE @T OR l.Titulo LIKE @T
                                  OR a.Nombre LIKE @T OR a.Apellido LIKE @T
                                  OR (a.Nombre + N' ' + a.Apellido) LIKE @T
                               ORDER BY l.Titulo",
                cmd => cmd.Parameters.AddWithValue("@T", "%" + texto + "%"));
        }

        public Libro ObtenerPorId(int id)
        {
            var r = Consultar(SelectBase + "WHERE l.IdLibro=@Id",
                              cmd => cmd.Parameters.AddWithValue("@Id", id));
            return r.FirstOrDefault();
        }

        private static List<Libro> Consultar(string sql, Action<SqlCommand> parametros)
        {
            var lista = new List<Libro>();
            using var cn = ConexionBD.CrearConexion();
            using var cmd = new SqlCommand(sql, cn);
            parametros?.Invoke(cmd);
            cn.Open();
            using var rd = cmd.ExecuteReader();
            while (rd.Read()) lista.Add(Mapear(rd));
            return lista;
        }

        public int Insertar(Libro l)
        {
            using var cn = ConexionBD.CrearConexion();
            using var cmd = new SqlCommand(
                @"INSERT INTO Libros (ISBN, Titulo, IdAutor, IdEditorial, AnioPublicacion, Existencias)
                  VALUES (@ISBN, @Titulo, @IdAutor, @IdEditorial, @Anio, @Existencias);
                  SELECT CAST(SCOPE_IDENTITY() AS INT);", cn);
            AgregarParametros(cmd, l);
            cn.Open();
            return (int)cmd.ExecuteScalar();
        }

        public bool Actualizar(Libro l)
        {
            using var cn = ConexionBD.CrearConexion();
            using var cmd = new SqlCommand(
                @"UPDATE Libros SET ISBN=@ISBN, Titulo=@Titulo, IdAutor=@IdAutor,
                         IdEditorial=@IdEditorial, AnioPublicacion=@Anio, Existencias=@Existencias
                  WHERE IdLibro=@Id", cn);
            AgregarParametros(cmd, l);
            cmd.Parameters.AddWithValue("@Id", l.Id);
            cn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Eliminar(int id)
        {
            using var cn = ConexionBD.CrearConexion();
            using var cmd = new SqlCommand("DELETE FROM Libros WHERE IdLibro=@Id", cn);
            cmd.Parameters.AddWithValue("@Id", id);
            cn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool ExisteISBN(string isbn, int idExcluir = 0)
        {
            using var cn = ConexionBD.CrearConexion();
            using var cmd = new SqlCommand(
                "SELECT COUNT(1) FROM Libros WHERE ISBN=@I AND IdLibro<>@Id", cn);
            cmd.Parameters.AddWithValue("@I", isbn);
            cmd.Parameters.AddWithValue("@Id", idExcluir);
            cn.Open();
            return (int)cmd.ExecuteScalar() > 0;
        }

        public bool TienePrestamos(int idLibro)
        {
            using var cn = ConexionBD.CrearConexion();
            using var cmd = new SqlCommand("SELECT COUNT(1) FROM Prestamos WHERE IdLibro=@Id", cn);
            cmd.Parameters.AddWithValue("@Id", idLibro);
            cn.Open();
            return (int)cmd.ExecuteScalar() > 0;
        }

        private static void AgregarParametros(SqlCommand cmd, Libro l)
        {
            cmd.Parameters.AddWithValue("@ISBN", l.ISBN);
            cmd.Parameters.AddWithValue("@Titulo", l.Titulo);
            cmd.Parameters.AddWithValue("@IdAutor", l.IdAutor);
            cmd.Parameters.AddWithValue("@IdEditorial", l.IdEditorial);
            cmd.Parameters.AddWithValue("@Anio", (short)l.AnioPublicacion);
            cmd.Parameters.AddWithValue("@Existencias", l.Existencias);
        }
    }

    public class PrestamoDAO
    {
        public List<Prestamo> Listar(string estado = null)
        {
            var lista = new List<Prestamo>();
            using var cn = ConexionBD.CrearConexion();
            string sql = @"SELECT IdPrestamo, IdUsuario, IdLibro, FechaPrestamo, FechaDevolucion,
                                  Estado, Usuario, Libro
                           FROM vw_PrestamosDetalle "
                         + (estado != null ? "WHERE Estado=@Estado " : "")
                         + "ORDER BY FechaPrestamo DESC, IdPrestamo DESC";
            using var cmd = new SqlCommand(sql, cn);
            if (estado != null) cmd.Parameters.AddWithValue("@Estado", estado);
            cn.Open();
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new Prestamo
                {
                    Id = rd.GetInt32(0),
                    IdUsuario = rd.GetInt32(1),
                    IdLibro = rd.GetInt32(2),
                    FechaPrestamo = rd.GetDateTime(3),
                    FechaDevolucion = rd.IsDBNull(4) ? (DateTime?)null : rd.GetDateTime(4),
                    Estado = rd.GetString(5),
                    UsuarioNombre = rd.GetString(6),
                    LibroTitulo = rd.GetString(7)
                });
            }
            return lista;
        }

        public Prestamo ObtenerPorId(int id)
        {
            using var cn = ConexionBD.CrearConexion();
            using var cmd = new SqlCommand(
                @"SELECT IdPrestamo, IdUsuario, IdLibro, FechaPrestamo, FechaDevolucion, Estado
                  FROM Prestamos WHERE IdPrestamo=@Id", cn);
            cmd.Parameters.AddWithValue("@Id", id);
            cn.Open();
            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;
            return new Prestamo
            {
                Id = rd.GetInt32(0),
                IdUsuario = rd.GetInt32(1),
                IdLibro = rd.GetInt32(2),
                FechaPrestamo = rd.GetDateTime(3),
                FechaDevolucion = rd.IsDBNull(4) ? (DateTime?)null : rd.GetDateTime(4),
                Estado = rd.GetString(5)
            };
        }

        /// <summary>
        /// Registra un préstamo y descuenta una existencia del libro en una sola transacción.
        /// </summary>
        public int Registrar(Prestamo p)
        {
            using var cn = ConexionBD.CrearConexion();
            cn.Open();
            using var tx = cn.BeginTransaction();
            try
            {
                // Descuento atómico: solo si hay existencias.
                using (var cmdStock = new SqlCommand(
                    "UPDATE Libros SET Existencias = Existencias - 1 WHERE IdLibro=@L AND Existencias > 0",
                    cn, tx))
                {
                    cmdStock.Parameters.AddWithValue("@L", p.IdLibro);
                    if (cmdStock.ExecuteNonQuery() == 0)
                        throw new InvalidOperationException("El libro no tiene existencias disponibles.");
                }

                int id;
                using (var cmd = new SqlCommand(
                    @"INSERT INTO Prestamos (IdUsuario, IdLibro, FechaPrestamo, FechaDevolucion, Estado)
                      VALUES (@U, @L, @F, NULL, @E);
                      SELECT CAST(SCOPE_IDENTITY() AS INT);", cn, tx))
                {
                    cmd.Parameters.AddWithValue("@U", p.IdUsuario);
                    cmd.Parameters.AddWithValue("@L", p.IdLibro);
                    cmd.Parameters.AddWithValue("@F", p.FechaPrestamo.Date);
                    cmd.Parameters.AddWithValue("@E", Prestamo.EstadoPrestado);
                    id = (int)cmd.ExecuteScalar();
                }

                tx.Commit();
                return id;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        /// <summary>
        /// Registra la devolución: actualiza estado y fecha, y repone la existencia del libro.
        /// </summary>
        public void Devolver(int idPrestamo, DateTime fechaDevolucion)
        {
            using var cn = ConexionBD.CrearConexion();
            cn.Open();
            using var tx = cn.BeginTransaction();
            try
            {
                int idLibro;
                using (var cmd = new SqlCommand(
                    @"UPDATE Prestamos SET Estado=@Dev, FechaDevolucion=@F
                      OUTPUT inserted.IdLibro
                      WHERE IdPrestamo=@Id AND Estado=@Pre", cn, tx))
                {
                    cmd.Parameters.AddWithValue("@Dev", Prestamo.EstadoDevuelto);
                    cmd.Parameters.AddWithValue("@Pre", Prestamo.EstadoPrestado);
                    cmd.Parameters.AddWithValue("@F", fechaDevolucion.Date);
                    cmd.Parameters.AddWithValue("@Id", idPrestamo);
                    object r = cmd.ExecuteScalar();
                    if (r == null)
                        throw new InvalidOperationException("El préstamo no existe o ya fue devuelto.");
                    idLibro = (int)r;
                }

                using (var cmd = new SqlCommand(
                    "UPDATE Libros SET Existencias = Existencias + 1 WHERE IdLibro=@L", cn, tx))
                {
                    cmd.Parameters.AddWithValue("@L", idLibro);
                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        /// <summary>
        /// Edita un préstamo (solo fecha de préstamo). El usuario y el libro no se cambian
        /// para no desajustar las existencias.
        /// </summary>
        public bool ActualizarFechaPrestamo(int idPrestamo, DateTime nuevaFecha)
        {
            using var cn = ConexionBD.CrearConexion();
            using var cmd = new SqlCommand(
                "UPDATE Prestamos SET FechaPrestamo=@F WHERE IdPrestamo=@Id", cn);
            cmd.Parameters.AddWithValue("@F", nuevaFecha.Date);
            cmd.Parameters.AddWithValue("@Id", idPrestamo);
            cn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// Elimina un préstamo. Si estaba activo, repone la existencia del libro.
        /// </summary>
        public bool Eliminar(int idPrestamo)
        {
            using var cn = ConexionBD.CrearConexion();
            cn.Open();
            using var tx = cn.BeginTransaction();
            try
            {
                int idLibro;
                string estado;
                using (var cmd = new SqlCommand(
                    "SELECT IdLibro, Estado FROM Prestamos WHERE IdPrestamo=@Id", cn, tx))
                {
                    cmd.Parameters.AddWithValue("@Id", idPrestamo);
                    using var rd = cmd.ExecuteReader();
                    if (!rd.Read()) return false;
                    idLibro = rd.GetInt32(0);
                    estado = rd.GetString(1);
                }

                using (var cmd = new SqlCommand("DELETE FROM Prestamos WHERE IdPrestamo=@Id", cn, tx))
                {
                    cmd.Parameters.AddWithValue("@Id", idPrestamo);
                    cmd.ExecuteNonQuery();
                }

                if (estado == Prestamo.EstadoPrestado)
                {
                    using var cmd = new SqlCommand(
                        "UPDATE Libros SET Existencias = Existencias + 1 WHERE IdLibro=@L", cn, tx);
                    cmd.Parameters.AddWithValue("@L", idLibro);
                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
                return true;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public bool UsuarioTienePrestamoActivoDelLibro(int idUsuario, int idLibro)
        {
            using var cn = ConexionBD.CrearConexion();
            using var cmd = new SqlCommand(
                @"SELECT COUNT(1) FROM Prestamos
                  WHERE IdUsuario=@U AND IdLibro=@L AND Estado=@E", cn);
            cmd.Parameters.AddWithValue("@U", idUsuario);
            cmd.Parameters.AddWithValue("@L", idLibro);
            cmd.Parameters.AddWithValue("@E", Prestamo.EstadoPrestado);
            cn.Open();
            return (int)cmd.ExecuteScalar() > 0;
        }
    }

    /// <summary>Consultas consolidadas para el módulo de reportes.</summary>
    public class ReporteDAO
    {
        public DataTable Ejecutar(string sql)
        {
            var dt = new DataTable();
            using var cn = ConexionBD.CrearConexion();
            using var da = new SqlDataAdapter(sql, cn);
            da.Fill(dt);
            return dt;
        }
    }
}
