    using System;

    namespace SistemaDeCreditoYDebitoPersonales.Model
    {
        // Clase que representa un débito
        public class Debito
        {
            public int Id { get; set; }
            public decimal Monto { get; set; }
            public DateTime FechaDebito { get; set; }
            public string Descripcion { get; set; }
            public string Categoria { get; set; }
            public string CedulaCliente { get; set; }
            public bool Pagado { get; set; }

            // Constructor que inicializa el débito
            public string MostrarEstado()
            {
                return $"  Débito: {Descripcion}\n" +
                       $"  ID: {Id}\n" +
                       $"  Monto: ₡{Monto:N2}\n" +
                       $"  Fecha: {FechaDebito.ToString("dd-MM-yyyy")}\n" +
                       $"  Categoría: {Categoria}\n" +
                       $"  Estado: {(Pagado ? "Pagado" : "Pendiente")}";
            }
        }
    }