using MedControl.Domain.Entities;

namespace MedControl.Application.Services
{
    public interface IUsuariosMedicosService
    {
        Task CreateAsync(RequestCreateUsuarioMedicoModel medico, int medicoId);
        Task<UsuariosMedicosModel> GetByIdAsync(int medicoId);
        Task<IEnumerable<UsuariosMedicosModel>> GetAllAsync();
        Task UpdateAsync(RequestUpdateUsuarioMedico user, int medicoId);
        Task DeleteAsync(int medicoId);
    }
}
