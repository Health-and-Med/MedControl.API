using MedControl.Domain.Interfaces;
using Dapper;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using MedControl.Domain.Entities;

namespace MedControl.Infrastructure.Repositories
{
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly IDbConnection _dbConnection;
        private readonly IMedicoService _medicoService;

        public DatabaseInitializer(IDbConnection dbConnection, IMedicoService medicoService)
        {
            _dbConnection = dbConnection;
            _medicoService = medicoService;
        }

        public void Initialize()
        {
            var createMedicosTableQuery = @"
                CREATE TABLE IF NOT EXISTS Medicos (
                    Id SERIAL PRIMARY KEY,
                    Nome VARCHAR(50) NOT NULL,
                    Crm VARCHAR(50) NOT NULL,
                    EspecialidadeId INT NOT NULL,
                    Email VARCHAR(255) NOT NULL,
                    PrecoConsulta DECIMAL(10,2)
                );
            ";

            var createUserMedicosTableQuery = @"
                CREATE TABLE IF NOT EXISTS UsuariosMedicos (
                    Id SERIAL PRIMARY KEY,
                    MedicoId INT,
                    SenhaHash TEXT
                );
            ";

            var createEspecialidadesTableQuery = @"
                CREATE TABLE IF NOT EXISTS Especialidades (
                    Id SERIAL PRIMARY KEY,
                    Nome TEXT
                );
            ";


            _dbConnection.Execute(createMedicosTableQuery);
            _dbConnection.Execute(createUserMedicosTableQuery);
            _dbConnection.Execute(createEspecialidadesTableQuery);
            

            // Adicionar usuário admin se não existir
            AddAdminUser();
        }

        private void AddAdminUser()
        {
            try
            {
                RequestCreateMedicosModel medico = new RequestCreateMedicosModel();
                medico.Crm = "123456-78/SP";
                medico.Email = "devs@email.com";
                medico.PrecoConsulta = 200;
                medico.EspecialidadeId = 1;
                medico.Nome = "Devs";
                medico.Usuario = new RequestCreateUsuarioMedicoModel();
                medico.Usuario.SenhaHash = "123456";

                var checkAdminUserQuery = "SELECT COUNT(*) FROM Medicos WHERE Crm = @Crm";
                var adminUserCount = _dbConnection.ExecuteScalar<int>(checkAdminUserQuery, new { crm = medico.Crm });

                

                if (adminUserCount == 0)
                {
                    _medicoService.CreateAsync(medico);
                }

                var checkEspecialidadesQuery = "SELECT COUNT(*) FROM Especialidades";
                var especialidadesCount = _dbConnection.ExecuteScalar<int>(checkEspecialidadesQuery);

                if(especialidadesCount == 0)
                    _dbConnection.ExecuteScalar<int>($@"
                        INSERT INTO Especialidades (Nome) VALUES
                            ('Alergologia'),
                            ('Anestesiologia'),
                            ('Angiologia'),
                            ('Cardiologia'),
                            ('Cirurgia Cardiovascular'),
                            ('Cirurgia da Mão'),
                            ('Cirurgia de Cabeça e Pescoço'),
                            ('Cirurgia do Aparelho Digestivo'),
                            ('Cirurgia Geral'),
                            ('Cirurgia Oncológica'),
                            ('Cirurgia Pediátrica'),
                            ('Cirurgia Plástica'),
                            ('Cirurgia Torácica'),
                            ('Cirurgia Vascular'),
                            ('Clínica Médica'),
                            ('Coloproctologia'),
                            ('Dermatologia'),
                            ('Endocrinologia e Metabologia'),
                            ('Endoscopia'),
                            ('Gastroenterologia'),
                            ('Genética Médica'),
                            ('Geriatria'),
                            ('Ginecologia e Obstetrícia'),
                            ('Hematologia e Hemoterapia'),
                            ('Homeopatia'),
                            ('Infectologia'),
                            ('Mastologia'),
                            ('Medicina de Emergência'),
                            ('Medicina de Família e Comunidade'),
                            ('Medicina do Trabalho'),
                            ('Medicina do Tráfego'),
                            ('Medicina Esportiva'),
                            ('Medicina Física e Reabilitação'),
                            ('Medicina Intensiva'),
                            ('Medicina Legal e Perícia Médica'),
                            ('Medicina Nuclear'),
                            ('Medicina Preventiva e Social'),
                            ('Nefrologia'),
                            ('Neurocirurgia'),
                            ('Neurologia'),
                            ('Nutrologia'),
                            ('Oftalmologia'),
                            ('Oncologia Clínica'),
                            ('Ortopedia e Traumatologia'),
                            ('Otorrinolaringologia'),
                            ('Patologia'),
                            ('Patologia Clínica/Medicina Laboratorial'),
                            ('Pediatria'),
                            ('Pneumologia'),
                            ('Psiquiatria'),
                            ('Radiologia e Diagnóstico por Imagem'),
                            ('Radioterapia'),
                            ('Reumatologia'),
                            ('Urologia');

                                                        
                                                        ");



            }
            catch (Exception e)
            {

                throw;
            }
            
        }
    }
}


