using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FormationApp.Codes;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FormationApp
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var host = CreateHostBuilder(args).Build();

			// Crée un Scope le temps de pouvoir créer l'admin de base.
			using (var scope = host.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				CreateAdmin(services);
			}

			host.Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});

		/// <summary>
		/// Créé un compte administrateur.
		/// </summary>
		/// <param name="serviceProvider"></param>
		private static void CreateAdmin(IServiceProvider serviceProvider)
		{
			// Création de l'utilisateur Root.
			string mailRoot = "noMail@email.com";
			var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
			
			var userAdmin = userManager.FindByNameAsync("root").Result;

			if (userAdmin == null)
			{
				var poweruser = new IdentityUser
				{
					UserName = "root",
					Email = mailRoot,
				};
				string userPwd = "Azerty123!";

				var createPowerUser = userManager.CreateAsync(poweruser, userPwd).Result;
				if (createPowerUser.Succeeded)
				{
					userManager.AddToRoleAsync(poweruser, Role.Administrateur.ToString()).Wait();
				}
			}
		}

	}
}
