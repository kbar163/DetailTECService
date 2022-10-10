using DetailTECService.Models;
namespace DetailTECService.Data
{
    //Comparador utilizado para objetos Address. Determina si hay igualdad.
    //Implementa la interfaz de comparacion para tipos primitivos IEqualityComparer.
    public class AddressComparer : IEqualityComparer<Address>
    {
        public bool Equals(Address x, Address y)
        {
            return x != null
                   && y != null
                   && x.provincia.Equals(y.provincia)
                   && x.canton.Equals(y.canton)
                   && x.distrito.Equals(y.distrito);
        }

        public int GetHashCode(Address codeh)
        {
            return codeh.GetHashCode();
        }
    }

}
