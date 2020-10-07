using AccessData;
using AccessData.Models;
using BlazorDownloadFile;
using FormationApp.Data;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore.Internal;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Radzen;

namespace FormationApp.Pages
{
	public partial class DetailSession : ComponentBase
	{
		#region Properties

		//[Inject]
		//protected CurrentUserService UserService { get; set; }

		[Inject]
		IBlazorDownloadFileService BlazorDownloadFileService { get; set; }

		[Inject]
		public SqlContext SqlService { get; set; }

		[Inject]
		protected IMatToaster Toaster { get; set; }

		public Session InfoSession { get; set; }

		[Parameter]
		public int Id { get; set; }

		/// <summary>
		/// Nom du fichier d'émargement, null si aucun.
		/// </summary>
		public string FileNameEmergement { get; set; }

		/// <summary>
		/// Liste du personnel suivant la formation
		/// </summary>
		public List<PersonnelInscritView> PersonnelsInscrit { get; set; }

		public RadzenGrid<PersonnelInscritView> PersonnelViewGrid { get; set; }

		/// <summary>
		/// Indicateur si la session est archiver
		/// </summary>
		public bool IsArchiver { get; set; }

		public SessionView Session { get; set; }

		#endregion

		#region Protected Methods

		protected async override Task OnInitializedAsync()
		{
			// Chargement pour savoir s'il y a un fichier d'émargement.
			//InfoSession = await SqlService.GetFileNameEmargementAsync(UserService.SessionView.IdSession);
			//PersonnelsInscrit = await SqlService.GetPersonnelsInscritSession(UserService.SessionView.IdSession);

			Session = await SqlService.GetSessionAsync(Id);
			InfoSession = await SqlService.GetFileNameEmargementAsync(Id);
			PersonnelsInscrit = await SqlService.GetPersonnelsInscritSession(Id);

			if (!string.IsNullOrEmpty(InfoSession.NomFichierEmargement))
				FileNameEmergement = InfoSession.NomFichierEmargement;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Pour pouvoir injecter le fichier d'émargement.
		/// </summary>
		/// <param name="files"></param>
		/// <returns></returns>
		public async void UploadFiles(IMatFileUploadEntry[] files)
		{
			try
			{
				if (files.Count() >= 1)
				{
					IMatFileUploadEntry fileMat = files.FirstOrDefault();
					FileNameEmergement = Path.GetFileName(fileMat.Name);

					using (var stream = new MemoryStream())
					{
						await fileMat.WriteToStreamAsync(stream);
						stream.Seek(0, SeekOrigin.Begin);

						//await SqlService.AddEmargementFile(UserService.SessionView.IdSession, stream.ToArray(), FileNameEmergement);
						await SqlService.AddEmargementFile(Id, stream.ToArray(), FileNameEmergement);
					}
				}
			}
			catch (Exception e)
			{
				FileNameEmergement = string.Empty;
				Toaster.Add("Erreur sur la sauvegarde du fichier d'émargement.", MatToastType.Danger);
			}
			finally
			{
				StateHasChanged();
			}
		}

		/// <summary>
		/// Event sur un click pour DL un fichier.
		/// </summary>
		/// <param name="e"></param>
		public async void DownloadOnClick(MouseEventArgs e)
		{
			try
			{
				byte[] fileTemp = await SqlService.GetEmargementFileAsync(Id);
				await BlazorDownloadFileService.DownloadFile(FileNameEmergement, fileTemp, "application/octet-stream");
			}
			catch (Exception)
			{
				Toaster.Add("Erreur sur la récupération du fichier d'émargement.", MatToastType.Danger);
			}
		}

		#endregion

		#region Event sur DataGrid

		/// <summary>
		/// Lors du click sur le bouton Edit
		/// </summary>
		/// <param name="currentPersonnel"></param>
		internal void EditRow(PersonnelInscritView currentPersonnel)
		{
			//CurrentLine = currentPersonnel;
			PersonnelViewGrid.EditRow(currentPersonnel);
		}

		internal void SaveRow(PersonnelInscritView currentPersonnel)
		{
			//SqlService.UpdateValidationUser(currentPersonnel.IsSessionValidate, UserService.SessionView.IdSession, currentPersonnel.IdPersonnel);

			SqlService.UpdateValidationUser(currentPersonnel.IsSessionValidate, Id, currentPersonnel.IdPersonnel);
			PersonnelViewGrid.UpdateRow(currentPersonnel);
		}

		/// <summary>
		/// Sauvegarde en BDD des modifications
		/// </summary>
		/// <param name="currentFormation"></param>
		internal async void OnUpdateRow(PersonnelInscritView currentPersonnel)
		{
			StateHasChanged();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="currentFormation"></param>
		internal async void CancelEdit(PersonnelInscritView currentPersonnel)
		{
			PersonnelViewGrid.CancelEditRow(currentPersonnel);

			// récupération de la valeur en BDD
			//PersonnelInscritView backup = await SqlService.GetFormationAsync(currentFormation.IdFormation);

			//AllFormations.Remove(currentFormation);
			//AllFormations.Add(backup);
		}




		#endregion

		#region Event sur Button

		public async void ClickArchiver(MouseEventArgs args)
		{
			try
			{
				IsArchiver = true;

				// 01 - Trouver les compétences ou se trouve la formation.
				List<int> idsCompetence = await SqlService.GetCompetencesIdByFormation(Session.IdFormation);

				// 02 - Récupère les ID formations, pour chaque compétence. 
				//Dictionary <int,List<int>> - idCompetence, List idFormation
				Dictionary<int, List<int>> competencesValuePairsFormations = new Dictionary<int, List<int>>();
				
				foreach (var idCompetence in idsCompetence)
				{
					List<int> idsFormations = await SqlService.GetFormationsByCompetence(idCompetence);
					competencesValuePairsFormations.Add(idCompetence, idsFormations);
				}

				// 03 - Récupère la liste des ID User qui sont validé sur cette session/formation.
				List<string> idsUsers = await SqlService.GetUsersValidateOnThisSession(Session.IdSession);

				//*** Boucle Pour chaque personnel de la session-validé
				// 04 - Récupérer la liste de ID formation validé dans les autres sessions.
				foreach (var user in idsUsers)
				{
					List<int> idFormationValide = await SqlService.GetFormationsValideByUser(user);

					// 05 - Comparaison entre 02 et 04.
					//		Valider les compétences qui besoin.
					

					foreach (var item in competencesValuePairsFormations)
					{
						List<FormationIsValidate> formationsIsValidate = item.Value.Select(x => new FormationIsValidate(x)).ToList();

						// Pour chaque formation
						foreach (var formation in formationsIsValidate)
						{
							// Si l'utilisateur a validé cette formation.
							if(idFormationValide.Contains(formation.IdFormation))
							{
								formation.IsValidate = true;
							}
						}

						// Si ne contient pas 1 seul FALSE, valider la compétence.
						if(!formationsIsValidate.Any(x => x.IsValidate == false))
						{
							// Vérifier que l'utilisateur n'a pas déjà validé cette compétence.
							if (!await SqlService.IsCompetenceValidate(user, item.Key))
							{
								await SqlService.AddCompetenceToUser(user, item.Key, 1, DateTime.Now);
							}
						}
					}
				}

				// Archivage de la session
				await SqlService.ArchiverSession(Id);
			}
			catch (Exception)
			{
				Toaster.Add("Erreur sur l'archivage de la session.", MatToastType.Danger);
			}

			StateHasChanged();
		}

		#endregion
	}


	internal class FormationIsValidate
	{
		public int IdFormation { get; set; }

		public bool IsValidate { get; set; }

		public FormationIsValidate(int id)
		{
			IdFormation = id;
			IsValidate = false;
		}
	}
}
