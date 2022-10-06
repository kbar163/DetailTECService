using System.Data;
using DetailTECService.Models;
using Microsoft.Data.SqlClient;

namespace DetailTECService.Data
{
    public class CustomerRepo : ICustomerRepository
    {

        private readonly string _connectionString;

        public CustomerRepo()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            _connectionString = config.GetValue<string>("ConnectionStrings:DetailTECDB");

        }
        
        //Proceso: Punto de entrada del proceso de obtener todos los clientes, hace uso de varias
        //funciones auxiliares que obtienen datos de la base de datos y le dan el formato necesario
        //utilizando los modelos creados.
        //Salida: ActionResponse response: un objeto que tiene una propiedad booleana que indica si la 
        //operacion fue exitosa o no, y una propiedad clientes con una lista de objetos Customer.    
        public MultivalueCustomer GetAllCustomers()
        {
            MultivalueCustomer response;
            string customerQuery= @"SELECT CLIENTE.CEDULA_CLIENTE, CLIENTE.NOMBRE, CLIENTE.PRIMER_APELLIDO,
            CLIENTE.SEGUNDO_APELLIDO, CLIENTE.CORREO_CLIENTE, CLIENTE.USUARIO, CLIENTE.PASSWORD_CLIENTE,
            CLIENTE.PUNTOS_ACUM
            FROM CLIENTE";
            DataTable customerData = GetTableData(customerQuery);
            response = AllCustomersMessage(customerData);
            return response;
        }

