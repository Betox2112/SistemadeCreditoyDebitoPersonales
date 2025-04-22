using System;

namespace SistemaDeCreditoYDebitoPersonales.Model
{
    // Clase que representa un préstamo
    public class Prestamo
    {
        private Log log;
        public decimal Monto { get; set; }
        public decimal Tasa { get; set; }
        public int Plazo { get; set; }
        public DateTime FechaInicio { get; set; }
        public string CedulaCliente { get; set; }
        public decimal CuotaMensual { get; set; }
        public decimal SaldoPendiente { get; set; }
        public int CuotasPagadas { get; set; }

        // Constructor que recibe una instancia de Log
        public Prestamo(Log log)
        {
            this.log = log;
            CuotasPagadas = 0;
        }

        // Método calcular cuotas mensuales del préstamo
        public void CalcularCuotas()
        {
            decimal tasaMensual = Tasa / 12 / 100;
            CuotaMensual = Monto * (tasaMensual * (decimal)Math.Pow((double)(1 + tasaMensual), Plazo)) / ((decimal)Math.Pow((double)(1 + tasaMensual), Plazo) - 1);
            SaldoPendiente = Monto;
        }

        // Método para registrar un pago mensual
        public void RegistrarPagoMensual()
        {
            if (SaldoPendiente > 0)
            {
                SaldoPendiente -= CuotaMensual;
                CuotasPagadas++;
                if (SaldoPendiente < 0) SaldoPendiente = 0;
            }
        }

        // Método que muestra el estado del préstamo
        public string MostrarEstado()
        {
            return $"Préstamo\n" +
                   $"  Monto: ₡{Monto:N2}\n" +
                   $"  Tasa Anual: {Tasa}%\n" +
                   $"  Plazo: {Plazo} meses\n" +
                   $"  Cuota Mensual: ₡{CuotaMensual:N2}\n" +
                   $"  Saldo Pendiente: ₡{SaldoPendiente:N2}\n" +
                   $"  Cuotas Pagadas: {CuotasPagadas}/{Plazo}\n" +
                   $"  Fecha de Inicio: {FechaInicio.ToString("dd-MM-yyyy")}";
        }
    }
}