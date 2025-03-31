using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

public class Sistema
{
    private readonly List<Cliente> clientes = new List<Cliente>();
    private readonly List<Credito> creditos = new List<Credito>();
    private readonly List<Debito> debitos = new List<Debito>();
    private readonly List<Prestamo> prestamos = new List<Prestamo>();
    private const string ArchivoClientes = "clientes.txt";
    private const string ArchivoUsuarios = "usuarios.txt";

    private bool sesionIniciada = false;

    public void Iniciar()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8; // Configurar codificación a UTF-8
        Console.Clear();
        Console.WriteLine("==========================================================");
        Console.WriteLine("   BIENVENIDO AL SISTEMA DE CRÉDITO Y DÉBITO PERSONALES   ");
        Console.WriteLine("==========================================================");
        Console.WriteLine();
        Console.WriteLine("  * Gestiona tus créditos, débitos y préstamos de forma fácil *");
        Console.WriteLine("  * Desarrollado por Asociados S.A. - Fecha: " + DateTime.Now.ToString("dd-MM-yyyy") + " *");
        Console.WriteLine();
        Console.WriteLine("-----------------------------------------------------");
        Console.WriteLine("  Presione ENTER para continuar...");
        Console.ReadLine();
        if (!AutenticarUsuario()) return;
        sesionIniciada = true;
        MostrarMenu();
    }

    private bool AutenticarUsuario()
    {
        Console.Clear();
        Console.WriteLine("====================================");
        Console.WriteLine("         MENÚ DE AUTENTICACIÓN      ");
        Console.WriteLine("====================================");
        Console.WriteLine();
        Console.WriteLine("  1. Registrarse");
        Console.WriteLine("  2. Iniciar sesión");
        Console.WriteLine();
        Console.WriteLine("------------------------------------");
        Console.Write("  Seleccione una opción: ");
        string opcion = Console.ReadLine();
        Console.WriteLine("------------------------------------");

        if (opcion == "1")
        {
            RegistrarUsuario();
            return true;
        }
        else if (opcion == "2")
        {
            bool sesionIniciada = IniciarSesion();
            if (!sesionIniciada)
            {
                return AutenticarUsuario();
            }
            return true;
        }
        else
        {
            Console.WriteLine();
            Console.WriteLine("  Opción inválida, por favor intente nuevamente.");
            Console.WriteLine("  Presione ENTER para continuar...");
            Console.ReadLine();
            return AutenticarUsuario();
        }
    }

    private bool IniciarSesion()
    {
        int intentos = 0;
        while (intentos < 3)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("         INICIAR SESIÓN            ");
            Console.WriteLine("====================================");
            Console.WriteLine();
            Console.Write("  Ingrese usuario: ");
            string usuario = Console.ReadLine();
            Console.Write("  Ingrese contraseña: ");
            string contrasena = OcultarContrasena();

            if (VerificarCredenciales(usuario, contrasena))
            {
                Console.WriteLine();
                Console.WriteLine($"  Bienvenido, {usuario}. Sesión iniciada en {DateTime.Now}.");
                Console.WriteLine("  Presione ENTER para continuar...");
                Console.ReadLine();
                return true;
            }

            intentos++;
            Console.WriteLine();
            Console.WriteLine($"  Credenciales incorrectas. Intento {intentos} de 3.");
            Console.WriteLine("  Presione ENTER para intentar nuevamente...");
            Console.ReadLine();
        }

        Console.WriteLine();
        Console.WriteLine("  Demasiados intentos fallidos. Espere 30 segundos.");

        for (int i = 30; i > 0; i--)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine($"  Espere {i} segundos para intentar nuevamente.");
            Console.WriteLine("====================================");
            Thread.Sleep(1000);
        }

        return false;
    }

    private void RegistrarUsuario()
    {
        Console.Clear();
        Console.WriteLine("====================================");
        Console.WriteLine("         REGISTRARSE               ");
        Console.WriteLine("====================================");
        Console.WriteLine();

        Console.Write("  Ingrese nuevo usuario: ");
        string usuario = Console.ReadLine();

        while (UsuarioExiste(usuario))
        {
            Console.WriteLine("  Ese usuario ya existe. Intente otro.");
            Console.Write("  Ingrese nuevo usuario: ");
            usuario = Console.ReadLine();
        }

        Console.Write("  Ingrese correo electrónico: ");
        string correo = Console.ReadLine();

        Console.Write("  Ingrese nueva contraseña: ");
        string contrasena = OcultarContrasena();

        string hash = ObtenerHash(contrasena);
        File.AppendAllText(ArchivoUsuarios, $"{usuario}|{hash}|{correo}\n");
        Console.WriteLine();
        Console.WriteLine("  Usuario registrado con éxito. Ahora puede iniciar sesión.");
        Console.WriteLine("  Presione ENTER para continuar...");
        Console.ReadLine();
    }

    private bool UsuarioExiste(string usuario)
    {
        if (!File.Exists(ArchivoUsuarios)) return false;
        string[] lineas = File.ReadAllLines(ArchivoUsuarios);
        foreach (string linea in lineas)
        {
            if (linea.Split('|')[0] == usuario) return true;
        }
        return false;
    }

    private bool VerificarCredenciales(string usuario, string contrasena)
    {
        if (!File.Exists(ArchivoUsuarios)) return false;
        string[] lineas = File.ReadAllLines(ArchivoUsuarios);
        string hash = ObtenerHash(contrasena);

        foreach (string linea in lineas)
        {
            string[] datos = linea.Split('|');
            if (datos[0] == usuario && datos[1] == hash) return true;
        }
        return false;
    }

    private string OcultarContrasena()
    {
        StringBuilder sb = new StringBuilder();
        while (true)
        {
            ConsoleKeyInfo tecla = Console.ReadKey(true);
            if (tecla.Key == ConsoleKey.Enter) break;
            if (tecla.Key == ConsoleKey.Backspace && sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
                Console.Write("\b \b");
            }
            else if (!char.IsControl(tecla.KeyChar))
            {
                sb.Append(tecla.KeyChar);
                Console.Write("*");
            }
        }
        Console.WriteLine();
        return sb.ToString();
    }

    private string ObtenerHash(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(bytes);
        }
    }

    private void MostrarMenu()
    {
        string opcion;
        do
        {
            Console.Clear(); // Limpiar pantalla al inicio del menú
            Console.WriteLine("====================================");
            Console.WriteLine("         MENÚ PRINCIPAL            ");
            Console.WriteLine("====================================");
            Console.WriteLine();
            Console.WriteLine("  1. Registrar nuevo cliente");
            Console.WriteLine("  2. Mostrar todos los clientes");
            Console.WriteLine("  3. Mostrar clientes que deben");
            Console.WriteLine("  4. Buscar cliente por cédula");
            Console.WriteLine("  5. Gestión de Créditos");
            Console.WriteLine("  6. Gestión de Débitos");
            Console.WriteLine("  7. Gestión de Préstamos Personales");
            Console.WriteLine("  8. Guardar en archivo (.txt)");
            Console.WriteLine("  9. Cargar desde archivo");
            Console.WriteLine("  10. Cerrar sesión");
            Console.WriteLine("  11. Salir del programa");
            Console.WriteLine();
            Console.WriteLine("------------------------------------");
            Console.Write("  Seleccione una opción: ");
            opcion = Console.ReadLine();
            Console.WriteLine("------------------------------------");
            Console.WriteLine($"  Usted seleccionó la opción: {opcion}");

            switch (opcion)
            {
                case "1": RegistrarCliente(); break;
                case "2": MostrarClientes(); break;
                case "3": MostrarClientesDeudores(); break;
                case "4": BuscarClientePorCedula(); break;
                case "5": GestionarCreditos(); break;
                case "6": GestionarDebitos(); break;
                case "7": GestionarPrestamos(); break;
                case "8": GuardarEnArchivo(); break;
                case "9": CargarDesdeArchivo(); break;
                case "10": CerrarSesion(); break;
                case "11": SalirDelPrograma(); break;
                default:
                    Console.WriteLine();
                    Console.WriteLine("  Opción inválida.");
                    Console.WriteLine("  Presione ENTER para continuar...");
                    Console.ReadLine();
                    break;
            }

        } while (opcion != "10");
    }

    private void CerrarSesion()
    {
        sesionIniciada = false;
        Console.WriteLine();
        Console.WriteLine("  Cerrando sesión...");
        Console.WriteLine("  Presione ENTER para continuar...");
        Console.ReadLine();
        Iniciar();
    }

    private void RegistrarCliente()
    {
        Console.Clear();
        Console.WriteLine("====================================");
        Console.WriteLine("     REGISTRAR NUEVO CLIENTE       ");
        Console.WriteLine("====================================");
        Console.WriteLine();

        Console.Write("  Ingrese el nombre del cliente: ");
        string nombre = Console.ReadLine();
        Console.Write("  Ingrese los apellidos del cliente: ");
        string apellidos = Console.ReadLine();
        Console.Write("  Ingrese la cédula del cliente: ");
        string cedula = Console.ReadLine();
        Console.Write("  Ingrese el correo del cliente: ");
        string correo = Console.ReadLine();
        Console.Write("  Ingrese la dirección del cliente: ");
        string direccion = Console.ReadLine();
        Console.Write("  Ingrese el teléfono celular del cliente: ");
        string telefonoCelular = Console.ReadLine();
        Console.Write("  Ingrese el teléfono de casa del cliente: ");
        string telefonoCasa = Console.ReadLine();
        Console.Write("  Ingrese el teléfono de oficina del cliente: ");
        string telefonoOficina = Console.ReadLine();
        Console.Write("  Ingrese los ingresos mensuales del cliente (₡): ");
        decimal ingresosMensuales = decimal.TryParse(Console.ReadLine(), out decimal ingresos) ? ingresos : 0;
        Console.Write("  Ingrese las fuentes de ingresos del cliente: ");
        string fuentesIngresos = Console.ReadLine();
        Console.Write("  Ingrese los gastos fijos mensuales del cliente (₡): ");
        decimal gastosFijosMensuales = decimal.TryParse(Console.ReadLine(), out decimal gastos) ? gastos : 0;
        Console.Write("  Ingrese el monto de ahorros del cliente (₡): ");
        decimal ahorros = decimal.TryParse(Console.ReadLine(), out decimal ahorro) ? ahorro : 0;

        Cliente nuevoCliente = new Cliente
        {
            Nombre = nombre,
            Apellidos = apellidos,
            Cedula = cedula,
            Correo = correo,
            Direccion = direccion,
            TelefonoCelular = telefonoCelular,
            TelefonoCasa = telefonoCasa,
            TelefonoOficina = telefonoOficina,
            IngresosMensuales = ingresosMensuales,
            FuentesIngresos = fuentesIngresos,
            GastosFijosMensuales = gastosFijosMensuales,
            Ahorros = ahorros,
            Deuda = 0
        };
        clientes.Add(nuevoCliente);
        GuardarEnArchivo(nuevoCliente);

        Console.WriteLine();
        Console.WriteLine("------------------------------------");
        Console.WriteLine("  Cliente registrado con éxito:");
        Console.WriteLine("------------------------------------");
        Console.WriteLine($"  Nombre: {nuevoCliente.Nombre} {nuevoCliente.Apellidos}");
        Console.WriteLine($"  Cédula: {nuevoCliente.Cedula}");
        Console.WriteLine($"  Correo: {nuevoCliente.Correo}");
        Console.WriteLine($"  Dirección: {nuevoCliente.Direccion}");
        Console.WriteLine($"  Teléfono Celular: {nuevoCliente.TelefonoCelular}");
        Console.WriteLine($"  Teléfono Casa: {nuevoCliente.TelefonoCasa}");
        Console.WriteLine($"  Teléfono Oficina: {nuevoCliente.TelefonoOficina}");
        Console.WriteLine($"  Ingresos Mensuales: ₡{nuevoCliente.IngresosMensuales:N2}");
        Console.WriteLine($"  Fuentes de Ingresos: {nuevoCliente.FuentesIngresos}");
        Console.WriteLine($"  Gastos Fijos Mensuales: ₡{nuevoCliente.GastosFijosMensuales:N2}");
        Console.WriteLine($"  Ahorros: ₡{nuevoCliente.Ahorros:N2}");
        Console.WriteLine($"  Deuda: ₡{nuevoCliente.Deuda:N2}");
        Console.WriteLine("------------------------------------");
        Console.WriteLine();
        Console.WriteLine("  Presione ENTER para volver al menú principal...");
        Console.ReadLine();
    }

    private void MostrarClientes()
    {
        Console.Clear();
        Console.WriteLine("====================================");
        Console.WriteLine("         LISTA DE CLIENTES         ");
        Console.WriteLine("====================================");
        Console.WriteLine();

        if (clientes.Count == 0)
        {
            Console.WriteLine("  No hay clientes registrados.");
        }
        else
        {
            foreach (var cliente in clientes)
            {
                Console.WriteLine("------------------------------------");
                Console.WriteLine($"  Nombre: {cliente.Nombre} {cliente.Apellidos}");
                Console.WriteLine($"  Cédula: {cliente.Cedula}");
                Console.WriteLine($"  Correo: {cliente.Correo}");
                Console.WriteLine($"  Dirección: {cliente.Direccion}");
                Console.WriteLine($"  Teléfono Celular: {cliente.TelefonoCelular}");
                Console.WriteLine($"  Teléfono Casa: {cliente.TelefonoCasa}");
                Console.WriteLine($"  Teléfono Oficina: {cliente.TelefonoOficina}");
                Console.WriteLine($"  Ingresos Mensuales: ₡{cliente.IngresosMensuales:N2}");
                Console.WriteLine($"  Fuentes de Ingresos: {cliente.FuentesIngresos}");
                Console.WriteLine($"  Gastos Fijos Mensuales: ₡{cliente.GastosFijosMensuales:N2}");
                Console.WriteLine($"  Ahorros: ₡{cliente.Ahorros:N2}");
                Console.WriteLine($"  Deuda: ₡{cliente.Deuda:N2}");
                Console.WriteLine("------------------------------------");
            }
        }

        Console.WriteLine();
        Console.WriteLine("  Fin de la lista de clientes.");
        Console.WriteLine("  Presione ENTER para volver al menú principal...");
        Console.ReadLine();
    }

    private void MostrarClientesDeudores()
    {
        Console.Clear();
        Console.WriteLine("====================================");
        Console.WriteLine("         CLIENTES DEUDORES         ");
        Console.WriteLine("====================================");
        Console.WriteLine();

        bool hayDeudores = false;
        foreach (var cliente in clientes)
        {
            cliente.ActualizarDeuda(prestamos);
            if (cliente.TieneDeuda())
            {
                hayDeudores = true;
                Console.WriteLine("------------------------------------");
                Console.WriteLine($"  Nombre: {cliente.Nombre} {cliente.Apellidos}");
                Console.WriteLine($"  Cédula: {cliente.Cedula}");
                Console.WriteLine($"  Correo: {cliente.Correo}");
                Console.WriteLine($"  Dirección: {cliente.Direccion}");
                Console.WriteLine($"  Teléfono Celular: {cliente.TelefonoCelular}");
                Console.WriteLine($"  Teléfono Casa: {cliente.TelefonoCasa}");
                Console.WriteLine($"  Teléfono Oficina: {cliente.TelefonoOficina}");
                Console.WriteLine($"  Ingresos Mensuales: ₡{cliente.IngresosMensuales:N2}");
                Console.WriteLine($"  Fuentes de Ingresos: {cliente.FuentesIngresos}");
                Console.WriteLine($"  Gastos Fijos Mensuales: ₡{cliente.GastosFijosMensuales:N2}");
                Console.WriteLine($"  Ahorros: ₡{cliente.Ahorros:N2}");
                Console.WriteLine($"  Deuda: ₡{cliente.Deuda:N2}");
                Console.WriteLine("------------------------------------");
            }
        }

        if (!hayDeudores)
        {
            Console.WriteLine("  No hay clientes con deudas.");
        }

        Console.WriteLine();
        Console.WriteLine("  Fin de la lista de clientes deudores.");
        Console.WriteLine("  Presione ENTER para volver al menú principal...");
        Console.ReadLine();
    }

    private void GestionarCreditos()
    {
        string opcion;
        do
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("         GESTIÓN DE CRÉDITOS       ");
            Console.WriteLine("====================================");
            Console.WriteLine();
            Console.WriteLine("  1. Registrar nuevo crédito");
            Console.WriteLine("  2. Ver créditos existentes");
            Console.WriteLine("  3. Usar crédito (aumentar monto utilizado)");
            Console.WriteLine("  4. Cerrar crédito");
            Console.WriteLine("  5. Volver al menú principal");
            Console.WriteLine();
            Console.WriteLine("------------------------------------");
            Console.Write("  Seleccione una opción: ");
            opcion = Console.ReadLine();
            Console.WriteLine("------------------------------------");

            switch (opcion)
            {
                case "1":
                    Console.Clear();
                    Console.WriteLine("====================================");
                    Console.WriteLine("      REGISTRAR NUEVO CRÉDITO     ");
                    Console.WriteLine("====================================");
                    Console.WriteLine();
                    Console.Write("  Ingrese la cédula del cliente: ");
                    string cedulaCredito = Console.ReadLine();
                    if (clientes.Find(c => c.Cedula == cedulaCredito) == null)
                    {
                        Console.WriteLine();
                        Console.WriteLine("  Cliente no encontrado.");
                    }
                    else
                    {
                        Console.Write("  Ingrese el nombre del crédito: ");
                        string nombre = Console.ReadLine();
                        Console.Write("  Ingrese el límite del crédito (₡): ");
                        decimal limite = Convert.ToDecimal(Console.ReadLine());
                        Console.Write("  Ingrese la tasa de interés anual (%): ");
                        decimal tasa = Convert.ToDecimal(Console.ReadLine());

                        Credito nuevoCredito = new Credito
                        {
                            Nombre = nombre,
                            MontoLimite = limite,
                            MontoUtilizado = 0,
                            TasaInteres = tasa,
                            FechaApertura = DateTime.Now,
                            FechaProximoPago = DateTime.Now.AddMonths(1),
                            CedulaCliente = cedulaCredito,
                            Activo = true
                        };
                        creditos.Add(nuevoCredito);
                        Console.WriteLine();
                        Console.WriteLine("------------------------------------");
                        Console.WriteLine("  Crédito registrado con éxito:");
                        Console.WriteLine("------------------------------------");
                        Console.WriteLine($"  {nuevoCredito.MostrarEstado()}");
                        Console.WriteLine("------------------------------------");
                    }
                    Console.WriteLine();
                    Console.WriteLine("  Presione ENTER para continuar...");
                    Console.ReadLine();
                    break;

                case "2":
                    Console.Clear();
                    Console.WriteLine("====================================");
                    Console.WriteLine("       CRÉDITOS EXISTENTES        ");
                    Console.WriteLine("====================================");
                    Console.WriteLine();
                    if (creditos.Count == 0)
                    {
                        Console.WriteLine("  No hay créditos registrados.");
                    }
                    else
                    {
                        foreach (var credito in creditos)
                        {
                            Console.WriteLine("------------------------------------");
                            Console.WriteLine($"  {credito.MostrarEstado()}");
                            Console.WriteLine("------------------------------------");
                        }
                    }
                    Console.WriteLine();
                    Console.WriteLine("  Presione ENTER para continuar...");
                    Console.ReadLine();
                    break;

                case "3":
                    Console.Clear();
                    Console.WriteLine("====================================");
                    Console.WriteLine("         USAR CRÉDITO             ");
                    Console.WriteLine("====================================");
                    Console.WriteLine();
                    Console.Write("  Ingrese la cédula del cliente: ");
                    string cedulaUso = Console.ReadLine();
                    Credito creditoUso = creditos.Find(c => c.CedulaCliente == cedulaUso && c.Activo);
                    if (creditoUso != null)
                    {
                        Console.Write("  Ingrese el monto a utilizar (₡): ");
                        decimal montoUso = Convert.ToDecimal(Console.ReadLine());
                        if (creditoUso.MontoUtilizado + montoUso <= creditoUso.MontoLimite)
                        {
                            creditoUso.MontoUtilizado += montoUso;
                            Console.WriteLine();
                            Console.WriteLine("------------------------------------");
                            Console.WriteLine("  Monto utilizado actualizado:");
                            Console.WriteLine("------------------------------------");
                            Console.WriteLine($"  {creditoUso.MostrarEstado()}");
                            Console.WriteLine("------------------------------------");
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.WriteLine("  Error: El monto excede el límite disponible.");
                        }
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("  Crédito no encontrado o no está activo.");
                    }
                    Console.WriteLine();
                    Console.WriteLine("  Presione ENTER para continuar...");
                    Console.ReadLine();
                    break;

                case "4":
                    Console.Clear();
                    Console.WriteLine("====================================");
                    Console.WriteLine("         CERRAR CRÉDITO           ");
                    Console.WriteLine("====================================");
                    Console.WriteLine();
                    Console.Write("  Ingrese la cédula del cliente: ");
                    string cedulaCierre = Console.ReadLine();
                    Credito creditoCierre = creditos.Find(c => c.CedulaCliente == cedulaCierre && c.Activo);
                    if (creditoCierre != null)
                    {
                        creditoCierre.Activo = false;
                        Console.WriteLine();
                        Console.WriteLine("------------------------------------");
                        Console.WriteLine("  Crédito cerrado con éxito:");
                        Console.WriteLine("------------------------------------");
                        Console.WriteLine($"  {creditoCierre.MostrarEstado()}");
                        Console.WriteLine("------------------------------------");
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("  Crédito no encontrado o ya está cerrado.");
                    }
                    Console.WriteLine();
                    Console.WriteLine("  Presione ENTER para continuar...");
                    Console.ReadLine();
                    break;

                case "5":
                    return;

                default:
                    Console.WriteLine();
                    Console.WriteLine("  Opción no válida.");
                    Console.WriteLine("  Presione ENTER para continuar...");
                    Console.ReadLine();
                    break;
            }
        } while (opcion != "5");
    }

    private void GestionarDebitos()
    {
        string opcion;
        do
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("         GESTIÓN DE DÉBITOS        ");
            Console.WriteLine("====================================");
            Console.WriteLine();
            Console.WriteLine("  1. Registrar nuevo débito");
            Console.WriteLine("  2. Ver débitos existentes");
            Console.WriteLine("  3. Marcar débito como pagado");
            Console.WriteLine("  4. Volver al menú principal");
            Console.WriteLine();
            Console.WriteLine("------------------------------------");
            Console.Write("  Seleccione una opción: ");
            opcion = Console.ReadLine();
            Console.WriteLine("------------------------------------");

            switch (opcion)
            {
                case "1":
                    Console.Clear();
                    Console.WriteLine("====================================");
                    Console.WriteLine("      REGISTRAR NUEVO DÉBITO      ");
                    Console.WriteLine("====================================");
                    Console.WriteLine();
                    Console.Write("  Ingrese la cédula del cliente: ");
                    string cedulaDebito = Console.ReadLine();
                    if (clientes.Find(c => c.Cedula == cedulaDebito) == null)
                    {
                        Console.WriteLine();
                        Console.WriteLine("  Cliente no encontrado.");
                    }
                    else
                    {
                        Console.Write("  Ingrese el monto del débito (₡): ");
                        decimal monto = Convert.ToDecimal(Console.ReadLine());
                        Console.Write("  Ingrese la descripción del débito: ");
                        string descripcion = Console.ReadLine();
                        Console.Write("  Ingrese la categoría (ej. Servicios, Alimentación): ");
                        string categoria = Console.ReadLine();

                        Debito nuevoDebito = new Debito
                        {
                            Monto = monto,
                            FechaDebito = DateTime.Now,
                            Descripcion = descripcion,
                            Categoria = categoria,
                            CedulaCliente = cedulaDebito,
                            Pagado = false
                        };
                        debitos.Add(nuevoDebito);

                        Console.WriteLine();
                        Console.WriteLine("------------------------------------");
                        Console.WriteLine("  Débito registrado con éxito:");
                        Console.WriteLine("------------------------------------");
                        Console.WriteLine($"  {nuevoDebito.MostrarEstado()}");
                        Console.WriteLine("------------------------------------");
                    }
                    Console.WriteLine();
                    Console.WriteLine("  Presione ENTER para continuar...");
                    Console.ReadLine();
                    break;

                case "2":
                    Console.Clear();
                    Console.WriteLine("====================================");
                    Console.WriteLine("       DÉBITOS EXISTENTES         ");
                    Console.WriteLine("====================================");
                    Console.WriteLine();
                    if (debitos.Count == 0)
                    {
                        Console.WriteLine("  No hay débitos registrados.");
                    }
                    else
                    {
                        foreach (var debito in debitos)
                        {
                            Console.WriteLine("------------------------------------");
                            Console.WriteLine($"  Cliente: {debito.CedulaCliente}");
                            Console.WriteLine($"  {debito.MostrarEstado()}");
                            Console.WriteLine("------------------------------------");
                        }
                    }
                    Console.WriteLine();
                    Console.WriteLine("  Presione ENTER para continuar...");
                    Console.ReadLine();
                    break;

                case "3":
                    Console.Clear();
                    Console.WriteLine("====================================");
                    Console.WriteLine("      MARCAR DÉBITO COMO PAGADO   ");
                    Console.WriteLine("====================================");
                    Console.WriteLine();
                    Console.Write("  Ingrese la cédula del cliente: ");
                    string cedulaPago = Console.ReadLine();
                    Debito debitoPago = debitos.Find(d => d.CedulaCliente == cedulaPago && !d.Pagado);
                    if (debitoPago != null)
                    {
                        debitoPago.Pagado = true;
                        Console.WriteLine();
                        Console.WriteLine("------------------------------------");
                        Console.WriteLine("  Débito marcado como pagado:");
                        Console.WriteLine("------------------------------------");
                        Console.WriteLine($"  {debitoPago.MostrarEstado()}");
                        Console.WriteLine("------------------------------------");
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("  Débito no encontrado o ya está pagado.");
                    }
                    Console.WriteLine();
                    Console.WriteLine("  Presione ENTER para continuar...");
                    Console.ReadLine();
                    break;

                case "4":
                    return;

                default:
                    Console.WriteLine();
                    Console.WriteLine("  Opción no válida.");
                    Console.WriteLine("  Presione ENTER para continuar...");
                    Console.ReadLine();
                    break;
            }
        } while (opcion != "4");
    }

    private void GestionarPrestamos()
    {
        string opcion;
        do
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("     GESTIÓN DE PRÉSTAMOS PERSONALES");
            Console.WriteLine("====================================");
            Console.WriteLine();
            Console.WriteLine("  1. Registrar nuevo préstamo");
            Console.WriteLine("  2. Ver préstamos existentes");
            Console.WriteLine("  3. Registrar pago mensual");
            Console.WriteLine("  4. Ver estado de un préstamo");
            Console.WriteLine("  5. Volver al menú principal");
            Console.WriteLine();
            Console.WriteLine("------------------------------------");
            Console.Write("  Seleccione una opción: ");
            opcion = Console.ReadLine();
            Console.WriteLine("------------------------------------");

            switch (opcion)
            {
                case "1":
                    Console.Clear();
                    Console.WriteLine("====================================");
                    Console.WriteLine("     REGISTRAR NUEVO PRÉSTAMO     ");
                    Console.WriteLine("====================================");
                    Console.WriteLine();
                    Console.Write("  Ingrese la cédula del cliente: ");
                    string cedula = Console.ReadLine();
                    if (clientes.Find(c => c.Cedula == cedula) == null)
                    {
                        Console.WriteLine();
                        Console.WriteLine("  Cliente no encontrado.");
                    }
                    else
                    {
                        Console.Write("  Ingrese el monto del préstamo (₡): ");
                        decimal monto = Convert.ToDecimal(Console.ReadLine());
                        Console.Write("  Ingrese la tasa de interés anual (%): ");
                        decimal tasa = Convert.ToDecimal(Console.ReadLine());
                        Console.Write("  Ingrese la cantidad de meses: ");
                        int plazo = Convert.ToInt32(Console.ReadLine());

                        Prestamo nuevoPrestamo = new Prestamo
                        {
                            Monto = monto,
                            Tasa = tasa,
                            Plazo = plazo,
                            FechaInicio = DateTime.Now,
                            CedulaCliente = cedula
                        };
                        nuevoPrestamo.CalcularCuotas();
                        prestamos.Add(nuevoPrestamo);

                        Console.WriteLine();
                        Console.WriteLine("------------------------------------");
                        Console.WriteLine("  Préstamo registrado con éxito:");
                        Console.WriteLine("------------------------------------");
                        Console.WriteLine($"  Cliente: {nuevoPrestamo.CedulaCliente}");
                        Console.WriteLine($"  {nuevoPrestamo.MostrarEstado()}");
                        Console.WriteLine("------------------------------------");
                    }
                    Console.WriteLine();
                    Console.WriteLine("  Presione ENTER para continuar...");
                    Console.ReadLine();
                    break;

                case "2":
                    Console.Clear();
                    Console.WriteLine("====================================");
                    Console.WriteLine("      PRÉSTAMOS EXISTENTES        ");
                    Console.WriteLine("====================================");
                    Console.WriteLine();
                    if (prestamos.Count == 0)
                    {
                        Console.WriteLine("  No hay préstamos registrados.");
                    }
                    else
                    {
                        foreach (var prestamo in prestamos)
                        {
                            Console.WriteLine("------------------------------------");
                            Console.WriteLine($"  Cliente: {prestamo.CedulaCliente}");
                            Console.WriteLine($"  {prestamo.MostrarEstado()}");
                            Console.WriteLine("------------------------------------");
                        }
                    }
                    Console.WriteLine();
                    Console.WriteLine("  Presione ENTER para continuar...");
                    Console.ReadLine();
                    break;

                case "3":
                    Console.Clear();
                    Console.WriteLine("====================================");
                    Console.WriteLine("      REGISTRAR PAGO MENSUAL      ");
                    Console.WriteLine("====================================");
                    Console.WriteLine();
                    Console.Write("  Ingrese la cédula del cliente: ");
                    string cedulaPago = Console.ReadLine();
                    Prestamo prestamoPago = prestamos.Find(p => p.CedulaCliente == cedulaPago);
                    if (prestamoPago != null && prestamoPago.SaldoPendiente > 0)
                    {
                        prestamoPago.RegistrarPagoMensual();
                        Console.WriteLine();
                        Console.WriteLine("------------------------------------");
                        Console.WriteLine("  Pago registrado:");
                        Console.WriteLine("------------------------------------");
                        Console.WriteLine($"  Cliente: {prestamoPago.CedulaCliente}");
                        Console.WriteLine($"  {prestamoPago.MostrarEstado()}");
                        Console.WriteLine("------------------------------------");
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("  Préstamo no encontrado o ya pagado.");
                    }
                    Console.WriteLine();
                    Console.WriteLine("  Presione ENTER para continuar...");
                    Console.ReadLine();
                    break;

                case "4":
                    Console.Clear();
                    Console.WriteLine("====================================");
                    Console.WriteLine("      ESTADO DE UN PRÉSTAMO       ");
                    Console.WriteLine("====================================");
                    Console.WriteLine();
                    Console.Write("  Ingrese la cédula del cliente: ");
                    string cedulaEstado = Console.ReadLine();
                    Prestamo prestamoEstado = prestamos.Find(p => p.CedulaCliente == cedulaEstado);
                    if (prestamoEstado != null)
                    {
                        Console.WriteLine();
                        Console.WriteLine("------------------------------------");
                        Console.WriteLine($"  Cliente: {prestamoEstado.CedulaCliente}");
                        Console.WriteLine($"  {prestamoEstado.MostrarEstado()}");
                        Console.WriteLine("------------------------------------");
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("  Préstamo no encontrado.");
                    }
                    Console.WriteLine();
                    Console.WriteLine("  Presione ENTER para continuar...");
                    Console.ReadLine();
                    break;

                case "5":
                    return;

                default:
                    Console.WriteLine();
                    Console.WriteLine("  Opción no válida.");
                    Console.WriteLine("  Presione ENTER para continuar...");
                    Console.ReadLine();
                    break;
            }
        } while (opcion != "5");
    }

    private void GuardarEnArchivo(Cliente cliente)
    {
        using (StreamWriter writer = new StreamWriter(ArchivoClientes, true))
        {
            writer.WriteLine($"{cliente.Nombre}|{cliente.Apellidos}|{cliente.Cedula}|{cliente.Correo}|{cliente.Direccion}|{cliente.TelefonoCelular}|{cliente.TelefonoCasa}|{cliente.TelefonoOficina}|{cliente.IngresosMensuales}|{cliente.FuentesIngresos}|{cliente.GastosFijosMensuales}|{cliente.Ahorros}");
        }
    }

    private void GuardarEnArchivo()
    {
        Console.Clear();
        Console.WriteLine("====================================");
        Console.WriteLine("         GUARDAR EN ARCHIVO        ");
        Console.WriteLine("====================================");
        Console.WriteLine();

        try
        {
            foreach (var cliente in clientes)
            {
                string linea = $"{cliente.Nombre}|{cliente.Apellidos}|{cliente.Cedula}|{cliente.Correo}|{cliente.Direccion}|{cliente.TelefonoCelular}|{cliente.TelefonoCasa}|{cliente.TelefonoOficina}|{cliente.IngresosMensuales}|{cliente.FuentesIngresos}|{cliente.GastosFijosMensuales}|{cliente.Ahorros}";
                File.AppendAllText("clientes.txt", linea + Environment.NewLine);
            }

            Console.WriteLine("  Clientes guardados en el archivo correctamente.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Error al guardar el archivo: {ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("  Presione ENTER para volver al menú principal...");
        Console.ReadLine();
    }

    private void CargarDesdeArchivo()
    {
        Console.Clear();
        Console.WriteLine("====================================");
        Console.WriteLine("         CARGAR DESDE ARCHIVO      ");
        Console.WriteLine("====================================");
        Console.WriteLine();

        if (File.Exists("clientes.txt"))
        {
            string[] lineas = File.ReadAllLines("clientes.txt");

            foreach (var linea in lineas)
            {
                string[] datos = linea.Split('|');

                if (datos.Length >= 12)
                {
                    Cliente cliente = new Cliente
                    {
                        Nombre = datos[0],
                        Apellidos = datos[1],
                        Cedula = datos[2],
                        Correo = datos[3],
                        Direccion = datos[4],
                        TelefonoCelular = datos[5],
                        TelefonoCasa = datos[6],
                        TelefonoOficina = datos[7],
                        IngresosMensuales = decimal.TryParse(datos[8], out decimal ingresos) ? ingresos : 0,
                        FuentesIngresos = datos[9],
                        GastosFijosMensuales = decimal.TryParse(datos[10], out decimal gastos) ? gastos : 0,
                        Ahorros = decimal.TryParse(datos[11], out decimal ahorros) ? ahorros : 0
                    };
                    clientes.Add(cliente);
                }
                else
                {
                    Console.WriteLine("  Error: Línea con datos incompletos.");
                }
            }
            Console.WriteLine("  Datos cargados desde el archivo correctamente.");
        }
        else
        {
            Console.WriteLine("  No se encontró el archivo clientes.txt.");
        }

        Console.WriteLine();
        Console.WriteLine("  Presione ENTER para volver al menú principal...");
        Console.ReadLine();
    }

    private void BuscarClientePorCedula()
    {
        Console.Clear();
        Console.WriteLine("====================================");
        Console.WriteLine("     BUSCAR CLIENTE POR CÉDULA     ");
        Console.WriteLine("====================================");
        Console.WriteLine();

        Console.Write("  Ingrese la cédula del cliente: ");
        string cedula = Console.ReadLine();

        Cliente cliente = clientes.Find(c => c.Cedula == cedula);

        if (cliente != null)
        {
            cliente.ActualizarDeuda(prestamos);
            Console.WriteLine();
            Console.WriteLine("------------------------------------");
            Console.WriteLine($"  Nombre: {cliente.Nombre} {cliente.Apellidos}");
            Console.WriteLine($"  Cédula: {cliente.Cedula}");
            Console.WriteLine($"  Correo: {cliente.Correo}");
            Console.WriteLine($"  Dirección: {cliente.Direccion}");
            Console.WriteLine($"  Teléfono Celular: {cliente.TelefonoCelular}");
            Console.WriteLine($"  Teléfono Casa: {cliente.TelefonoCasa}");
            Console.WriteLine($"  Teléfono Oficina: {cliente.TelefonoOficina}");
            Console.WriteLine($"  Ingresos Mensuales: ₡{cliente.IngresosMensuales:N2}");
            Console.WriteLine($"  Fuentes de Ingresos: {cliente.FuentesIngresos}");
            Console.WriteLine($"  Gastos Fijos Mensuales: ₡{cliente.GastosFijosMensuales:N2}");
            Console.WriteLine($"  Ahorros: ₡{cliente.Ahorros:N2}");
            Console.WriteLine($"  Deuda: ₡{cliente.Deuda:N2}");
            Console.WriteLine("------------------------------------");
        }
        else
        {
            Console.WriteLine();
            Console.WriteLine("  Cliente no encontrado.");
        }

        Console.WriteLine();
        Console.WriteLine("  Presione ENTER para volver al menú principal...");
        Console.ReadLine();
    }

    private void SalirDelPrograma()
    {
        Console.WriteLine();
        Console.WriteLine("  Saliendo del programa...");
        Console.WriteLine("  Presione ENTER para cerrar...");
        Console.ReadLine();
        Environment.Exit(0);
    }
}