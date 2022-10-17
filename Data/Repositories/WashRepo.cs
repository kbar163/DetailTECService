using System.Data;
using DetailTECService.Models;
using Microsoft.Data.SqlClient;

namespace DetailTECService.Data
{
    //Implementacion de la logica para cada una de los endpoints expuesos en WashController,
    //esta clase extiende la interfaz IWashRepository, e implementa los metodos relacionados
    //a la manipulacion de datos necesaria para cumplir con los requerimientos funcionales
    //de la aplicacion.
    public class WashRepo : IWashRepository
    {

        private readonly string _connectionString;

        public WashRepo()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            _connectionString = config.GetValue<string>("ConnectionStrings:DetailTECDB");

        }


        //Proceso: Punto de entrada del proceso de obtener todos los tipos de lavados, hace uso de varias
        //funciones auxiliares que obtienen datos de la base de datos y le dan el formato necesario
        //utilizando los modelos creados.
        //Salida: ActionResponse response: un objeto que tiene una propiedad booleana que indica si la 
        //operacion fue exitosa o no, y una propiedad lavados con una lista de objetos WashType.    
        public MultivalueWash GetAllWashTypes()
        {
            MultivalueWash response;
            string washQuery = @"SELECT LAVADO.NOMBRE_LAVADO, LAVADO.COSTO_PERSONAL, LAVADO.PRECIO,
            LAVADO.DURACION, LAVADO.PUNTOS_OTORGADOS, LAVADO.COSTO_PUNTOS
            FROM LAVADO";
            DataTable washData = GetTableData(washQuery);
            response = AllWashTypesMessage(washData);
            return response;
        }


        //Proceso: Punto de entrada del proceso de crear un tipo de lavado, hace uso de una funcion
        //auxiliar que inserta informacion a la base de datos. 
        //Salida: ActionResponse response: un objeto que tiene una propiedad booleana que indica si la 
        //operacion fue exitosa o no, y una propiedad message con un string que describe el resultado de
        //la operacion.
        public ActionResponse AddWashType(WashType newWash)
        {
            ActionResponse response;
            string query = @"INSERT INTO LAVADO
            VALUES (@nombre , @costo ,
            @precio , @duracion , @puntos_otorgados , @costo_puntos)";
            response = WriteWashDB(query, newWash);
            return response;
        }


        //Proceso: Punto de entrada del proceso de editar un tipo de lavado, hace uso de una funcion
        //auxiliar que actualiza informacion a la base de datos. 
        //Salida: ActionResponse response: un objeto que tiene una propiedad booleana que indica si la 
        //operacion fue exitosa o no, y una propiedad message con un string que describe el resultado de
        //la operacion.
        public ActionResponse ModifyWashType(WashType newWash)
        {
            ActionResponse response;
            string query = @"UPDATE LAVADO
            SET NOMBRE_LAVADO = @nombre ,
            COSTO_PERSONAL = @costo ,
            PRECIO = @precio ,
            DURACION = @duracion ,
            PUNTOS_OTORGADOS = @puntos_otorgados ,
            COSTO_PUNTOS = @costo_puntos
            WHERE NOMBRE_LAVADO = @nombre";
            response = WriteWashDB(query, newWash);
            return response;
        }


        //Proceso: Punto de entrada del proceso de borrar un tipo de lavado, hace uso de una funcion
        //auxiliar que elimina informacion a la base de datos. 
        //Salida: ActionResponse response: un objeto que tiene una propiedad booleana que indica si la 
        //operacion fue exitosa o no, y una propiedad message con un string que describe el resultado de
        //la operacion.
        public ActionResponse DeleteWashType(WashIdRequest deleteName)
        {
            var response = new ActionResponse();
            
            var washQuery = @"DELETE FROM LAVADO
            WHERE NOMBRE_LAVADO = @nombre";
            var productQuery = @"DELETE FROM LAVADO_INSUMO
            WHERE NOMBRE_LAVADO = @nombre";
            var roleQuery = @"DELETE FROM LAVADO_ROL
            WHERE NOMBRE_LAVADO = @nombre";
            var appQuery = @"SELECT CITA.ID_CITA
            FROM CITA
            WHERE CITA.NOMBRE_LAVADO = @nombre";

            var appTable = GetDataById(appQuery,deleteName.nombre_lavado);

            if(appTable.Rows.Count != 0)
            {
                response.mensaje = "Error: Para borrar este lavado, debe eliminar las citas asociadas";
                return response;
            }

            var deleteRoles = DeleteDataById(roleQuery, deleteName.nombre_lavado);
            var deleteProducts = DeleteDataById(productQuery, deleteName.nombre_lavado);
            if(deleteRoles.actualizado & deleteProducts.actualizado)
            {
                response = DeleteDataById(washQuery, deleteName.nombre_lavado);
            } else {
                if (!deleteRoles.actualizado) {
                    response = deleteRoles;
                } else {
                    response = deleteProducts;
                }
            }
    
            return response;
        }


