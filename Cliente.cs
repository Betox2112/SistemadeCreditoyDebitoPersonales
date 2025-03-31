using System.Collections.Generic;
using System.Linq;

public class Cliente
{
    public string Nombre { get; set; }
    public string Apellidos { get; set; }
    public string Cedula { get; set; }
    public string Correo { get; set; }
    public string Direccion { get; set; }
    public string TelefonoCelular { get; set; }
    public string TelefonoCasa { get; set; }
    public string TelefonoOficina { get; set; }
    public decimal IngresosMensuales { get; set; }
    public string FuentesIngresos { get; set; }
    public decimal GastosFijosMensuales { get; set; }
    public decimal Ahorros { get; set; }
    public decimal Deuda { get; set; } // usaremos para sumar préstamos

    public bool TieneDeuda()
    {
        return Deuda > 0;
    }

    // Método para actualizar la deuda basada en préstamos
    public void ActualizarDeuda(List<Prestamo> prestamos)
    {
        Deuda = prestamos.Where(p => p.CedulaCliente == Cedula).Sum(p => p.SaldoPendiente);
    }
}
