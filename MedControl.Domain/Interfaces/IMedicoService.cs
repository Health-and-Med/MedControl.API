using MedControl.Domain.Entities;

namespace MedControl.Domain.Interfaces
{
    public interface IMedicoService
    {
        Task<MedicosModel> AuthenticateAsync(string crm, string password);
        Task CreateAsync(RequestCreateMedicosModel medico);
        Task<MedicosModel> GetByIdAsync(int id);
        Task<IEnumerable<MedicosModel>> GetAllAsync();
        Task<List<MedicosModel>> GetBySpecialty(int especialidadeId);
        Task UpdateAsync(RequestUpdateMedicoModel medico);
        Task DeleteAsync(int medicoId);
        Task<IEnumerable<EspecialidadesModel>> GetAllspecialtiesAsync();
    }
}
