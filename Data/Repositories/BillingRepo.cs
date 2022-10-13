using System.Data;
using DetailTECService.Models;
using Microsoft.Data.SqlClient;

namespace DetailTECService.Data
{
    public class BillingRepo : IBillingRepository
    {

        private readonly string _connectionString;

        public BillingRepo()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            _connectionString = config.GetValue<string>("ConnectionStrings:DetailTECDB");

        }

        public ActionResponse CreateBill(BillRequest newBill)
        {
            ActionResponse response = new ActionResponse();

            string billQuery = @"INSERT INTO FACTURA
            VALUES (@cedula_cliente , @id_cita , @cantidad_snacks ,
            @cantidad_bebidas , @pago_puntos)";

            string appQuery = @"SELECT CITA.ID_CITA , CITA.CEDULA_CLIENTE ,
            CITA.PLACA_VEHICULO , CITA.NOMBRE_SUCURSAL , CITA.NOMBRE_LAVADO ,
            CITA.CEDULA_TRABAJADOR , CITA.HORA , CITA.FACTURADA
            FROM CITA WHERE CITA.ID_CITA = @id";

            var appTable = GetDataById(appQuery,newBill.id_cita);
            var facturada = (int)appTable.Rows[0]["FACTURADA"];
            if(facturada == 0)
            {
                response = WriteBillDB(billQuery,newBill,appTable);
            }

            GenerateBill(appTable);
            response.mensaje = "Factura creada exitosamente";
            return response;
        }

