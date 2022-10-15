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

            if(washData.Rows.Count != 0)
            {
                
                washData.DefaultView.Sort = "NOMBRE_LAVADO";
                washData = washData.DefaultView.ToTable();
                BuildWashReport(nombre_cliente,primer_apellido_cliente,segundo_apellido_cliente,cedula,washData);
            }
            
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

            ReportDoc.Save(@"Data/File Generation/ReportDoc.html");
            var Renderer = new ChromePdfRenderer();
            var pdf = Renderer.RenderHtmlFileAsPdf("Data/File Generation/ReportDoc.html");
            pdf.SaveAs("Data/File Generation/Generated/Reports/Reporte-lavados" + cedula + ".pdf");
            File.Delete("Data/File Generation/ReportDoc.html");

        }

        private void PayrollReport()
        {
            Console.WriteLine("Not implemented");
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


    }
}