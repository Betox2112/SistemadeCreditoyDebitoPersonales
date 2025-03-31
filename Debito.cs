using System;

public class Debito
{
    public decimal Monto { get; set; } // Monto del débito (en colones)
    public string Descripcion { get; set; } // Descripción del débito (ej. "Pago de luz", "Compra en supermercado")
    public DateTime FechaDebito { get; set; } // Fecha en que se realizó el débito
    public string CedulaCliente { get; set; } // Vincular al cliente
    public string Categoria { get; set; } // Categoría del gasto (ej. "Servicios", "Alimentación")
    public bool Pagado { get; set; } // Indica si ya se pagó o está pendiente

    // Método para mostrar el estado del débito
    public string MostrarEstado()
    {
        return $"Débito: {Descripcion}\n" +
               $"Monto: ₡{Monto:N2}\n" +
               $"Fecha: {FechaDebito:dd-MM-yyyy}\n" +
               $"Cliente: {CedulaCliente}\n" +
               $"Categoría: {Categoria}, Estado: {(Pagado ? "Pagado" : "Pendiente")}";
    }
}