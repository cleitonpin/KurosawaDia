﻿using DataBaseController.Contexts;
using DataBaseController.Modelos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseController.DAOs
{
    public sealed class ConfiguracoesServidoresDAO
    {
        public async Task<ConfiguracoesServidores> Get(ConfiguracoesServidores config)
        {
            using (Kurosawa_DiaContext context = new Kurosawa_DiaContext())
            {
                return (await context.ConfiguracoesServidores.FromSqlRaw("call GetServerConfig({0}, {1})", config.Servidor.ID, config.Configuracoes).ToArrayAsync()).FirstOrDefault();
            }
        }

        public async Task Add(ConfiguracoesServidores config)
        {
            using(Kurosawa_DiaContext context = new Kurosawa_DiaContext())
            {
                IDbContextTransaction transaction = await context.Database.BeginTransactionAsync();
                await context.Database.ExecuteSqlRawAsync("call SetServerConfig ({0}, {1}, {2})", config.Servidor.ID, config.Configuracoes, config.Value);
                await transaction.CommitAsync();
            }
        }
    }
}
