using System;
using System.Collections.Generic;
using System.Globalization;

// Excepción personalizada para stock insuficiente
class StockInsuficienteException : Exception
{
    public StockInsuficienteException(string mensaje) : base(mensaje) { }
}

// Clase base Producto
class Producto
{
    public string Nombre { get; set; }
    public decimal Precio { get; set; }
    public int Existencia { get; set; }
    public string Descripcion { get; set; }

    public Producto(string nombre, decimal precio, int existencia, string descripcion)
    {
        Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre), "El nombre no puede ser nulo");
        Descripcion = descripcion ?? throw new ArgumentNullException(nameof(descripcion), "La descripción no puede ser nula");
        Precio = precio;
        Existencia = existencia;
    }

    // Método para vender un producto
    public virtual decimal Vender(int cantidad)
    {
        if (cantidad <= 0)
        {
            throw new ArgumentException("La cantidad debe ser un entero positivo.");
        }

        if (cantidad > Existencia)
        {
            throw new StockInsuficienteException($"No hay suficiente stock para vender {cantidad} unidades de {Nombre}.");
        }

        Existencia -= cantidad;
        decimal total = cantidad * Precio;
        Console.WriteLine($" Venta realizada: {cantidad} unidades de {Nombre}.");
        Console.WriteLine($" Total gastado: LPS {total:F2}");
        Console.WriteLine($" Stock restante: {Existencia} unidades.");
        return total;
    }

    public override string ToString()
    {
        return $"Producto: {Nombre} | Precio: LPS {Precio:F2} | Existencia: {Existencia} | Descripción: {Descripcion}";
    }
}

// Clase derivada ProductoPerecedero
class ProductoPerecedero : Producto
{
    public DateTime FechaExpiracion { get; set; }

    public ProductoPerecedero(string nombre, decimal precio, int existencia, string descripcion, DateTime fechaExpiracion)
        : base(nombre, precio, existencia, descripcion)
    {
        if (fechaExpiracion < DateTime.Today)
        {
            throw new ArgumentException(" No puedes registrar un producto con fecha de expiración pasada.");
        }

        FechaExpiracion = fechaExpiracion;
    }

    public override string ToString()
    {
        return base.ToString() + $" | Fecha de Expiración: {FechaExpiracion:dd/MM/yyyy}";
    }
}

// Programa principal
class Program
{
    static List<Producto> inventario = new List<Producto>();

    static void Main()
    {
        int opcion;
        do
        {
            Console.WriteLine("\n GESTIÓN DE PRODUCTOS ");
            Console.WriteLine("1. Agregar Producto");
            Console.WriteLine("2. Vender Producto");
            Console.WriteLine("3. Mostrar Inventario");
            Console.WriteLine("4. Salir");
            Console.Write("Seleccione una opción: ");

            if (!int.TryParse(Console.ReadLine(), out opcion))
            {
                Console.WriteLine(" Opción inválida. Intente nuevamente.");
                continue;
            }

            switch (opcion)
            {
                case 1:
                    AgregarProducto();
                    break;
                case 2:
                    VenderProducto();
                    break;
                case 3:
                    MostrarInventario();
                    break;
                case 4:
                    Console.WriteLine(" Saliendo del sistema...");
                    break;
                default:
                    Console.WriteLine(" Opción inválida.");
                    break;
            }
        } while (opcion != 4);
    }

    static void AgregarProducto()
    {
        Console.Write("Ingrese el nombre del producto: ");
        string nombre = Console.ReadLine()?.Trim() ?? "Producto sin nombre";

        Console.Write("Ingrese el precio: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal precio) || precio < 0)
        {
            Console.WriteLine(" Precio inválido.");
            return;
        }

        Console.Write("Ingrese la cantidad en existencia: ");
        if (!int.TryParse(Console.ReadLine(), out int existencia) || existencia < 0)
        {
            Console.WriteLine(" La existencia debe ser un número entero positivo.");
            return;
        }

        Console.Write("Ingrese la descripción del producto: ");
        string descripcion = Console.ReadLine()?.Trim() ?? "Sin descripción";

        Console.Write("¿Es un producto perecedero? (S/N): ");
        string esPerecedero = Console.ReadLine()?.Trim().ToUpper() ?? "N";

        if (esPerecedero == "S")
        {
            Console.Write("Ingrese la fecha de expiración (dd/mm/yyyy): ");
            if (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaExpiracion))
            {
                Console.WriteLine(" Fecha inválida.");
                return;
            }

            try
            {
                inventario.Add(new ProductoPerecedero(nombre, precio, existencia, descripcion, fechaExpiracion));
                Console.WriteLine($" Producto perecedero '{nombre}' agregado exitosamente.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($" Error: {ex.Message}");
            }
        }
        else
        {
            inventario.Add(new Producto(nombre, precio, existencia, descripcion));
            Console.WriteLine($" Producto '{nombre}' agregado exitosamente.");
        }
    }

    static void VenderProducto()
    {
        if (inventario.Count == 0)
        {
            Console.WriteLine(" No hay productos en el inventario.");
            return;
        }

        Console.Write("Ingrese el nombre del producto a vender: ");
        string nombre = Console.ReadLine()?.Trim() ?? "";

        Producto? producto = inventario.Find(p => p.Nombre != null && p.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase));

        if (producto == null)
        {
            Console.WriteLine(" Producto no encontrado.");
            return;
        }

        Console.Write("Ingrese la cantidad a vender: ");
        if (!int.TryParse(Console.ReadLine(), out int cantidad) || cantidad <= 0)
        {
            Console.WriteLine(" La cantidad debe ser un entero positivo.");
            return;
        }

        try
        {
            producto.Vender(cantidad);
        }
        catch (StockInsuficienteException ex)
        {
            Console.WriteLine($" {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($" Error inesperado: {ex.Message}");
        }
    }

    static void MostrarInventario()
    {
        if (inventario.Count == 0)
        {
            Console.WriteLine(" El inventario está vacío.");
            return;
        }

        Console.WriteLine("\n Inventario Actual:");
        foreach (var producto in inventario)
        {
            Console.WriteLine(producto);
        }
    }
}
