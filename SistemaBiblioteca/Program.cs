using SistemaBiblioteca.UI;

namespace SistemaBiblioteca
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            ApplicationConfiguration.Initialize();

            Application.ThreadException += (s, e) =>
                MessageBox.Show("Error inesperado: " + e.Exception.Message, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);

            Application.Run(new MainForm());
        }
    }
}
