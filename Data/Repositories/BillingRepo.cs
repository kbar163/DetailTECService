using System.Data;
using DetailTECService.Models;
using HtmlAgilityPack;
using IronPdf;
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

        public BillResponse CreateBill(BillRequest newBill)
        {
            BillResponse response = new BillResponse();

            string billQuery = @"INSERT INTO FACTURA
            VALUES (@cedula_cliente , @id_cita , @cantidad_snacks ,
            @cantidad_bebidas , @pago_puntos)";

            string appQuery = @"SELECT CITA.ID_CITA , CITA.CEDULA_CLIENTE ,
            CITA.PLACA_VEHICULO , CITA.NOMBRE_SUCURSAL , CITA.NOMBRE_LAVADO ,
            CITA.CEDULA_TRABAJADOR , CITA.HORA , CITA.FACTURADA
            FROM CITA WHERE CITA.ID_CITA = @id";

            var appTable = GetDataById(appQuery,newBill.id_cita);
            var facturada = (bool)appTable.Rows[0]["FACTURADA"];
            if(!facturada)
            {
                response = WriteBillDB(billQuery,newBill,appTable);
            }

            RequestBill(appTable);
            response.facturada = true;
            response.mensaje = "Factura creada exitosamente";
            return response;
        }

        private BillResponse WriteBillDB(string billQuery, BillRequest newBill, DataTable appTable)
        {
            BillResponse response = new BillResponse();

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
                        response.facturada = true;
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
                    response.facturada = false;
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

        private void RequestBill(DataTable appTable)
        {
            var cedula_cliente = (string)appTable.Rows[0]["CEDULA_CLIENTE"];
            var cedula_trabajador = (string)appTable.Rows[0]["CEDULA_TRABAJADOR"];
            var nombre_lavado = (string)appTable.Rows[0]["NOMBRE_LAVADO"];
            var id_cita = (int)appTable.Rows[0]["ID_CITA"];
            var placa_vehiculo = (string)appTable.Rows[0]["PLACA_VEHICULO"];

            string workerQuery = @"SELECT TRABAJADOR.NOMBRE,
            TRABAJADOR.PRIMER_APELLIDO, TRABAJADOR.SEGUNDO_APELLIDO
            FROM TRABAJADOR WHERE TRABAJADOR.CEDULA_TRABAJADOR = @id";

            string washQuery = @"SELECT LAVADO.PRECIO
            FROM LAVADO WHERE LAVADO.NOMBRE_LAVADO = @id";

            string billQuery = @"SELECT FACTURA.ID_FACTURA,
            FACTURA.CANTIDAD_SNACKS , FACTURA.CANTIDAD_BEBIDAS ,
            FACTURA.PAGO_PUNTOS
            FROM FACTURA WHERE FACTURA.ID_CITA = @id";

            string customerQuery =@"SELECT CLIENTE.NOMBRE ,
            CLIENTE.PRIMER_APELLIDO , CLIENTE.SEGUNDO_APELLIDO
            FROM CLIENTE WHERE CLIENTE.CEDULA_CLIENTE = @id";

            var workerData = GetDataById(workerQuery,cedula_trabajador);
            var washData = GetDataById(washQuery,nombre_lavado);
            var billData = GetDataById(billQuery,id_cita);
            var customerData = GetDataById(customerQuery,cedula_cliente);
            var nombre_cliente = (string)customerData.Rows[0]["NOMBRE"];
            var primer_apellido_cliente = (string)customerData.Rows[0]["PRIMER_APELLIDO"];
            var segundo_apellido_cliente = (string)customerData.Rows[0]["SEGUNDO_APELLIDO"];
            var nombre_trabajador = (string)workerData.Rows[0]["NOMBRE"];
            var primer_apellido_trabajador = (string)workerData.Rows[0]["PRIMER_APELLIDO"];
            var segundo_apellido_trabajador = (string)workerData.Rows[0]["SEGUNDO_APELLIDO"];
            var nombre_sucursal = (string)appTable.Rows[0]["NOMBRE_SUCURSAL"];
            var precio_lavado = (int)washData.Rows[0]["PRECIO"];
            var precio_snacks = 1500;
            var precio_bebida = 1000;
            var id_factura = (int)billData.Rows[0]["ID_FACTURA"];
            var cantidad_snacks = (int)billData.Rows[0]["CANTIDAD_SNACKS"];
            var cantidad_bebidas = (int)billData.Rows[0]["CANTIDAD_BEBIDAS"];
            var pago_puntos = (bool)billData.Rows[0]["PAGO_PUNTOS"];
            var fecha = (DateTime)appTable.Rows[0]["HORA"];
            

            GenerateBill(cedula_cliente,cedula_trabajador,nombre_lavado,id_cita,
            placa_vehiculo,nombre_trabajador,primer_apellido_trabajador,segundo_apellido_trabajador,
            precio_lavado,precio_bebida,precio_snacks,id_factura,cantidad_bebidas,cantidad_snacks,
            nombre_cliente, primer_apellido_cliente,segundo_apellido_cliente, nombre_sucursal, fecha,pago_puntos);
            
        }

        private void GenerateBill(string cedula_cliente, string cedula_trabajador, string nombre_lavado, int id_cita, string placa_vehiculo, string nombre_trabajador, string primer_apellido_trabajador, string segundo_apellido_trabajador, int precio_lavado, int precio_bebida, int precio_snacks, int id_factura, int cantidad_bebidas, int cantidad_snacks, string nombre_cliente, string primer_apellido_cliente, string segundo_apellido_cliente, string nombre_sucursal, DateTime fecha_hora, bool pago_puntos)
        {
            HtmlDocument billDoc = new HtmlDocument();
            billDoc.Load(@"Data/File Generation/Templates/BillBase.html");
            var trabajador = billDoc.GetElementbyId("trabajador");
            trabajador.InnerHtml = nombre_trabajador+" "+primer_apellido_trabajador+" "+segundo_apellido_trabajador;
            var sucursal = billDoc.GetElementbyId("sucursal");
            sucursal.InnerHtml = nombre_sucursal;
            var cliente = billDoc.GetElementbyId("cliente");
            cliente.InnerHtml = nombre_cliente +" "+primer_apellido_cliente+" "+segundo_apellido_cliente;
            var placa = billDoc.GetElementbyId("placa");
            placa.InnerHtml = placa_vehiculo;
            var fecha = billDoc.GetElementbyId("fecha");
            fecha.InnerHtml = fecha_hora.Date.ToShortDateString();
            var idFactura = billDoc.GetElementbyId("id");
            idFactura.InnerHtml = "ID: "+id_factura.ToString();
            var lavado = billDoc.GetElementbyId("lavado");
            lavado.InnerHtml = nombre_lavado;
            var puntos = billDoc.GetElementbyId("puntos01");
            var montoLavado = billDoc.GetElementbyId("precio01");
            var subLavado = billDoc.GetElementbyId("subtotal01");
            var subtotalWash = 0;
            if(pago_puntos)
            {   
                
                puntos.InnerHtml = "SI";
                montoLavado.InnerHtml = "₡0.00";
                subLavado.InnerHtml = "₡0.00";
            }
            else
            {   
                puntos.InnerHtml = "NO";
                montoLavado.InnerHtml = "₡"+precio_lavado.ToString();
                subLavado.InnerHtml = "₡"+precio_lavado.ToString();
                subtotalWash = precio_lavado;
            }
            var montoSnack = billDoc.GetElementbyId("precio02");
            var subSnack = billDoc.GetElementbyId("subtotal02");
            var unidadesSnack = billDoc.GetElementbyId("unidades02");
            unidadesSnack.InnerHtml = cantidad_snacks.ToString();
            int billedSnacks = cantidad_snacks - 1;
            int calcSnack = billedSnacks * precio_snacks;
            montoSnack.InnerHtml = "₡"+calcSnack.ToString();
            subSnack.InnerHtml = "₡"+calcSnack.ToString();

            var montoBebida = billDoc.GetElementbyId("precio03");
            var subBebida = billDoc.GetElementbyId("subtotal03");
            var unidadesBebida = billDoc.GetElementbyId("unidades03");
            unidadesBebida.InnerHtml = cantidad_bebidas.ToString();
            int billedBeverage = cantidad_bebidas - 1;
            int calcBeverage = billedBeverage * precio_bebida;
            montoBebida.InnerHtml = "₡"+calcBeverage.ToString();
            subBebida.InnerHtml = "₡"+calcBeverage.ToString();
            var totalBill = calcBeverage+calcSnack+subtotalWash;
            var total = billDoc.GetElementbyId("total");
            total.InnerHtml = "₡"+totalBill.ToString();

            billDoc.Save(@"Data/File Generation/BillDoc.html");
            var Renderer = new ChromePdfRenderer();
            var pdf = Renderer.RenderHtmlFileAsPdf("Data/File Generation/BillDoc.html");
            pdf.SaveAs("Data/File Generation/Generated/Bills/factura-" + id_factura + ".pdf");
            File.Delete("Data/File Generation/BillDoc.html");
            

            
            

        }
    }
}