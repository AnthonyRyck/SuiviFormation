using AccessData;
using AccessData.Models;
using FormationApp.Codes.ViewModel;
using MatBlazor;
using Microsoft.AspNetCore.Identity;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormationApp.ViewModel
{
	public class SuiviPersonnelViewModel : ISuiviPersonnelViewModel
	{
		#region Properties

		public IEnumerable<UtilisateurView> UsersList { get; set; }

		public RadzenGrid<UtilisateurView> UsersViewGrid { get; set; }


		public IEnumerable<SessionInscritUserView> FormationUser { get; set; }


		public IEnumerable<CompetenceUserView> CompetenceUser { get; set; }


		public UtilisateurView UserSelected { get; set; }

		#endregion

		private IMatToaster Toaster;
		private SqlContext SqlService;
		private UserManager<IdentityUser> UserManager;

		public SuiviPersonnelViewModel(IMatToaster toaster, SqlContext sqlContext, UserManager<IdentityUser> userManager)
		{
			Toaster = toaster;
			SqlService = sqlContext;
			UserManager = userManager;
		}

		#region Public Methods

		public async Task LoadAllUsers()
		{
			IEnumerable<Personnel> allPersonnels = await SqlService.GetAllPersonnel();

			List<UtilisateurView> userView = new List<UtilisateurView>();
			foreach (var personnel in allPersonnels)
			{
				var userIdentity = await UserManager.FindByIdAsync(personnel.IdPersonnel);
				UtilisateurView view = new UtilisateurView(personnel, userIdentity, new List<string>());
				userView.Add(view);
			}

			UsersList = userView;
		}


		public async Task LoadFormationsCompetences(string idPersonnel)
		{
			UserSelected = UsersList.FirstOrDefault(x => x.Personnel.IdPersonnel == idPersonnel);

			CompetenceUser = await SqlService.GetCompetencesUser(idPersonnel);
			FormationUser = await SqlService.GetInscriptionSessionUserFinishAsync(idPersonnel);
		}

		#endregion

	}
}
