using System.Data;
using DetailTECService.Models;
using Microsoft.Data.SqlClient;

//Implementacion de la logica para cada una de los endpoints expuesos en OfficeController,
//esta clase extiende la interfaz IOfficeRepository, e implementa los metodos relacionados
//a la manipulacion de datos necesaria para cumplir con los requerimientos funcionales
//de la aplicacion.
namespace DetailTECService.Data
{
     public class ProviderRepo : IProviderRepository
    {
        private readonly string _connectionString;

        public ProviderRepo()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            _connectionString = config.GetValue<string>("ConnectionStrings:DetailTECDB");

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

        public MultivalueProvider GetAllProviders()
        {
            MultivalueProvider response;
            string query = @"SELECT PROVEEDOR.CEDULA_JURIDICA_PROVEEDOR, PROVEEDOR.NOMBRE, PROVEEDOR.CONTACTO, 
             PROVEEDOR.PROVINCIA, PROVEEDOR.CANTON, PROVEEDOR.DISTRITO, PROVEEDOR.CORREO_ELECTRONICO 
             FROM PROVEEDOR";
            DataTable dbTable = GetTableData(query);
            response = AllProvidersMessage(dbTable);
            return response;
        }
        //Entradas: 
        //DataTable dbTable: DataTable que potencialmente contiene informacion obtenida de la base de datos.
        //Proceso: Se revisa si dbTable tiene contenido, de ser asi, se cambia la propiedad boolean de la respuesta
        //a true y se mapea cada uno de las filas de la tabla a objetos Provider que son agregados a la propiedad lista .
        //Si la tabla no tiene contenido, se cambia el booleano exito a false.
        //Multivalue response: Un objeto que representa el mensaje a enviar al frontend.
         private MultivalueProvider AllProvidersMessage(DataTable dbTable)
        {
            var response = new MultivalueProvider();
            response.proveedores = new List<Provider>();
            
            if(dbTable.Rows.Count !=0)
            {
                response.exito = true;
                for(int index = 0; index < dbTable.Rows.Count; index++)
                {
                    Provider provider = new Provider();
                    provider.cedula_juridica_proveedor = (string)dbTable.Rows[index]["CEDULA_JURIDICA_PROVEEDOR"];
                    provider.nombre = (string)dbTable.Rows[index]["NOMBRE"];
                    provider.telefono = (string)dbTable.Rows[index]["CONTACTO"];
                    provider.provincia = (string)dbTable.Rows[index]["PROVINCIA"];
                    provider.canton = (string)dbTable.Rows[index]["CANTON"];
                    provider.distrito = (string)dbTable.Rows[index]["DISTRITO"];
                    provider.correo_electronico = (string)dbTable.Rows[index]["CORREO_ELECTRONICO"];
                    response.proveedores.Add(provider);
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
        //Intenta ejecutar INSERT o UPDATE sobre la base de datos en la tabla PROVEEDOR
        //Salida: ActionResponse response: un objeto que tiene una propiedad booleana que indica si la 
        //operacion fue exitosa o no, y una propiedad message con un string que describe el resultado de
        //la operacion.
        public ActionResponse WriteProviderDB(string query, Provider newProvider)
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
                
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@cedula_juridica_proveedor", newProvider.cedula_juridica_proveedor));
                        command.Parameters.Add(new SqlParameter("@nombre", newProvider.nombre));
                        command.Parameters.Add(new SqlParameter("@telefono", newProvider.telefono));
                        command.Parameters.Add(new SqlParameter("@provincia", newProvider.provincia));
                        command.Parameters.Add(new SqlParameter("@canton", newProvider.canton));
                        command.Parameters.Add(new SqlParameter("@distrito", newProvider.distrito));
                        command.Parameters.Add(new SqlParameter("@correo_electronico", newProvider.correo_electronico));
                        connection.Open();
                        Console.WriteLine("Connection to DB stablished");
                        command.ExecuteNonQuery();    
                        response.actualizado = true;
                        response.mensaje = $"Proveedor {verb} exitosamente";

                    } 
                }   
            }
            catch (Exception ex)
            {
                if(ex is ArgumentException ||
                   ex is SqlException || ex is InvalidOperationException)
                {
                    Console.WriteLine("ERROR: " + ex.Message +  "triggered by " + ex.Source);
                    response.actualizado = false;
                    response.mensaje = $"Error al {infinitive} al proveedor";
                }
            }
            return response;
        }

