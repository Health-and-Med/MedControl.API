using MedControl.Domain.Entities;

namespace MedControl.Infrastructure.Repositories
{
    public interface IUsuariosMedicosRepository
    {
        Task CreateAsync(RequestCreateUsuarioMedicoModel user , int medicoId);
        Task<UsuariosMedicosModel> GetByIdAsync(int medicoId);
        Task<IEnumerable<UsuariosMedicosModel>> GetAllAsync();
        Task UpdateAsync(RequestUpdateUsuarioMedico userj, int medicoId);
        Task DeleteAsync(int medicoId);
    }
}
