﻿using CottonCandy.Application.AppPostagem.Interfaces;
using CottonCandy.Domain.Core.Interfaces;
using CottonCandy.Domain.Entities;
using CottonCandy.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CottonCandy.Application.AppPostagem
{
    public class CurtidasAppService : ICurtidasAppService
    {
        private readonly ICurtidasRepository _curtidasRepository;
        private readonly IPostagemRepository _postagemRepository;
        private readonly IAmigosRepository _amigosRepository;
        private readonly ILogado _logado;

        public CurtidasAppService(ICurtidasRepository CurtidasRepository,
                                    ILogado logado,
                                    IPostagemRepository postagemRepository,
                                    IAmigosRepository amigosRepository)
        {
            _curtidasRepository = CurtidasRepository;
            _postagemRepository = postagemRepository;
            _amigosRepository = amigosRepository;
            _logado = logado;
        }

        public async Task<int> GetQtdeCurtidasByPostagemIdAsync(int postagemId)
        {
            return await _curtidasRepository
                            .GetQtdeCurtidasByPostagemIdAsync(postagemId)
                            .ConfigureAwait(false);
        }

        public async Task<List<Curtidas>> GetByPostagemIdAsync(int postagemId)
        {
            return await _curtidasRepository
                            .GetByPostagemIdAsync(postagemId)
                            .ConfigureAwait(false);
        }

        public async Task<List<Curtidas>> GetByUsuarioIdAsync(int usuarioId)
        {
            return await _curtidasRepository
                            .GetByUsuarioIdAsync(usuarioId)
                            .ConfigureAwait(false);
        }

        public async Task<string> InsertAsync(int postagemId)
        {
            var postagens = await _postagemRepository
                                    .GetIdPostagensAsync()
                                    .ConfigureAwait(false);

            if (!postagens.Contains(postagemId))
            {
                throw new ArgumentException("Não existe publicação com esse ID");
            }

            var usuarioId = _logado.GetUsuarioLogadoId();

            var curtida = await _curtidasRepository
                                    .GetByUsuarioIdAndPostagemIdAsync(usuarioId, postagemId)
                                    .ConfigureAwait(false);

            var usuarioPostagemId = await _postagemRepository
                                             .GetUsuarioIdByPostagemId(postagemId);

            var amigosId = await _amigosRepository
                                    .ObterListaDeAmigos(usuarioId)
                                    .ConfigureAwait(false);

            if (amigosId.Contains(usuarioPostagemId) || usuarioPostagemId == usuarioId)
            {
                //Corrigir msg de retorno
                if (curtida != null)
                {
                    var descurtiu = await _curtidasRepository
                                             .DeleteAsync(curtida.Id)
                                             .ConfigureAwait(false);

                    var resultado = ("Você descurtiu a postagem de número " + postagemId);
                    return resultado;
                }
                else
                {
                    var novaCurtida = new Curtidas(usuarioId, postagemId);
                    
                    var curtiu = await _curtidasRepository
                                            .InsertAsync(novaCurtida)
                                            .ConfigureAwait(false);

                    var resultado = ("Você curtiu a postagem de número " + postagemId);
                    return resultado;
                }
            }
            else
            {
                throw new Exception("Você não pode curtir essa publicação, porque você não segue este usuário");
            }
        }
    }
}