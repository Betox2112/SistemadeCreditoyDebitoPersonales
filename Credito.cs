using System;

public class Credito
{
    public string Nombre { get; set; }
    public decimal MontoLimite { get; set; }
    public decimal MontoUtilizado { get; set; }
    public decimal TasaInteres { get; set; }
    public DateTime FechaApertura { get; set; }
    public DateTime FechaProximoPago { get; set; }
    public string CedulaCliente { get; set; }
    public bool Activo { get; set; }

    public string MostrarEstado()
    {
        return $"  Crédito: {Nombre}\n" +
               $"  Límite: ₡{MontoLimite:N2}\n" +
               $"  Utilizado: ₡{MontoUtilizado:N2}\n" +
               $"  Saldo Disponible: ₡{(MontoLimite - MontoUtilizado):N2}\n" +
               $"  Tasa de Interés: {TasaInteres}%\n" +
               $"  Fecha Apertura: {FechaApertura:dd-MM-yyyy}\n" +
               $"  Próximo Pago: {FechaProximoPago:dd-MM-yyyy}\n" +
               $"  Cliente: {CedulaCliente}, Estado: {(Activo ? "Activo" : "Cerrado")}";
    }
}