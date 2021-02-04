using AccessData.Models;
using FormationApp.Codes.ViewModel;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormationApp.ViewModel
{
	public interface ISuiviPersonnelViewModel
	{
		IEnumerable<UtilisateurView> UsersList { get; set; }

		RadzenGrid<UtilisateurView> UsersViewGrid { get; set; }

		UtilisateurView UserSelected { get; set; }

		IEnumerable<CompetenceUserView> CompetenceUser { get; set; }

		IEnumerable<SessionInscritUserView> FormationUser { get; set; }

		Task LoadAllUsers();

		Task LoadFormationsCompetences(string idPersonnel);
	}
}
