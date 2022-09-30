using System.Data;
using DetailTECService.Models;
using Microsoft.Data.SqlClient;


namespace DetailTECService.Data
{
    //Implementacion de la logica para cada una de los endpoints expuesos en LoginController,
    //esta clase extiende la interfaz ILoginRepository, e implementa los metodos relacionados
    //a la manipulacion de datos necesaria para cumplir con los requerimientos funcionales
    //de la aplicacion.

    public class LoginRepo : ILoginRepository
    {
        private readonly string _connectionString;

        public LoginRepo()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            _connectionString = config.GetValue<string>("ConnectionStrings:DetailTECDB");

        }

        //Entrada: LoginData userData: Instancia de clase de modelo de datos que contiene
        //la informacion que fue enviada desde la aplicacion cliente.
        //Proceso: Punto de entrada del proceso de autenticacion, hace uso de funciones
        //auxiliares que obtienen informacion de la base de datos y la comparan con la informacion
        //recibida.
        //Salida: AuthResponse response: Indica mediante valores booleanos el resultado del intento
        //de login.
        public AuthResponse AuthCheck(LoginData userData)
        {
            AuthResponse response;

            DataTable dbTable = GetUserData("TRABAJADOR","CEDULA_TRABAJADOR", userData.usuario);
            response = LoginAttempt(dbTable,"CEDULA_TRABAJADOR", "PASSWORD_TRABAJADOR",userData.password);

            if(!response.logged)
            {
                dbTable = GetUserData("CLIENTE","USUARIO", userData.usuario);
                response = LoginAttempt(dbTable,"USUARIO","PASSWORD_CLIENTE",userData.password);
            }

            return response;
        }

        //Entradas: 
        //string tableName: nombre de la tabla a usar en query.
        //string userColumn: nombre de la columna a usar en query, pueder ser USUARIO o CEDULA_TRABAJADOR.
        //string usuario: el dato de nombre de usuario que envio la aplicacion cliente.
        //Proceso: Crea el query necesario utilizando los parametros de la funcion,
        //Intenta conectarse a la base de datos haciendo uso de un SqlConnection,
        //Intenta ejecutar el query sobre la base de datos y escribir el resultado al DataTable data
        //Salida: DataTable data con la informacion solicitada en el query de ser exitoso,
        //DataTable data vacio en caso de que el query no fuese exitoso.
        private DataTable GetUserData(string tableName, string userColumn, string? usuario)
        {
            var data = new DataTable();

            string query= $@"SELECT *
                     FROM {tableName}
                     WHERE {userColumn} = @usuario";

            try
            {

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@usuario", usuario));
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
        //string userColumn: nombre de columna que se revisa en el DataTable, pueder ser USUARIO o CEDULA_TRABAJADOR.
        //string passwordColumn: nombre de columna que se revisa en el DataTable, puede ser PASSWORD_CLIENTE o PASSWORD_TRABAJADOR.
        //string passwordAttempt: string de password para intento de login enviado por la aplicacion clinte.
        //Proceso: Se revisa si dbTable tiene contenido, de ser asi, se toman los valores de los campos de usuario y contrasena
        //y se hacen las validaciones correspondientes para determinar si los datos proporcionados por el cliente son validos para login
        //AuthResponse response: Indica mediante valores booleanos el resultado del intento de login.
        
        private AuthResponse LoginAttempt(DataTable dbTable,string userColumn, string passwordColumn,string? passwordAttempt)
        {
            var response = new AuthResponse();

            string? usuario = "";
            string? password = "";

            
            if (dbTable.Rows.Count != 0)
                {
                    usuario = dbTable.Rows[0][userColumn].ToString();
                    password = dbTable.Rows[0][passwordColumn].ToString();

                    if (usuario != null && password == passwordAttempt)
                    {
                        if(userColumn == "CEDULA_TRABAJADOR") response.administrador = true;
                        response.logged = true;
                    }
                }

            return response;

        }

        
    }


}