        //Proceso: Punto de entrada del proceso de crear un proveedor, hace uso de una funcion
        //auxiliar que inserta informacion a la base de datos. 
        //Salida: ActionResponse response: un objeto que tiene una propiedad booleana que indica si la 
        //operacion fue exitosa o no, y una propiedad message con un string que describe el resultado de
        //la operacion.
        public ActionResponse AddProvider(Provider newProvider)
        {
            ActionResponse response;
            string query = @"INSERT INTO PROVEEDOR
            VALUES (@cedula_juridica_proveedor , @nombre , @telefono ,
            @provincia , @canton , @distrito , @correo_electronico)";
            response = WriteProviderDB(query, newProvider);
            return response;
        }

        //Proceso: Punto de entrada del proceso de modificar un proveedor, hace uso de una funcion
        //auxiliar que inserta informacion a la base de datos. 
        //Salida: ActionResponse response: un objeto que tiene una propiedad booleana que indica si la 
        //operacion fue exitosa o no, y una propiedad message con un string que describe el resultado de
        //la operacion.
        public ActionResponse ModifyProvider(Provider newProvider)
        {
            ActionResponse response;
            string query = @"UPDATE PROVEEDOR
            SET NOMBRE= @nombre,
            CONTACTO = @telefono ,
            PROVINCIA = @provincia ,
            CANTON = @canton ,
            DISTRITO = @distrito ,
            CORREO_ELECTRONICO = @correo_electronico 
            WHERE CEDULA_JURIDICA_PROVEEDOR = @cedula_juridica_proveedor";
            response = WriteProviderDB(query, newProvider);
            return response;
        }

        //Entrada: ProviderIdRequest deleteId, tiene una propiedad string que representa la cedula juridica de un proveedor
        //Proceso: Se crea un query para eliminar de la DB al proveedorcuyo CEDULA__JURIDICA_PROVEEDOR haga match con
        //la propiedad cedula_juridica_proveedor de deleteId.
        //Intenta conectarse a la base de datos haciendo uso de un SqlConnection,
        //Intenta ejecutar DELETE sobre la base de datos en la tabla PROVEEDOR
        //Salida: ActionResponse response: un objeto que tiene una propiedad booleana que indica si la 
        //operacion fue exitosa o no, y una propiedad message con un string que describe el resultado de
        //la operacion.
        public ActionResponse DeleteProvider(ProviderIdRequest deleteId)
        {
            ActionResponse response = new ActionResponse();
            string query = @"DELETE FROM PROVEEDOR
            WHERE CEDULA_JURIDICA_PROVEEDOR = @cedula_juridica_proveedor";
            
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@cedula_juridica_proveedor", deleteId.cedula_juridica_proveedor));
                        connection.Open();
                        Console.WriteLine("Connection to DB stablished");
                        command.ExecuteNonQuery();
                        response.actualizado = true;
                        response.mensaje = "Proveedor eliminado exitosamente";
                        
                    } 
                }   
            }

            catch (Exception ex)
            {
                if(ex is ArgumentException ||
                   ex is SqlException || ex is InvalidOperationException)
                {
                    Console.WriteLine("ERROR: " + ex.Message +  "triggered by " + ex.Source);
                    response.actualizado = false;
                    response.mensaje = "Error al eliminar proveedor";
                }
            }
            return response;
        }
    }
}