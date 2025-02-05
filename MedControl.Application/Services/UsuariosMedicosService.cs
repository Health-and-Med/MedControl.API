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
using System.Threading.Tasks;

namespace MedControl.Application.Services
{
    public class UsuariosMedicosService : IUsuariosMedicosService
    {
        private readonly IUsuariosMedicosRepository _usuariosMedicosRepository;
        private readonly byte[] _key;

        public UsuariosMedicosService(IUsuariosMedicosRepository usuariosMedicosRepository)
        {
            _usuariosMedicosRepository = usuariosMedicosRepository;
        }

        public async Task<IEnumerable<UsuariosMedicosModel>> GetAllAsync()
        {
            return await _usuariosMedicosRepository.GetAllAsync();
        }

        public async Task<UsuariosMedicosModel> GetByIdAsync(int medicoId)
        {
            return await _usuariosMedicosRepository.GetByIdAsync(medicoId);
        }

        public async Task CreateAsync(RequestCreateUsuarioMedicoModel medico, int medicoId)
        {
            await _usuariosMedicosRepository.CreateAsync(medico, medicoId);
        }

        public async Task UpdateAsync(RequestUpdateUsuarioMedico medico, int medicoId)
        {
            await _usuariosMedicosRepository.UpdateAsync(medico, medicoId);
        }

        public async Task DeleteAsync(int medicoId)
        {
            await _usuariosMedicosRepository.DeleteAsync(medicoId);
        }
    }
}


