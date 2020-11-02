using AccessData;
using AccessData.Models;
using FormationApp.Data;
using MatBlazor;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FormationApp.ViewModel
{
	public class SessionsViewModel : ISessions
	{
		#region Properties

		SqlContext SqlService { get; set; }

		IMatToaster Toaster { get; set; }

		public CurrentUserService UserService { get; set; }

		/// <summary>
		/// Liste de toutes les sessions.
		/// </summary>
		public List<SessionView> AllSessions { get; set; }

		#endregion

		#region Override methods

		public SessionsViewModel(SqlContext sqlContext, IMatToaster toaster, CurrentUserService currentUser)
		{
			SqlService = sqlContext;
			Toaster = toaster;
			UserService = currentUser;

			try
			{
				AllSessions = SqlService.GetAllOpenSessionAsync().GetAwaiter().GetResult();

				foreach (var session in AllSessions)
				{
					var tempIdUserInscrit = SqlService.GetInscriptionAsync(session.IdSession).GetAwaiter().GetResult();
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
				await SqlService.InsertInscriptionAsync(idSession, UserService.UserId);

				AllSessions.FirstOrDefault(x => x.IdSession == idSession).UsersInscrits.Add(UserService.UserId);

				Toaster.Add("Inscription effectuée.", MatToastType.Success);
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
				await SqlService.DeleteInscriptionAsync(idSession, UserService.UserId);

				AllSessions.FirstOrDefault(x => x.IdSession == idSession).UsersInscrits.Remove(UserService.UserId);

				Toaster.Add("Désinscription effectuée.", MatToastType.Success);
			}
			catch (Exception ex)
			{
				Toaster.Add("Erreur votre désinscription à la session.", MatToastType.Danger);
			}
		}

		#endregion
	}
}
