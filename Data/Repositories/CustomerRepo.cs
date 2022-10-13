using System.Data;
using DetailTECService.Models;
using Microsoft.Data.SqlClient;
using DetailTECService.Coms;
using MlkPwgen;

namespace DetailTECService.Data
{
    //Implementacion de la logica para cada una de los endpoints expuesos en CustomerController,
    //esta clase extiende la interfaz ICustomerRepository, e implementa los metodos relacionados
    //a la manipulacion de datos necesaria para cumplir con los requerimientos funcionales
    //de la aplicacion.
    public class CustomerRepo : ICustomerRepository
    {

        private readonly string _connectionString;

        public CustomerRepo()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            _connectionString = config.GetValue<string>("ConnectionStrings:DetailTECDB");

        }


        //Proceso: Punto de entrada del proceso de crear un cliente, hace uso de una funcion
        //auxiliar que inserta informacion a la base de datos. 
        //Salida: ActionResponse response: un objeto que tiene una propiedad booleana que indica si la 
        //operacion fue exitosa o no, y una propiedad message con un string que describe el resultado de
        //la operacion.
        public ActionResponse AddCustomer(Customer newCustomer)
        {
            ActionResponse response;
            string query = @"INSERT INTO CLIENTE
            VALUES (@cedula , @nombre ,
            @apellido_1 , @apellido_2 , @correo , @usuario , @password ,
            @puntos_acum, @puntos_obt , @puntos_redim)";
            response = WriteCustomerDB(query, newCustomer);
            if(response.actualizado) EmailSender.SendCreationEmail(newCustomer);
            return response;
        }

        //Proceso: Punto de entrada del proceso de editar un cliente, hace uso de una funcion
        //auxiliar que actualiza informacion a la base de datos. 
        //Salida: ActionResponse response: un objeto que tiene una propiedad booleana que indica si la 
        //operacion fue exitosa o no, y una propiedad message con un string que describe el resultado de
        //la operacion.
        public ActionResponse ModifyCustomer(Customer newCustomer)
        {
            ActionResponse response;
            string query = @"UPDATE CLIENTE
            SET CEDULA_CLIENTE = @cedula ,
            NOMBRE = @nombre ,
            PRIMER_APELLIDO = @apellido_1 ,
            SEGUNDO_APELLIDO = @apellido_2 ,
            CORREO_CLIENTE = @correo ,
            USUARIO = @usuario ,
            PASSWORD_CLIENTE = @password,
            PUNTOS_ACUM = @puntos
            WHERE CEDULA_CLIENTE = @cedula";
            response = WriteCustomerDB(query, newCustomer);
            return response;
        }

        //Proceso: Punto de entrada del proceso de borrar un cliente, hace uso de una funcion
        //auxiliar que elimina informacion a la base de datos. 
        //Salida: ActionResponse response: un objeto que tiene una propiedad booleana que indica si la 
        //operacion fue exitosa o no, y una propiedad message con un string que describe el resultado de
        //la operacion.
        public ActionResponse DeleteCustomer (CustomerIdRequest customerId)
        {
            var response = new ActionResponse();
            
            var customerQuery = @"DELETE FROM CLIENTE
            WHERE CEDULA_CLIENTE = @cedula";
            var addressQuery = @"DELETE FROM CLIENTE_DIRECCION
            WHERE CEDULA_CLIENTE = @cedula";
            var phoneQuery = @"DELETE FROM CLIENTE_TELEFONO
            WHERE CEDULA_CLIENTE = @cedula";

            var deletePhone = DeleteDatabyId(phoneQuery, customerId.cedula_cliente);
            var deleteAddress = DeleteDatabyId(addressQuery, customerId.cedula_cliente);
            var deleteCustomer = DeleteDatabyId(customerQuery, customerId.cedula_cliente);

            if( deletePhone && deleteAddress && deleteCustomer)
            {
                response.actualizado = true;
                response.mensaje = "Cliente eliminado exitosamente";
            }
            
            return response;

    
        }


