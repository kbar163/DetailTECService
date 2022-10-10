using System.Data;
using DetailTECService.Models;
using Microsoft.Data.SqlClient;

namespace DetailTECService.Data
{
    public class AppointmentRepo : IAppointmentRepository
    {
        private readonly string _connectionString;

        public AppointmentRepo()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            _connectionString = config.GetValue<string>("ConnectionStrings:DetailTECDB");

        }

        //Proceso: Punto de entrada del proceso de obtener todos las citas, hace uso de varias
        //funciones auxiliares que obtienen datos de la base de datos y le dan el formato necesario
        //utilizando los modelos creados.
        //Salida: ActionResponse response: un objeto que tiene una propiedad booleana que indica si la 
        //operacion fue exitosa o no, y una propiedad clientes con una lista de objetos Customer.
        public MultivalueAppointment GetAllAppointments()
        {
            MultivalueAppointment response;
            string appointmentQuery = @"SELECT CITA.ID_CITA, CITA.CEDULA_CLIENTE, CITA.PLACA_VEHICULO,
            CITA.NOMBRE_SUCURSAL, CITA.NOMBRE_LAVADO, CITA.CEDULA_TRABAJADOR, CITA.HORA,
            CITA.FACTURADA
            FROM CITA";
            DataTable appointmentData = GetTableData(appointmentQuery);
            response = AllAppointmentsMessage(appointmentData);
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
        //a true y se mapea cada uno de las filas de la tabla a objetos Appointment que son agregados a la propiedad lista citas.
        //Si la tabla no tiene contenido, se cambia el booleano exito a false.
        //Multivalue response: Un objeto que representa el mensaje a enviar al frontend.
        private MultivalueAppointment AllAppointmentsMessage(DataTable appointments)
        {
            var response = new MultivalueAppointment();
            response.citas = new List<Appointment>();

            string durationQuery = @"SELECT LAVADO.DURACION
            FROM LAVADO WHERE LAVADO.NOMBRE_LAVADO = @nombre";

            string workerQuery = @"SELECT TRABAJADOR.NOMBRE,
            TRABAJADOR.PRIMER_APELLIDO
            FROM TRABAJADOR WHERE TRABAJADOR.CEDULA_TRABAJADOR = @cedula";
            
            string customerQuery = @"SELECT CLIENTE.NOMBRE,
            CLIENTE.PRIMER_APELLIDO
            FROM CLIENTE WHERE CLIENTE.CEDULA_CLIENTE = @cedula";


            if (appointments.Rows.Count != 0)
            {

                try
                {
                    response.exito = true;
                    for (int index = 0; index < appointments.Rows.Count; index++)
                    {
                        Appointment appointment = new Appointment();
                        appointment.id_cita = (int)appointments.Rows[index]["ID_CITA"];
                        appointment.cedula_cliente = (string)appointments.Rows[index]["CEDULA_CLIENTE"];
                        appointment.placa_vehiculo = (string)appointments.Rows[index]["PLACA_VEHICULO"];
                        appointment.nombre_sucursal = (string)appointments.Rows[index]["NOMBRE_SUCURSAL"];
                        appointment.nombre_lavado = (string)appointments.Rows[index]["NOMBRE_LAVADO"];
                        appointment.cedula_trabajador = (string)appointments.Rows[index]["CEDULA_TRABAJADOR"];
                        DateTime appTime = (DateTime)appointments.Rows[index]["HORA"];
                        string formattedTime = appTime
                        .ToString("o", System.Globalization.CultureInfo.InvariantCulture);
                        appointment.hora = formattedTime;
                        appointment.facturada = (int)appointments.Rows[index]["FACTURADA"];
                        var washtypeTable = GetDataByName(durationQuery, appointment.nombre_lavado);
                        appointment.duracion = (int)washtypeTable.Rows[0]["DURACION"];
                        var workerTable = GetDataById(workerQuery, appointment.cedula_trabajador);
                        appointment.nombre_trabajador = (string)workerTable.Rows[0]["NOMBRE"];
                        appointment.apellido_trabajador = (string)workerTable.Rows[0]["PRIMER_APELLIDO"];
                        var customerTable = GetDataById(customerQuery, appointment.cedula_cliente);
                        appointment.nombre_cliente = (string)customerTable.Rows[0]["NOMBRE"];
                        appointment.apellido_cliente = (string)customerTable.Rows[0]["PRIMER_APELLIDO"];
                        response.citas.Add(appointment);
                    }
                }
                catch(Exception ex)
                {
                    response.exito = false;
                    Console.WriteLine("ERROR: " + ex.Message + "triggered by " + ex.Source);
                }
                
            }
            else
            {
                response.exito = false;
                Console.WriteLine("ERROR: no app info on db" );
            }

            return response;
        }

        //Proceso: 
        //Intenta conectarse a la base de datos haciendo uso de un SqlConnection,
        //Intenta ejecutar el query parametrizado con un nombre (de sucursal o lavado) que se
        //desea obtener datos en la base de datos y escribe el resultado al DataTable dbTable
        //Salida: DataTable dbTable con la informacion solicitada en el query de ser exitoso,
        //DataTable data vacio en caso de que el query no fuese exitoso.
        private DataTable GetDataByName(string query, string name)
        {
            var dbTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    connection.Open();
                    command.Parameters.Add(new SqlParameter("@nombre", name));
                    Console.WriteLine("Connection to DB stablished");
                    adapter.Fill(dbTable);
                }
            }

            return dbTable;
        }


        //Proceso: 
        //Intenta conectarse a la base de datos haciendo uso de un SqlConnection,
        //Intenta ejecutar el query parametrizado con un id cedula que representa un individuo del que se
        //desea obtener datos en la base de datos y escribe el resultado al DataTable dbTable
        //Salida: DataTable dbTable con la informacion solicitada en el query de ser exitoso,
        //DataTable data vacio en caso de que el query no fuese exitoso.
        private DataTable GetDataById(string query, string id)
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
                        command.Parameters.Add(new SqlParameter("@cedula", id));
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

        
    }
}