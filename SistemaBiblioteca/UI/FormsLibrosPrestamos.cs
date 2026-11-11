using SistemaBiblioteca.Business;
using SistemaBiblioteca.Models;

namespace SistemaBiblioteca.UI
{
    public class LibrosForm : ModuloForm
    {
        private readonly LibroService _servicio = new();
        private readonly AutorService _autores = new();
        private readonly EditorialService _editoriales = new();

        private readonly TextBox txtIsbn = UiHelper.Texto(130, 32, 220, 20);
        private readonly TextBox txtTitulo = UiHelper.Texto(490, 32, 280, 200);
        private readonly ComboBox cboAutor = UiHelper.Combo(130, 67, 220);
        private readonly ComboBox cboEditorial = UiHelper.Combo(490, 67, 280);
        private readonly NumericUpDown nudAnio = UiHelper.Numerico(130, 102, 100, 1000, 2100, DateTime.Today.Year);
        private readonly NumericUpDown nudExistencias = UiHelper.Numerico(490, 102, 100, 0, 10000, 1);
        private readonly TextBox txtBuscar = new TextBox { Width = 200, Font = new Font("Segoe UI", 9.5f) };
        private int _idActual;

        public LibrosForm() : base("Gestión de Libros", 150)
        {
            gbDatos.Controls.AddRange(new Control[]
            {
                UiHelper.Etiqueta("ISBN / Código *", 15, 35), txtIsbn,
                UiHelper.Etiqueta("Título *", 400, 35), txtTitulo,
                UiHelper.Etiqueta("Autor *", 15, 70), cboAutor,
                UiHelper.Etiqueta("Editorial *", 400, 70), cboEditorial,
                UiHelper.Etiqueta("Año publicación", 15, 105), nudAnio,
                UiHelper.Etiqueta("Existencias", 400, 105), nudExistencias
            });

            AgregarBoton("Registrar", UiHelper.Verde, Registrar);
            AgregarBoton("Modificar", UiHelper.Acento, Modificar);
            AgregarBoton("Eliminar", UiHelper.Rojo, Eliminar);
            AgregarBoton("Limpiar", UiHelper.Gris, Limpiar);

            var lblBuscar = new Label
            {
                Text = "Buscar (ISBN, título o autor):",
                AutoSize = true,
                Margin = new Padding(20, 12, 5, 0),
                Font = new Font("Segoe UI", 9.5f)
            };
            txtBuscar.Margin = new Padding(0, 8, 8, 0);
            txtBuscar.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; UiHelper.Ejecutar(Buscar); }
            };
            pnlBotones.Controls.Add(lblBuscar);
            pnlBotones.Controls.Add(txtBuscar);
            AgregarBoton("Buscar", UiHelper.Primario, Buscar).Width = 85;
            AgregarBoton("Ver todos", UiHelper.Gris, () => { txtBuscar.Clear(); Cargar(); }).Width = 95;

            UiHelper.Columna(dgv, "Id", "Id", 25);
            UiHelper.Columna(dgv, "ISBN", "ISBN", 70);
            UiHelper.Columna(dgv, "Título", "Titulo", 130);
            UiHelper.Columna(dgv, "Autor", "AutorNombre", 90);
            UiHelper.Columna(dgv, "Editorial", "EditorialNombre", 80);
            UiHelper.Columna(dgv, "Año", "AnioPublicacion", 35);
            UiHelper.Columna(dgv, "Existencias", "Existencias", 50);
            dgv.SelectionChanged += (s, e) => CargarSeleccion();

