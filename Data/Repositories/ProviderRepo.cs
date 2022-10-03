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


    }
}