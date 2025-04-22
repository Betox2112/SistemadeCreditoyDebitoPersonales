using System;
using System.Collections.Generic;
using System.IO;

namespace SistemaDeCreditoYDebitoPersonales.Model.Dao
{
    // Clase para manejar el archivo donde guardamos los clientes
    public class Archivo
    {
        // Método para guardar todos los clientes en un archivo
        public void GuardarClientes(List<Cliente> listaDeClientes, string nombreArchivo, bool v)
        {
            try
            {
                using (StreamWriter escritor = new StreamWriter(nombreArchivo))
                {
                    for (int i = 0; i < listaDeClientes.Count; i++)
                    {
                        Cliente cliente = listaDeClientes[i];
                        escritor.WriteLine($"{cliente.Nombre}|{cliente.Apellidos}|{cliente.Cedula}|{cliente.Correo}|{cliente.Direccion}|{cliente.TelefonoCelular}|{cliente.TelefonoCasa}|{cliente.TelefonoOficina}|{cliente.IngresosMensuales}|{cliente.FuentesIngresos}|{cliente.GastosFijosMensuales}|{cliente.Ahorros}");
                    }
                }
            }
            catch (IOException ex)
            {
                throw new Exception($"Error al guardar clientes: {ex.Message}");
            }
        }
        public void GuardarCreditos(List<Credito> listaDeCreditos, string nombreArchivo, bool v)
        {
            try
            {
                using (StreamWriter escritor = new StreamWriter(nombreArchivo))
                {
                    for (int i = 0; i < listaDeCreditos.Count; i++)
                    {
                        Credito credito = listaDeCreditos[i];
                        escritor.WriteLine($"{credito.Nombre}|{credito.MontoLimite}|{credito.MontoUtilizado}|{credito.TasaInteres}|{credito.FechaApertura}|{credito.FechaProximoPago}|{credito.CedulaCliente}|{credito.Activo}");
                    }
                }
            }
            catch (IOException ex)
            {
                throw new Exception($"Error al guardar créditos: {ex.Message}");
            }
        }

        public void GuardarDebitos(List<Debito> listaDeDebitos, string nombreArchivo)
        {
            try
            {
                using (StreamWriter escritor = new StreamWriter(nombreArchivo))
                {
                    for (int i = 0; i < listaDeDebitos.Count; i++)
                    {
                        Debito debito = listaDeDebitos[i];
                        escritor.WriteLine($"{debito.Id}|{debito.Monto}|{debito.FechaDebito}|{debito.Descripcion}|{debito.Categoria}|{debito.CedulaCliente}|{debito.Pagado}");
                    }
                }
            }
            catch (IOException ex)
            {
                throw new Exception($"Error al guardar débitos: {ex.Message}");
            }
        }

        public void GuardarPrestamos(List<Prestamo> listaDePrestamos, string nombreArchivo, bool v)
        {
            try
            {
                using (StreamWriter escritor = new StreamWriter(nombreArchivo))
                {
                    for (int i = 0; i < listaDePrestamos.Count; i++)
                    {
                        Prestamo prestamo = listaDePrestamos[i];
                        escritor.WriteLine($"{prestamo.Monto}|{prestamo.Tasa}|{prestamo.Plazo}|{prestamo.FechaInicio}|{prestamo.CedulaCliente}|{prestamo.SaldoPendiente}|{prestamo.CuotasPagadas}");
                    }
                }
            }
            catch (IOException ex)
            {
                throw new Exception($"Error al guardar préstamos: {ex.Message}");
            }
        }
        // Método para cargar los clientes desde un archivo
        public List<Cliente> CargarClientes(string nombreArchivo)
        {
            // Creamos una lista vacía para guardar los clientes
            List<Cliente> listaDeClientes = new List<Cliente>();

            // Verificamos si el archivo existe
            if (File.Exists(nombreArchivo))
            {
                // Leemos todas las líneas del archivo
                string[] lineas = File.ReadAllLines(nombreArchivo);

                // Usamos un for para recorrer cada línea
                for (int i = 0; i < lineas.Length; i++)
                {
                    // Separamos los datos de la línea usando "|"
                    string[] datos = lineas[i].Split('|');

                    // Creamos una nueva instancia de Cliente
                    Cliente cliente = new Cliente();
                    cliente.Cedula = datos[0];
                    cliente.Nombre = datos[1];
                    cliente.Apellidos = datos[2];
                    cliente.Correo = datos[3];
                    cliente.Direccion = datos[4];
                    cliente.TelefonoCelular = datos[5];

                    // Convertimos el ingreso mensual de texto a número
                    if (decimal.TryParse(datos[6], out decimal ingresos))
                    {
                        cliente.IngresosMensuales = ingresos;
                    }
                    else
                    {
                        // Si hay error, ponemos 0
                        cliente.IngresosMensuales = 0;
                    }

                    // Agregamos el cliente a la lista
                    listaDeClientes.Add(cliente);
                }
            }

            return listaDeClientes;
        }
    }
}