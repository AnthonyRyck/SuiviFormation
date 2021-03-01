using AccessData;
using AccessData.Models;
using FormationApp.Codes;
using FormationApp.Codes.ViewModel;
using MatBlazor;
using Microsoft.AspNetCore.Identity;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FormationApp.ViewModel
{
	public class AdminSettingsViewModel : IAdminSettingsViewModel
	{
		#region Properties

		public SqlContext SqlService { get; set; }

		public UserManager<IdentityUser> UserManager { get; set; }


		public IEnumerable<UtilisateurView> UsersList { get; set; }


		public RadzenGrid<UtilisateurView> UsersViewGrid { get; set; }

		private IMatToaster Toaster { get; set; }

		#endregion

		#region Constructeur

		public AdminSettingsViewModel(UserManager<IdentityUser> userManager, SqlContext SqlSvc, IMatToaster toaster)
		{
			SqlService = SqlSvc;
			UserManager = userManager;
			Toaster = toaster;
		}

		#endregion

		#region Internal Methods

		public async Task LoadAllUsers()
		{
			IEnumerable<Personnel> allPersonnels = await SqlService.GetAllPersonnel();

			List<UtilisateurView> userView = new List<UtilisateurView>();
			foreach (var personnel in allPersonnels)
			{
				var userIdentity = await UserManager.FindByIdAsync(personnel.IdPersonnel);
				IList<string> roleIdentity = await UserManager.GetRolesAsync(userIdentity);

				UtilisateurView view = new UtilisateurView(personnel, userIdentity, roleIdentity);
				userView.Add(view);
			}

			UsersList = userView;
		}

		#endregion

		#region Event sur DataGrid

		public void EditRow(UtilisateurView user)
		{
			UsersViewGrid.EditRow(user);
		}

		public void SaveRow(UtilisateurView user)
		{
			UsersViewGrid.UpdateRow(user);
		}

		/// <summary>
		/// Sauvegarde en BDD du changement de role
		/// </summary>
		/// <param name="userModifie"></param>
		public async void OnUpdateRow(UtilisateurView userModifie)
		{
			switch (userModifie.RoleUtilisateur)
			{
				case "Agent":
					// Suppression de tous les rôles.
					await UserManager.RemoveFromRoleAsync(userModifie.UserIdentity, Role.Gestionnaire.ToString());
					await UserManager.RemoveFromRoleAsync(userModifie.UserIdentity, Role.Administrateur.ToString());
					await UserManager.RemoveFromRoleAsync(userModifie.UserIdentity, Role.Chef.ToString());

					break;

				case "Administrateur":
					await UserManager.RemoveFromRoleAsync(userModifie.UserIdentity, Role.Chef.ToString());
					await UserManager.RemoveFromRoleAsync(userModifie.UserIdentity, Role.Gestionnaire.ToString());

					await UserManager.AddToRoleAsync(userModifie.UserIdentity, Role.Administrateur.ToString());
					break;

				case "Gestionnaire":
					await UserManager.RemoveFromRoleAsync(userModifie.UserIdentity, Role.Administrateur.ToString());
					await UserManager.RemoveFromRoleAsync(userModifie.UserIdentity, Role.Chef.ToString());

					await UserManager.AddToRoleAsync(userModifie.UserIdentity, Role.Gestionnaire.ToString());
					break;

				case "Chef":
					await UserManager.RemoveFromRoleAsync(userModifie.UserIdentity, Role.Administrateur.ToString());
					await UserManager.RemoveFromRoleAsync(userModifie.UserIdentity, Role.Gestionnaire.ToString());

					await UserManager.AddToRoleAsync(userModifie.UserIdentity, Role.Chef.ToString());
					break;
				default:
					break;
			}
		}

		public void CancelEdit(UtilisateurView user)
		{
			UsersViewGrid.CancelEditRow(user);
		}

		public void DeleteRow(UtilisateurView user)
		{
			//dbContext.Remove<Order>(order);

			//// For demo purposes only
			//orders.Remove(order);

			//// For production
			////dbContext.SaveChanges();

			//ordersGrid.Reload();
		}


		public async Task ReinitMdp(UtilisateurView user)
		{
			try
			{
				await UserManager.RemovePasswordAsync(user.UserIdentity);
				await UserManager.AddPasswordAsync(user.UserIdentity, "Azerty123!");

				Toaster.Add("Mot de passe réinitialisé pour " + user.Personnel.Login, MatToastType.Success);
			}
			catch (Exception)
			{
				//Log.Error("Erreur sur REINIT de mot de passe");
				Toaster.Add("Erreur sur le réinit MDP pour " + user.Personnel.Login, MatToastType.Danger);
			}
		}

		#endregion
	}
}