        private ActionResponse WriteBillDB(string billQuery, BillRequest newBill, DataTable appTable)
        {
            ActionResponse response = new ActionResponse();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand(billQuery, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@cedula_cliente", 
                        appTable.Rows[0]["CEDULA_CLIENTE"]));
                        command.Parameters.Add(new SqlParameter("@id_cita", newBill.id_cita));
                        command.Parameters.Add(new SqlParameter("@cantidad_snacks", newBill.cantidad_snacks));
                        command.Parameters.Add(new SqlParameter("@cantidad_bebidas", newBill.cantidad_bebidas));
                        command.Parameters.Add(new SqlParameter("@pago_puntos", newBill.pago_puntos));
                        connection.Open();
                        Console.WriteLine("Connection to DB stablished");
                        command.ExecuteNonQuery();    
                        response.actualizado = true;
                        response.mensaje = "Factura creada exitosamente";
                    } 
                }
                PointOperations(newBill,appTable);
                SetAsPaid(appTable);
            }

            catch (Exception ex)
            {
                if(ex is ArgumentException ||
                   ex is SqlException || ex is InvalidOperationException)
                {
                    Console.WriteLine("ERROR: " + ex.Message +  "triggered by " + ex.Source);
                    response.actualizado = false;
                    response.mensaje = "Error al crear factura";
                }
            }
            return response;
        }

        private void SetAsPaid(DataTable appTable)
        {
            var query = @"UPDATE CITA
            SET CITA.FACTURADA = @facturada
            WHERE CITA.ID_CITA = @id_cita";

            var id = (int)appTable.Rows[0]["ID_CITA"];
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@facturada", 1));
                        command.Parameters.Add(new SqlParameter("@id_cita", id));
                        connection.Open();
                        Console.WriteLine("Connection to DB stablished");
                        command.ExecuteNonQuery();
                    }
                }
            }

            catch (Exception ex)
            {
                if (ex is ArgumentException ||
                   ex is SqlException || ex is InvalidOperationException)
                {
                    Console.WriteLine("ERROR: " + ex.Message + "triggered by " + ex.Source);
                }
            }
        }

        private void PointOperations(BillRequest newBill, DataTable appTable)
        {
            string customerQuery = @"SELECT CLIENTE.CEDULA_CLIENTE ,
            CLIENTE.NOMBRE , CLIENTE.PRIMER_APELLIDO , CLIENTE.SEGUNDO_APELLIDO ,
            CLIENTE.PUNTOS_ACUM , CLIENTE.PUNTOS_OBT , CLIENTE.PUNTOS_REDIM
            FROM CLIENTE WHERE CLIENTE.CEDULA_CLIENTE = @id";

            string washQuery = @"SELECT LAVADO.COSTO_PERSONAL , LAVADO.PRECIO ,
            LAVADO.PUNTOS_OTORGADOS , LAVADO.COSTO_PUNTOS
            FROM LAVADO WHERE LAVADO.NOMBRE_LAVADO = @id";

            var customerTable = GetDataById(customerQuery,(string)appTable.Rows[0]["CEDULA_CLIENTE"]);
            var washtypeTable = GetDataById(washQuery,(string)appTable.Rows[0]["NOMBRE_LAVADO"]);
            var cedula_cliente = (string)customerTable.Rows[0]["CEDULA_CLIENTE"];
            var puntos_acum = (int)customerTable.Rows[0]["PUNTOS_ACUM"];
            var puntos_obt = (int)customerTable.Rows[0]["PUNTOS_OBT"];
            var puntos_redim = (int)customerTable.Rows[0]["PUNTOS_REDIM"];
            var puntos_otorg = (int)washtypeTable.Rows[0]["PUNTOS_OTORGADOS"];
            var costo_puntos = (int)washtypeTable.Rows[0]["COSTO_PUNTOS"];
            
            
            if(newBill.pago_puntos == 1)
            {
                if(puntos_acum > costo_puntos)
                {
                    puntos_redim = puntos_redim + costo_puntos;
                    puntos_acum = puntos_acum - costo_puntos;
                    
                }
            }
            else
            {
                puntos_acum = puntos_acum + puntos_otorg;
                puntos_obt = puntos_obt + puntos_otorg;
            }

            AssignPoints(puntos_acum, puntos_obt, puntos_redim, cedula_cliente);


        }

        private void AssignPoints(int puntos_acum, int puntos_obt, int puntos_redim,
         string cedula_cliente)
        {
            var pointsQuery = @"UPDATE CLIENTE
            SET CLIENTE.PUNTOS_ACUM = @puntos_acum ,
            CLIENTE.PUNTOS_REDIM = @puntos_redim ,
            CLIENTE.PUNTOS_OBT = @puntos_obt
            WHERE CLIENTE.CEDULA_CLIENTE = @cedula_cliente";

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand(pointsQuery, connection))
                    {
                        
                        command.Parameters.Add(new SqlParameter("@puntos_acum", puntos_acum));
                        command.Parameters.Add(new SqlParameter("@puntos_redim", puntos_redim));
                        command.Parameters.Add(new SqlParameter("@puntos_obt", puntos_obt)); 
                        command.Parameters.Add(new SqlParameter("@cedula_cliente", cedula_cliente));
                        connection.Open();
                        Console.WriteLine("Connection to DB stablished");
                        command.ExecuteNonQuery();
                    }
                }
            }

            catch (Exception ex)
            {
                if (ex is ArgumentException ||
                   ex is SqlException || ex is InvalidOperationException)
                {
                    Console.WriteLine("ERROR: " + ex.Message + "triggered by " + ex.Source);
                }
            }
        }

        private DataTable GetDataById(string query, int id)
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
                        command.Parameters.Add(new SqlParameter("@id", id));
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
                        command.Parameters.Add(new SqlParameter("@id", id));
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

        private void GenerateBill(DataTable appTable)
        {
            var cedula_cliente = (string)appTable.Rows[0]["CEDULA_CLIENTE"];
            var cedula_trabajador = (string)appTable.Rows[0]["CEDULA_TRABAJADOR"];
            var nombre_lavado = (string)appTable.Rows[0]["NOMBRE_LAVADO"];
            var id_cita = (int)appTable.Rows[0]["ID_CITA"];
            var placa_vehiculo = (string)appTable.Rows[0]["PLACA_VEHICULO"];

            string workerQuery = @"SELECT TRABAJADOR.NOMBRE,
            TRABAJADOR.PRIMER_APELLIDO
            FROM TRABAJADOR WHERE TRABAJADOR.CEDULA_TRABAJADOR = @id";

            string washQuery = @"SELECT LAVADO.PRECIO
            FROM LAVADO WHERE LAVADO.NOMBRE_LAVADO = @id";

            string billQuery = @"SELECT FACTURA.ID_FACTURA,
            FACTURA.CANTIDAD_SNACKS , FACTURA.CANTIDAD_BEBIDAS ,
            FACTURA.PAGO_PUNTOS
            FROM FACTURA WHERE FACTURA.ID_CITA = @id";

            var workerData = GetDataById(workerQuery,cedula_trabajador);
            var washData = GetDataById(washQuery,nombre_lavado);
            var billData = GetDataById(billQuery,id_cita);
            var nombre_trabajador = (string)workerData.Rows[0]["NOMBRE"];
            var primer_apellido_trabajador = (string)workerData.Rows[0]["PRIMER_APELLIDO"];
            var segundo_apellido_trabajador = (string)workerData.Rows[0]["SEGUNDO_APELLIDO"];
            var precio_lavado = (int)washData.Rows[0]["PRECIO"];
            var precio_snacks = 1500;
            var precio_bebida = 1000;
            var id_factura = (int)billData.Rows[0]["ID_FACTURA"];
            var cantidad_snacks = (int)billData.Rows[0]["CANTIDAD_SNACKS"];
            var cantidad_bebidas = (int)billData.Rows[0]["CANTIDAD_BEBIDAS"];
        }
    }
}