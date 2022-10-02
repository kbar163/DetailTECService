using System.Data;
using DetailTECService.Models;
using Microsoft.Data.SqlClient;

//Implementacion de la logica para cada una de los endpoints expuesos en WorkerController,
//esta clase extiende la interfaz ILoginRepository, e implementa los metodos relacionados
//a la manipulacion de datos necesaria para cumplir con los requerimientos funcionales
//de la aplicacion.
namespace DetailTECService.Data
{
    public class WorkerRepo : IWorkerRepository
    {
        private readonly string _connectionString;

        public WorkerRepo()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            _connectionString = config.GetValue<string>("ConnectionStrings:DetailTECDB");

        }

        //Proceso: Punto de entrada del proceso de crear un trabajador, hace uso de una funcion
        //auxiliar que inserta informacion a la base de datos. 
        //Salida: ActionResponse response: un objeto que tiene una propiedad booleana que indica si la 
        //operacion fue exitosa o no, y una propiedad message con un string que describe el resultado de
        //la operacion.
        public ActionResponse AddWorker(Worker newWorker)
        {
            ActionResponse response;
            string query = @"INSERT INTO TRABAJADOR
            VALUES (@cedula , @nacimiento , @nombre ,
            @apellido_1 , @apellido_2 , @rol , @pago , @ingreso , @password)";
            response = WriteWorkerDB(query, newWorker);
            return response;
        
        }

        //Proceso: Punto de entrada del proceso de modifica un trabajador, hace uso de una funcion
        //auxiliar que inserta informacion a la base de datos. 
        //Salida: ActionResponse response: un objeto que tiene una propiedad booleana que indica si la 
        //operacion fue exitosa o no, y una propiedad message con un string que describe el resultado de
        //la operacion.
        public ActionResponse ModifyWorker(Worker newWorker)
        {
            ActionResponse response;
            string query = @"UPDATE TRABAJADOR
            SET FECHA_NACIMIENTO = @nacimiento ,
            NOMBRE = @nombre ,
            PRIMER_APELLIDO = @apellido_1 ,
            SEGUNDO_APELLIDO = @apellido_2 ,
            ID_ROL = @rol ,
            ID_PAGO = @pago ,
            FECHA_INGRESO = @ingreso ,
            PASSWORD_TRABAJADOR = @password
            WHERE CEDULA_TRABAJADOR = @cedula";
            response = WriteWorkerDB(query, newWorker);
            return response;
        
        }


