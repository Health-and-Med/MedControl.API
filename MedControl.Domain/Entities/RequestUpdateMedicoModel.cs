﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MedControl.Domain.Entities
{
    public class RequestUpdateMedicoModel
    {

        public string Nome { get; set; }
        public int? EspecialidadeId { get; set; }
        public string Email { get; set; }
        public decimal? PrecoConsulta { get; set; }
        public RequestUpdateUsuarioMedico Usuario { get; set; }


        public List<string> ValidateModel()
        {
            try
            {
                List<string> erros = new List<string>();
                if (string.IsNullOrEmpty(Nome))
                    erros.Add("Nome é obrigatório.");


                if (EspecialidadeId == 0 || EspecialidadeId == null)
                    erros.Add("EspecialidadeId é obrigatório.");

                if (PrecoConsulta == null)
                    erros.Add("PrecoConsulta é obrigatório.");

                if (Usuario == null)
                    erros.Add("Usuario é obrigatório.");

                if (string.IsNullOrEmpty(Usuario.SenhaHash))
                    erros.Add("SenhaHash é obrigatória.");

                return erros;
            }
            catch (Exception e)
            {

                throw;
            }
        }

    }
}
