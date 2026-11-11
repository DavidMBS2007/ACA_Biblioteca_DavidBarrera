using Microsoft.Data.SqlClient;
using SistemaBiblioteca.Business;
using SistemaBiblioteca.Models;

namespace SistemaBiblioteca.UI
{
    /// <summary>Utilidades compartidas por los formularios: estilos, mensajes y fábrica de controles.</summary>
    public static class UiHelper
    {
        public static readonly Color Primario = Color.FromArgb(31, 58, 95);
        public static readonly Color Acento = Color.FromArgb(46, 134, 193);
        public static readonly Color Verde = Color.FromArgb(39, 150, 90);
        public static readonly Color Rojo = Color.FromArgb(192, 57, 43);
        public static readonly Color Gris = Color.FromArgb(110, 120, 130);
        public static readonly Color Fondo = Color.FromArgb(245, 247, 250);

        // ---------- Mensajes ----------
        public static void Info(string mensaje) =>
            MessageBox.Show(mensaje, "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

        public static void Advertencia(string mensaje) =>
            MessageBox.Show(mensaje, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        public static void Error(string mensaje) =>
            MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        public static bool Confirmar(string mensaje) =>
            MessageBox.Show(mensaje, "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            == DialogResult.Yes;

        /// <summary>Ejecuta una acción manejando las excepciones de forma centralizada.</summary>
        public static bool Ejecutar(Action accion)
        {
            try
            {
                accion();
                return true;
            }
            catch (ReglaNegocioException ex)
            {
                Advertencia(ex.Message);
            }
            catch (SqlException ex)
            {
                Error("No se pudo completar la operación con la base de datos.\n" +
                      "Verifique que SQL Server esté en ejecución y que la cadena de conexión en App.config sea correcta.\n\n" +
                      "Detalle: " + ex.Message);
            }
            catch (Exception ex)
            {
                Error("Ocurrió un error inesperado:\n" + ex.Message);
            }
            return false;
        }

        // ---------- Fábrica de controles ----------
        public static Label Etiqueta(string texto, int x, int y) => new Label
        {
            Text = texto,
            Location = new Point(x, y),
            AutoSize = true,
            Font = new Font("Segoe UI", 9.5f)
        };

        public static TextBox Texto(int x, int y, int ancho, int maxLongitud) => new TextBox
        {
            Location = new Point(x, y),
            Width = ancho,
            MaxLength = maxLongitud,
            Font = new Font("Segoe UI", 9.5f)
        };

        public static ComboBox Combo(int x, int y, int ancho) => new ComboBox
        {
            Location = new Point(x, y),
            Width = ancho,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 9.5f)
        };

        public static NumericUpDown Numerico(int x, int y, int ancho, int min, int max, int valor) => new NumericUpDown
        {
            Location = new Point(x, y),
            Width = ancho,
            Minimum = min,
            Maximum = max,
            Value = valor,
            Font = new Font("Segoe UI", 9.5f)
        };

        public static DateTimePicker Fecha(int x, int y, int ancho) => new DateTimePicker
        {
            Location = new Point(x, y),
            Width = ancho,
            Format = DateTimePickerFormat.Short,
            MaxDate = DateTime.Today,
            Font = new Font("Segoe UI", 9.5f)
        };

        public static Button Boton(string texto, Color color, int ancho = 115) => new Button
        {
            Text = texto,
            Width = ancho,
            Height = 34,
            BackColor = color,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            Margin = new Padding(0, 4, 10, 4),
            Cursor = Cursors.Hand
        };

        public static DataGridView Grilla() => new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            MultiSelect = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoGenerateColumns = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            RowHeadersVisible = false,
            EnableHeadersVisualStyles = false,
            ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Primario,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            },
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
            AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(238, 243, 249) }
        };

        public static void Columna(DataGridView grid, string encabezado, string propiedad, float peso = 100,
                                   string formato = null)
        {
            var col = new DataGridViewTextBoxColumn
            {
                HeaderText = encabezado,
                DataPropertyName = propiedad,
                FillWeight = peso
            };
            if (formato != null) col.DefaultCellStyle.Format = formato;
            grid.Columns.Add(col);
        }

        /// <summary>Selecciona en un ComboBox el elemento cuyo Id coincide.</summary>
        public static void SeleccionarPorId<T>(ComboBox combo, int id) where T : Entidad
        {
            for (int i = 0; i < combo.Items.Count; i++)
            {
                if (combo.Items[i] is T item && item.Id == id)
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }
            combo.SelectedIndex = -1;
        }
    }

    /// <summary>
    /// Formulario base de los módulos: encabezado, grupo de datos, barra de botones y grilla.
    /// Se incrusta dentro del formulario principal.
    /// </summary>
    public class ModuloForm : Form
    {
        protected readonly GroupBox gbDatos = new GroupBox
        {
            Text = "Datos",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
        };

        protected readonly FlowLayoutPanel pnlBotones = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false
        };

        protected readonly DataGridView dgv = UiHelper.Grilla();

        protected ModuloForm(string titulo, int altoDatos)
        {
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;
            BackColor = UiHelper.Fondo;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(15)
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, altoDatos));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var lblTitulo = new Label
            {
                Text = titulo,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = UiHelper.Primario,
                TextAlign = ContentAlignment.MiddleLeft
            };

            layout.Controls.Add(lblTitulo, 0, 0);
            layout.Controls.Add(gbDatos, 0, 1);
            layout.Controls.Add(pnlBotones, 0, 2);
            layout.Controls.Add(dgv, 0, 3);
            Controls.Add(layout);
        }

        protected Button AgregarBoton(string texto, Color color, Action accion)
        {
            var b = UiHelper.Boton(texto, color);
            b.Click += (s, e) => UiHelper.Ejecutar(accion);
            pnlBotones.Controls.Add(b);
            return b;
        }

        /// <summary>Elemento de la grilla actualmente seleccionado (o null).</summary>
        protected T Seleccionado<T>() where T : class =>
            dgv.SelectedRows.Count > 0 ? dgv.SelectedRows[0].DataBoundItem as T : null;
    }
}
