using DetailTECService.Models;

namespace DetailTECService.Data
{
    public interface IAppointmentRepository
    {
        MultivalueAppointment GetAllAppointments();
    }
}