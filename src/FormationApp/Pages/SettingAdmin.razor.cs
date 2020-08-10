using AccessData;
using AccessData.Models;
using FormationApp.Codes.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Options;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace FormationApp.Pages
{
	public partial class SettingAdmin : ComponentBase
	{
		#region Properties

		[Inject]
		public SqlContext SqlService { get; set; }

		[Inject]
		public UserManager<IdentityUser> UserManager { get; set; }


		public IEnumerable<UtilisateurView> UsersList { get; set; }
		

		public RadzenGrid<UtilisateurView> UsersViewGrid { get; set; }

		#endregion

		#region Constructeur

		

		#endregion

		#region Hérités

		protected override async Task OnInitializedAsync()
		{
			await LoadAllUsers();
		}

		#endregion

		#region Internal Methods

		internal async Task LoadAllUsers()
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

			StateHasChanged();
		}

		#endregion

		#region Event sur DataGrid

		internal void EditRow(UtilisateurView user)
		{
			UsersViewGrid.EditRow(user);
		}

		internal void SaveRow(UtilisateurView user)
		{
			UsersViewGrid.UpdateRow(user);
		}

		/// <summary>
		/// Sauvegarde en BDD du changement de role
		/// </summary>
		/// <param name="userModifie"></param>
		internal async void OnUpdateRow(UtilisateurView userModifie)
		{
			switch (userModifie.RoleUtilisateur)
			{
				case "Agent":
					// Suppression de tous les rôles.
					await UserManager.RemoveFromRoleAsync(userModifie.UserIdentity, "Administrateur");
					await UserManager.RemoveFromRoleAsync(userModifie.UserIdentity, "Gestionnaire");
					break;

				case "Administrateur":
					await UserManager.AddToRoleAsync(userModifie.UserIdentity, "Administrateur");
					await UserManager.RemoveFromRoleAsync(userModifie.UserIdentity, "Gestionnaire");
					break;

				case "Gestionnaire":
					await UserManager.RemoveFromRoleAsync(userModifie.UserIdentity, "Administrateur"); ;
					await UserManager.AddToRoleAsync(userModifie.UserIdentity, "Gestionnaire");
					break;
				default:
					break;
			}

			StateHasChanged();
		}

		internal void CancelEdit(UtilisateurView user)
		{
			UsersViewGrid.CancelEditRow(user);
		}

		internal void DeleteRow(UtilisateurView user)
		{
			//dbContext.Remove<Order>(order);

			//// For demo purposes only
			//orders.Remove(order);

			//// For production
			////dbContext.SaveChanges();

			//ordersGrid.Reload();
		}

		#endregion
	}
}
