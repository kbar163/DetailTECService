using System.Data;
using DetailTECService.Models;
using Microsoft.Data.SqlClient;
//Implementacion de la logica para cada una de los endpoints expuesos en OfficeController,
//esta clase extiende la interfaz IProductRepository, e implementa los metodos relacionados
//a la manipulacion de datos necesaria para cumplir con los requerimientos funcionales
//de la aplicacion.
namespace DetailTECService.Data
{
     public class ProductRepo : IProductRepository
    {
        private readonly string _connectionString;

        public ProductRepo()
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
        
        public MultivalueProduct GetAllProducts()
        {
            MultivalueProduct response;
            string query = @"SELECT INSUMO.NOMBRE_INSUMO, INSUMO.COSTO, INSUMO.MARCA, 
             INSUMO.CEDULA_JURIDICA_PROVEEDOR
             FROM INSUMO";
            DataTable dbTable = GetTableData(query);
            response = AllProductsMessage(dbTable);
            return response;
        }
        
        //Proceso: 
        //Intenta conectarse a la base de datos haciendo uso de un SqlConnection,
        //Intenta ejecutar el query parametrizado con la cedula del proveedor del que se
        //desea obtener datos en la base de datos y escribe el resultado al DataTable dbTable
        //Salida: DataTable dbTable con la informacion solicitada en el query de ser exitoso,
        //DataTable data vacio en caso de que el query no fuese exitoso.
        private DataTable GetDataById(string query, string cedula_juridica_proveedor)
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
                        command.Parameters.Add(new SqlParameter("@cedula",cedula_juridica_proveedor));
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


        //Entradas: 
        //DataTable dbTable: DataTable que potencialmente contiene informacion obtenida de la base de datos.
        //Proceso: Se revisa si dbTable tiene contenido, de ser asi, se cambia la propiedad boolean de la respuesta
        //a true y se mapea cada uno de las filas de la tabla a objetos Provider que son agregados a la propiedad lista .
        //Si la tabla no tiene contenido, se cambia el booleano exito a false.
        //Multivalue response: Un objeto que representa el mensaje a enviar al frontend.
         private MultivalueProduct AllProductsMessage(DataTable dbTable)
        {
            var response = new MultivalueProduct();
            response.productos = new List<Product>();

            string nameQuery = @"SELECT PROVEEDOR.NOMBRE 
            FROM PROVEEDOR
            WHERE PROVEEDOR.CEDULA_JURIDICA_PROVEEDOR = @cedula";
            var cmd = new SqlCommand(nameQuery);

            if(dbTable.Rows.Count !=0)
            {
                response.exito = true;
                for(int index = 0; index < dbTable.Rows.Count; index++)
                {
                    Product product = new Product();
                    product.nombre_insumo = (string)dbTable.Rows[index]["NOMBRE_INSUMO"];
                    product.costo = (int)dbTable.Rows[index]["COSTO"];
                    product.marca = (string)dbTable.Rows[index]["MARCA"];
                    product.cedula_juridica_proveedor = (string)dbTable.Rows[index]["CEDULA_JURIDICA_PROVEEDOR"];
                    var nameTable = GetDataById(nameQuery,product.cedula_juridica_proveedor);
                    SetName(nameTable,product,nameQuery);
                    response.productos.Add(product);
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
        //Intenta ejecutar el query parametrizado con la cedula del proveedor del que se
        //desea obtener datos en la base de datos y escribe el resultado al DataTable dbTable
        //Salida: DataTable dbTable con la informacion solicitada en el query de ser exitoso,
        //DataTable data vacio en caso de que el query no fuese exitoso.
        private void SetName(DataTable nameTable, Product product,string nameQuery)
        {
            DataTable namebyid = new DataTable();
            using(SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(nameQuery, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    connection.Open();
                    command.Parameters.Add(new SqlParameter("@cedula",product.cedula_juridica_proveedor));
                    adapter.Fill(namebyid);
                } 
            }
            product.nombre_proveedor = (string)namebyid.Rows[0]["NOMBRE"];
        }
    }
}