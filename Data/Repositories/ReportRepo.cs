using System.Data;
using DetailTECService.Models;
using Microsoft.Data.SqlClient;
using HtmlAgilityPack;
using IronPdf;

namespace DetailTECService.Data
{
    //
    public class ReportRepo : IReportRepository
    {

        private readonly string _connectionString;

        public ReportRepo()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            _connectionString = config.GetValue<string>("ConnectionStrings:DetailTECDB");

        }

        public ReportResponse GenerateReport(ReportRequest newReport)
        {   
            var response = new ReportResponse();
            switch (newReport.tipo_reporte)
            {
                case 1:
                    PayrollReport();
                    response.generado = true;
                    response.mensaje = "Reporte generado exitosamente";
                break;
                case 2:
                    WashReport(newReport.cedula_cliente);
                    response.generado = true;
                    response.mensaje = "Reporte generado exitosamente";
                break;
                case 3:
                    PointReport();
                    response.generado = true;
                    response.mensaje = "Reporte generado exitosamente";
                break;
                default:
                    response.mensaje = "Error: Tipo de reporte no reconocido";
                break;
            }

            return response;
        }

        private void PointReport()
        {

            var customerQuery = @"SELECT
            CLIENTE.CEDULA_CLIENTE,CLIENTE.NOMBRE,
            CLIENTE.PRIMER_APELLIDO,CLIENTE.SEGUNDO_APELLIDO,
            CLIENTE.PUNTOS_REDIM
            FROM CLIENTE";

            var customerData = GetTableData(customerQuery);
            
            customerData.DefaultView.Sort = "PUNTOS_REDIM";
            customerData = customerData.DefaultView.ToTable();
            BuildPointReport(customerData);
            
        }

        private void BuildPointReport(DataTable customerData)
        {
            HtmlDocument ReportDoc = new HtmlDocument();
            ReportDoc.Load(@"Data/File Generation/Templates/puntos_redimidos.html");
            var fecha = ReportDoc.GetElementbyId("fecha");
            fecha.InnerHtml = DateTime.Today.ToShortDateString();
            var htmlTable = ReportDoc.GetElementbyId("table-body");
            if(customerData.Rows.Count != 0)
            {
                
                for(int index = customerData.Rows.Count -1 ; index >= 0 ;index--)
                {
                    var cedula_cliente = (string)customerData.Rows[index]["CEDULA_CLIENTE"];
                    var nombre_cliente = (string)customerData.Rows[index]["NOMBRE"];
                    var primer_apellido_cliente = (string)customerData.Rows[index]["PRIMER_APELLIDO"];
                    var segundo_apellido_cliente = (string)customerData.Rows[index]["SEGUNDO_APELLIDO"];
                    var puntos_redim = (int)customerData.Rows[index]["PUNTOS_REDIM"];
                    var nombre_completo = nombre_cliente + " "+primer_apellido_cliente+" "+segundo_apellido_cliente;
                    htmlTable.InnerHtml = htmlTable.InnerHtml+
                    $"<tr><td><span class=\"text-inverse\">{nombre_completo}</span></td><td class=\"text-center\">{cedula_cliente}</td><td class=\"text-right\">{puntos_redim.ToString()}</td></tr>";
                }

            }
            else
            {
                htmlTable.InnerHtml = htmlTable.InnerHtml+
                    $"<tr><td><span class=\"text-inverse\"> Sin datos </span></td><td class=\"text-right\"> Sin datos </td></tr>";
            }
            

            ReportDoc.Save(@"Data/File Generation/ReportDoc.html");
            var Renderer = new ChromePdfRenderer();
            var pdf = Renderer.RenderHtmlFileAsPdf("Data/File Generation/ReportDoc.html");
            pdf.SaveAs("Data/File Generation/Generated/Reports/Reporte-puntos"+".pdf");
            File.Delete("Data/File Generation/ReportDoc.html");
        }

        private void PayrollReport()
        {
            Console.WriteLine("Not implemented");
        }

        private void WashReport(string cedula)
        {
            var washQuery = @"SELECT
            CITA.NOMBRE_LAVADO
            FROM CITA
            WHERE CITA.CEDULA_CLIENTE = @id
            AND CITA.FACTURADA = 'TRUE'";

            var customerQuery = @"SELECT
            CLIENTE.NOMBRE, CLIENTE.PRIMER_APELLIDO,
            CLIENTE.SEGUNDO_APELLIDO
            FROM CLIENTE
            WHERE CLIENTE.CEDULA_CLIENTE = @id";

            var customerData = GetDataById(customerQuery, cedula);
            var washData = GetDataById(washQuery,cedula);
            var nombre_cliente = (string)customerData.Rows[0]["NOMBRE"];
            var primer_apellido_cliente = (string)customerData.Rows[0]["PRIMER_APELLIDO"];
            var segundo_apellido_cliente = (string)customerData.Rows[0]["SEGUNDO_APELLIDO"];
            washData.Clear();//delete
            washData.DefaultView.Sort = "NOMBRE_LAVADO";
            washData = washData.DefaultView.ToTable();
            BuildWashReport(nombre_cliente,primer_apellido_cliente,segundo_apellido_cliente,cedula,washData);  
        }

        private void BuildWashReport(string nombre_cliente, string primer_apellido_cliente, string segundo_apellido_cliente, string cedula, DataTable washData)
        {
            HtmlDocument ReportDoc = new HtmlDocument();
            ReportDoc.Load(@"Data/File Generation/Templates/lavados_cliente.html");
            var nombre = ReportDoc.GetElementbyId("nombre");
            nombre.InnerHtml = nombre_cliente + " " +primer_apellido_cliente + " " + segundo_apellido_cliente;
            var textCedula = ReportDoc.GetElementbyId("cedula");
            textCedula.InnerHtml = cedula;
            var fecha = ReportDoc.GetElementbyId("fecha");
            fecha.InnerHtml = DateTime.Today.ToShortDateString();
            var htmlTable = ReportDoc.GetElementbyId("table-body");
            if(washData.Rows.Count != 0)
            {
                var apparitions = washData.AsEnumerable()
                                  .GroupBy(e => new { Name = e.Field<string>("NOMBRE_LAVADO")})
                                  .Select(group => new
                                  {
                                    Name = group.Key.Name,
                                    Count = group.Count()
                                  });

                foreach (var lavado in apparitions)
                {
                    htmlTable.InnerHtml = htmlTable.InnerHtml+
                    $"<tr><td><span class=\"text-inverse\">{lavado.Name}</span><br></td><td class=\"text-right\">{lavado.Count}</td></tr>";
                }

            }
            else
            {
                htmlTable.InnerHtml = htmlTable.InnerHtml+
                    $"<tr><td><span class=\"text-inverse\"> Sin datos </span><br></td><td class=\"text-right\"> Sin datos </td></tr>";
            }
            

            ReportDoc.Save(@"Data/File Generation/ReportDoc.html");
            var Renderer = new ChromePdfRenderer();
            var pdf = Renderer.RenderHtmlFileAsPdf("Data/File Generation/ReportDoc.html");
            pdf.SaveAs("Data/File Generation/Generated/Reports/Reporte-lavados" + cedula + ".pdf");
            File.Delete("Data/File Generation/ReportDoc.html");

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


    }
}