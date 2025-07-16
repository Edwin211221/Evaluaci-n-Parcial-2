using ControlPagosUniversidad.DATA;
using ControlPagosUniversidad.MODELS;
using Dapper;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ControlPagosUniversidad.VIEWS
{
    public partial class frmPagos : Form
    {
        public frmPagos()
        {
            InitializeComponent();
            CargarEstudiantes();
            CargarSemestres();
            CargarPagos();
            dgvPagos.CellClick += dgvPagos_CellClick;
            this.Load += frmPagos_Load;
            btnRefrescar.Click += btnRefrescar_Click; // Por si acaso no está en el diseñador
        }

        // Carga los estudiantes en el ComboBox
        void CargarEstudiantes()
        {
            using (var connection = Conexion.ObtenerConexion())
            {
                var estudiantes = connection.Query<Estudiante>("SELECT IdEstudiante, Nombre, Apellido FROM Estudiantes").ToList();
                cmbEstudiante.DataSource = estudiantes;
                cmbEstudiante.DisplayMember = "NombreCompleto";
                cmbEstudiante.ValueMember = "IdEstudiante";
            }
        }

        // Carga los semestres en el ComboBox
        void CargarSemestres()
        {
            using (var connection = Conexion.ObtenerConexion())
            {
                var semestres = connection.Query<Semestre>("SELECT * FROM Semestres").ToList();
                cmbSemestre.DataSource = semestres;
                cmbSemestre.DisplayMember = "NombreSemestre";
                cmbSemestre.ValueMember = "IdSemestre";
            }
        }

        // Carga los pagos en el DataGridView
        void CargarPagos()
        {
            using (var connection = Conexion.ObtenerConexion())
            {
                var query = @"SELECT p.IdPago, CONCAT(e.Nombre,' ',e.Apellido) AS Estudiante, s.NombreSemestre, p.FechaPago, p.Valor 
                      FROM Pagos p 
                      JOIN Estudiantes e ON p.IdEstudiante = e.IdEstudiante 
                      JOIN Semestres s ON p.IdSemestre = s.IdSemestre";
                dgvPagos.DataSource = connection.Query(query).ToList();
            }
        }

        // Botón Agregar pago
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (cmbEstudiante.SelectedIndex == -1 || cmbSemestre.SelectedIndex == -1 || string.IsNullOrWhiteSpace(txtValor.Text))
            {
                MessageBox.Show("Todos los campos son obligatorios.");
                return;
            }
            if (!decimal.TryParse(txtValor.Text, out decimal valorPago) || valorPago <= 0)
            {
                MessageBox.Show("Ingrese un valor válido y positivo para el pago.");
                return;
            }

            try
            {
                using (var connection = Conexion.ObtenerConexion())
                {
                    var query = @"INSERT INTO Pagos(IdEstudiante, IdSemestre, FechaPago, Valor) 
                                  VALUES(@IdEstudiante, @IdSemestre, @FechaPago, @Valor)";
                    connection.Execute(query, new
                    {
                        IdEstudiante = cmbEstudiante.SelectedValue,
                        IdSemestre = cmbSemestre.SelectedValue,
                        FechaPago = dtpFechaPago.Value,
                        Valor = valorPago
                    });
                    CargarPagos();
                    MessageBox.Show("Pago agregado correctamente.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar pago: " + ex.Message);
            }
        }

        // Botón Editar pago
        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvPagos.CurrentRow != null)
            {
                if (cmbEstudiante.SelectedIndex == -1 || cmbSemestre.SelectedIndex == -1 || string.IsNullOrWhiteSpace(txtValor.Text))
                {
                    MessageBox.Show("Todos los campos son obligatorios.");
                    return;
                }
                if (!decimal.TryParse(txtValor.Text, out decimal valorPago) || valorPago <= 0)
                {
                    MessageBox.Show("Ingrese un valor válido y positivo para el pago.");
                    return;
                }

                try
                {
                    int id = Convert.ToInt32(dgvPagos.CurrentRow.Cells["IdPago"].Value);
                    using (var connection = Conexion.ObtenerConexion())
                    {
                        var query = @"UPDATE Pagos SET IdEstudiante=@IdEstudiante, IdSemestre=@IdSemestre, FechaPago=@FechaPago, Valor=@Valor 
                                      WHERE IdPago=@Id";
                        connection.Execute(query, new
                        {
                            IdEstudiante = cmbEstudiante.SelectedValue,
                            IdSemestre = cmbSemestre.SelectedValue,
                            FechaPago = dtpFechaPago.Value,
                            Valor = valorPago,
                            Id = id
                        });
                        CargarPagos();
                        MessageBox.Show("Pago actualizado correctamente.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al actualizar pago: " + ex.Message);
                }
            }
        }

        // Botón Eliminar pago
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvPagos.CurrentRow != null)
            {
                var confirm = MessageBox.Show("¿Estás seguro de eliminar este pago?", "Confirmar", MessageBoxButtons.YesNo);
                if (confirm == DialogResult.No) return;

                try
                {
                    int id = Convert.ToInt32(dgvPagos.CurrentRow.Cells["IdPago"].Value);
                    using (var connection = Conexion.ObtenerConexion())
                    {
                        connection.Execute("DELETE FROM Pagos WHERE IdPago=@Id", new { Id = id });
                        CargarPagos();
                        MessageBox.Show("Pago eliminado correctamente.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar pago: " + ex.Message);
                }
            }
        }

        // Botón Refrescar (ahora también limpia campos y deselecciona filas)
        private void btnRefrescar_Click(object sender, EventArgs e)
        {
            CargarPagos();
            LimpiarCampos();
        }

        // Método para limpiar campos y deseleccionar filas
        private void LimpiarCampos()
        {
            cmbEstudiante.SelectedIndex = -1;
            cmbSemestre.SelectedIndex = -1;
            dtpFechaPago.Value = DateTime.Now;
            txtValor.Text = "";
            dgvPagos.ClearSelection();
        }

        // Evento para auto-rellenar campos al seleccionar una fila
        private void dgvPagos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvPagos.Rows[e.RowIndex];
                cmbEstudiante.Text = row.Cells["Estudiante"].Value.ToString();
                cmbSemestre.Text = row.Cells["NombreSemestre"].Value.ToString();
                dtpFechaPago.Value = Convert.ToDateTime(row.Cells["FechaPago"].Value);
                txtValor.Text = row.Cells["Valor"].Value.ToString();
            }
        }

        // Mejoras visuales del formulario y DataGridView
        private void frmPagos_Load(object sender, EventArgs e)
        {
            dgvPagos.EnableHeadersVisualStyles = false;
            dgvPagos.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(40, 40, 70);
            dgvPagos.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvPagos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            foreach (Control c in this.Controls)
            {
                if (c is Button btn)
                {
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.BackColor = Color.FromArgb(60, 120, 200);
                    btn.ForeColor = Color.White;
                }
            }
        }
    }
}