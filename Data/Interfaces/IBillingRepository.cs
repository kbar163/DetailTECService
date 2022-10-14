using DetailTECService.Models;

namespace DetailTECService.Data
{
    public interface IBillingRepository
    {
        BillResponse CreateBill(BillRequest newBill);

    }
}