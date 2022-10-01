using System.Data;
using DetailTECService.Models;
using Microsoft.Data.SqlClient;


namespace DetailTECService.Data
{
    public class WorkerRepo : IWorkerRepository
    {
        private readonly string _connectionString;

        public WorkerRepo()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            _connectionString = config.GetValue<string>("ConnectionStrings:DetailTECDB");

        }

        public MultivalueRole GetRoles()
        {
            throw new NotImplementedException();
        }
    }
}