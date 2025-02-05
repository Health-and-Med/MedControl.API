using MedControl.Domain.Entities;

namespace MedControl.Infrastructure.Repositories
{
    public interface IMedicoRepository
    {
        Task<MedicosModel> GetUserByCrmAsync(string crm);
        Task<MedicosModel> CreateAsync(RequestCreateMedicosModel medico);
        Task UpdateAsync(RequestUpdateMedicoModel medico);
        Task DeleteAsync(int medicoId);
        Task<MedicosModel> GetByIdAsync(int id);
        Task<IEnumerable<MedicosModel>> GetAllAsync();
        Task<List<MedicosModel>> GetBySpecialty(int specialty);
        Task<IEnumerable<EspecialidadesModel>> GetAllspecialtiesAsync();
    }
}
