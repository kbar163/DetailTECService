using DetailTECService.Models;
namespace DetailTECService.Data
{
    //Comparador utilizado para objetos Role. Determina si hay igualdad.
    //Implementa la interfaz de comparacion para tipos primitivos IEqualityComparer.
    public class RoleComparer : IEqualityComparer<Role>
    {
        public bool Equals(Role x, Role y)
        {
            return x != null
                   && y != null
                   && x.id_rol.Equals(y.id_rol)
                   && x.tipo_rol.Equals(y.tipo_rol);
                   
        }

        public int GetHashCode(Role codeh)
        {
            return codeh.GetHashCode();
        }
    }

}
