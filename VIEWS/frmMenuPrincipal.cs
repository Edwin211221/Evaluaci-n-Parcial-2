using System;
using System.Drawing;
using System.Windows.Forms;

namespace ControlPagosUniversidad.VIEWS
{
    public partial class frmMenuPrincipal : Form
    {
        public frmMenuPrincipal()
        {
            InitializeComponent();
            this.Load += frmMenuPrincipal_Load;
        }

        // Mejoras visuales al cargar el formulario
        private void frmMenuPrincipal_Load(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(230, 230, 255);

            /*foreach (Control c in this.Controls)
            {
                if (c is Button btn)
                {
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.BackColor = Color.FromArgb(40, 100, 180);
                    btn.ForeColor = Color.White;
                    btn.Font = new Font("Segoe UI", 16, FontStyle.Bold);
                    btn.Height = 50;
                    btn.Width = 180;
                }
            }*/
        }

        // Botón para abrir el formulario de Estudiantes
        private void btnEstudiantes_Click(object sender, EventArgs e)
        {
            frmEstudiantes estudiantesForm = new frmEstudiantes();
            estudiantesForm.ShowDialog();
        }

        // Botón para abrir el formulario de Pagos
        private void btnPagos_Click(object sender, EventArgs e)
        {
            frmPagos pagosForm = new frmPagos();
            pagosForm.ShowDialog();
        }

        // Botón para salir de la aplicación
        private void btnSalir_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Seguro que desea salir?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
                Application.Exit();
        }
    }
}