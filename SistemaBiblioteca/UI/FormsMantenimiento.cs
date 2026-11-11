using SistemaBiblioteca.Business;
using SistemaBiblioteca.Models;

namespace SistemaBiblioteca.UI
{
    public class AutoresForm : ModuloForm
    {
        private readonly AutorService _servicio = new();
        private readonly TextBox txtNombre = UiHelper.Texto(120, 32, 220, 60);
        private readonly TextBox txtApellido = UiHelper.Texto(480, 32, 220, 60);
        private readonly TextBox txtNacionalidad = UiHelper.Texto(120, 67, 220, 60);
        private int _idActual;

        public AutoresForm() : base("Gestión de Autores", 115)
        {
            gbDatos.Controls.AddRange(new Control[]
            {
                UiHelper.Etiqueta("Nombre *", 15, 35), txtNombre,
                UiHelper.Etiqueta("Apellido *", 385, 35), txtApellido,
                UiHelper.Etiqueta("Nacionalidad", 15, 70), txtNacionalidad
            });

            AgregarBoton("Registrar", UiHelper.Verde, Registrar);
            AgregarBoton("Modificar", UiHelper.Acento, Modificar);
            AgregarBoton("Eliminar", UiHelper.Rojo, Eliminar);
            AgregarBoton("Limpiar", UiHelper.Gris, Limpiar);

            UiHelper.Columna(dgv, "Id", "Id", 30);
            UiHelper.Columna(dgv, "Nombre", "Nombre", 100);
            UiHelper.Columna(dgv, "Apellido", "Apellido", 100);
            UiHelper.Columna(dgv, "Nacionalidad", "Nacionalidad", 100);
            dgv.SelectionChanged += (s, e) => CargarSeleccion();

            Load += (s, e) => UiHelper.Ejecutar(Cargar);
        }

        private void Cargar()
        {
            dgv.DataSource = _servicio.Listar();
            dgv.ClearSelection();
        }

        private void CargarSeleccion()
        {
            var a = Seleccionado<Autor>();
            if (a == null) return;
            _idActual = a.Id;
            txtNombre.Text = a.Nombre;
            txtApellido.Text = a.Apellido;
            txtNacionalidad.Text = a.Nacionalidad;
        }

        private Autor LeerFormulario() => new Autor
        {
            Id = _idActual,
            Nombre = txtNombre.Text,
            Apellido = txtApellido.Text,
            Nacionalidad = txtNacionalidad.Text
        };

        private void Registrar()
        {
            var a = LeerFormulario();
            a.Id = 0;
            _servicio.Registrar(a);
            UiHelper.Info("Autor registrado correctamente.");
            Cargar();
            Limpiar();
        }

        private void Modificar()
        {
            _servicio.Actualizar(LeerFormulario());
            UiHelper.Info("Autor actualizado correctamente.");
            Cargar();
            Limpiar();
        }

        private void Eliminar()
        {
            if (_idActual <= 0) { UiHelper.Advertencia("Seleccione un autor de la lista."); return; }
            if (!UiHelper.Confirmar("¿Desea eliminar el autor seleccionado?")) return;
            _servicio.Eliminar(_idActual);
            UiHelper.Info("Autor eliminado correctamente.");
            Cargar();
            Limpiar();
        }

        private void Limpiar()
        {
            _idActual = 0;
            txtNombre.Clear();
            txtApellido.Clear();
            txtNacionalidad.Clear();
            dgv.ClearSelection();
            txtNombre.Focus();
        }
    }

    public class EditorialesForm : ModuloForm
    {
        private readonly EditorialService _servicio = new();
        private readonly TextBox txtNombre = UiHelper.Texto(120, 32, 280, 100);
        private readonly TextBox txtPais = UiHelper.Texto(120, 67, 220, 60);
        private int _idActual;

        public EditorialesForm() : base("Gestión de Editoriales", 115)
        {
            gbDatos.Controls.AddRange(new Control[]
            {
                UiHelper.Etiqueta("Nombre *", 15, 35), txtNombre,
                UiHelper.Etiqueta("País", 15, 70), txtPais
            });

            AgregarBoton("Registrar", UiHelper.Verde, Registrar);
            AgregarBoton("Modificar", UiHelper.Acento, Modificar);
            AgregarBoton("Eliminar", UiHelper.Rojo, Eliminar);
            AgregarBoton("Limpiar", UiHelper.Gris, Limpiar);

            UiHelper.Columna(dgv, "Id", "Id", 30);
            UiHelper.Columna(dgv, "Nombre", "Nombre", 200);
            UiHelper.Columna(dgv, "País", "Pais", 100);
            dgv.SelectionChanged += (s, e) => CargarSeleccion();

            Load += (s, e) => UiHelper.Ejecutar(Cargar);
        }

        private void Cargar()
        {
            dgv.DataSource = _servicio.Listar();
            dgv.ClearSelection();
        }

        private void CargarSeleccion()
        {
            var ed = Seleccionado<Editorial>();
            if (ed == null) return;
            _idActual = ed.Id;
            txtNombre.Text = ed.Nombre;
            txtPais.Text = ed.Pais;
        }

