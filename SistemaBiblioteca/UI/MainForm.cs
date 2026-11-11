using SistemaBiblioteca.Data;

namespace SistemaBiblioteca.UI
{
    /// <summary>Formulario principal: menú lateral y área de contenido donde se incrustan los módulos.</summary>
    public class MainForm : Form
    {
        private readonly Panel pnlMenu = new Panel { Dock = DockStyle.Left, Width = 210, BackColor = UiHelper.Primario };
        private readonly Panel pnlContenido = new Panel { Dock = DockStyle.Fill, BackColor = UiHelper.Fondo };
        private readonly List<Button> _botones = new();
        private Form _moduloActual;

        public MainForm()
        {
            Text = "Sistema de Gestión de Biblioteca";
            Size = new Size(1150, 720);
            MinimumSize = new Size(1000, 620);
            StartPosition = FormStartPosition.CenterScreen;

            var lblMarca = new Label
            {
                Text = "📚 Biblioteca",
                Dock = DockStyle.Top,
                Height = 70,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Los botones se agregan en orden inverso porque Dock=Top apila el último agregado arriba.
            var modulos = new (string Texto, Func<Form> Crear)[]
            {
                ("Inicio",      () => new InicioForm()),
                ("Libros",      () => new LibrosForm()),
                ("Usuarios",    () => new UsuariosForm()),
                ("Autores",     () => new AutoresForm()),
                ("Editoriales", () => new EditorialesForm()),
                ("Préstamos",   () => new PrestamosForm()),
                ("Reportes",    () => new ReportesForm())
            };

            var btnSalir = CrearBotonMenu("Salir");
            btnSalir.Click += (s, e) => Close();
            btnSalir.Dock = DockStyle.Bottom;

            foreach (var m in modulos.Reverse())
            {
                var btn = CrearBotonMenu(m.Texto);
                var crear = m.Crear;
                btn.Click += (s, e) => AbrirModulo(crear, btn);
                _botones.Insert(0, btn);
                pnlMenu.Controls.Add(btn);
            }

            pnlMenu.Controls.Add(btnSalir);
            pnlMenu.Controls.Add(lblMarca);

            Controls.Add(pnlContenido);
            Controls.Add(pnlMenu);

            Load += (s, e) =>
            {
                if (!ConexionBD.ProbarConexion(out string mensaje))
                {
                    UiHelper.Error("No fue posible conectar con SQL Server.\n\n" + mensaje +
                                   "\n\nVerifique la cadena de conexión en App.config y que la base de datos " +
                                   "BibliotecaDB exista (ejecute database/BibliotecaDB.sql).");
                }
                AbrirModulo(modulos[0].Crear, _botones[0]);
            };
        }

        private Button CrearBotonMenu(string texto) => new Button
        {
            Text = "   " + texto,
            Dock = DockStyle.Top,
            Height = 48,
            FlatStyle = FlatStyle.Flat,
            ForeColor = Color.White,
            BackColor = UiHelper.Primario,
            Font = new Font("Segoe UI", 11f),
            TextAlign = ContentAlignment.MiddleLeft,
            Cursor = Cursors.Hand,
            FlatAppearance = { BorderSize = 0 }
        };

        private void AbrirModulo(Func<Form> crear, Button activo)
        {
            foreach (var b in _botones) b.BackColor = UiHelper.Primario;
            activo.BackColor = UiHelper.Acento;

            _moduloActual?.Close();
            _moduloActual?.Dispose();
            pnlContenido.Controls.Clear();

            _moduloActual = crear();
            pnlContenido.Controls.Add(_moduloActual);
            _moduloActual.Show();
        }
    }
}
