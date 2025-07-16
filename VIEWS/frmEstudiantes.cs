using ControlPagosUniversidad.DATA;
using ControlPagosUniversidad.MODELS;
using Dapper;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ControlPagosUniversidad.VIEWS
{
    public partial class frmEstudiantes : Form
    {
        public frmEstudiantes()
        {
            InitializeComponent();
            CargarCarreras();
            CargarEstudiantes();
            dgvEstudiantes.CellClick += dgvEstudiantes_CellClick;
            this.Load += frmEstudiantes_Load;
            btnRefrescar.Click += btnRefrescar_Click; // Por si acaso no lo tienes asociado en el diseñador
        }

        // Carga todas las carreras en el ComboBox
        void CargarCarreras()
        {
            using (var connection = Conexion.ObtenerConexion())
            {
                var carreras = connection.Query<Carrera>("SELECT * FROM Carreras").ToList();
                cmbCarrera.DataSource = carreras;
                cmbCarrera.DisplayMember = "NombreCarrera";
                cmbCarrera.ValueMember = "IdCarrera";
            }
        }

        // Carga los estudiantes en el DataGridView
        void CargarEstudiantes()
        {
            using (var connection = Conexion.ObtenerConexion())
            {
                var query = @"SELECT e.IdEstudiante, e.Nombre, e.Apellido, c.NombreCarrera 
                      FROM Estudiantes e 
                      JOIN Carreras c ON e.IdCarrera = c.IdCarrera";
                var lista = connection.Query(query).ToList();
                dgvEstudiantes.DataSource = lista;
            }
        }

        // Botón Agregar estudiante
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtApellido.Text) || cmbCarrera.SelectedIndex == -1)
            {
                MessageBox.Show("Todos los campos son obligatorios.");
                return;
            }

            try
            {
                using (var connection = Conexion.ObtenerConexion())
                {
                    var query = @"INSERT INTO Estudiantes(Nombre, Apellido, IdCarrera) 
                                  VALUES (@Nombre, @Apellido, @IdCarrera)";
                    connection.Execute(query, new
                    {
                        Nombre = txtNombre.Text,
                        Apellido = txtApellido.Text,
                        IdCarrera = cmbCarrera.SelectedValue
                    });
                    CargarEstudiantes();
                    MessageBox.Show("Estudiante agregado correctamente.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar estudiante: " + ex.Message);
            }
        }

        // Botón Editar estudiante
        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvEstudiantes.CurrentRow != null)
            {
                if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtApellido.Text) || cmbCarrera.SelectedIndex == -1)
                {
                    MessageBox.Show("Todos los campos son obligatorios.");
                    return;
                }

                try
                {
                    int id = Convert.ToInt32(dgvEstudiantes.CurrentRow.Cells["IdEstudiante"].Value);
                    using (var connection = Conexion.ObtenerConexion())
                    {
                        var query = @"UPDATE Estudiantes SET Nombre=@Nombre, Apellido=@Apellido, IdCarrera=@IdCarrera WHERE IdEstudiante=@Id";
                        connection.Execute(query, new
                        {
                            Nombre = txtNombre.Text,
                            Apellido = txtApellido.Text,
                            IdCarrera = cmbCarrera.SelectedValue,
                            Id = id
                        });
                        CargarEstudiantes();
                        MessageBox.Show("Estudiante actualizado correctamente.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al actualizar estudiante: " + ex.Message);
                }
            }
        }

        // Botón Eliminar estudiante
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvEstudiantes.CurrentRow != null)
            {
                var confirm = MessageBox.Show("¿Estás seguro de eliminar este estudiante?", "Confirmar", MessageBoxButtons.YesNo);
                if (confirm == DialogResult.No) return;

                try
                {
                    int id = Convert.ToInt32(dgvEstudiantes.CurrentRow.Cells["IdEstudiante"].Value);
                    using (var connection = Conexion.ObtenerConexion())
                    {
                        connection.Execute("DELETE FROM Estudiantes WHERE IdEstudiante=@Id", new { Id = id });
                        CargarEstudiantes();
                        MessageBox.Show("Estudiante eliminado correctamente.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar estudiante: " + ex.Message);
                }
            }
        }

        // Botón Refrescar (ahora también limpia campos y deselecciona filas)
        private void btnRefrescar_Click(object sender, EventArgs e)
        {
            CargarEstudiantes();
            LimpiarCampos();
        }

        // Método para limpiar campos y deseleccionar filas
        private void LimpiarCampos()
        {
            txtNombre.Text = "";
            txtApellido.Text = "";
            cmbCarrera.SelectedIndex = -1;
            dgvEstudiantes.ClearSelection();
        }

        // Evento para auto-rellenar campos al seleccionar una fila
        private void dgvEstudiantes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvEstudiantes.Rows[e.RowIndex];
                txtNombre.Text = row.Cells["Nombre"].Value.ToString();
                txtApellido.Text = row.Cells["Apellido"].Value.ToString();
                cmbCarrera.Text = row.Cells["NombreCarrera"].Value.ToString();
            }
        }

        // Mejoras visuales del formulario y DataGridView
        private void frmEstudiantes_Load(object sender, EventArgs e)
        {
            dgvEstudiantes.EnableHeadersVisualStyles = false;
            dgvEstudiantes.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(40, 40, 70);
            dgvEstudiantes.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvEstudiantes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

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