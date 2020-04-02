﻿using ConfigController.DAOs;
using ConfigController.EntityConfiguration;
using ConfigController.Models;
using KurosawaCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using AConsole = Colorful.Console;

namespace ConsoleNetCore
{
    public class Program
    {
        private BaseConfig Config;
        private ApiConfig[] ApiConfig;

        public static async Task Main(string[] args)
        {
            Program programa = new Program();
            await programa.LigarBot();
        }

        private async Task CriarDB()
        {

            if (await new KurosawaConfigContext().Database.EnsureCreatedAsync())
            {
                AConsole.WriteLine("Definindo as Configurações do Bot:", Color.Yellow);
                Config = new BaseConfig();
                Console.Write("Token Bot: ");
                Config.Token = Console.ReadLine();
                Console.Write("ID Dono: ");
                Config.IdDono = ulong.Parse(Console.ReadLine());
                Console.Write("Prefixo: ");
                Config.Prefixo = Console.ReadLine();
                await new BaseConfigDAO().Adicionar(Config);
                AConsole.WriteLine("Definindo as Configurações das Apis (0 no nome termina as adições):", Color.Yellow);
                ApiConfigDAO api = new ApiConfigDAO();
                List<ApiConfig> temp = new List<ApiConfig>();
                while(true)
                {
                    ApiConfig configapi = new ApiConfig();
                    Console.Write("Nome da Api: ");
                    configapi.Nome = Console.ReadLine();
                    if (configapi.Nome == "0")
                        break;
                    Console.Write("Key da Api: ");
                    configapi.Key = Console.ReadLine();
                    temp.Add(configapi);
                }
                ApiConfig = temp.ToArray();
                await api.Adicionar(ApiConfig);
            }
            else
            {
                Config = await new BaseConfigDAO().Ler();
                ApiConfig = await new ApiConfigDAO().Ler();
            }

            AConsole.ResetColor();
        }

        private async Task LigarBot()
        {
            AConsole.WriteAscii("Kurosawa Dia <3", Color.DarkMagenta);
            await CriarDB();
            Kurosawa kud = new Kurosawa(Config, ApiConfig);
            await kud.Iniciar();
        }
    }
}
