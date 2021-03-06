﻿using CottonCandy.Application.AppUsuario.Interfaces;
using CottonCandy.Application.AppUsuario.Input;
using CottonCandy.Application.AppUsuario.Output;
using CottonCandy.Domain.Core.Interfaces;
using CottonCandy.Domain.Entities;
using CottonCandy.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace CottonCandy.Application.AppUsuario
{
    public class UsuarioAppService : IUsuarioAppService
    {
        private readonly IGeneroRepository _generoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPostagemRepository _postagemRepository;
        private readonly ILogado _logado;

        public UsuarioAppService(IGeneroRepository generoRepository,
                                IUsuarioRepository usuarioRepository,
                                IPostagemRepository postagemRepository,
                                ILogado logado)
        {
            _generoRepository = generoRepository;
            _usuarioRepository = usuarioRepository;
            _postagemRepository = postagemRepository;
            _logado = logado;
        }
        public async Task<UsuarioViewModel> ObterUsuario(int id)
        {
            var usuario = await _usuarioRepository
                                    .ObterUsuario(id)
                                    .ConfigureAwait(false);

            if (usuario is null)
                return default;

            return new UsuarioViewModel()
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                DataNascimento = usuario.DataNascimento,
                Email = usuario.Email,
                Genero = usuario.Genero,
                FotoPerfil = usuario.FotoPerfil,
                Cargo = usuario.Cargo,
                Cidade = usuario.Cidade,
                FotoCapa = usuario.FotoCapa
            };
        }

        public async Task<UsuarioViewModel> InserirUsuario(UsuarioInput input)
        {
            var genero = await _generoRepository
                                   .ObterGenero(input.GeneroId)
                                   .ConfigureAwait(false);

            var listaEmails = await _usuarioRepository.GetEmail();

            if (listaEmails.Contains(input.Email))
            {
                throw new ArgumentException("Esse e-mail já está cadastrado");
            }
            else
            {

                if (genero is null)
                {
                    throw new ArgumentException("O gênero que está tentando associar ao usuário não existe!");
                }

                var usuario = new Usuario(input.Nome,
                                          input.Email,
                                          input.Senha,
                                          input.DataNascimento,
                                          new Genero(genero.Id, genero.Descricao),
                                          input.FotoPerfil,
                                          input.Cargo,
                                          input.Cidade,
                                          input.FotoCapa);

                if (!usuario.EhValido())
                {
                    throw new ArgumentException("Dados obrigatórios não preenchidos");
                }

                var id = await _usuarioRepository
                                    .InserirUsuario(usuario)
                                    .ConfigureAwait(false);

                return new UsuarioViewModel()
                {
                    Id = id,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    DataNascimento = usuario.DataNascimento,
                    Genero = usuario.Genero,
                    FotoPerfil = usuario.FotoPerfil,
                    Cargo = usuario.Cargo,
                    Cidade = usuario.Cidade,
                    FotoCapa = usuario.FotoCapa
                };
            }
        }

        public async Task<PerfilUsuarioViewModel> ObterPerfil(int id)
        {
            var infos = await _usuarioRepository
                                   .ObterPerfil(id)
                                   .ConfigureAwait(false);

            var postagens = await _postagemRepository
                                        .ObterPerfil(id)
                                        .ConfigureAwait(false);

            if (infos is null)
                return default;

            if (postagens.Count == 0)
                postagens = null;

            return new PerfilUsuarioViewModel()
            {
                Id = infos.Id,
                Nome = infos.Nome,
                DataNascimento = infos.DataNascimento,
                Email = infos.Email,
                Genero = infos.Genero,
                FotoPerfil = infos.FotoPerfil,
                Cargo = infos.Cargo,
                Cidade = infos.Cidade,
                FotoCapa = infos.FotoCapa,
                Postagens = postagens
            };
        }
    }
}