            Load += (s, e) => UiHelper.Ejecutar(() => { CargarCombos(); Cargar(); });
        }

        private void CargarCombos()
        {
            cboAutor.DataSource = null;
            cboAutor.DataSource = _autores.Listar();
            cboEditorial.DataSource = null;
            cboEditorial.DataSource = _editoriales.Listar();
            cboAutor.SelectedIndex = -1;
            cboEditorial.SelectedIndex = -1;
        }

        private void Cargar()
        {
            dgv.DataSource = _servicio.Listar();
            dgv.ClearSelection();
        }

        private void Buscar()
        {
            dgv.DataSource = _servicio.Buscar(txtBuscar.Text);
            dgv.ClearSelection();
        }

        private void CargarSeleccion()
        {
            var l = Seleccionado<Libro>();
            if (l == null) return;
            _idActual = l.Id;
            txtIsbn.Text = l.ISBN;
            txtTitulo.Text = l.Titulo;
            UiHelper.SeleccionarPorId<Autor>(cboAutor, l.IdAutor);
            UiHelper.SeleccionarPorId<Editorial>(cboEditorial, l.IdEditorial);
            nudAnio.Value = Math.Clamp(l.AnioPublicacion, (int)nudAnio.Minimum, (int)nudAnio.Maximum);
            nudExistencias.Value = Math.Clamp(l.Existencias, (int)nudExistencias.Minimum, (int)nudExistencias.Maximum);
        }

        private Libro LeerFormulario() => new Libro
        {
            Id = _idActual,
            ISBN = txtIsbn.Text,
            Titulo = txtTitulo.Text,
            IdAutor = (cboAutor.SelectedItem as Autor)?.Id ?? 0,
            IdEditorial = (cboEditorial.SelectedItem as Editorial)?.Id ?? 0,
            AnioPublicacion = (int)nudAnio.Value,
            Existencias = (int)nudExistencias.Value
        };

        private void Registrar()
        {
            var l = LeerFormulario();
            l.Id = 0;
            _servicio.Registrar(l);
            UiHelper.Info("Libro registrado correctamente.");
            Cargar();
            Limpiar();
        }

        private void Modificar()
        {
            _servicio.Actualizar(LeerFormulario());
            UiHelper.Info("Libro actualizado correctamente.");
            Cargar();
            Limpiar();
        }

        private void Eliminar()
        {
            if (_idActual <= 0) { UiHelper.Advertencia("Seleccione un libro de la lista."); return; }
            if (!UiHelper.Confirmar("¿Desea eliminar el libro seleccionado?")) return;
            _servicio.Eliminar(_idActual);
            UiHelper.Info("Libro eliminado correctamente.");
            Cargar();
            Limpiar();
        }

        private void Limpiar()
        {
            _idActual = 0;
            txtIsbn.Clear();
            txtTitulo.Clear();
            cboAutor.SelectedIndex = -1;
            cboEditorial.SelectedIndex = -1;
            nudAnio.Value = DateTime.Today.Year;
            nudExistencias.Value = 1;
            dgv.ClearSelection();
            txtIsbn.Focus();
        }
    }

    public class PrestamosForm : ModuloForm
    {
        private readonly PrestamoService _servicio = new();
        private readonly UsuarioService _usuarios = new();
        private readonly LibroService _libros = new();

        private readonly ComboBox cboUsuario = UiHelper.Combo(130, 32, 300);
        private readonly ComboBox cboLibro = UiHelper.Combo(130, 67, 300);
        private readonly DateTimePicker dtpPrestamo = UiHelper.Fecha(590, 32, 140);
        private readonly DateTimePicker dtpDevolucion = UiHelper.Fecha(590, 67, 140);
        private readonly ComboBox cboFiltro = UiHelper.Combo(130, 102, 140);
        private readonly Label lblSeleccion = new Label
        {
            AutoSize = true,
            Location = new Point(400, 105),
            Font = new Font("Segoe UI", 9f, FontStyle.Italic),
            ForeColor = UiHelper.Gris
        };

        public PrestamosForm() : base("Gestión de Préstamos y Devoluciones", 150)
        {
            cboFiltro.Items.AddRange(new object[] { "Todos", Prestamo.EstadoPrestado, Prestamo.EstadoDevuelto });
            cboFiltro.SelectedIndex = 0;
            cboFiltro.SelectedIndexChanged += (s, e) => UiHelper.Ejecutar(Cargar);

            gbDatos.Controls.AddRange(new Control[]
            {
                UiHelper.Etiqueta("Usuario *", 15, 35), cboUsuario,
                UiHelper.Etiqueta("Libro *", 15, 70), cboLibro,
                UiHelper.Etiqueta("Fecha préstamo", 465, 35), dtpPrestamo,
                UiHelper.Etiqueta("Fecha devolución", 465, 70), dtpDevolucion,
                UiHelper.Etiqueta("Filtrar estado", 15, 105), cboFiltro,
                lblSeleccion
            });

            AgregarBoton("Prestar", UiHelper.Verde, Prestar);
            AgregarBoton("Devolver", UiHelper.Acento, Devolver, 110);
            AgregarBoton("Editar fecha", UiHelper.Primario, EditarFecha, 120);
            AgregarBoton("Eliminar", UiHelper.Rojo, Eliminar);
            AgregarBoton("Limpiar", UiHelper.Gris, Limpiar);

            UiHelper.Columna(dgv, "Id", "Id", 25);
            UiHelper.Columna(dgv, "Usuario", "UsuarioNombre", 100);
            UiHelper.Columna(dgv, "Libro", "LibroTitulo", 120);
            UiHelper.Columna(dgv, "F. préstamo", "FechaPrestamo", 60, "d");
            UiHelper.Columna(dgv, "F. devolución", "FechaDevolucion", 60, "d");
            UiHelper.Columna(dgv, "Estado", "Estado", 55);
            dgv.SelectionChanged += (s, e) => MostrarSeleccion();

            Load += (s, e) => UiHelper.Ejecutar(() => { CargarCombos(); Cargar(); });
        }

        private Button AgregarBoton(string texto, Color color, Action accion, int ancho)
        {
            var b = AgregarBoton(texto, color, accion);
            b.Width = ancho;
            return b;
        }

        private void CargarCombos()
        {
            cboUsuario.DataSource = null;
            cboUsuario.DataSource = _usuarios.Listar();
            cboUsuario.SelectedIndex = -1;

            // Solo se ofrecen libros con existencias disponibles.
            cboLibro.DataSource = null;
            cboLibro.DataSource = _libros.Listar().Where(l => l.Disponible).ToList();
            cboLibro.SelectedIndex = -1;
        }

        private void Cargar()
        {
            string estado = cboFiltro.SelectedIndex <= 0 ? null : cboFiltro.SelectedItem.ToString();
            dgv.DataSource = _servicio.Listar(estado);
            dgv.ClearSelection();
            lblSeleccion.Text = string.Empty;
        }

        private void MostrarSeleccion()
        {
            var p = Seleccionado<Prestamo>();
            if (p == null) return;
            lblSeleccion.Text = $"Seleccionado: #{p.Id} - {p.LibroTitulo} ({p.Estado})";
            dtpPrestamo.Value = p.FechaPrestamo.Date > DateTime.Today ? DateTime.Today : p.FechaPrestamo.Date;
            if (p.FechaDevolucion.HasValue)
                dtpDevolucion.Value = p.FechaDevolucion.Value.Date > DateTime.Today
                    ? DateTime.Today : p.FechaDevolucion.Value.Date;
        }

        private void Prestar()
        {
            int idUsuario = (cboUsuario.SelectedItem as Usuario)?.Id ?? 0;
            int idLibro = (cboLibro.SelectedItem as Libro)?.Id ?? 0;
            _servicio.Registrar(idUsuario, idLibro, dtpPrestamo.Value);
            UiHelper.Info("Préstamo registrado correctamente.");
            CargarCombos();
            Cargar();
            Limpiar();
        }

        private void Devolver()
        {
            var p = Seleccionado<Prestamo>();
            if (p == null) { UiHelper.Advertencia("Seleccione un préstamo de la lista."); return; }
            _servicio.Devolver(p.Id, dtpDevolucion.Value);
            UiHelper.Info("Devolución registrada correctamente.");
            CargarCombos();
            Cargar();
            Limpiar();
        }

        private void EditarFecha()
        {
            var p = Seleccionado<Prestamo>();
            if (p == null) { UiHelper.Advertencia("Seleccione un préstamo de la lista."); return; }
            _servicio.EditarFecha(p.Id, dtpPrestamo.Value);
            UiHelper.Info("Fecha del préstamo actualizada.");
            Cargar();
        }

        private void Eliminar()
        {
            var p = Seleccionado<Prestamo>();
            if (p == null) { UiHelper.Advertencia("Seleccione un préstamo de la lista."); return; }
            if (!UiHelper.Confirmar("¿Desea eliminar el préstamo seleccionado?\n" +
                                    "Si está activo, el libro volverá a estar disponible.")) return;
            _servicio.Eliminar(p.Id);
            UiHelper.Info("Préstamo eliminado correctamente.");
            CargarCombos();
            Cargar();
            Limpiar();
        }

        private void Limpiar()
        {
            cboUsuario.SelectedIndex = -1;
            cboLibro.SelectedIndex = -1;
            dtpPrestamo.Value = DateTime.Today;
            dtpDevolucion.Value = DateTime.Today;
            dgv.ClearSelection();
            lblSeleccion.Text = string.Empty;
        }
    }
}
