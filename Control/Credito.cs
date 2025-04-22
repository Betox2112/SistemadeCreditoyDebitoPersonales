using System;

namespace SistemaDeCreditoYDebitoPersonales.Model
{
    // Clase que representa un crédito
    public class Credito
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal MontoLimite { get; set; }
        public decimal MontoUtilizado { get; set; }
        public decimal TasaInteres { get; set; }
        public DateTime FechaApertura { get; set; }
        public DateTime FechaProximoPago { get; set; }
        public string CedulaCliente { get; set; }
        public bool Activo { get; set; }

        // Constructor que inicializa el crédito
        public Credito(string nombre, decimal montoLimite, decimal tasaInteres, string cedulaCliente)
        {
            Nombre = nombre;
            MontoLimite = montoLimite;
            MontoUtilizado = 0;
            TasaInteres = tasaInteres;
            FechaApertura = DateTime.Now;
            FechaProximoPago = DateTime.Now.AddMonths(1);
            CedulaCliente = cedulaCliente;
            Activo = true;
        }

        // Método para mostrar el estado del crédito
        public string MostrarEstado()
        {
            return 
                   
                   $"  Crédito: {Nombre}\n" +
                   $"  ID: {Id}\n" +
                   $"  Límite: ₡{MontoLimite:N2}\n" +
                   $"  Utilizado: ₡{MontoUtilizado:N2}\n" +
                   $"  Tasa de Interés: {TasaInteres}%\n" +
                   $"  Fecha de Apertura: {FechaApertura.ToString("dd-MM-yyyy")}\n" +
                   $"  Próximo Pago: {FechaProximoPago.ToString("dd-MM-yyyy")}\n" +
                   $"  Estado: {(Activo ? "Activo" : "Cerrado")}";
        }

        // Método para calcular el interés acumulado
        public decimal CalcularInteresAcumulado()
        {
            var meses = (DateTime.Now.Year - FechaApertura.Year) * 12 + DateTime.Now.Month - FechaApertura.Month;
            return (MontoUtilizado * TasaInteres / 100) * meses;
        }

        // Método para calcular el saldo total
        public decimal CalcularSaldoTotal()
        {
            return MontoUtilizado + CalcularInteresAcumulado();
        }

        // Método para realizar un pago
        public void RealizarPago(decimal monto)
        {
            if (monto > 0 && monto <= CalcularSaldoTotal())
            {
                MontoUtilizado -= monto;
                if (MontoUtilizado < 0) MontoUtilizado = 0;
            }
            else
            {
                throw new ArgumentException("El monto del pago no es válido.");
            }
        }

        // Método para cerrar el crédito
        public void CerrarCredito()
        {
            if (MontoUtilizado == 0)
            {
                Activo = false;
            }
            else
            {
                throw new InvalidOperationException("No se puede cerrar el crédito mientras haya saldo pendiente.");
            }
        }
    }
}