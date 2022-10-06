using System.Data;
using DetailTECService.Models;
using Microsoft.Data.SqlClient;

//Implementacion de la logica para cada una de los endpoints expuesos en OfficeController,
//esta clase extiende la interfaz IOfficeRepository, e implementa los metodos relacionados
//a la manipulacion de datos necesaria para cumplir con los requerimientos funcionales
//de la aplicacion.
namespace DetailTECService.Data
{
    public class OfficeRepo : IOfficeRepository
    {
        private readonly string _connectionString;

        public OfficeRepo()
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


        public MultivalueOffice GetAllOffices()
        {
            MultivalueOffice response;
            string query= @"SELECT SUCURSAL.NOMBRE_SUCURSAL, SUCURSAL.TELEFONO, SUCURSAL.CEDULA_TRABAJADOR_GERENTE,
            SUCURSAL.PROVINCIA, SUCURSAL.CANTON, SUCURSAL.DISTRITO, SUCURSAL.FECHA_APERTURA, SUCURSAL.FECHA_INICIO_GERENCIA,
            TRABAJADOR.NOMBRE AS NOMBRE_TRABAJADOR_GERENTE, TRABAJADOR.PRIMER_APELLIDO AS PRIMER_APELLIDO_TRABAJADOR_GERENTE
            FROM SUCURSAL
            INNER JOIN TRABAJADOR ON SUCURSAL.CEDULA_TRABAJADOR_GERENTE=TRABAJADOR.CEDULA_TRABAJADOR;";
            DataTable dbTable = GetTableData(query);
            response = AllOfficesMessage(dbTable);
            return response;
        }
 


        //Entradas: 
        //DataTable dbTable: DataTable que potencialmente contiene informacion obtenida de la base de datos.
        //Proceso: Se revisa si dbTable tiene contenido, de ser asi, se cambia la propiedad boolean de la respuesta
        //a true y se mapea cada uno de las filas de la tabla a objetos Office que son agregados a la propiedad lista sucursales.
        //Si la tabla no tiene contenido, se cambia el booleano exito a false.
        //Multivalue response: Un objeto que representa el mensaje a enviar al frontend.
        private MultivalueOffice AllOfficesMessage(DataTable dbTable)
        {
            var response = new MultivalueOffice();
            response.sucursales = new List<Office>();
            
            if(dbTable.Rows.Count !=0)
            {
                response.exito = true;
                for(int index = 0; index < dbTable.Rows.Count; index++)
                {
                    Office office = new Office();
                    office.nombre_sucursal = (string)dbTable.Rows[index]["NOMBRE_SUCURSAL"];
                    DateTime openingDate = (DateTime)dbTable.Rows[index]["FECHA_APERTURA"];
                    string formatedOpeningDate = openingDate
                    .ToString("o",System.Globalization.CultureInfo.InvariantCulture);
                    office.fecha_apertura = formatedOpeningDate;
                    office.telefono = (string)dbTable.Rows[index]["TELEFONO"];
                    office.cedula_trabajador_gerente = (string)dbTable.Rows[index]["CEDULA_TRABAJADOR_GERENTE"];
                    office.nombre_trabajador_gerente = (string)dbTable.Rows[index]["NOMBRE_TRABAJADOR_GERENTE"];
                    office.primer_apellido_trabajador_gerente = (string)dbTable.Rows[index]["PRIMER_APELLIDO_TRABAJADOR_GERENTE"];
                    DateTime managmentStartDate = (DateTime)dbTable.Rows[index]["FECHA_INICIO_GERENCIA"];
                    string formatedManagmentStartDate = managmentStartDate
                    .ToString("o",System.Globalization.CultureInfo.InvariantCulture);
                    office.fecha_inicio_gerencia = formatedManagmentStartDate;
                    office.provincia = (string)dbTable.Rows[index]["PROVINCIA"];
                    office.canton = (string)dbTable.Rows[index]["CANTON"];
                    office.distrito = (string)dbTable.Rows[index]["DISTRITO"];
                    response.sucursales.Add(office);

                }
            }
            else
            {
                response.exito = false;
            }
            
            return response;
        }

        //Proceso: Punto de entrada del proceso de crear una sucursal, hace uso de una funcion
        //auxiliar que inserta informacion a la base de datos. 
        //Salida: ActionResponse response: un objeto que tiene una propiedad booleana que indica si la 
        //operacion fue exitosa o no, y una propiedad message con un string que describe el resultado de
        //la operacion.
        public ActionResponse AddOffice(Office newOffice)
        {
            ActionResponse response;
            string query = @"INSERT INTO SUCURSAL
            VALUES (@nombre_sucursal , @telefono , @cedula_trabajador_gerente ,
            @provincia , @canton , @distrito , @fecha_apertura , @fecha_inicio_gerencia )";
            response = WriteOfficeDB(query, newOffice);
            return response;
        
        }

