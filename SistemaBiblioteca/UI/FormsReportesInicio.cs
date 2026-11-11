using System.Data;
using System.Text;
using SistemaBiblioteca.Business;

namespace SistemaBiblioteca.UI
{
    public class ReportesForm : ModuloForm
    {
        private readonly ReporteService _servicio = new();
        private readonly ComboBox cboReporte = UiHelper.Combo(130, 32, 320);
        private readonly Label lblTotal = new Label
        {
            AutoSize = true,
            Location = new Point(130, 70),
            Font = new Font("Segoe UI", 9.5f, FontStyle.Italic),
            ForeColor = UiHelper.Gris
        };
        private DataTable _actual;

        public ReportesForm() : base("Consultas y Reportes", 110)
        {
            dgv.AutoGenerateColumns = true;

            cboReporte.Items.AddRange(ReporteService.Reportes.Keys.Cast<object>().ToArray());
            cboReporte.SelectedIndex = 0;

            gbDatos.Controls.AddRange(new Control[]
            {
                UiHelper.Etiqueta("Reporte", 15, 35), cboReporte,
                lblTotal
            });

            AgregarBoton("Generar", UiHelper.Verde, Generar);
            AgregarBoton("Exportar CSV", UiHelper.Acento, Exportar).Width = 140;

            Load += (s, e) => UiHelper.Ejecutar(Generar);
        }

        private void Generar()
        {
            if (cboReporte.SelectedItem == null)
                throw new ReglaNegocioException("Seleccione un reporte.");
            _actual = _servicio.Generar(cboReporte.SelectedItem.ToString());
            dgv.DataSource = _actual;
            dgv.ClearSelection();
            lblTotal.Text = $"{_actual.Rows.Count} registro(s) encontrados.";
        }

        private void Exportar()
        {
            if (_actual == null || _actual.Rows.Count == 0)
                throw new ReglaNegocioException("Genere un reporte con datos antes de exportar.");

            using var dlg = new SaveFileDialog
            {
                Filter = "Archivo CSV (*.csv)|*.csv",
                FileName = cboReporte.SelectedItem.ToString().Replace(' ', '_') + ".csv"
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            var sb = new StringBuilder();
            sb.AppendLine(string.Join(";", _actual.Columns.Cast<DataColumn>().Select(c => Escapar(c.ColumnName))));
            foreach (DataRow fila in _actual.Rows)
                sb.AppendLine(string.Join(";", fila.ItemArray.Select(v => Escapar(Convert.ToString(v)))));

            File.WriteAllText(dlg.FileName, sb.ToString(), new UTF8Encoding(true));
            UiHelper.Info("Reporte exportado correctamente.");
        }

        private static string Escapar(string valor)
        {
            valor ??= string.Empty;
            return valor.Contains(';') || valor.Contains('"') || valor.Contains('\n')
                ? "\"" + valor.Replace("\"", "\"\"") + "\""
                : valor;
        }
    }

    public class InicioForm : Form
    {
        public InicioForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;
            BackColor = UiHelper.Fondo;

            var titulo = new Label
            {
                Text = "Sistema de Gestión de Biblioteca",
                Font = new Font("Segoe UI", 24f, FontStyle.Bold),
                ForeColor = UiHelper.Primario,
                AutoSize = true,
                Location = new Point(30, 25)
            };
            var subtitulo = new Label
            {
                Text = "Seleccione un módulo del menú lateral para comenzar.",
                Font = new Font("Segoe UI", 11f),
                ForeColor = UiHelper.Gris,
                AutoSize = true,
                Location = new Point(34, 80)
            };
            Controls.Add(titulo);
            Controls.Add(subtitulo);

            Load += (s, e) => UiHelper.Ejecutar(CargarResumen);
        }

        private void CargarResumen()
        {
            var tabla = new ReporteService().Generar("Resumen consolidado");
            int x = 34, y = 130;
            foreach (DataRow fila in tabla.Rows)
            {
                var tarjeta = new Panel
                {
                    Size = new Size(200, 90),
                    Location = new Point(x, y),
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };
                tarjeta.Controls.Add(new Label
                {
                    Text = Convert.ToString(fila["Total"]),
                    Font = new Font("Segoe UI", 22f, FontStyle.Bold),
                    ForeColor = UiHelper.Acento,
                    Dock = DockStyle.Top,
                    Height = 50,
                    TextAlign = ContentAlignment.MiddleCenter
                });
                tarjeta.Controls.Add(new Label
                {
                    Text = Convert.ToString(fila["Concepto"]),
                    Font = new Font("Segoe UI", 9.5f),
                    ForeColor = UiHelper.Primario,
                    Dock = DockStyle.Bottom,
                    Height = 30,
                    TextAlign = ContentAlignment.MiddleCenter
                });
                Controls.Add(tarjeta);

                x += 215;
                if (x + 200 > 900) { x = 34; y += 105; }
            }
        }
    }
}
