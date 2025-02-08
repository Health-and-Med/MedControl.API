using MedControl.Domain.Entities;
using MedControl.Domain.Interfaces;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace MedControl.Infrastructure.Repositories
{
    public class MedicoRepository : IMedicoRepository
    {
        private readonly IUsuariosMedicosRepository _usuariosMedicosRepository;
        private string _connectionString;
        public MedicoRepository(IUsuariosMedicosRepository usuariosMedicosRepository, IConfiguration configuration)
        {
            _usuariosMedicosRepository = usuariosMedicosRepository;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<IEnumerable<MedicosModel>> GetAllAsync()
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString)) // Cria uma nova conexão
                {
                    await connection.OpenAsync();

                    return await connection.QueryAsync<MedicosModel>(
                         "SELECT * FROM Medicos");
                }
            }
            catch (Exception e)
            {

                throw;
            }

        }
        public async Task<MedicosModel> GetByIdAsync(int id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString)) // Cria uma nova conexão
                {
                    await connection.OpenAsync();

                    return await connection.QueryFirstOrDefaultAsync<MedicosModel>("SELECT * FROM Medicos WHERE Id = @Id", new { Id = id });
                }
            }
            catch (Exception e)
            {

                throw;
            }

        }
        public async Task<MedicosModel> CreateAsync(RequestCreateMedicosModel medico)
        {

            try
            {
                var query = @"INSERT INTO Medicos (Nome, CRM, EspecialidadeId, Email, PrecoConsulta) 
                  VALUES (@Nome, @CRM, @EspecialidadeId, @Email, @PrecoConsulta) 
                  RETURNING Id;"; // Retorna apenas o ID gerado

                using (var connection = new NpgsqlConnection(_connectionString)) // Cria uma nova conexão
                {
                    await connection.OpenAsync();

                    int id = await connection.ExecuteScalarAsync<int>(query, medico);
                    return await GetByIdAsync(id);
                }


            }
            catch (Exception e)
            {

                throw;
            }

        }
        public async Task<MedicosModel> GetUserByCrmAsync(string crm)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString)) // Cria uma nova conexão
                {
                    await connection.OpenAsync();

                    var medico = await connection.QueryFirstOrDefaultAsync<MedicosModel>("SELECT * FROM Medicos WHERE Crm = @crm", new { crm });

                    if (medico == null)
                        return null;
                    medico.Usuario = await _usuariosMedicosRepository.GetByIdAsync(medico.Id.Value);

                    return medico;
                }
            }
            catch (Exception e)
            {

                throw;
            }



        }
        public async Task<List<MedicosModel>> GetBySpecialty(int specialty)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString)) // Cria uma nova conexão
                {
                    await connection.OpenAsync();

                    var medicos = await connection.QueryAsync<MedicosModel>("SELECT M.*, E.nome Especialidade FROM Medicos M INNER JOIN Especialidades E ON M.EspecialidadeId = E.ID WHERE EspecialidadeId = @specialty", new { specialty });


                    if (medicos == null)
                        return null;

                    foreach (var medico in medicos)
                    {
                        medico.Usuario = await _usuariosMedicosRepository.GetByIdAsync(medico.Id.Value);
                    }

                    return medicos.ToList();
                }
            }
            catch (Exception e)
            {

                throw;
            }



        }
        public async Task UpdateAsync(RequestUpdateMedicoModel medico, int medicoId)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString)) // Cria uma nova conexão
                {
                    await connection.OpenAsync();

                    await connection.ExecuteAsync(
                        $@"UPDATE 
                            Medicos 
                          SET 
                                Nome = @Nome, 
                                EspecialidadeId = @EspecialidadeId, 
                                Email = @Email,
                                PrecoConsulta = @PrecoConsulta 
                          WHERE 
                            Id = @Id", new { medico.Nome, medico.EspecialidadeId, medico.Email, medico.PrecoConsulta, Id = medicoId });
                }

                await _usuariosMedicosRepository.UpdateAsync(medico.Usuario, medicoId);
            }
            catch (Exception e)
            {

                throw;
            }
        }
        public async Task DeleteAsync(int medicoId)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString)) // Cria uma nova conexão
                {
                    await connection.OpenAsync();

                    await connection.ExecuteAsync(
                        "DELETE FROM Medicos WHERE Id = @medicoId", new { medicoId });
                    await _usuariosMedicosRepository.DeleteAsync(medicoId);
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<IEnumerable<EspecialidadesModel>> GetAllspecialtiesAsync()
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString)) // Cria uma nova conexão
                {
                    await connection.OpenAsync();

                    return await connection.QueryAsync<EspecialidadesModel>(
                         "SELECT * FROM Especialidades");
                }
            }
            catch (Exception e)
            {

                throw;
            }

        }


    }
}


