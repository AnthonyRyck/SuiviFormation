using AccessData.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormationApp.Codes.ViewModel
{

	public class UtilisateurView
	{
		public Personnel Personnel { get; private set; }

		public IdentityUser	UserIdentity { get; private set; }

		public string RoleUtilisateur { get; set; }

		public IEnumerable<string> ListRolesPossible { get; set; }


		public UtilisateurView(Personnel personnel, IdentityUser userIdentity)
		{
			Personnel = personnel;
			UserIdentity = userIdentity;
		}

		public UtilisateurView(Personnel personnel, IdentityUser userIdentity, IList<string> roleIdentity) 
		{
			ListRolesPossible = new List<string>()
			{
				Role.Agent.ToString(),
				Role.Gestionnaire.ToString(),
				Role.Chef.ToString(),
				Role.Administrateur.ToString()
			};

			Personnel = personnel;
			UserIdentity = userIdentity;

			RoleUtilisateur = roleIdentity.Any()
				? roleIdentity.FirstOrDefault()
				: Role.Agent.ToString();
		}

	}
}
