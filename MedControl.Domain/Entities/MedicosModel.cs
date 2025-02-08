using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MedControl.Domain.Entities
{
    public class MedicosModel
    {
        public int? Id { get; set; }
        public string Nome { get; set; }
        public string Especialidade { get; set; }
        public string Crm { get; set; }
        public int? EspecialidadeId { get; set; }
        public string Email { get; set; }
        public decimal? PrecoConsulta { get; set; }
        public UsuariosMedicosModel Usuario { get; set; }

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
    }
}


