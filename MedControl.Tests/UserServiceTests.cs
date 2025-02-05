using MedControl.Application.Services;
using MedControl.Domain.Entities;
using MedControl.Domain.Interfaces;
using MedControl.Infrastructure.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MedControl.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IMedicoRepository> _mockContactRepository;
        private readonly Mock<IUsuariosMedicosRepository> mockUsuarioRepository;
        private readonly IMedicoService _userService;


        public UserServiceTests()
        {
            _mockContactRepository = new Mock<IMedicoRepository>();
            mockUsuarioRepository = new Mock<IUsuariosMedicosRepository>();
            _userService = new MedicoService(_mockContactRepository.Object, mockUsuarioRepository.Object);
        }

        [Fact]
        public async Task AddUser_ShouldThrowValidationException_WhenEmailIsEmpty()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _userService.CreateAsync(new RequestCreateMedicosModel { Email = "", Crm = "", EspecialidadeId = 0, Nome = "", PrecoConsulta = 0, Usuario = new RequestCreateUsuarioMedicoModel { SenhaHash = "123456" } }));
        }
    }
}


