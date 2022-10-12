using DetailTECService.Models;

namespace DetailTECService.Data
{
    public interface IAppointmentRepository
    {
        MultivalueAppointment GetAllAppointments();

        ActionResponse CreateAppointment(NewAppRequest newApp);

        ActionResponse DeleteAppointment(AppIdRequest deletionId);
    }
}