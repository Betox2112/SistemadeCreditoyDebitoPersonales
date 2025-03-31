using System;

public class Prestamo
{
    private Log log; // Recibir log desde Sistema

    public Prestamo(Log logInstance)
    {
        log = logInstance;
    }
    public decimal Monto { get; set; } // Monto inicial del préstamo
    public decimal Tasa { get; set; }  // Tasa de interés anual (%)
    public int Plazo { get; set; }     // Plazo en meses
    public decimal TotalPagar { get; set; } // Total a pagar (incluye intereses)
    public decimal CuotaMensual { get; set; } // Cuota fija mensual
    public decimal SaldoPendiente { get; set; } // Saldo que falta por pagar
    public DateTime FechaInicio { get; set; } // Fecha de inicio del préstamo
    public string CedulaCliente { get; set; } // Vincular el préstamo a un cliente

    // Método para calcular la cuota mensual y total a pagar
    public void CalcularCuotas()
    {
        // Convertir tasa anual a mensual (dividir entre 12 y pasar a decimal)
        decimal tasaMensual = (Tasa / 100) / 12;
        // Fórmula de amortización: Cuota = [Monto * tasaMensual * (1 + tasaMensual)^Plazo] / [(1 + tasaMensual)^Plazo - 1]
        CuotaMensual = (Monto * tasaMensual * (decimal)Math.Pow((double)(1 + tasaMensual), Plazo))
                       / (decimal)(Math.Pow((double)(1 + tasaMensual), Plazo) - 1);
        TotalPagar = CuotaMensual * Plazo;
        SaldoPendiente = Monto; // Al inicio, el saldo pendiente es el monto total
    }

    // Método para registrar un pago y actualizar el saldo
    public void RegistrarPagoMensual()
    {
        if (SaldoPendiente > 0)
        {
            SaldoPendiente -= CuotaMensual;
            if (SaldoPendiente < 0) SaldoPendiente = 0; // Evitar valores negativos
            log.RegistrarAccion("Sistema", $"Pago mensual registrado para préstamo de {CedulaCliente}, Cuota: ₡{CuotaMensual}");
        }
    }

    // Método para mostrar el estado del préstamo
    public string MostrarEstado()
    {
        return $"Préstamo: ₡{Monto:N2}, Tasa: {Tasa}%, Plazo: {Plazo} meses\n" +
               $"Cuota Mensual: ₡{CuotaMensual:N2}, Total a Pagar: ₡{TotalPagar:N2}\n" +
               $"Saldo Pendiente: ₡{SaldoPendiente:N2}";
    }
}