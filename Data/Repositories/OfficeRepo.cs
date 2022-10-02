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

        
    }
}
