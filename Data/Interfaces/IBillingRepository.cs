using DetailTECService.Models;

namespace DetailTECService.Data
{
    public interface IBillingRepository
    {
        ActionResponse CreateBill(BillRequest newBill);

    }
}