using SistemaDeCreditoYDebitoPersonales.Model;
using SistemaDeCreditoYDebitoPersonales.Model.Dao;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

// Este espacio de nombres contiene la lógica del controlador del sistema de crédito y débito personales
namespace SistemaDeCreditoYDebitoPersonales.Control
{
    // Clase que representa el controlador del sistema
    public class SistemaController
    {
        private List<Cliente> listaDeClientes;
        private List<Credito> listaDeCreditos;
        private List<Debito> listaDeDebitos;
        private List<Prestamo> listaDePrestamos;
        private const string archivoClientes = "clientes.txt";
        private const string archivoCreditos = "creditos.txt";
        private const string archivoDebitos = "debitos.txt";
        private const string archivoPrestamos = "prestamos.txt";
        private const string archivoUsuarios = "usuarios.txt";
        private bool sesionIniciada;
        private string usuarioActual;
        private Log log;
        private Vista.vistaPrincipal vista;

        // Este constructor inicializa las listas y el log
        public SistemaController(Vista.vistaPrincipal vista)
        {
            listaDeClientes = new List<Cliente>();
            listaDeCreditos = new List<Credito>();
            listaDeDebitos = new List<Debito>();
            listaDePrestamos = new List<Prestamo>();
            sesionIniciada = false;
            usuarioActual = "";
            log = Log.GetInstance("log.txt");
            this.vista = vista;
        }
        // Este método se encarga de iniciar el sistema
        public void Iniciar()
        {
            log.RegistrarAccion("Sistema", "Programa iniciado");


            if (File.Exists(archivoClientes))
            {
                CargarClientesDesdeArchivoSilencioso();
                log.RegistrarAccion("Sistema", $"Clientes cargados automáticamente al iniciar: {listaDeClientes.Count} clientes encontrados");
            }
            else
            {
                log.RegistrarAccion("Sistema", "No se encontró el archivo clientes.txt al iniciar, comenzando con lista vacía");
            }

            if (File.Exists(archivoCreditos))
            {
                CargarCreditosDesdeArchivoSilencioso();
                log.RegistrarAccion("Sistema", $"Créditos cargados automáticamente al iniciar: {listaDeCreditos.Count} créditos encontrados");
            }
            else
            {
                log.RegistrarAccion("Sistema", "No se encontró el archivo creditos.txt al iniciar, comenzando con lista vacía");
            }

            if (File.Exists(archivoDebitos))
            {
                CargarDebitosDesdeArchivoSilencioso();
                log.RegistrarAccion("Sistema", $"Débitos cargados automáticamente al iniciar: {listaDeDebitos.Count} débitos encontrados");
            }
            else
            {
                log.RegistrarAccion("Sistema", "No se encontró el archivo debitos.txt al iniciar, comenzando con lista vacía");
            }

            if (File.Exists(archivoPrestamos))
            {
                CargarPrestamosDesdeArchivoSilencioso();
                log.RegistrarAccion("Sistema", $"Préstamos cargados automáticamente al iniciar: {listaDePrestamos.Count} préstamos encontrados");
            }
            else
            {
                log.RegistrarAccion("Sistema", "No se encontró el archivo prestamos.txt al iniciar, comenzando con lista vacía");
            }

            // Mostramos un mensaje de bienvenida
            Console.Clear();
            Console.WriteLine("==========================================================");
            Console.WriteLine("   BIENVENIDO AL SISTEMA DE CRÉDITO Y DÉBITO PERSONALES   ");
            Console.WriteLine("==========================================================");
            Console.WriteLine();
            Console.WriteLine("  * Gestiona tus créditos, débitos y préstamos de forma fácil *");
            Console.WriteLine("  * Desarrollado por Asociados S.A. - Fecha: " + DateTime.Now.ToString("dd-MM-yyyy") + " *");
            Console.WriteLine();
            Console.WriteLine("-----------------------------------------------------");
            Console.WriteLine("  Presiona ENTER para continuar...");
            Console.ReadLine();

            if (!AutenticarUsuario()) return;

            sesionIniciada = true;
            MostrarMenu();
        }

        // Este metodo se encarga de Mostrar Cargados.
        private void MostrarDatosCargados()
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("         DATOS CARGADOS            ");
            Console.WriteLine("====================================");
            Console.WriteLine();
            Console.WriteLine($"  Clientes cargados: {listaDeClientes.Count}");
            Console.WriteLine($"  Créditos cargados: {listaDeCreditos.Count}");
            Console.WriteLine($"  Débitos cargados: {listaDeDebitos.Count}");
            Console.WriteLine($"  Préstamos cargados: {listaDePrestamos.Count}");
            Console.WriteLine();
            Console.WriteLine("------------------------------------");
            Console.WriteLine("  Presiona ENTER para volver al menú principal...");
            Console.ReadLine();
        }
        //Este método se encarga de mostrar el menú principal
        private void MostrarMenu()
        {
            string opcion;

            do
            {
                Console.Clear();
                Console.WriteLine("====================================");
                Console.WriteLine("         MENÚ PRINCIPAL            ");
                Console.WriteLine("====================================");
                Console.WriteLine();
                Console.WriteLine("  1. Registrar nuevo cliente");
                Console.WriteLine("  2. Mostrar todos los clientes");
                Console.WriteLine("  3. Mostrar clientes deudores");
                Console.WriteLine("  4. Buscar cliente por cédula");
                Console.WriteLine("  5. Gestionar créditos");
                Console.WriteLine("  6. Gestionar débitos");
                Console.WriteLine("  7. Gestionar préstamos");
                Console.WriteLine("  8. Guardar en archivo");
                Console.WriteLine("  9. Cargar desde archivo");
                Console.WriteLine("  10. Cerrar sesión");
                Console.WriteLine("  11. Salir del programa");
                Console.WriteLine();
                Console.WriteLine("------------------------------------");
                Console.Write("  Digite el Numero de Opción ");
                opcion = Console.ReadLine();
                Console.WriteLine("------------------------------------");

                switch (opcion)
                {
                    case "1":
                        RegistrarCliente();
                        break;

                    case "2":
                        MostrarClientes();
                        break;

                    case "3":
                        MostrarClientesDeudores();
                        break;

                    case "4":
                        BuscarClientePorCedula();
                        break;

                    case "5":
                        GestionarCreditos();
                        break;

                    case "6":
                        GestionarDebitos();
                        break;

                    case "7":
                        GestionarPrestamos();
                        break;

                    case "8":
                        GuardarEnArchivo();
                        break;

                    case "9":
                        CargarDesdeArchivo();
                        break;

                    case "10":
                        CerrarSesion();
                        break;

                    case "11":
                        SalirDelPrograma();
                        break;

                    default:
                        log.RegistrarAccion(usuarioActual, $"Opción inválida seleccionada: {opcion}");
                        Console.WriteLine();
                        Console.WriteLine("  ¡Opción no válida!");
                        Console.WriteLine("  Presiona ENTER para continuar...");
                        Console.ReadLine();
                        break;
                }
            } while (opcion != "10");
        }

