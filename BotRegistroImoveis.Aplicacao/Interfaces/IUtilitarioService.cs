﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRegistroImoveis.Aplicacao.Interfaces
{
    public interface IUtilitarioService
    {
        Task<bool> JsonValido(string Json);
    }
}
