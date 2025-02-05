using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedControl.Domain.Entities
{
    public class UsuariosMedicosModel
    {
        public int? Id { get; set; }
        public int? MedicoId { get; set; }
        public string SenhaHash;
    }
}


