﻿using CottonCandy.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CottonCandy.Domain.Interfaces
{
    public interface IPostagemRepository
    {
        Task<int> InsertAsync(Postagem postagem);
        Task<List<Postagem>> ObterInformacoesPorIdAsync(int usuarioId);
        Task<List<string>> GetByUserIdOnlyPhotosAsync(int usuarioId);
        Task<List<Postagem>> GetLinhaDoTempoDosAmigosAsync(int id);
        Task<int> GetUsuarioIdByPostagemId(int postagemId);
        Task<List<int>> GetIdPostagensAsync();
    }
}