        // Cargar Clientes desde archivo sin mostrar datos
        private void CargarClientesDesdeArchivoSilencioso()
        {
            if (File.Exists(archivoClientes))
            {
                string[] lineas = File.ReadAllLines(archivoClientes);
                listaDeClientes.Clear();

                for (int i = 0; i < lineas.Length; i++)
                {
                    string[] datos = lineas[i].Split('|');
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
                        listaDeClientes.Add(cliente);
                    }
                    else
                    {
                        log.RegistrarAccion("Sistema", "Error al cargar clientes desde archivo: Línea con datos incompletos");
                    }
                }
            }
        }
        //Cargar Creditos desde archivo sin mostrar datos
        private void CargarCreditosDesdeArchivoSilencioso()
        {
            if (File.Exists(archivoCreditos))
            {
                string[] lineas = File.ReadAllLines(archivoCreditos);
                listaDeCreditos.Clear();
                int nextId = listaDeCreditos.Count > 0 ? listaDeCreditos.Max(c => c.Id) + 1 : 1; // Inicializar ID

                for (int i = 0; i < lineas.Length; i++)
                {
                    string[] datos = lineas[i].Split('|');
                    if (datos.Length >= 8)
                    {
                        Credito credito = new Credito(
                            datos[0], // Nombre
                            decimal.TryParse(datos[1], out decimal limite) ? limite : 0, // MontoLimite
                            decimal.TryParse(datos[3], out decimal tasa) ? tasa : 0, // TasaInteres
                            datos[6] // CedulaCliente
                        )
                        {
                            Id = nextId++, // Asignar ID único
                            MontoUtilizado = decimal.TryParse(datos[2], out decimal utilizado) ? utilizado : 0,
                            FechaApertura = DateTime.TryParse(datos[4], out DateTime fechaApertura) ? fechaApertura : DateTime.Now,
                            FechaProximoPago = DateTime.TryParse(datos[5], out DateTime fechaProximoPago) ? fechaProximoPago : DateTime.Now,
                            Activo = bool.TryParse(datos[7], out bool activo) ? activo : true
                        };
                        listaDeCreditos.Add(credito);
                    }
                    else
                    {
                        log.RegistrarAccion("Sistema", "Error al cargar créditos desde archivo: Línea con datos incompletos");
                    }
                }
            }
        }
        //Cargar Debitos desde archivo sin mostrar datos
        private void CargarDebitosDesdeArchivoSilencioso()
        {
            if (File.Exists(archivoDebitos))
            {
                string[] lineas = File.ReadAllLines(archivoDebitos);
                listaDeDebitos.Clear();

                for (int i = 0; i < lineas.Length; i++)
                {
                    string[] datos = lineas[i].Split('|');
                    if (datos.Length >= 7)
                    {
                        Debito debito = new Debito
                        {
                            Id = int.TryParse(datos[0], out int id) ? id : 0,
                            Monto = decimal.TryParse(datos[1], out decimal monto) ? monto : 0,
                            FechaDebito = DateTime.TryParse(datos[2], out DateTime fecha) ? fecha : DateTime.Now,
                            Descripcion = datos[3],
                            Categoria = datos[4],
                            CedulaCliente = datos[5],
                            Pagado = bool.TryParse(datos[6], out bool pagado) ? pagado : false
                        };
                        listaDeDebitos.Add(debito);
                    }
                    else
                    {
                        log.RegistrarAccion("Sistema", "Error al cargar débitos desde archivo: Línea con datos incompletos");
                    }
                }
            }
        }
        //Cargar Prestamos desde archivo sin mostrar datos
        private void CargarPrestamosDesdeArchivoSilencioso()
        {
            if (File.Exists(archivoPrestamos))
            {
                string[] lineas = File.ReadAllLines(archivoPrestamos);
                listaDePrestamos.Clear();

                for (int i = 0; i < lineas.Length; i++)
                {
                    string[] datos = lineas[i].Split('|');
                    if (datos.Length >= 7)
                    {
                        Prestamo prestamo = new Prestamo(log)
                        {
                            Monto = decimal.TryParse(datos[0], out decimal monto) ? monto : 0,
                            Tasa = decimal.TryParse(datos[1], out decimal tasa) ? tasa : 0,
                            Plazo = int.TryParse(datos[2], out int plazo) ? plazo : 0,
                            FechaInicio = DateTime.TryParse(datos[3], out DateTime fecha) ? fecha : DateTime.Now,
                            CedulaCliente = datos[4],
                            SaldoPendiente = decimal.TryParse(datos[5], out decimal saldo) ? saldo : 0,
                            CuotasPagadas = int.TryParse(datos[6], out int cuotas) ? cuotas : 0
                        };
                        prestamo.CalcularCuotas();
                        listaDePrestamos.Add(prestamo);
                    }
                    else
                    {
                        log.RegistrarAccion("Sistema", "Error al cargar préstamos desde archivo: Línea con datos incompletos");
                    }
                }
            }
        }

        // Este método se encarga de autenticar al usuario
        private bool AutenticarUsuario()
        {
            string opcion;

            do
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
                Console.Write("  Digite el Numero de Opción ");
                opcion = Console.ReadLine();
                Console.WriteLine("------------------------------------");

                switch (opcion)
                {
                    case "1":
                        RegistrarUsuario();
                        return true;

                    case "2":
                        bool sesionIniciada = IniciarSesion();
                        if (!sesionIniciada)
                        {
                            return AutenticarUsuario();
                        }
                        return true;

                    default:
                        log.RegistrarAccion("Sistema", "Opción inválida seleccionada en autenticación");
                        Console.WriteLine();
                        Console.WriteLine("  ¡Opción no válida! Por favor, elige 1 o 2.");
                        Console.WriteLine("  Presiona ENTER para intentar de nuevo...");
                        Console.ReadLine();
                        break;
                }
            } while (opcion != "1" && opcion != "2");

            return false;
        }
        // Este método se encarga de iniciar sesión
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
                Console.Write("  Ingresa tu usuario: ");
                string usuario = Console.ReadLine();
                Console.Write("  Ingresa tu contraseña: ");
                string contrasena = OcultarContrasena();

                if (VerificarCredenciales(usuario, contrasena))
                {
                    usuarioActual = usuario;
                    log.RegistrarAccion(usuarioActual, "Inicio de sesión exitoso");
                    Console.WriteLine();
                    Console.WriteLine($"  ¡Bienvenido, {usuario}! Sesión iniciada en {DateTime.Now}.");
                    Console.WriteLine("  Presiona ENTER para continuar...");
                    Console.ReadLine();
                    return true;
                }

                intentos++;
                log.RegistrarAccion("Sistema", $"Intento de inicio de sesión fallido ({intentos}/3) para usuario: {usuario}");
                Console.WriteLine();
                Console.WriteLine($"  Credenciales incorrectas. Intento {intentos} de 3.");
                Console.WriteLine("  Presiona ENTER para intentar de nuevo...");
                Console.ReadLine();
            }

            log.RegistrarAccion("Sistema", "Demasiados intentos fallidos, bloqueo temporal");
            Console.WriteLine();
            Console.WriteLine("  Demasiados intentos fallidos. Espera 5 segundos.");

            for (int i = 5; i > 0; i--)
            {
                Console.Clear();
                Console.WriteLine("====================================");
                Console.WriteLine($"  Espera {i} segundos para intentar de nuevo...");
                Console.WriteLine("====================================");
                Thread.Sleep(1000);
            }

            return false;
        }
        // Este método se encarga de registrar un nuevo usuario
        private void RegistrarUsuario()
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("         REGISTRARSE               ");
            Console.WriteLine("====================================");
            Console.WriteLine();

            string usuario;
            do
            {
                Console.Write("  Ingresa un nuevo usuario: ");
                usuario = Console.ReadLine();
                if (UsuarioExiste(usuario))
                {
                    log.RegistrarAccion("Sistema", $"Intento de registro fallido, usuario ya existe: {usuario}");
                    Console.WriteLine("  Ese usuario ya existe. Intenta con otro.");
                }
            } while (UsuarioExiste(usuario));

            Console.Write("  Ingresa tu correo electrónico: ");
            string correo = Console.ReadLine();

            Console.Write("  Ingresa tu nueva contraseña: ");
            string contrasena = OcultarContrasena();

            string hash = ObtenerHash(contrasena);
            File.AppendAllText(archivoUsuarios, $"{usuario}|{hash}|{correo}\n");
            log.RegistrarAccion(usuario, "Usuario registrado con éxito");
            Console.WriteLine();
            Console.WriteLine("  ¡Usuario registrado con éxito! Ahora puedes iniciar sesión.");
            Console.WriteLine("  Presiona ENTER para continuar...");
            Console.ReadLine();
        }
        // Este método verifica si el usuario ya existe
        private bool UsuarioExiste(string usuario)
        {
            if (!File.Exists(archivoUsuarios)) return false;

            string[] lineas = File.ReadAllLines(archivoUsuarios);
            for (int i = 0; i < lineas.Length; i++)
            {
                string[] datos = lineas[i].Split('|');
                if (datos[0] == usuario) return true;
            }
            return false;
        }
        // Este método verifica las credenciales del usuario
        private bool VerificarCredenciales(string usuario, string contrasena)
        {
            if (!File.Exists(archivoUsuarios)) return false;

            string[] lineas = File.ReadAllLines(archivoUsuarios);
            string hash = ObtenerHash(contrasena);

            for (int i = 0; i < lineas.Length; i++)
            {
                string[] datos = lineas[i].Split('|');
                if (datos[0] == usuario && datos[1] == hash) return true;
            }
            return false;
        }
        // Este método se encarga de ocultar la contraseña mientras el usuario la escribe
        private string OcultarContrasena()
        {
            StringBuilder contrasena = new StringBuilder();
            while (true)
            {
                ConsoleKeyInfo tecla = Console.ReadKey(true);
                if (tecla.Key == ConsoleKey.Enter) break;
                if (tecla.Key == ConsoleKey.Backspace && contrasena.Length > 0)
                {
                    contrasena.Remove(contrasena.Length - 1, 1);
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(tecla.KeyChar))
                {
                    contrasena.Append(tecla.KeyChar);
                    Console.Write("*");
                }
            }
            Console.WriteLine();
            return contrasena.ToString();
        }
        //Este método se encarga de obtener el hash de la contraseña
        private string ObtenerHash(string texto)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(texto));
                return Convert.ToBase64String(bytes);
            }
        }
        // Método para cerrar sesión
        private void CerrarSesion()
        {
            sesionIniciada = false;
            log.RegistrarAccion(usuarioActual, "Sesión cerrada");
            Console.WriteLine();
            Console.WriteLine("  Cerrando sesión...");
            Console.WriteLine("  Presiona ENTER para continuar...");
            Console.ReadLine();
            Iniciar();
        }
        // Método para salir del programa
        private void RegistrarCliente()
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("     REGISTRAR NUEVO CLIENTE       ");
            Console.WriteLine("====================================");
            Console.WriteLine();

            string nombre, apellidos, cedula;

            do
            {
                Console.Write("  Ingresa el nombre del cliente: ");
                nombre = Console.ReadLine();
                if (string.IsNullOrEmpty(nombre))
                {
                    Console.WriteLine("  ¡Error! El nombre no puede estar vacío. Intenta de nuevo.");
                }
            } while (string.IsNullOrEmpty(nombre));

            Console.Write("  Ingresa los apellidos del cliente: ");
            apellidos = Console.ReadLine();

            do
            {
                Console.Write("  Ingresa la cédula del cliente: ");
                cedula = Console.ReadLine();
                if (string.IsNullOrEmpty(cedula))
                {
                    Console.WriteLine("  ¡Error! La cédula no puede estar vacía. Intenta de nuevo.");
                }
                else if (listaDeClientes.Exists(c => c.Cedula == cedula))
                {
                    Console.WriteLine("  ¡Error! Ya existe un cliente con esa cédula. Intenta de nuevo.");
                    cedula = "";
                }
            } while (string.IsNullOrEmpty(cedula));

            Console.Write("  Ingresa el correo del cliente: ");
            string correo = Console.ReadLine();
            Console.Write("  Ingresa la dirección del cliente: ");
            string direccion = Console.ReadLine();
            Console.Write("  Ingresa el teléfono celular del cliente: ");
            string telefonoCelular = Console.ReadLine();
            Console.Write("  Ingresa el teléfono de casa del cliente: ");
            string telefonoCasa = Console.ReadLine();
            Console.Write("  Ingresa el teléfono de oficina del cliente: ");
            string telefonoOficina = Console.ReadLine();
            Console.Write("  Ingresa los ingresos mensuales del cliente (₡): ");
            decimal ingresosMensuales = decimal.TryParse(Console.ReadLine(), out decimal ingresos) ? ingresos : 0;
            Console.Write("  Ingresa las fuentes de ingresos del cliente: ");
            string fuentesIngresos = Console.ReadLine();
            Console.Write("  Ingresa los gastos fijos mensuales del cliente (₡): ");
            decimal gastosFijosMensuales = decimal.TryParse(Console.ReadLine(), out decimal gastos) ? gastos : 0;
            Console.Write("  Ingresa el monto de ahorros del cliente (₡): ");
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

            listaDeClientes.Add(nuevoCliente);
            GuardarClienteEnArchivo(nuevoCliente);
            log.RegistrarAccion(usuarioActual, $"Cliente registrado: {nuevoCliente.Cedula}");

            Console.WriteLine();
            Console.WriteLine("------------------------------------");
            Console.WriteLine("  ¡Cliente registrado con éxito!");
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
            Console.WriteLine("  Presiona ENTER para volver al menú principal...");
            Console.ReadLine();
        }
        // Método para guardar un cliente en el archivo
        private void MostrarClientes()
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("         LISTA DE CLIENTES         ");
            Console.WriteLine("====================================");
            Console.WriteLine();

            if (listaDeClientes.Count == 0)
            {
                log.RegistrarAccion(usuarioActual, "Consulta de lista de clientes: No hay clientes registrados");
                Console.WriteLine("  No hay clientes registrados.");
            }
            else
            {
                log.RegistrarAccion(usuarioActual, "Consulta de lista de clientes realizada");
                for (int i = 0; i < listaDeClientes.Count; i++)
                {
                    Cliente cliente = listaDeClientes[i];
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
                    var creditos = listaDeCreditos.Where(c => c.CedulaCliente == cliente.Cedula).ToList();
                    if (creditos.Any())
                    {
                        Console.WriteLine("  Créditos asociados:");
                        foreach (var credito in creditos)
                        {
                            Console.WriteLine($"    - {credito.Nombre} (ID: {credito.Id})");
                        }
                    }
                    else
                    {
                        Console.WriteLine("  No tiene créditos asociados.");
                    }
                    var debitos = listaDeDebitos.Where(d => d.CedulaCliente == cliente.Cedula).ToList();
                    if (debitos.Any())
                    {
                        Console.WriteLine("  Débitos asociados:");
                        foreach (var debito in debitos)
                        {
                            Console.WriteLine($"    - {debito.Descripcion} (ID: {debito.Id})");
                        }
                    }
                    else
                    {
                        Console.WriteLine("  No tiene débitos asociados.");
                    }
                    Console.WriteLine("------------------------------------");
                }
            }

            Console.WriteLine();
            Console.WriteLine("  Fin de la lista de clientes.");
            Console.WriteLine("  Presiona ENTER para volver al menú principal...");
            Console.ReadLine();
        }
        // Método para mostrar un clientes deudores
        private void MostrarClientesDeudores()
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("         CLIENTES DEUDORES         ");
            Console.WriteLine("====================================");
            Console.WriteLine();

            bool hayDeudores = false;
            for (int i = 0; i < listaDeClientes.Count; i++)
            {
                Cliente cliente = listaDeClientes[i];
                cliente.ActualizarDeuda(listaDePrestamos);
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
                log.RegistrarAccion(usuarioActual, "Consulta de clientes deudores: No hay deudores");
                Console.WriteLine("  No hay clientes con deudas.");
            }
            else
            {
                log.RegistrarAccion(usuarioActual, "Consulta de clientes deudores realizada");
            }

            Console.WriteLine();
            Console.WriteLine("  Fin de la lista de clientes deudores.");
            Console.WriteLine("  Presiona ENTER para volver al menú principal...");
            Console.ReadLine();
        }
        // Método para gestionar los créditos
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
                Console.WriteLine("  5. Editar crédito");
                Console.WriteLine("  6. Volver al menú principal");
                Console.WriteLine();
                Console.WriteLine("------------------------------------");
                Console.Write("  Digite el Numero de Opción ");
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
                        Console.Write("  Ingresa la cédula del cliente: ");
                        string cedulaCredito = Console.ReadLine();
                        if (!listaDeClientes.Exists(c => c.Cedula == cedulaCredito))
                        {
                            log.RegistrarAccion(usuarioActual, $"Intento de registro de crédito fallido, cliente no encontrado: {cedulaCredito}");
                            Console.WriteLine();
                            Console.WriteLine("  Cliente no encontrado.");
                        }
                        else
                        {
                            Console.Write("  Ingresa el nombre del crédito: ");
                            string nombre = Console.ReadLine();
                            Console.Write("  Ingresa el límite del crédito (₡): ");
                            decimal limite = decimal.TryParse(Console.ReadLine(), out decimal lim) ? lim : 0;
                            Console.Write("  Ingresa la tasa de interés anual (%): ");
                            decimal tasa = decimal.TryParse(Console.ReadLine(), out decimal tas) ? tas : 0;

                            // Generar ID único
                            int nuevoId = 1;
                            if (listaDeCreditos.Count > 0)
                            {
                                int maxId = listaDeCreditos.Max(c => c.Id);
                                nuevoId = maxId > 0 ? maxId + 1 : listaDeCreditos.Count + 1;
                            }
                            // Crear nuevo crédito
                            Credito nuevoCredito = new Credito(
                               nombre, // Nombre del crédito
                               limite, // Monto límite
                               tasa,   // Tasa de interés
                               cedulaCredito // Cédula del cliente
                           )
                            {
                                Id = nuevoId,
                                MontoUtilizado = 0,
                                FechaApertura = DateTime.Now,
                                FechaProximoPago = DateTime.Now.AddMonths(1),
                                Activo = true
                            };
                            listaDeCreditos.Add(nuevoCredito);
                            GuardarCreditoEnArchivo(nuevoCredito);
                            log.RegistrarAccion(usuarioActual, $"Crédito registrado: {nuevoCredito.Nombre} (ID: {nuevoCredito.Id}) para cliente {cedulaCredito}");
                            Console.WriteLine();
                            Console.WriteLine("------------------------------------");
                            Console.WriteLine("  ¡Crédito registrado con éxito!");
                            Console.WriteLine("------------------------------------");
                            Console.WriteLine($"  {nuevoCredito.MostrarEstado()}");
                            Console.WriteLine("------------------------------------");
                        }
                        Console.WriteLine();
                        Console.WriteLine("  Presiona ENTER para continuar...");
                        Console.ReadLine();
                        break;

                    case "2":
                        Console.Clear();
                        Console.WriteLine("====================================");
                        Console.WriteLine("       CRÉDITOS EXISTENTES        ");
                        Console.WriteLine("====================================");
                        Console.WriteLine();
                        if (listaDeCreditos.Count == 0)
                        {
                            log.RegistrarAccion(usuarioActual, "Consulta de créditos: No hay créditos registrados");
                            Console.WriteLine("  No hay créditos registrados.");
                        }
                        else
                        {
                            log.RegistrarAccion(usuarioActual, "Consulta de créditos existentes realizada");
                            for (int i = 0; i < listaDeCreditos.Count; i++)
                            {
                                Console.WriteLine("------------------------------------");
                                Console.WriteLine($"  {listaDeCreditos[i].MostrarEstado()}");
                                Console.WriteLine("------------------------------------");
                            }
                        }
                        Console.WriteLine();
                        Console.WriteLine("  Presiona ENTER para continuar...");
                        Console.ReadLine();
                        break;

                    case "3":
                        Console.Clear();
                        Console.WriteLine("====================================");
                        Console.WriteLine("         USAR CRÉDITO             ");
                        Console.WriteLine("====================================");
                        Console.WriteLine();
                        Console.Write("  Ingresa la cédula del cliente: ");
                        string cedulaUso = Console.ReadLine();
                        Credito creditoUso = listaDeCreditos.Find(c => c.CedulaCliente == cedulaUso && c.Activo);
                        if (creditoUso != null)
                        {
                            Console.Write("  Ingresa el monto a utilizar (₡): ");
                            decimal montoUso = decimal.TryParse(Console.ReadLine(), out decimal monto) ? monto : 0;
                            if (creditoUso.MontoUtilizado + montoUso <= creditoUso.MontoLimite)
                            {
                                creditoUso.MontoUtilizado += montoUso;
                                GuardarTodosLosCreditosEnArchivo();
                                log.RegistrarAccion(usuarioActual, $"Crédito usado: {creditoUso.Nombre}, Monto: ₡{montoUso}");
                                Console.WriteLine();
                                Console.WriteLine("------------------------------------");
                                Console.WriteLine("  Monto utilizado actualizado:");
                                Console.WriteLine("------------------------------------");
                                Console.WriteLine($"  {creditoUso.MostrarEstado()}");
                                Console.WriteLine("------------------------------------");
                            }
                            else
                            {
                                log.RegistrarAccion(usuarioActual, $"Intento de uso de crédito fallido, monto excede límite: {creditoUso.Nombre}");
                                Console.WriteLine();
                                Console.WriteLine("  ¡Error! El monto excede el límite disponible.");
                            }
                        }
                        else
                        {
                            log.RegistrarAccion(usuarioActual, $"Intento de uso de crédito fallido, crédito no encontrado o no activo: {cedulaUso}");
                            Console.WriteLine();
                            Console.WriteLine("  Crédito no encontrado o no está activo.");
                        }
                        Console.WriteLine();
                        Console.WriteLine("  Presiona ENTER para continuar...");
                        Console.ReadLine();
                        break;

                    case "4":
                        Console.Clear();
                        Console.WriteLine("====================================");
                        Console.WriteLine("         CERRAR CRÉDITO           ");
                        Console.WriteLine("====================================");
                        Console.WriteLine();
                        Console.Write("  Ingresa la cédula del cliente: ");
                        string cedulaCierre = Console.ReadLine();
                        Credito creditoCierre = listaDeCreditos.Find(c => c.CedulaCliente == cedulaCierre && c.Activo);
                        if (creditoCierre != null)
                        {
                            creditoCierre.Activo = false;
                            GuardarTodosLosCreditosEnArchivo();
                            log.RegistrarAccion(usuarioActual, $"Crédito cerrado: {creditoCierre.Nombre} para cliente {cedulaCierre}");
                            Console.WriteLine();
                            Console.WriteLine("------------------------------------");
                            Console.WriteLine("  ¡Crédito cerrado con éxito!");
                            Console.WriteLine("------------------------------------");
                            Console.WriteLine($"  {creditoCierre.MostrarEstado()}");
                            Console.WriteLine("------------------------------------");
                        }
                        else
                        {
                            log.RegistrarAccion(usuarioActual, $"Intento de cierre de crédito fallido, crédito no encontrado o ya cerrado: {cedulaCierre}");
                            Console.WriteLine();
                            Console.WriteLine("  Crédito no encontrado o ya está cerrado.");
                        }
                        Console.WriteLine();
                        Console.WriteLine("  Presiona ENTER para continuar...");
                        Console.ReadLine();
                        break;

                    case "5":
                        Console.Clear();
                        Console.WriteLine("====================================");
                        Console.WriteLine("         EDITAR CRÉDITO           ");
                        Console.WriteLine("====================================");
                        Console.WriteLine();
                        Console.Write("  Ingresa el ID del crédito: ");
                        if (int.TryParse(Console.ReadLine(), out int idCredito))
                        {
                            Credito credito = listaDeCreditos.Find(c => c.Id == idCredito && c.Activo);
                            if (credito != null)
                            {
                                Console.Write("  Nuevo nombre del crédito (dejar en blanco para no cambiar): ");
                                string nuevoNombre = Console.ReadLine();
                                if (!string.IsNullOrEmpty(nuevoNombre))
                                    credito.Nombre = nuevoNombre;

                                Console.Write("  Nuevo límite del crédito (₡, dejar en blanco para no cambiar): ");
                                if (decimal.TryParse(Console.ReadLine(), out decimal nuevoLimite) && nuevoLimite >= credito.MontoUtilizado)
                                    credito.MontoLimite = nuevoLimite;

                                Console.Write("  Nueva tasa de interés anual (%, dejar en blanco para no cambiar): ");
                                if (decimal.TryParse(Console.ReadLine(), out decimal nuevaTasa))
                                    credito.TasaInteres = nuevaTasa;

                                GuardarTodosLosCreditosEnArchivo();
                                log.RegistrarAccion(usuarioActual, $"Crédito editado: ID {idCredito}");
                                Console.WriteLine();
                                Console.WriteLine("------------------------------------");
                                Console.WriteLine("  ¡Crédito editado con éxito!");
                                Console.WriteLine("------------------------------------");
                                Console.WriteLine($"  {credito.MostrarEstado()}");
                                Console.WriteLine("------------------------------------");
                            }
                            else
                            {
                                log.RegistrarAccion(usuarioActual, $"Intento de edición de crédito fallido, ID no encontrado o no activo: {idCredito}");
                                Console.WriteLine();
                                Console.WriteLine("  Crédito no encontrado o no está activo.");
                            }
                        }
                        else
                        {
                            log.RegistrarAccion(usuarioActual, "Intento de edición de crédito fallido, ID inválido");
                            Console.WriteLine();
                            Console.WriteLine("  ID inválido.");
                        }
                        Console.WriteLine();
                        Console.WriteLine("  Presiona ENTER para continuar...");
                        Console.ReadLine();
                        break;

                    case "6":
                        return;

                    default:
                        log.RegistrarAccion(usuarioActual, $"Opción inválida seleccionada en gestión de créditos: {opcion}");
                        Console.WriteLine();
                        Console.WriteLine("  ¡Opción no válida!");
                        Console.WriteLine("  Presiona ENTER para continuar...");
                        Console.ReadLine();
                        break;
                }
            } while (opcion != "6");
        }

        // Método para gestionar débitos
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
                Console.WriteLine("  4. Editar débito");
                Console.WriteLine("  5. Volver al menú principal");
                Console.WriteLine();
                Console.WriteLine("------------------------------------");
                Console.Write("  Digite el Numero de Opción ");
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
                        Console.Write("  Ingresa la cédula del cliente: ");
                        string cedulaDebito = Console.ReadLine();
                        if (!listaDeClientes.Exists(c => c.Cedula == cedulaDebito))
                        {
                            log.RegistrarAccion(usuarioActual, $"Intento de registro de débito fallido, cliente no encontrado: {cedulaDebito}");
                            Console.WriteLine();
                            Console.WriteLine("  Cliente no encontrado.");
                        }
                        else
                        {
                            Console.Write("  Ingresa el monto del débito (₡): ");
                            decimal monto = decimal.TryParse(Console.ReadLine(), out decimal mon) ? mon : 0;
                            Console.Write("  Ingresa la descripción del débito: ");
                            string descripcion = Console.ReadLine();
                            Console.Write("  Ingresa la categoría (ej. Servicios, Alimentación): ");
                            string categoria = Console.ReadLine();

                            int nuevoId = listaDeDebitos.Count > 0 ? listaDeDebitos.Max(d => d.Id) + 1 : 1;

                            Debito nuevoDebito = new Debito
                            {
                                Id = nuevoId,
                                Monto = monto,
                                FechaDebito = DateTime.Now,
                                Descripcion = descripcion,
                                Categoria = categoria,
                                CedulaCliente = cedulaDebito,
                                Pagado = false
                            };
                            listaDeDebitos.Add(nuevoDebito);
                            GuardarDebitoEnArchivo(nuevoDebito);
                            log.RegistrarAccion(usuarioActual, $"Débito registrado: {descripcion} (ID: {nuevoId}) para cliente {cedulaDebito}");

                            Console.WriteLine();
                            Console.WriteLine("------------------------------------");
                            Console.WriteLine("  ¡Débito registrado con éxito!");
                            Console.WriteLine("------------------------------------");
                            Console.WriteLine($"  {nuevoDebito.MostrarEstado()}");
                            Console.WriteLine("------------------------------------");
                        }
                        Console.WriteLine();
                        Console.WriteLine("  Presiona ENTER para continuar...");
                        Console.ReadLine();
                        break;

                    case "2":
                        Console.Clear();
                        Console.WriteLine("====================================");
                        Console.WriteLine("       DÉBITOS EXISTENTES         ");
                        Console.WriteLine("====================================");
                        Console.WriteLine();
                        if (listaDeDebitos.Count == 0)
                        {
                            log.RegistrarAccion(usuarioActual, "Consulta de débitos: No hay débitos registrados");
                            Console.WriteLine("  No hay débitos registrados.");
                        }
                        else
                        {
                            log.RegistrarAccion(usuarioActual, "Consulta de débitos existentes realizada");
                            for (int i = 0; i < listaDeDebitos.Count; i++)
                            {
                                Console.WriteLine("------------------------------------");
                                Console.WriteLine($"  Cliente: {listaDeDebitos[i].CedulaCliente}");
                                Console.WriteLine($"  {listaDeDebitos[i].MostrarEstado()}");
                                Console.WriteLine("------------------------------------");
                            }
                        }
                        Console.WriteLine();
                        Console.WriteLine("  Presiona ENTER para continuar...");
                        Console.ReadLine();
                        break;

                    case "3":
                        Console.Clear();
                        Console.WriteLine("====================================");
                        Console.WriteLine("      MARCAR DÉBITO COMO PAGADO   ");
                        Console.WriteLine("====================================");
                        Console.WriteLine();
                        Console.Write("  Ingresa la cédula del cliente: ");
                        string cedulaPago = Console.ReadLine();
                        Debito debitoPago = listaDeDebitos.Find(d => d.CedulaCliente == cedulaPago && !d.Pagado);
                        if (debitoPago != null)
                        {
                            debitoPago.Pagado = true;
                            GuardarTodosLosDebitosEnArchivo();
                            log.RegistrarAccion(usuarioActual, $"Débito marcado como pagado: {debitoPago.Descripcion} para cliente {cedulaPago}");
                            Console.WriteLine();
                            Console.WriteLine("------------------------------------");
                            Console.WriteLine("  ¡Débito marcado como pagado!");
                            Console.WriteLine("------------------------------------");
                            Console.WriteLine($"  {debitoPago.MostrarEstado()}");
                            Console.WriteLine("------------------------------------");
                        }
                        else
                        {
                            log.RegistrarAccion(usuarioActual, $"Intento de marcar débito como pagado fallido, no encontrado o ya pagado: {cedulaPago}");
                            Console.WriteLine();
                            Console.WriteLine("  Débito no encontrado o ya está pagado.");
                        }
                        Console.WriteLine();
                        Console.WriteLine("  Presiona ENTER para continuar...");
                        Console.ReadLine();
                        break;

                    case "4":
                        Console.Clear();
                        Console.WriteLine("====================================");
                        Console.WriteLine("         EDITAR DÉBITO            ");
                        Console.WriteLine("====================================");
                        Console.WriteLine();
                        Console.Write("  Ingresa el ID del débito: ");
                        if (int.TryParse(Console.ReadLine(), out int idDebito))
                        {
                            Debito debito = listaDeDebitos.Find(d => d.Id == idDebito && !d.Pagado);
                            if (debito != null)
                            {
                                Console.Write("  Nueva descripción (dejar en blanco para no cambiar): ");
                                string nuevaDescripcion = Console.ReadLine();
                                if (!string.IsNullOrEmpty(nuevaDescripcion))
                                    debito.Descripcion = nuevaDescripcion;

                                Console.Write("  Nuevo monto (₡, dejar en blanco para no cambiar): ");
                                if (decimal.TryParse(Console.ReadLine(), out decimal nuevoMonto) && nuevoMonto > 0)
                                    debito.Monto = nuevoMonto;

                                Console.Write("  Nueva categoría (dejar en blanco para no cambiar): ");
                                string nuevaCategoria = Console.ReadLine();
                                if (!string.IsNullOrEmpty(nuevaCategoria))
                                    debito.Categoria = nuevaCategoria;

                                GuardarTodosLosDebitosEnArchivo();
                                log.RegistrarAccion(usuarioActual, $"Débito editado: ID {idDebito}");
                                Console.WriteLine();
                                Console.WriteLine("------------------------------------");
                                Console.WriteLine("  ¡Débito editado con éxito!");
                                Console.WriteLine("------------------------------------");
                                Console.WriteLine($"  {debito.MostrarEstado()}");
                                Console.WriteLine("------------------------------------");
                            }
                            else
                            {
                                log.RegistrarAccion(usuarioActual, $"Intento de edición de débito fallido, ID no encontrado o ya pagado: {idDebito}");
                                Console.WriteLine();
                                Console.WriteLine("  Débito no encontrado o ya está pagado.");
                            }
                        }
                        else
                        {
                            log.RegistrarAccion(usuarioActual, "Intento de edición de débito fallido, ID inválido");
                            Console.WriteLine();
                            Console.WriteLine("  ID inválido.");
                        }
                        Console.WriteLine();
                        Console.WriteLine("  Presiona ENTER para continuar...");
                        Console.ReadLine();
                        break;

                    case "5":
                        return;

                    default:
                        log.RegistrarAccion(usuarioActual, $"Opción inválida seleccionada en gestión de débitos: {opcion}");
                        Console.WriteLine();
                        Console.WriteLine("  ¡Opción no válida!");
                        Console.WriteLine("  Presiona ENTER para continuar...");
                        Console.ReadLine();
                        break;
                }
            } while (opcion != "5");
        }
        // Método para gestionar préstamos
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
                Console.Write("  Digite el Numero de Opción ");
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
                        Console.Write("  Ingresa la cédula del cliente: ");
                        string cedula = Console.ReadLine();
                        if (!listaDeClientes.Exists(c => c.Cedula == cedula))
                        {
                            log.RegistrarAccion(usuarioActual, $"Intento de registro de préstamo fallido, cliente no encontrado: {cedula}");
                            Console.WriteLine();
                            Console.WriteLine("  Cliente no encontrado.");
                        }
                        else
                        {
                            Console.Write("  Ingresa el monto del préstamo (₡): ");
                            decimal monto = decimal.TryParse(Console.ReadLine(), out decimal mon) ? mon : 0;
                            Console.Write("  Ingresa la tasa de interés anual (%): ");
                            decimal tasa = decimal.TryParse(Console.ReadLine(), out decimal tas) ? tas : 0;
                            Console.Write("  Ingresa la cantidad de meses: ");
                            int plazo = int.TryParse(Console.ReadLine(), out int pla) ? pla : 0;

                            Prestamo nuevoPrestamo = new Prestamo(log)
                            {
                                Monto = monto,
                                Tasa = tasa,
                                Plazo = plazo,
                                FechaInicio = DateTime.Now,
                                CedulaCliente = cedula
                            };
                            nuevoPrestamo.CalcularCuotas();
                            listaDePrestamos.Add(nuevoPrestamo);
                            GuardarPrestamoEnArchivo(nuevoPrestamo);
                            log.RegistrarAccion(usuarioActual, $"Préstamo registrado para cliente {cedula}, Monto: ₡{monto}");

                            Console.WriteLine();
                            Console.WriteLine("------------------------------------");
                            Console.WriteLine("  ¡Préstamo registrado con éxito!");
                            Console.WriteLine("------------------------------------");
                            Console.WriteLine($"  Cliente: {nuevoPrestamo.CedulaCliente}");
                            Console.WriteLine($"  {nuevoPrestamo.MostrarEstado()}");
                            Console.WriteLine("------------------------------------");
                        }
                        Console.WriteLine();
                        Console.WriteLine("  Presiona ENTER para continuar...");
                        Console.ReadLine();
                        break;

                    case "2":
                        Console.Clear();
                        Console.WriteLine("====================================");
                        Console.WriteLine("      PRÉSTAMOS EXISTENTES        ");
                        Console.WriteLine("====================================");
                        Console.WriteLine();
                        if (listaDePrestamos.Count == 0)
                        {
                            log.RegistrarAccion(usuarioActual, "Consulta de préstamos: No hay préstamos registrados");
                            Console.WriteLine("  No hay préstamos registrados.");
                        }
                        else
                        {
                            log.RegistrarAccion(usuarioActual, "Consulta de préstamos existentes realizada");
                            for (int i = 0; i < listaDePrestamos.Count; i++)
                            {
                                Console.WriteLine("------------------------------------");
                                Console.WriteLine($"  Cliente: {listaDePrestamos[i].CedulaCliente}");
                                Console.WriteLine($"  {listaDePrestamos[i].MostrarEstado()}");
                                Console.WriteLine("------------------------------------");
                            }
                        }
                        Console.WriteLine();
                        Console.WriteLine("  Presiona ENTER para continuar...");
                        Console.ReadLine();
                        break;

                    case "3":
                        Console.Clear();
                        Console.WriteLine("====================================");
                        Console.WriteLine("      REGISTRAR PAGO MENSUAL      ");
                        Console.WriteLine("====================================");
                        Console.WriteLine();
                        Console.Write("  Ingresa la cédula del cliente: ");
                        string cedulaPago = Console.ReadLine();
                        Prestamo prestamoPago = listaDePrestamos.Find(p => p.CedulaCliente == cedulaPago);
                        if (prestamoPago != null && prestamoPago.SaldoPendiente > 0)
                        {
                            prestamoPago.RegistrarPagoMensual();
                            GuardarTodosLosPrestamosEnArchivo();
                            log.RegistrarAccion(usuarioActual, $"Pago mensual registrado para préstamo de cliente {cedulaPago}, Cuota: ₡{prestamoPago.CuotaMensual}");
                            Console.WriteLine();
                            Console.WriteLine("------------------------------------");
                            Console.WriteLine("  ¡Pago registrado!");
                            Console.WriteLine("------------------------------------");
                            Console.WriteLine($"  Cliente: {prestamoPago.CedulaCliente}");
                            Console.WriteLine($"  {prestamoPago.MostrarEstado()}");
                            Console.WriteLine("------------------------------------");
                        }
                        else
                        {
                            log.RegistrarAccion(usuarioActual, $"Intento de registro de pago fallido, préstamo no encontrado o ya pagado: {cedulaPago}");
                            Console.WriteLine();
                            Console.WriteLine("  Préstamo no encontrado o ya pagado.");
                        }
                        Console.WriteLine();
                        Console.WriteLine("  Presiona ENTER para continuar...");
                        Console.ReadLine();
                        break;

                    case "4":
                        Console.Clear();
                        Console.WriteLine("====================================");
                        Console.WriteLine("      ESTADO DE UN PRÉSTAMO       ");
                        Console.WriteLine("====================================");
                        Console.WriteLine();
                        Console.Write("  Ingresa la cédula del cliente: ");
                        string cedulaEstado = Console.ReadLine();
                        Prestamo prestamoEstado = listaDePrestamos.Find(p => p.CedulaCliente == cedulaEstado);
                        if (prestamoEstado != null)
                        {
                            log.RegistrarAccion(usuarioActual, $"Consulta de estado de préstamo para cliente {cedulaEstado}");
                            Console.WriteLine();
                            Console.WriteLine("------------------------------------");
                            Console.WriteLine($"  Cliente: {prestamoEstado.CedulaCliente}");
                            Console.WriteLine($"  {prestamoEstado.MostrarEstado()}");
                            Console.WriteLine("------------------------------------");
                        }
                        else
                        {
                            log.RegistrarAccion(usuarioActual, $"Consulta de estado de préstamo fallida, no encontrado: {cedulaEstado}");
                            Console.WriteLine();
                            Console.WriteLine("  Préstamo no encontrado.");
                        }
                        Console.WriteLine();
                        Console.WriteLine("  Presiona ENTER para continuar...");
                        Console.ReadLine();
                        break;

                    case "5":
                        return;

                    default:
                        log.RegistrarAccion(usuarioActual, $"Opción inválida seleccionada en gestión de préstamos: {opcion}");
                        Console.WriteLine();
                        Console.WriteLine("  ¡Opción no válida!");
                        Console.WriteLine("  Presiona ENTER para continuar...");
                        Console.ReadLine();
                        break;
                }
            } while (opcion != "5");
        }

        // Método para guardar un cliente en el archivo  
        private void GuardarClienteEnArchivo(Cliente cliente)
        {
            try
            {
                Archivo archivo = new Archivo();
                List<Cliente> tempList = new List<Cliente> { cliente };
                archivo.GuardarClientes(tempList, archivoClientes, true);
                log.RegistrarAccion(usuarioActual, $"Cliente guardado en {archivoClientes}: {cliente.Cedula}");
            }
            catch (Exception ex)
            {
                log.RegistrarAccion(usuarioActual, $"Error al guardar cliente en archivo: {ex.Message}");
            }
        }
        // Método para guardar créditos en archivo
        private void GuardarCreditoEnArchivo(Credito credito)
        {
            try
            {
                Archivo archivo = new Archivo();
                List<Credito> tempList = new List<Credito> { credito };
                archivo.GuardarCreditos(tempList, archivoCreditos, true);
                log.RegistrarAccion(usuarioActual, $"Crédito guardado en {archivoCreditos}: {credito.Nombre}");
            }
            catch (Exception ex)
            {
                log.RegistrarAccion(usuarioActual, $"Error al guardar crédito en archivo: {ex.Message}");
            }
        }
        // Método para guardar débitos en archivo
        private void GuardarDebitoEnArchivo(Debito debito)
        {
            using (StreamWriter writer = new StreamWriter(archivoDebitos, true))
            {
                writer.WriteLine($"{debito.Id}|{debito.Monto}|{debito.FechaDebito}|{debito.Descripcion}|{debito.Categoria}|{debito.CedulaCliente}|{debito.Pagado}");
            }
        }
        // Método para guardar préstamos en archivo
        private void GuardarPrestamoEnArchivo(Prestamo prestamo)
        {
            try
            {
                Archivo archivo = new Archivo();
                List<Prestamo> tempList = new List<Prestamo> { prestamo };
                archivo.GuardarPrestamos(tempList, archivoPrestamos, true);
                log.RegistrarAccion(usuarioActual, $"Préstamo guardado en {archivoPrestamos}: {prestamo.CedulaCliente}");
            }
            catch (Exception ex)
            {
                log.RegistrarAccion(usuarioActual, $"Error al guardar préstamo en archivo: {ex.Message}");
            }
        }
        private void GuardarTodosLosDebitosEnArchivo()
        {
            try
            {
                Archivo archivo = new Archivo();
                archivo.GuardarDebitos(listaDeDebitos, archivoDebitos);
                log.RegistrarAccion(usuarioActual, $"Débitos guardados en {archivoDebitos}: {listaDeDebitos.Count} débitos");
            }
            catch (Exception ex)
            {
                log.RegistrarAccion(usuarioActual, $"Error al guardar débitos en archivo: {ex.Message}");
                throw;
            }
        }
        // Método para guardar todos los créditos en archivo
        private void GuardarTodosLosCreditosEnArchivo()
        {
            try
            {
                Archivo archivo = new Archivo();
                archivo.GuardarCreditos(listaDeCreditos, archivoCreditos, true);
                log.RegistrarAccion(usuarioActual, $"Créditos guardados en {archivoCreditos}: {listaDeCreditos.Count} créditos");
            }
            catch (Exception ex)
            {
                log.RegistrarAccion(usuarioActual, $"Error al guardar créditos en archivo: {ex.Message}");
                throw;
            }
        }
        // Método para guardar todos los préstamos en archivo
        private void GuardarTodosLosPrestamosEnArchivo()
        {
            try
            {
                Archivo archivo = new Archivo();
                archivo.GuardarPrestamos(listaDePrestamos, archivoPrestamos, true);
                log.RegistrarAccion(usuarioActual, $"Préstamos guardados en {archivoPrestamos}: {listaDePrestamos.Count} préstamos");
            }
            catch (Exception ex)
            {
                log.RegistrarAccion(usuarioActual, $"Error al guardar préstamos en archivo: {ex.Message}");
                throw;
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
                GuardarTodosLosClientesEnArchivo();
                GuardarTodosLosCreditosEnArchivo();
                GuardarTodosLosDebitosEnArchivo();
                GuardarTodosLosPrestamosEnArchivo();
                log.RegistrarAccion(usuarioActual, "Todos los datos guardados en archivos");
                Console.WriteLine("  ¡Todos los datos guardados en los archivos correctamente!");
            }
            catch (Exception ex)
            {
                log.RegistrarAccion(usuarioActual, $"Error al guardar en archivos: {ex.Message}");
                Console.WriteLine($"  Error al guardar los datos: {ex.Message}. Verifica los permisos de escritura o el espacio en disco.");
            }

            Console.WriteLine();
            Console.WriteLine("  Presiona ENTER para volver al menú principal...");
            Console.ReadLine();
        }
        // Método para guardar todos los clientes en archivo
        private void GuardarTodosLosClientesEnArchivo()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(archivoClientes))
                {
                    for (int i = 0; i < listaDeClientes.Count; i++)
                    {
                        Cliente cliente = listaDeClientes[i];
                        writer.WriteLine($"{cliente.Nombre}|{cliente.Apellidos}|{cliente.Cedula}|{cliente.Correo}|{cliente.Direccion}|{cliente.TelefonoCelular}|{cliente.TelefonoCasa}|{cliente.TelefonoOficina}|{cliente.IngresosMensuales}|{cliente.FuentesIngresos}|{cliente.GastosFijosMensuales}|{cliente.Ahorros}");
                    }
                }
                log.RegistrarAccion(usuarioActual, $"Clientes guardados en {archivoClientes}: {listaDeClientes.Count} clientes");
            }
            catch (IOException ex)
            {
                log.RegistrarAccion(usuarioActual, $"Error al guardar clientes en archivo: {ex.Message}");
                throw new Exception($"Error al guardar clientes: {ex.Message}");
            }
        }
        // Método para cargar datos desde archivos  
        private void CargarDesdeArchivo()
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("         CARGAR DESDE ARCHIVO      ");
            Console.WriteLine("====================================");
            Console.WriteLine();

            if (File.Exists(archivoClientes))
            {
                string[] lineas = File.ReadAllLines(archivoClientes);
                listaDeClientes.Clear();

                for (int i = 0; i < lineas.Length; i++)
                {
                    string[] datos = lineas[i].Split('|');
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
                        listaDeClientes.Add(cliente);
                    }
                    else
                    {
                        log.RegistrarAccion(usuarioActual, "Error al cargar clientes desde archivo: Línea con datos incompletos");
                        Console.WriteLine("  Error: Línea con datos incompletos en clientes.txt.");
                    }
                }
                Console.WriteLine($"  Clientes cargados: {listaDeClientes.Count}");
            }
            else
            {
                log.RegistrarAccion(usuarioActual, "Carga desde archivo fallida: No se encontró clientes.txt");
                Console.WriteLine("  No se encontró el archivo clientes.txt.");
            }

            if (File.Exists(archivoCreditos))
            {
                string[] lineas = File.ReadAllLines(archivoCreditos);
                listaDeCreditos.Clear();

                for (int i = 0; i < lineas.Length; i++)
                {
                    string[] datos = lineas[i].Split('|');
                    if (datos.Length >= 8)
                    {
                        Credito credito = new Credito(
                            datos[0], // Nombre  
                            decimal.TryParse(datos[1], out decimal limite) ? limite : 0, // MontoLimite  
                            decimal.TryParse(datos[3], out decimal tasa) ? tasa : 0, // TasaInteres  
                            datos[6] // CedulaCliente  
                        )
                        {
                            Id = int.TryParse(datos[7], out int id) ? id : 0,
                            MontoUtilizado = decimal.TryParse(datos[2], out decimal utilizado) ? utilizado : 0,
                            FechaApertura = DateTime.TryParse(datos[4], out DateTime fechaApertura) ? fechaApertura : DateTime.Now,
                            FechaProximoPago = DateTime.TryParse(datos[5], out DateTime fechaProximoPago) ? fechaProximoPago : DateTime.Now,
                            Activo = bool.TryParse(datos[7], out bool activo) ? activo : true
                        };
                        listaDeCreditos.Add(credito);
                    }
                    else
                    {
                        log.RegistrarAccion(usuarioActual, "Error al cargar créditos desde archivo: Línea con datos incompletos");
                        Console.WriteLine("  Error: Línea con datos incompletos en creditos.txt.");
                    }
                }
                Console.WriteLine($"  Créditos cargados: {listaDeCreditos.Count}");
            }
            else
            {
                log.RegistrarAccion(usuarioActual, "Carga desde archivo fallida: No se encontró creditos.txt");
                Console.WriteLine("  No se encontró el archivo creditos.txt.");
            }

            if (File.Exists(archivoDebitos))
            {
                string[] lineas = File.ReadAllLines(archivoDebitos);
                listaDeDebitos.Clear();

                for (int i = 0; i < lineas.Length; i++)
                {
                    string[] datos = lineas[i].Split('|');
                    if (datos.Length >= 6)
                    {
                        Debito debito = new Debito
                        {
                            Monto = decimal.TryParse(datos[0], out decimal monto) ? monto : 0,
                            FechaDebito = DateTime.TryParse(datos[1], out DateTime fecha) ? fecha : DateTime.Now,
                            Descripcion = datos[2],
                            Categoria = datos[3],
                            CedulaCliente = datos[4],
                            Pagado = bool.TryParse(datos[5], out bool pagado) ? pagado : false
                        };
                        listaDeDebitos.Add(debito);
                    }
                    else
                    {
                        log.RegistrarAccion(usuarioActual, "Error al cargar débitos desde archivo: Línea con datos incompletos");
                        Console.WriteLine("  Error: Línea con datos incompletos en debitos.txt.");
                    }
                }
                Console.WriteLine($"  Débitos cargados: {listaDeDebitos.Count}");
            }
            else
            {
                log.RegistrarAccion(usuarioActual, "Carga desde archivo fallida: No se encontró debitos.txt");
                Console.WriteLine("  No se encontró el archivo debitos.txt.");
            }

            if (File.Exists(archivoPrestamos))
            {
                string[] lineas = File.ReadAllLines(archivoPrestamos);
                listaDePrestamos.Clear();

                for (int i = 0; i < lineas.Length; i++)
                {
                    string[] datos = lineas[i].Split('|');
                    if (datos.Length >= 7)
                    {
                        Prestamo prestamo = new Prestamo(log)
                        {
                            Monto = decimal.TryParse(datos[0], out decimal monto) ? monto : 0,
                            Tasa = decimal.TryParse(datos[1], out decimal tasa) ? tasa : 0,
                            Plazo = int.TryParse(datos[2], out int plazo) ? plazo : 0,
                            FechaInicio = DateTime.TryParse(datos[3], out DateTime fecha) ? fecha : DateTime.Now,
                            CedulaCliente = datos[4],
                            SaldoPendiente = decimal.TryParse(datos[5], out decimal saldo) ? saldo : 0,
                            CuotasPagadas = int.TryParse(datos[6], out int cuotas) ? cuotas : 0
                        };
                        prestamo.CalcularCuotas();
                        listaDePrestamos.Add(prestamo);
                    }
                    else
                    {
                        log.RegistrarAccion(usuarioActual, "Error al cargar préstamos desde archivo: Línea con datos incompletos");
                        Console.WriteLine("  Error: Línea con datos incompletos en prestamos.txt.");
                    }
                }
                Console.WriteLine($"  Préstamos cargados: {listaDePrestamos.Count}");
            }
            else
            {
                log.RegistrarAccion(usuarioActual, "Carga desde archivo fallida: No se encontró prestamos.txt");
                Console.WriteLine("  No se encontró el archivo prestamos.txt.");
            }

            log.RegistrarAccion(usuarioActual, "Datos cargados desde archivos correctamente");
            Console.WriteLine();
            Console.WriteLine("  Presiona ENTER para volver al menú principal...");
            Console.ReadLine();
        }
        // Método para buscar cliente por cédula
        private void BuscarClientePorCedula()
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("     BUSCAR CLIENTE POR CÉDULA     ");
            Console.WriteLine("====================================");
            Console.WriteLine();

            Console.Write("  Ingresa la cédula del cliente: ");
            string cedula = Console.ReadLine();

            Cliente cliente = listaDeClientes.Find(c => c.Cedula == cedula);

            if (cliente != null)
            {
                cliente.ActualizarDeuda(listaDePrestamos);
                log.RegistrarAccion(usuarioActual, $"Cliente encontrado por cédula: {cedula}");
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
                var creditos = listaDeCreditos.Where(c => c.CedulaCliente == cliente.Cedula).ToList();
                if (creditos.Any())
                {
                    Console.WriteLine("  Créditos asociados:");
                    foreach (var credito in creditos)
                    {
                        Console.WriteLine($"    - {credito.Nombre} (ID: {credito.Id})");
                    }
                }
                else
                {
                    Console.WriteLine("  No tiene créditos asociados.");
                }
                Console.WriteLine("------------------------------------");
            }
            else
            {
                log.RegistrarAccion(usuarioActual, $"Búsqueda de cliente fallida, cédula no encontrada: {cedula}");
                Console.WriteLine();
                Console.WriteLine("  Cliente no encontrado.");
            }

            Console.WriteLine();
            Console.WriteLine("  Presiona ENTER para volver al menú principal...");
            Console.ReadLine();
        }

        private void SalirDelPrograma()
        {
            log.RegistrarAccion(usuarioActual, "Guardando todos los datos antes de cerrar el programa");
            GuardarTodosLosClientesEnArchivo();
            GuardarTodosLosCreditosEnArchivo();
            GuardarTodosLosDebitosEnArchivo();
            GuardarTodosLosPrestamosEnArchivo();
            log.RegistrarAccion(usuarioActual, "Programa cerrado");
            Console.WriteLine();
            Console.WriteLine("  ¡Saliendo del programa! Gracias por usar el sistema.");
            Console.WriteLine("  Presiona ENTER para cerrar...");
            Console.ReadLine();
            Environment.Exit(0);
        }

    }
}