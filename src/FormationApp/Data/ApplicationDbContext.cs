using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FormationApp.Data
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
		}


		#region Tables


		#endregion

		#region Public Methods

		/// <summary>
		/// Fait une sauvegarde la base.
		/// </summary>
		/// <returns></returns>
		public override int SaveChanges()
		{
			return base.SaveChanges();
		}

		#endregion
	}
}
