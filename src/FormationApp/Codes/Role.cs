using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormationApp.Codes
{
	public enum Role
	{
		Agent,
		Administrateur,
		Gestionnaire
	}


	public static class RoleExtension
	{
		public static Role ToRole(this string roleString)
		{
			switch (roleString)
			{
				case "Administrateur":
					return Role.Administrateur;

				case "Gestionnaire":
					return Role.Gestionnaire;

				default:
					return Role.Agent;
			}
		}
	}
}
