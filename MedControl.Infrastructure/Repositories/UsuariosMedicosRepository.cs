using MedControl.Domain.Entities;
using MedControl.Domain.Interfaces;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Microsoft.Extensions.Configuration;


namespace MedControl.Infrastructure.Repositories
{
    public class UsuariosMedicosRepository : IUsuariosMedicosRepository
    {
        private string _connectionString;
        public UsuariosMedicosRepository(IDbConnection dbConnection, IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<UsuariosMedicosModel>> GetAllAsync()
        {
            
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString)) // Cria uma nova conexão
                {
                    await connection.OpenAsync();

                    return await connection.QueryAsync<UsuariosMedicosModel>(
                         "SELECT * FROM UsuariosMedicos");
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<UsuariosMedicosModel> GetByIdAsync(int medicoId)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString)) // Cria uma nova conexão
                {
                    await connection.OpenAsync();

                    return await connection.QueryFirstOrDefaultAsync<UsuariosMedicosModel>(
                         "SELECT * FROM UsuariosMedicos WHERE MedicoId = @medicoId", new { MedicoId = medicoId });
                }
            }
            catch (Exception e)
            {

                throw;
            }
            
        }

        public async Task CreateAsync(RequestCreateUsuarioMedicoModel user, int medicoId)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString)) // Cria uma nova conexão
                {
                    await connection.OpenAsync();

                    await connection.ExecuteAsync(
                        "INSERT INTO UsuariosMedicos (MedicoId, SenhaHash) VALUES (@MedicoId, @SenhaHash)",
                        new { user.SenhaHash, medicoId });
                }
            }
            catch (Exception e)
            {

                throw;
            }
            
        }

        public async Task UpdateAsync(RequestUpdateUsuarioMedico user, int medicoId)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString)) // Cria uma nova conexão
                {
                    await connection.OpenAsync();

                    await connection.ExecuteAsync(
                        "UPDATE UsuariosMedicos SET SenhaHash = @SenhaHash WHERE MedicoId = @medicoId",new { SenhaHash = user.SenhaHash, medicoId });
                }
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
                        "DELETE FROM UsuariosMedicos WHERE MedicoId = @medicoId", new { medicoId });
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }
    }
}