        //Proceso: Punto de entrada del proceso de modifica una sucursal, hace uso de una funcion
        //auxiliar que inserta informacion a la base de datos. 
        //Salida: ActionResponse response: un objeto que tiene una propiedad booleana que indica si la 
        //operacion fue exitosa o no, y una propiedad message con un string que describe el resultado de
        //la operacion.
        public ActionResponse ModifyOffice(Office newOffice)
        {
            ActionResponse response;
            string query = @"UPDATE SUCURSAL
            SET FECHA_APERTURA = @fecha_apertura ,
            NOMBRE_SUCURSAL = @nombre_sucursal ,
            TELEFONO = @telefono ,
            CEDULA_TRABAJADOR_GERENTE = @cedula_trabajador_gerente ,
            PROVINCIA = @provincia ,
            CANTON = @canton ,
            DISTRITO = @distrito ,
            FECHA_INICIO_GERENCIA = @fecha_inicio_gerencia 
            WHERE NOMBRE_SUCURSAL = @nombre_sucursal";
            response = WriteOfficeDB(query, newOffice);
            return response;
        
        }
        //Entrada: OfficeIdRequest deleteId, tiene una propiedad string que representa el nombre de una sucrsal
        //Proceso: Se crea un query para eliminar de la DB a la sucursal cuyo NOMBRE_SUCURSAL haga match con
        //la propiedad nombre_sucursal de deleteId.
        //Intenta conectarse a la base de datos haciendo uso de un SqlConnection,
        //Intenta ejecutar DELETE sobre la base de datos en la tabla SUCURSAL
        //Salida: ActionResponse response: un objeto que tiene una propiedad booleana que indica si la 
        //operacion fue exitosa o no, y una propiedad message con un string que describe el resultado de
        //la operacion.
        public ActionResponse DeleteOffice(OfficeIdRequest deleteId)
        {
            ActionResponse response = new ActionResponse();
            string query = @"DELETE FROM SUCURSAL
            WHERE NOMBRE_SUCURSAL = @nombre_sucursal";
            
            try
            {

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@nombre_sucursal", deleteId.nombre_sucursal));
                        connection.Open();
                        Console.WriteLine("Connection to DB stablished");
                        command.ExecuteNonQuery();
                        response.actualizado = true;
                        response.mensaje = "Sucursal eliminada exitosamente";
                        
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
                    response.mensaje = "Error al eliminar sucursal";
                }
            }

            return response;
        }

        //Proceso: 
        //Intenta conectarse a la base de datos haciendo uso de un SqlConnection,
        //Intenta ejecutar INSERT o UPDATE sobre la base de datos en la tabla TRABAJADOR
        //Salida: ActionResponse response: un objeto que tiene una propiedad booleana que indica si la 
        //operacion fue exitosa o no, y una propiedad message con un string que describe el resultado de
        //la operacion.
        public ActionResponse WriteOfficeDB(string query, Office newOffice)
        {
            ActionResponse response = new ActionResponse();
            string verb = "";
            string infinitive = "";

                 
            
            try
            {
                if (query.Contains("INSERT"))
                {
                    verb = "creada";
                    infinitive = "crear";
                }

                if (query.Contains("UPDATE"))
                {
                    verb = "actualizada";
                    infinitive = "actualizar";
                }
                
                if(newOffice.fecha_apertura != null && newOffice.fecha_inicio_gerencia != null)
                {
                    newOffice.fecha_apertura = newOffice.fecha_apertura.Substring(0,10);
                    newOffice.fecha_inicio_gerencia = newOffice.fecha_inicio_gerencia.Substring(0,10);
                }
                
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@nombre_sucursal", newOffice.nombre_sucursal));
                        command.Parameters.Add(new SqlParameter("@telefono", newOffice.telefono));
                        command.Parameters.Add(new SqlParameter("@cedula_trabajador_gerente", newOffice.cedula_trabajador_gerente));
                        command.Parameters.Add(new SqlParameter("@provincia", newOffice.provincia));
                        command.Parameters.Add(new SqlParameter("@canton", newOffice.canton));
                        command.Parameters.Add(new SqlParameter("@distrito", newOffice.distrito));
                        command.Parameters.Add(new SqlParameter("@fecha_apertura", newOffice.fecha_apertura));
                        command.Parameters.Add(new SqlParameter("@fecha_inicio_gerencia", newOffice.fecha_inicio_gerencia));
                        connection.Open();
                        Console.WriteLine("Connection to DB stablished");
                        command.ExecuteNonQuery();    
                        response.actualizado = true;
                        response.mensaje = $"Sucursal {verb} exitosamente";

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
                    response.mensaje = $"Error al {infinitive} al sucursal";
                }
            }

            
            return response;
        }

        
    }
}
