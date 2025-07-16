using System;

namespace ControlPagosUniversidad.MODELS
{
    public class Pago
    {
        public int IdPago { get; set; }
        public int IdEstudiante { get; set; }
        public int IdSemestre { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal Valor { get; set; }
    }
}