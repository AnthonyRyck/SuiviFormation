using AccessData;
using FormationApp.Codes.ViewModel;
using Microsoft.AspNetCore.Identity;
using Radzen.Blazor;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FormationApp.ViewModel
{
	public interface IAdminSettingsViewModel
	{
		SqlContext SqlService { get; set; }
		UserManager<IdentityUser> UserManager { get; set; }
		IEnumerable<UtilisateurView> UsersList { get; set; }
		RadzenGrid<UtilisateurView> UsersViewGrid { get; set; }

		void CancelEdit(UtilisateurView user);
		void DeleteRow(UtilisateurView user);
		void EditRow(UtilisateurView user);
		Task LoadAllUsers();
		void OnUpdateRow(UtilisateurView userModifie);
		void SaveRow(UtilisateurView user);

		Task ReinitMdp(UtilisateurView user);
	}
}