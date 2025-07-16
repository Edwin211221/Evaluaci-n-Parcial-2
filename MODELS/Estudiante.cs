namespace ControlPagosUniversidad.MODELS
{
    public class Estudiante
    {
        public int IdEstudiante { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; } 
        public string NombreCompleto => $"{Nombre} {Apellido}";
    }
}