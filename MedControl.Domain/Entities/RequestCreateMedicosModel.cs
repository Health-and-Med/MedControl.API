using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MedControl.Domain.Entities
{
    public class RequestCreateMedicosModel
    {
        public string Nome { get; set; }
        public string Crm { get; set; }
        public int? EspecialidadeId { get; set; }
        public string Email { get; set; }
        public decimal? PrecoConsulta { get; set; }
        public RequestCreateUsuarioMedicoModel Usuario { get; set; }

        private bool ValidarCrm(string crm)
        {
            try
            {
                string pattern = @"^\d{6}-\d{2}/[A-Z]{2}$";
                Regex regex = new Regex(pattern);

                //string crm = "123456-78/SP";

                if (regex.IsMatch(crm))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {

                throw;
            }

        }

        public List<string> ValidateModel()
        {
            try
            {
                List<string> erros = new List<string>();
                if (string.IsNullOrEmpty(Nome))
                    erros.Add("Medico.Nome é obrigatório.");

                if (string.IsNullOrEmpty(Crm))
                {
                    erros.Add("Crm is required");
                }
                else
                {
                    if (!ValidarCrm(Crm))
                        erros.Add("Crm Inválido");
                }

                if (EspecialidadeId == 0 || EspecialidadeId == null)
                    erros.Add("Medico.EspecialidadeId é obrigatório.");

                if (PrecoConsulta == null)
                    erros.Add("Medico.PrecoConsulta é obrigatório.");

                if (Usuario == null)
                    erros.Add("Usuario");

                if (string.IsNullOrEmpty(Usuario.SenhaHash))
                    erros.Add("Usuario.SenhaHash é obrigatório.");

                return erros;
            }
            catch (Exception e)
            {

                throw;
            }
        }
    }
}
