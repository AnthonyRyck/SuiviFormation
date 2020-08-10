using AccessData;
using AccessData.Models;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormationApp.Pages
{
	public partial class Sessions : ComponentBase
	{
		#region Properties

		[Inject]
		public SqlContext SqlService { get; set; }

		[Inject]
		protected IMatToaster Toaster { get; set; }

		[Inject]
		protected SignInManager<IdentityUser> SignInManager { get; set; }

		/// <summary>
		/// Liste de toutes les sessions.
		/// </summary>
		public List<SessionView> AllSessions { get; set; }

		#endregion

		#region Override methods

		protected async override Task OnInitializedAsync()
		{
			try
			{
				AllSessions = await SqlService.GetAllOpenSessionAsync();

				foreach (var session in AllSessions)
				{
					var tempIdUserInscrit = await SqlService.GetInscriptionAsync(session.IdSession);
					session.UsersInscrits = tempIdUserInscrit.Select(x => x.IdPersonnel).ToList();
				}
			}
			catch (Exception ex)
			{
				Toaster.Add(ex.Message, MatToastType.Danger);
			}
		}

		#endregion

		#region Events


		/// <summary>
		/// Event sur un click pour s'inscrire à une session
		/// </summary>
		/// <param name="e"></param>
		/// <param name="idSession">ID de la Session.</param>
		public async void InscriptionOnClick(MouseEventArgs e, int idSession)
		{
			try
			{
				string userID = SignInManager.UserManager.GetUserId(SignInManager.Context.User);
				await SqlService.InsertInscriptionAsync(idSession, userID);

				AllSessions.FirstOrDefault(x => x.IdSession == idSession).UsersInscrits.Add(userID);

				Toaster.Add("Inscription effectuée.", MatToastType.Success);
				StateHasChanged();
			}
			catch (Exception ex)
			{
				Toaster.Add("Erreur sur l'inscription à la session.", MatToastType.Danger);
			}
		}


		/// <summary>
		/// Event sur un click pour se désinscrire à une session
		/// </summary>
		/// <param name="e"></param>
		/// <param name="idSession">ID de la Session.</param>
		public async void DesinscriptionOnClick(MouseEventArgs e, int idSession)
		{
			try
			{
				string userID = SignInManager.UserManager.GetUserId(SignInManager.Context.User);
				await SqlService.DeleteInscriptionAsync(idSession, userID);

				AllSessions.FirstOrDefault(x => x.IdSession == idSession).UsersInscrits.Remove(userID);

				Toaster.Add("Désinscription effectuée.", MatToastType.Success);
				StateHasChanged();
			}
			catch (Exception ex)
			{
				Toaster.Add("Erreur votre désinscription à la session.", MatToastType.Danger);
			}
		}

		#endregion
	}
}
