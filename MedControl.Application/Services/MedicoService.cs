using MedControl.Domain.Entities;
using MedControl.Domain.Interfaces;
using MedControl.Infrastructure.Repositories;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MedControl.Application.Services
{
    public class MedicoService : IMedicoService
    {
        private readonly IMedicoRepository _medicoRepository;
        private readonly IUsuariosMedicosRepository _usuariosMedicosRepository;
        private readonly byte[] _key;

        public MedicoService(IMedicoRepository userRepository, IUsuariosMedicosRepository usuariosMedicosRepository)
        {
            _medicoRepository = userRepository;
            _usuariosMedicosRepository = usuariosMedicosRepository;
            _key = Encoding.UTF8.GetBytes("a-secure-key-of-your-choice");
        }

        public async Task<IEnumerable<MedicosModel>> GetAllAsync()
        {
            try
            {
                return await _medicoRepository.GetAllAsync();

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<MedicosModel> GetByIdAsync(int id)
        {
            try
            {
                return await _medicoRepository.GetByIdAsync(id);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task CreateAsync(RequestCreateMedicosModel medico)
        {
            try
            {

                List<string> erros = medico.ValidateModel();

                if (erros.Count > 0)
                    throw new Exception(string.Join("\n", erros));

                var medicoExist = await _medicoRepository.GetUserByCrmAsync(medico.Crm);

                if (medicoExist != null)
                    throw new Exception("Médico já registrado.");

                medico.Usuario.SenhaHash = CreatePasswordHash(medico.Usuario.SenhaHash);

                var newMedico = await _medicoRepository.CreateAsync(medico);

                if (newMedico != null)
                {
                    newMedico.Usuario = new UsuariosMedicosModel { MedicoId = newMedico.Id, SenhaHash = medico.Usuario.SenhaHash };

                    if (newMedico.Id == 0)
                        throw new ArgumentNullException("Erro ao Criar médico.");
                    await _usuariosMedicosRepository.CreateAsync(medico.Usuario, newMedico.Id.Value);
                }
                else 
                {
                    throw new Exception("Erro ao Cadastrar médico");
                }
            }
            catch (Exception e)
            {

                throw;
            }


        }


        private string CreatePasswordHash(string password)
        {
            try
            {

                using (var hmac = new HMACSHA512(_key))
                {
                    var passwordBytes = Encoding.UTF8.GetBytes(password);
                    var hash = hmac.ComputeHash(passwordBytes);
                    return Convert.ToBase64String(hash);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private bool VerifyPasswordHash(string password, string storedHash)
        {
            try
            {
                using (var hmac = new HMACSHA512(_key))
                {
                    var passwordBytes = Encoding.UTF8.GetBytes(password);
                    var hash = hmac.ComputeHash(passwordBytes);
                    var hashString = Convert.ToBase64String(hash);
                    return hashString == storedHash;
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<MedicosModel> AuthenticateAsync(string crm, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(crm))
                    throw new ArgumentNullException("Crm");

                if (string.IsNullOrEmpty(password))
                    throw new ArgumentNullException("password");


                var medico = await _medicoRepository.GetUserByCrmAsync(crm);

                if (medico == null)
                    return null;

                medico.Usuario = await _usuariosMedicosRepository.GetByIdAsync(medico.Id.Value);


                if (!VerifyPasswordHash(password, medico.Usuario.SenhaHash))
                    return null;

                return medico;
            }
            catch (Exception e)
            {

                throw;
            }

        }


        public Task<List<MedicosModel>> GetBySpecialty(int especialidadeId)
        {
            try
            {
                return _medicoRepository.GetBySpecialty(especialidadeId);

            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task UpdateAsync(RequestUpdateMedicoModel medico)
        {
            List<string> erros = medico.ValidateModel();

            if (erros.Count > 0)
                throw new Exception(string.Join("\n", erros));

            var medicoExist = await _medicoRepository.GetByIdAsync(medico.Id);
            if (medicoExist == null)
                throw new Exception("Médico não encontrado.");

            await _medicoRepository.UpdateAsync(medico);
        }

        public async Task DeleteAsync(int medicoId)
        {
            MedicosModel medico = await _medicoRepository.GetByIdAsync(medicoId);
            if (medico == null)
                throw new Exception($"Não existe na base.");

            if (medico.Email == "devs@email.com")
                throw new Exception($"Não é possível Deletar esse perfil");

            await _medicoRepository.DeleteAsync(medicoId);
        }

        public async Task<IEnumerable<EspecialidadesModel>> GetAllspecialtiesAsync()
        {
            return await _medicoRepository.GetAllspecialtiesAsync();
        }
    }
}