        //Entrada: Un string query que contiene un SQL Query y un string deleteId que contiene
        //un primary key para una tabla en la base de datos.
        //Proceso: Se conecta a la base de datos y ejecuta el query de eliminacion segun fue especificado.
        //Salida: booleano que indica si la operacion fue exitosa o no.
        private bool DeleteDatabyId(string query, string deleteId)
        {
            var isSuccessful = false;
            try
            {

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@cedula", deleteId));
                        connection.Open();
                        Console.WriteLine("Connection to DB stablished");
                        command.ExecuteNonQuery();
                        isSuccessful = true;   
                    } 
                }   
            }

            catch (Exception ex)
            {
                if(ex is ArgumentException ||
                   ex is SqlException || ex is InvalidOperationException)
                {
                    isSuccessful = false;
                }
            }

            return isSuccessful;
        }

        

        //Proceso: Punto de entrada del proceso de obtener todos los clientes, hace uso de varias
        //funciones auxiliares que obtienen datos de la base de datos y le dan el formato necesario
        //utilizando los modelos creados.
        //Salida: ActionResponse response: un objeto que tiene una propiedad booleana que indica si la 
        //operacion fue exitosa o no, y una propiedad clientes con una lista de objetos Customer.    
        public MultivalueCustomer GetAllCustomers()
        {
            MultivalueCustomer response;
            string customerQuery = @"SELECT CLIENTE.CEDULA_CLIENTE, CLIENTE.NOMBRE, CLIENTE.PRIMER_APELLIDO,
            CLIENTE.SEGUNDO_APELLIDO, CLIENTE.CORREO_CLIENTE, CLIENTE.USUARIO, CLIENTE.PASSWORD_CLIENTE,
            CLIENTE.PUNTOS_ACUM , CLIENTE.PUNTOS_OBT , CLIENTE.PUNTOS_REDIM
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
                if (ex is ArgumentException || ex is SqlException)
                {
                    Console.WriteLine("ERROR: " + ex.Message + "triggered by " + ex.Source);
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


            if (customers.Rows.Count != 0)
            {
                response.exito = true;
                for (int index = 0; index < customers.Rows.Count; index++)
                {
                    Customer customer = new Customer();
                    customer.cedula_cliente = (string)customers.Rows[index]["CEDULA_CLIENTE"];
                    customer.nombre = (string)customers.Rows[index]["NOMBRE"];
                    customer.primer_apellido = (string)customers.Rows[index]["PRIMER_APELLIDO"];
                    customer.segundo_apellido = (string)customers.Rows[index]["SEGUNDO_APELLIDO"];
                    customer.correo_cliente = (string)customers.Rows[index]["CORREO_CLIENTE"];
                    customer.usuario = (string)customers.Rows[index]["USUARIO"];
                    customer.password_cliente = (string)customers.Rows[index]["PASSWORD_CLIENTE"];
                    customer.puntos_acum = (int)customers.Rows[index]["PUNTOS_ACUM"];
                    customer.puntos_obt = (int)customers.Rows[index]["PUNTOS_OBT"];
                    customer.puntos_redim = (int)customers.Rows[index]["PUNTOS_REDIM"];
                    var addressTable = GetDataById(addressQuery, customer.cedula_cliente);
                    GetCustomerAddress(addressTable, customer);
                    var phoneTable = GetDataById(phoneQuery, customer.cedula_cliente);
                    GetCustomerPhone(phoneTable, customer);
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
                        command.Parameters.Add(new SqlParameter("@cedula", cedula_cliente));
                        Console.WriteLine("Connection to DB stablished");
                        adapter.Fill(dbTable);
                    }
                }
            }

            catch (Exception ex)
            {
                if (ex is ArgumentException || ex is SqlException)
                {
                    Console.WriteLine("ERROR: " + ex.Message + "triggered by " + ex.Source);
                }

            }
            return dbTable;
        }

        //Entradas: DataTable que potencialmente contiene informacion sobre las direcciones un cliente
        // y el objeto Customer relacionado a ese cliente.
        //Proceso: Se revisa si dbTable tiene contenido, de ser asi, se mapea cada uno de las filas de
        // la tabla a una lista de objetos Address que es asignada a la propiedad lista direcciones.
        //Si la tabla no tiene contenido, no se genera ningun cambio.
        private void GetCustomerAddress(DataTable addressTable, Customer customer)
        {
            if (addressTable.Rows.Count != 0)
            {
                var addressList = new List<Address>();
                for (int index = 0; index < addressTable.Rows.Count; index++)
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
        private void GetCustomerPhone(DataTable phoneTable, Customer customer)
        {
            if (phoneTable.Rows.Count != 0)
            {
                var phoneList = new List<string>();
                for (int index = 0; index < phoneTable.Rows.Count; index++)
                {

                    var phone = (string)phoneTable.Rows[index]["TELEFONO"];

                    phoneList.Add(phone);
                }

                customer.telefonos = phoneList;
            }
        }


        //Proceso: 
        //Intenta conectarse a la base de datos haciendo uso de un SqlConnection,
        //Intenta ejecutar INSERT o UPDATE sobre la base de datos en la tabla CLIENTE
        //Salida: ActionResponse response: un objeto que tiene una propiedad booleana que indica si la 
        //operacion fue exitosa o no, y una propiedad message con un string que describe el resultado de
        //la operacion.
        private ActionResponse WriteCustomerDB(string query, Customer newCustomer)
        {
            ActionResponse response = new ActionResponse();
            string verb = "";
            string infinitive = "";
            newCustomer.password_cliente = PasswordGenerator.Generate();

            try
            {
                if (query.Contains("INSERT"))
                {
                    verb = "creado";
                    infinitive = "crear";
                }

                if (query.Contains("UPDATE"))
                {
                    verb = "actualizado";
                    infinitive = "actualizar";
                }

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@cedula", newCustomer.cedula_cliente));
                        command.Parameters.Add(new SqlParameter("@nombre", newCustomer.nombre));
                        command.Parameters.Add(new SqlParameter("@apellido_1", newCustomer.primer_apellido));
                        command.Parameters.Add(new SqlParameter("@apellido_2", newCustomer.segundo_apellido));
                        command.Parameters.Add(new SqlParameter("@correo", newCustomer.correo_cliente));
                        command.Parameters.Add(new SqlParameter("@usuario", newCustomer.usuario));
                        command.Parameters.Add(new SqlParameter("@password", newCustomer.password_cliente));
                        command.Parameters.Add(new SqlParameter("@puntos_acum", 10));
                        command.Parameters.Add(new SqlParameter("@puntos_obt", 11));
                        command.Parameters.Add(new SqlParameter("@puntos_redim", 1));
                        connection.Open();
                        Console.WriteLine("Connection to DB stablished");
                        command.ExecuteNonQuery();
                        response.actualizado = true;
                        response.mensaje = $"Cliente {verb} exitosamente";
                    }
                }
                
                SetAddress(newCustomer, infinitive);
                SetPhoneNumber(newCustomer, infinitive);
            }

            catch (Exception ex)
            {
                if (ex is ArgumentException ||
                    ex is InvalidOperationException ||
                    ex is SqlException)
                {
                    Console.WriteLine("ERROR: " + ex.Message + "triggered by " + ex.Source);
                    response.actualizado = false;
                    response.mensaje = $"Error al {infinitive} al cliente";
                }
            }
            return response;
        }


        //Proceso: 
        //Intenta conectarse a la base de datos haciendo uso de un SqlConnection,
        //Intenta ejecutar INSERT o UPDATE sobre la base de datos en la tabla CLIENTE_DIRECCION
        //Salida: ActionResponse response: un objeto que tiene una propiedad booleana que indica si la 
        //operacion fue exitosa o no, y una propiedad message con un string que describe el resultado de
        //la operacion.
        private void SetAddress(Customer customer, string infinitive)
        {
            var uniqueAddresses = customer.direcciones.Distinct(new AddressComparer()).ToList();
            if (infinitive == "crear")
            {
                foreach (Address address in uniqueAddresses)
                {
                    var query = @"INSERT INTO CLIENTE_DIRECCION
                    VALUES (@cedula_direccion , @provincia ,
                    @canton , @distrito)";

                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {

                            command.Parameters.Add(new SqlParameter("@cedula_direccion", customer.cedula_cliente));
                            command.Parameters.Add(new SqlParameter("@provincia", address.provincia));
                            command.Parameters.Add(new SqlParameter("@canton", address.canton));
                            command.Parameters.Add(new SqlParameter("@distrito", address.distrito));
                            connection.Open();
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }

            if (infinitive == "actualizar")
            {
                var addressQuery = @"DELETE FROM CLIENTE_DIRECCION
                WHERE CEDULA_CLIENTE = @cedula";
                DeleteDatabyId(addressQuery,customer.cedula_cliente);
            
                foreach (Address address in uniqueAddresses)
                {
                    var query = @"INSERT INTO CLIENTE_DIRECCION
                    VALUES (@cedula_direccion , @provincia ,
                    @canton , @distrito)";

                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {

                            command.Parameters.Add(new SqlParameter("@cedula_direccion", customer.cedula_cliente));
                            command.Parameters.Add(new SqlParameter("@provincia", address.provincia));
                            command.Parameters.Add(new SqlParameter("@canton", address.canton));
                            command.Parameters.Add(new SqlParameter("@distrito", address.distrito));
                            connection.Open();
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        //Proceso: 
        //Intenta conectarse a la base de datos haciendo uso de un SqlConnection,
        //Intenta ejecutar INSERT o UPDATE sobre la base de datos en la tabla CLIENTE
        //Salida: ActionResponse response: un objeto que tiene una propiedad booleana que indica si la 
        //operacion fue exitosa o no, y una propiedad message con un string que describe el resultado de
        //la operacion.
        private void SetPhoneNumber(Customer customer, string infinitive)
        {
            var uniquePhones = customer.telefonos.Distinct().ToList();
            if (infinitive == "crear")
            {
                foreach (string phoneNumber in uniquePhones)
                {
                    var query = @"INSERT INTO CLIENTE_TELEFONO
                    VALUES (@cedula_telefono , @telefono)";

                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.Add(new SqlParameter("@cedula_telefono", customer.cedula_cliente));
                            command.Parameters.Add(new SqlParameter("@telefono", phoneNumber));
                            connection.Open();
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }

            if (infinitive == "actualizar")
            {
                var phoneQuery = @"DELETE FROM CLIENTE_TELEFONO
                WHERE CEDULA_CLIENTE = @cedula";
                DeleteDatabyId(phoneQuery, customer.cedula_cliente);

                foreach (string phoneNumber in uniquePhones)
                {
                    var query = @"INSERT INTO CLIENTE_TELEFONO
                    VALUES (@cedula_telefono , @telefono)";

                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.Add(new SqlParameter("@cedula_telefono", customer.cedula_cliente));
                            command.Parameters.Add(new SqlParameter("@telefono", phoneNumber));
                            connection.Open();
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
    }
}
