using System.Collections.Generic;
using System.IO;

public static class Archivo
{
    public static void GuardarClientes(List<Cliente> clientes, string rutaArchivo)
    {
        using (StreamWriter writer = new StreamWriter(rutaArchivo))
        {
            foreach (var cliente in clientes)
            {
                writer.WriteLine($"{cliente.Cedula}|{cliente.Nombre}|{cliente.Apellidos}|{cliente.Correo}|{cliente.Direccion}|{cliente.TelefonoCelular}|{cliente.IngresosMensuales}");
            }
        }
    }

    public static List<Cliente> CargarClientes(string rutaArchivo)
    {
        List<Cliente> clientes = new List<Cliente>();
        if (File.Exists(rutaArchivo))
        {
            string[] lineas = File.ReadAllLines(rutaArchivo);
            foreach (var linea in lineas)
            {
                var datos = linea.Split('|');
                Cliente cliente = new Cliente
                {
                    Cedula = datos[0],
                    Nombre = datos[1],
                    Apellidos = datos[2],
                    Correo = datos[3],
                    Direccion = datos[4],
                    TelefonoCelular = datos[5],
                    IngresosMensuales = decimal.Parse(datos[6])
                };
                clientes.Add(cliente);
            }
        }
        return clientes;
    }
}