        //Proceso: 
        //Intenta conectarse a la base de datos haciendo uso de un SqlConnection,
        //Intenta ejecutar el query sobre la base de datos y escribir el resultado al DataTable data
        //Salida: DataTable data con la informacion solicitada en el query de ser exitoso,
        //DataTable data vacio en caso de que el query no fuese exitoso.
        private DataTable GetTableData(string query)
        {
            var data = new DataTable();
        
            try
            {

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        connection.Open();
                        Console.WriteLine("Connection to DB stablished");
                        adapter.Fill(data);
                    } 
                }   
            }

            catch (Exception ex)
            {
                if(ex is ArgumentException || ex is SqlException)
                {
                    Console.WriteLine("ERROR: " + ex.Message +  "triggered by " + ex.Source);
                }

            }

            
            return data;
        }

        //Entradas: 
        //DataTable dbTable: DataTable que potencialmente contiene informacion obtenida de la base de datos.
        //Proceso: Se revisa si dbTable tiene contenido, de ser asi, se cambia la propiedad boolean de la respuesta
        //a true y se mapea cada uno de las filas de la tabla a objetos Customer que son agregados a la propiedad lista clientes.
        //Si la tabla no tiene contenido, se cambia el booleano exito a false.
        //Multivalue response: Un objeto que representa el mensaje a enviar al frontend.  
        private MultivalueCustomer AllCustomersMessage(DataTable customers)
        {
            var response = new MultivalueCustomer();
            response.clientes = new List<Customer>();

            string phoneQuery = @"SELECT CLIENTE_TELEFONO.TELEFONO
            FROM CLIENTE_TELEFONO WHERE CLIENTE_TELEFONO.CEDULA_CLIENTE = @cedula";
            
            string addressQuery = @"SELECT CLIENTE_DIRECCION.PROVINCIA,
            CLIENTE_DIRECCION.CANTON, CLIENTE_DIRECCION.DISTRITO
            FROM CLIENTE_DIRECCION
            WHERE CLIENTE_DIRECCION.CEDULA_CLIENTE = @cedula";

            
            if(customers.Rows.Count !=0)
            {
                response.exito = true;
                for(int index = 0; index < customers.Rows.Count; index++)
                {
                    Customer customer = new Customer();
                    customer.cedula_cliente = (string)customers.Rows[index]["CEDULA_CLIENTE"];
                    customer.nombre = (string)customers.Rows[index]["NOMBRE"];
                    customer.primer_apellido = (string)customers.Rows[index]["PRIMER_APELLIDO"];
                    customer.segundo_apellido = (string)customers.Rows[index]["SEGUNDO_APELLIDO"];
                    customer.correo = (string)customers.Rows[index]["CORREO_CLIENTE"];
                    customer.usuario = (string)customers.Rows[index]["USUARIO"];
                    customer.password_cliente = (string)customers.Rows[index]["PASSWORD_CLIENTE"];
                    customer.puntos_acum = (int)customers.Rows[index]["PUNTOS_ACUM"];
                    var addressTable = GetDataById(addressQuery, customer.cedula_cliente);
                    SetCustomerAddress(addressTable, customer);
                    var phoneTable = GetDataById(phoneQuery, customer.cedula_cliente);
                    SetCustomerPhone(phoneTable, customer);
                    response.clientes.Add(customer);

                }
            }
            else
            {
                response.exito = false;
            }
            
            return response;
        }

        
            
        //Proceso: 
        //Intenta conectarse a la base de datos haciendo uso de un SqlConnection,
        //Intenta ejecutar el query parametrizado con la cedula del cliente del que se
        //desea obtener datos en la base de datos y escribe el resultado al DataTable dbTable
        //Salida: DataTable dbTable con la informacion solicitada en el query de ser exitoso,
        //DataTable data vacio en caso de que el query no fuese exitoso.
        private DataTable GetDataById(string query, string cedula_cliente)
        {
           
            var dbTable = new DataTable();
        
            try
            {

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        connection.Open();
                        command.Parameters.Add(new SqlParameter("@cedula",cedula_cliente));
                        Console.WriteLine("Connection to DB stablished");
                        adapter.Fill(dbTable);
                    } 
                }   
            }

            catch (Exception ex)
            {
                if(ex is ArgumentException || ex is SqlException)
                {
                    Console.WriteLine("ERROR: " + ex.Message +  "triggered by " + ex.Source);
                }

            }
            return dbTable;
        }

        //Entradas: DataTable que potencialmente contiene informacion sobre las direcciones un cliente
        // y el objeto Customer relacionado a ese cliente.
        //Proceso: Se revisa si dbTable tiene contenido, de ser asi, se mapea cada uno de las filas de
        // la tabla a una lista de objetos Address que es asignada a la propiedad lista direcciones.
        //Si la tabla no tiene contenido, no se genera ningun cambio.
        private void SetCustomerAddress(DataTable addressTable, Customer customer)
        {
            if(addressTable.Rows.Count != 0 )
            {
                var addressList = new List<Address>();
                for(int index = 0; index < addressTable.Rows.Count; index++)
                {
                    var address = new Address();
                    address.provincia = (string)addressTable.Rows[index]["PROVINCIA"];
                    address.canton = (string)addressTable.Rows[index]["CANTON"];
                    address.distrito = (string)addressTable.Rows[index]["DISTRITO"];
                    addressList.Add(address);
                }
                
                customer.direcciones = addressList;
            }
        }

        //Entradas: DataTable que potencialmente contiene informacion sobre los telefonos un cliente
        // y el objeto Customer relacionado a ese cliente.
        //Proceso: Se revisa si dbTable tiene contenido, de ser asi, se mapea cada uno de las filas de
        // la tabla a una lista de strings que es asignada a la propiedad lista telefonos.
        //Si la tabla no tiene contenido, no se genera ningun cambio.
        private void SetCustomerPhone(DataTable phoneTable, Customer customer)
        {
            if(phoneTable.Rows.Count != 0 )
            {
                var phoneList = new List<string>();
                for(int index = 0; index < phoneTable.Rows.Count; index++)
                {
                    
                    var phone = (string)phoneTable.Rows[index]["TELEFONO"];
                    
                    phoneList.Add(phone);
                }
                
                customer.telefonos = phoneList;
            }
        }
    }
}