        //Entrada: Un string query que contiene un SQL Query y un string nombre_lavado que contiene
        //un primary key para una tabla en la base de datos.
        //Proceso: Se conecta a la base de datos y ejecuta el query de eliminacion segun fue especificado.
        //Salida: booleano que indica si la operacion fue exitosa o no.
        private ActionResponse DeleteDataById(string query, string nombre_lavado)
        {
            ActionResponse response = new ActionResponse();
            try
            {

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@nombre", nombre_lavado));
                        connection.Open();
                        Console.WriteLine("Connection to DB stablished");
                        command.ExecuteNonQuery();
                        response.actualizado = true;
                        response.mensaje = "Lavado eliminado exitosamente";   
                    } 
                }   
            }
            catch (Exception ex)
            {
                if(ex is ArgumentException ||
                   ex is SqlException || ex is InvalidOperationException)
                {
                    response.actualizado = false;
                    response.mensaje = "Error al eliminar lavado";
                }
            }

            return response;
        }

        private ActionResponse WriteWashDB(string query, WashType newWash)
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
                        command.Parameters.Add(new SqlParameter("@nombre", newWash.nombre_lavado));
                        command.Parameters.Add(new SqlParameter("@costo", newWash.costo_personal));
                        command.Parameters.Add(new SqlParameter("@precio", newWash.precio));
                        command.Parameters.Add(new SqlParameter("@duracion", newWash.duracion));
                        command.Parameters.Add(new SqlParameter("@puntos_otorgados", newWash.puntos_otorgados));
                        command.Parameters.Add(new SqlParameter("@costo_puntos", newWash.costo_puntos));
                        connection.Open();
                        Console.WriteLine("Connection to DB stablished");
                        command.ExecuteNonQuery();
                        response.actualizado = true;
                        response.mensaje = $"Lavado {verb} exitosamente";
                    }
                }
                
                SetProducts(newWash, infinitive);
                SetRoles(newWash, infinitive);
            }

            catch (Exception ex)
            {
                if (ex is ArgumentException ||
                    ex is InvalidOperationException ||
                    ex is SqlException)
                {
                    Console.WriteLine("ERROR: " + ex.Message + "triggered by " + ex.Source);
                    response.actualizado = false;
                    response.mensaje = $"Error al {infinitive} el lavado";
                }
            }
            return response;
        }

        //Proceso: 
        //Intenta conectarse a la base de datos haciendo uso de un SqlConnection,
        //Intenta ejecutar INSERT o UPDATE sobre la base de datos en la tabla LAVADO_ROL
        //Salida: ActionResponse response: un objeto que tiene una propiedad booleana que indica si la 
        //operacion fue exitosa o no, y una propiedad message con un string que describe el resultado de
        //la operacion.
        private void SetRoles(WashType newWash, string infinitive)
        {
            var uniqueRoles = newWash.roles.Distinct(new RoleComparer()).ToList();
            if (infinitive == "crear")
            {
                foreach (Role role in uniqueRoles)
                {
                    var query = @"INSERT INTO LAVADO_ROL
                    VALUES (@nombre_lavado , @id_rol)";

                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {

                            command.Parameters.Add(new SqlParameter("@nombre_lavado", newWash.nombre_lavado));
                            command.Parameters.Add(new SqlParameter("@id_rol", role.id_rol));
                            connection.Open();
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }

            if (infinitive == "actualizar")
            {
                var roleQuery = @"DELETE FROM LAVADO_ROL
                WHERE NOMBRE_LAVADO = @nombre";
                DeleteDataById(roleQuery,newWash.nombre_lavado);
            
                foreach (Role role in uniqueRoles)
                {
                    var query = @"INSERT INTO LAVADO_ROL
                    VALUES (@nombre , @idRol)";

                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {

                            command.Parameters.Add(new SqlParameter("@nombre", newWash.nombre_lavado));
                            command.Parameters.Add(new SqlParameter("@idRol", role.id_rol));
                            connection.Open();
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        //Proceso: 
        //Intenta conectarse a la base de datos haciendo uso de un SqlConnection,
        //Intenta ejecutar INSERT o UPDATE sobre la base de datos en la tabla LAVADO_INSUMO
        //Salida: ActionResponse response: un objeto que tiene una propiedad booleana que indica si la 
        //operacion fue exitosa o no, y una propiedad message con un string que describe el resultado de
        //la operacion.
        private void SetProducts(WashType newWash, string infinitive)
        {
            var uniqueProducts = newWash.insumos.Distinct().ToList();
            if (infinitive == "crear")
            {
                foreach (string product in uniqueProducts)
                {
                    var query = @"INSERT INTO LAVADO_INSUMO 
                    VALUES (@nombre_lavado , @nombre_insumo)";

                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.Add(new SqlParameter("@nombre_lavado", newWash.nombre_lavado));
                            command.Parameters.Add(new SqlParameter("@nombre_insumo", product));
                            connection.Open();
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }

            if (infinitive == "actualizar")
            {
                var productQuery = @"DELETE FROM LAVADO_INSUMO
                WHERE NOMBRE_LAVADO = @nombre";
                DeleteDataById(productQuery, newWash.nombre_lavado);

                foreach (string product in uniqueProducts)
                {
                    var query = @"INSERT INTO LAVADO_INSUMO
                    VALUES (@nombre_lavado , @nombre_producto)";

                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.Add(new SqlParameter("@nombre_lavado", newWash.nombre_lavado));
                            command.Parameters.Add(new SqlParameter("@nombre_producto", product));
                            connection.Open();
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }

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
        //a true y se mapea cada uno de las filas de la tabla a objetos WashType que son agregados a la propiedad lista lavados.
        //Si la tabla no tiene contenido, se cambia el booleano exito a false.
        //Multivalue response: Un objeto que representa el mensaje a enviar al frontend.  
        private MultivalueWash AllWashTypesMessage(DataTable washTypes)
        {
            var response = new MultivalueWash();
            response.lavados = new List<WashType>();
            var roleList = new List<Role>();

            string productQuery = @"SELECT LAVADO_INSUMO.NOMBRE_INSUMO
            FROM LAVADO_INSUMO WHERE LAVADO_INSUMO.NOMBRE_LAVADO = @nombre";

            string roleIdQuery = @"SELECT LAVADO_ROL.ID_ROL
            FROM LAVADO_ROL WHERE LAVADO_ROL.NOMBRE_LAVADO = @nombre";

        
            if (washTypes.Rows.Count != 0)
            {
                response.exito = true;
                for (int index = 0; index < washTypes.Rows.Count; index++)
                {
                    WashType wash = new WashType();
                    wash.nombre_lavado = (string)washTypes.Rows[index]["NOMBRE_LAVADO"];
                    wash.costo_personal = (int)washTypes.Rows[index]["COSTO_PERSONAL"];
                    wash.precio = (int)washTypes.Rows[index]["PRECIO"];
                    wash.duracion = (int)washTypes.Rows[index]["DURACION"];
                    wash.puntos_otorgados = (int)washTypes.Rows[index]["PUNTOS_OTORGADOS"];
                    wash.costo_puntos = (int)washTypes.Rows[index]["COSTO_PUNTOS"];
                    var productTable = GetDataById(productQuery, wash.nombre_lavado);
                    GetWashProducts(productTable, wash);
                    var roleTable = GetDataById(roleIdQuery, wash.nombre_lavado);
                    GetRoles(roleTable, wash);
                    response.lavados.Add(wash);
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
        //Intenta ejecutar el query parametrizado con el nombre del lavado del que se
        //desea obtener datos en la base de datos y escribe el resultado al DataTable dbTable
        //Salida: DataTable dbTable con la informacion solicitada en el query de ser exitoso,
        //DataTable data vacio en caso de que el query no fuese exitoso.
        private DataTable GetDataById(string query, string nombre_lavado)
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
                        command.Parameters.Add(new SqlParameter("@nombre", nombre_lavado));
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

        //Entradas: DataTable que potencialmente contiene informacion sobre los insumos de un lavado
        // y el objeto WashType relacionado a ese lavado.
        //Proceso: Se revisa si dbTable tiene contenido, de ser asi, se mapea cada uno de las filas de
        // la tabla a una lista de strings que es asignada a la propiedad lista insumos.
        //Si la tabla no tiene contenido, no se genera ningun cambio.
        private void GetWashProducts(DataTable productTable, WashType wash)
        {
            if (productTable.Rows.Count != 0)
            {
                var productList = new List<string>();
                for (int index = 0; index < productTable.Rows.Count; index++)
                {
                    var nombre_insumo = (string)productTable.Rows[index]["NOMBRE_INSUMO"];
                    productList.Add(nombre_insumo);
                }

                wash.insumos = productList;
            }
        }

        //Entradas: DataTable que potencialmente contiene informacion sobre los roles asignados a un tipo de lavado
        // y el objeto WashType relacionado a ese lavado.
        //Proceso: Se revisa si dbTable tiene contenido, de ser asi, se mapea cada uno de las filas de
        // la tabla a una lista de objetos Role que es asignada a la propiedad roles.
        //Si la tabla no tiene contenido, no se genera ningun cambio.
        private void GetRoles(DataTable roleTable, WashType wash)
        {

            string roleQuery = @"SELECT ROL.TIPO
            FROM ROL WHERE ROL.ID_ROL = @id";
            var roleList = new List<Role>();
            var dbTable = new DataTable();


            try
            {
                for (int index = 0; index < roleTable.Rows.Count; index++)
                {
                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        using (SqlCommand command = new SqlCommand(roleQuery, connection))
                        {
                            var role = new Role();
                            SqlDataAdapter adapter = new SqlDataAdapter(command);
                            connection.Open();
                            command.Parameters.Add(new SqlParameter("@id", (int)roleTable.Rows[index]["ID_ROL"]));
                            Console.WriteLine("Connection to DB stablished");
                            adapter.Fill(dbTable);
                            role.id_rol = (int)roleTable.Rows[index]["ID_ROL"];
                            role.tipo_rol = (string)dbTable.Rows[index]["TIPO"];
                            roleList.Add(role);

                        }
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
            wash.roles = roleList;
        }

        
    }
}