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
            throw new NotImplementedException();
        }

        private void WashReport(string cedula)
        {
            var washQuery = @"SELECT
            CITA.NOMBRE_LAVADO
            FROM CITA
            WHERE CITA.CEDULA_CLIENTE = @id
            AND CITA.FACTURADA = TRUE";

            // var customerQuery = @"SELECT
            // CLIENTE.NOMBRE, CLIENTE.PRIMER_APELLIDO,
            // CLIENTE.SEGUNDO_APELLIDO
            // FROM CLIENTE
            // WHERE CLIENTE.CEDULA_CLIENTE = @id";

            var washData = GetDataById(washQuery,cedula);
            if(washData.Rows.Count != 0)
            {
                
                DataTable distinct = washData.DefaultView.ToTable(true,"NOMBRE_LAVADO");

            }
            
        }

        private void PayrollReport()
        {
            throw new NotImplementedException();
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