        //Proceso: 
        //Intenta conectarse a la base de datos haciendo uso de un SqlConnection,
        //Intenta ejecutar INSERT o UPDATE sobre la base de datos en la tabla TRABAJADOR
        //Salida: ActionResponse response: un objeto que tiene una propiedad booleana que indica si la 
        //operacion fue exitosa o no, y una propiedad message con un string que describe el resultado de
        //la operacion.
        public ActionResponse WriteWorkerDB(string query, Worker newWorker)
        {
            ActionResponse response = new ActionResponse();
            string verb = "";
            string infinitive = "";

                 
            
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
                
                if(newWorker.fecha_ingreso != null && newWorker.fecha_nacimiento != null)
                {
                    newWorker.fecha_nacimiento = newWorker.fecha_nacimiento.Substring(0,10);
                    newWorker.fecha_nacimiento = newWorker.fecha_ingreso.Substring(0,10);
                }
                
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@cedula", newWorker.cedula_trabajador));
                        command.Parameters.Add(new SqlParameter("@nacimiento", newWorker.fecha_nacimiento));
                        command.Parameters.Add(new SqlParameter("@nombre", newWorker.nombre));
                        command.Parameters.Add(new SqlParameter("@apellido_1", newWorker.primer_apellido));
                        command.Parameters.Add(new SqlParameter("@apellido_2", newWorker.segundo_apellido));
                        command.Parameters.Add(new SqlParameter("@rol", newWorker.id_rol));
                        command.Parameters.Add(new SqlParameter("@pago", newWorker.id_tipo_pago));
                        command.Parameters.Add(new SqlParameter("@ingreso", newWorker.fecha_nacimiento));
                        command.Parameters.Add(new SqlParameter("@password", newWorker.password_trabajador));
                        connection.Open();
                        Console.WriteLine("Connection to DB stablished");
                        command.ExecuteNonQuery();    
                        response.actualizado = true;
                        response.mensaje = $"Trabajador {verb} exitosamente";

                    } 
                }   
            }

            catch (ArgumentException e)
            {
                Console.WriteLine("ERROR: " + e.Message +  "triggered by " + e.Source);
                response.actualizado = false;
                response.mensaje = $"Error al {infinitive} al trabajador";
                
                
            }

            catch(SqlException e)
            {
                Console.WriteLine("ERROR: " + e.Message + "triggered by " + e.Source);
                response.actualizado = false;
                response.mensaje = $"Error al {infinitive} al trabajador";
            }

            return response;
        }


        //Proceso: Punto de entrada del proceso de obtener los tipos de pago de trabajadores, hace uso de funciones
        //auxiliares que obtienen informacion de la base de datos y dan formato a la respuesta
        //Salida: MultivaluePayment response: un objeto que tiene una propiedad booleana que indica si la 
        //operacion fue exitosa o no, y una propiedad que contiene una lista de valores de tipos de pago en caso
        //de que la operacion fuese exitosa.
        public MultivaluePayment GetPaymentTypes()
        {
            MultivaluePayment response;
            string query= @"SELECT *
            FROM TIPO_PAGO";
            
            DataTable dbTable = GetTableData(query);
            response = PaymentMessage(dbTable);
            return response;
        }

        
        //Proceso: Punto de entrada del proceso de obtener los roles de trabajadores, hace uso de funciones
        //auxiliares que obtienen informacion de la base de datos y dan formato a la respuesta
        //Salida: MultivalueRole response: un objeto que tiene una propiedad booleana que indica si la 
        //operacion fue exitosa o no, y una propiedad que contiene una lista de valores de rol en caso
        //de que la operacion fuese exitosa.
        public MultivalueRole GetRoles()
        {
            MultivalueRole response;
            string query= @"SELECT *
                     FROM ROL";

            DataTable dbTable = GetTableData(query);
            response = RoleMessage(dbTable);
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

            catch (ArgumentException e)
            {
                Console.WriteLine("ERROR: " + e.Message +  "triggered by " + e.Source);
            }

            catch(SqlException e)
            {
                Console.WriteLine("ERROR: " + e.Message + "triggered by " + e.Source);
            }

            return data;
        }

        //Entradas: 
        //DataTable dbTable: DataTable que potencialmente contiene informacion obtenida de la base de datos.
        //Proceso: Se revisa si dbTable tiene contenido, de ser asi, se cambia la propiedad boolean de la respuesta
        //a true y se mapea cada uno de las filas de la tabla a objetos Role que son agregados a la propiedad lista Roles.
        //Si la tabla no tiene contenido, se cambia el booleano exito a false.
        //Multivalue response: Un objeto que representa el mensaje a enviar al frontend.
        private MultivalueRole RoleMessage(DataTable dbTable)
        {
            var response = new MultivalueRole();
            response.roles = new List<Role>();
            
            if(dbTable.Rows.Count !=0)
            {
                response.exito = true;
                for(int index = 0; index < dbTable.Rows.Count; index++)
                {
                    Role role = new Role();
                    role.id_rol = (int)dbTable.Rows[index]["ID_ROL"];
                    role.tipo_rol = (string)dbTable.Rows[index]["TIPO"];
                    response.roles.Add(role);

                }
            }
            else
            {
                response.exito = false;
            }
            
            return response;
        }

        
        //Entradas: 
        //DataTable dbTable: DataTable que potencialmente contiene informacion obtenida de la base de datos.
        //Proceso: Se revisa si dbTable tiene contenido, de ser asi, se cambia la propiedad boolean de la respuesta
        //a true y se mapea cada uno de las filas de la tabla a objetos Payment que son agregados a la propiedad lista tipo_pago.
        //Si la tabla no tiene contenido, se cambia el booleano exito a false.
        //Multivalue response: Un objeto que representa el mensaje a enviar al frontend.
        private MultivaluePayment PaymentMessage(DataTable dbTable)
        {
            var response = new MultivaluePayment();
            response.tipos_pago = new List<Payment>();
            
            if(dbTable.Rows.Count !=0)
            {
                response.exito = true;
                for(int index = 0; index < dbTable.Rows.Count; index++)
                {
                    Payment payment = new Payment();
                    payment.id_tipo_pago = (int)dbTable.Rows[index]["ID_TIPO_PAGO"];
                    payment.tipo_pago = (string)dbTable.Rows[index]["TIPO"];
                    response.tipos_pago.Add(payment);

                }
            }
            else
            {
                response.exito = false;
            }
            
            return response;
        }

        
    }
}