        private Editorial LeerFormulario() => new Editorial
        {
            Id = _idActual,
            Nombre = txtNombre.Text,
            Pais = txtPais.Text
        };

        private void Registrar()
        {
            var ed = LeerFormulario();
            ed.Id = 0;
            _servicio.Registrar(ed);
            UiHelper.Info("Editorial registrada correctamente.");
            Cargar();
            Limpiar();
        }

        private void Modificar()
        {
            _servicio.Actualizar(LeerFormulario());
            UiHelper.Info("Editorial actualizada correctamente.");
            Cargar();
            Limpiar();
        }

        private void Eliminar()
        {
            if (_idActual <= 0) { UiHelper.Advertencia("Seleccione una editorial de la lista."); return; }
            if (!UiHelper.Confirmar("¿Desea eliminar la editorial seleccionada?")) return;
            _servicio.Eliminar(_idActual);
            UiHelper.Info("Editorial eliminada correctamente.");
            Cargar();
            Limpiar();
        }

        private void Limpiar()
        {
            _idActual = 0;
            txtNombre.Clear();
            txtPais.Clear();
            dgv.ClearSelection();
            txtNombre.Focus();
        }
    }

    public class UsuariosForm : ModuloForm
    {
        private readonly UsuarioService _servicio = new();
        private readonly TextBox txtNombre = UiHelper.Texto(120, 32, 220, 60);
        private readonly TextBox txtApellido = UiHelper.Texto(480, 32, 220, 60);
        private readonly TextBox txtDocumento = UiHelper.Texto(120, 67, 220, 20);
        private readonly TextBox txtTelefono = UiHelper.Texto(480, 67, 220, 20);
        private readonly TextBox txtCorreo = UiHelper.Texto(120, 102, 580, 100);
        private int _idActual;

        public UsuariosForm() : base("Gestión de Usuarios", 150)
        {
            gbDatos.Controls.AddRange(new Control[]
            {
                UiHelper.Etiqueta("Nombre *", 15, 35), txtNombre,
                UiHelper.Etiqueta("Apellido *", 385, 35), txtApellido,
                UiHelper.Etiqueta("Documento *", 15, 70), txtDocumento,
                UiHelper.Etiqueta("Teléfono", 385, 70), txtTelefono,
                UiHelper.Etiqueta("Correo", 15, 105), txtCorreo
            });

            AgregarBoton("Registrar", UiHelper.Verde, Registrar);
            AgregarBoton("Modificar", UiHelper.Acento, Modificar);
            AgregarBoton("Eliminar", UiHelper.Rojo, Eliminar);
            AgregarBoton("Limpiar", UiHelper.Gris, Limpiar);

            UiHelper.Columna(dgv, "Id", "Id", 30);
            UiHelper.Columna(dgv, "Nombre", "Nombre", 90);
            UiHelper.Columna(dgv, "Apellido", "Apellido", 90);
            UiHelper.Columna(dgv, "Documento", "Documento", 80);
            UiHelper.Columna(dgv, "Teléfono", "Telefono", 80);
            UiHelper.Columna(dgv, "Correo", "Correo", 140);
            dgv.SelectionChanged += (s, e) => CargarSeleccion();

            Load += (s, e) => UiHelper.Ejecutar(Cargar);
        }

        private void Cargar()
        {
            dgv.DataSource = _servicio.Listar();
            dgv.ClearSelection();
        }

        private void CargarSeleccion()
        {
            var u = Seleccionado<Usuario>();
            if (u == null) return;
            _idActual = u.Id;
            txtNombre.Text = u.Nombre;
            txtApellido.Text = u.Apellido;
            txtDocumento.Text = u.Documento;
            txtTelefono.Text = u.Telefono;
            txtCorreo.Text = u.Correo;
        }

        private Usuario LeerFormulario() => new Usuario
        {
            Id = _idActual,
            Nombre = txtNombre.Text,
            Apellido = txtApellido.Text,
            Documento = txtDocumento.Text,
            Telefono = txtTelefono.Text,
            Correo = txtCorreo.Text
        };

        private void Registrar()
        {
            var u = LeerFormulario();
            u.Id = 0;
            _servicio.Registrar(u);
            UiHelper.Info("Usuario registrado correctamente.");
            Cargar();
            Limpiar();
        }

        private void Modificar()
        {
            _servicio.Actualizar(LeerFormulario());
            UiHelper.Info("Usuario actualizado correctamente.");
            Cargar();
            Limpiar();
        }

        private void Eliminar()
        {
            if (_idActual <= 0) { UiHelper.Advertencia("Seleccione un usuario de la lista."); return; }
            if (!UiHelper.Confirmar("¿Desea eliminar el usuario seleccionado?")) return;
            _servicio.Eliminar(_idActual);
            UiHelper.Info("Usuario eliminado correctamente.");
            Cargar();
            Limpiar();
        }

        private void Limpiar()
        {
            _idActual = 0;
            txtNombre.Clear();
            txtApellido.Clear();
            txtDocumento.Clear();
            txtTelefono.Clear();
            txtCorreo.Clear();
            dgv.ClearSelection();
            txtNombre.Focus();
        }
    }
}
