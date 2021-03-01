using AccessData;
using AccessData.Models;
using BlazorDownloadFile;
using BlazorInputFile;
using MatBlazor;
using Microsoft.AspNetCore.Components.Web;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FormationApp.ViewModel
{
	public class DetailSessionViewModel : IDetailSession
	{
		#region Properties

		IBlazorDownloadFileService BlazorDownloadFileService { get; set; }

		SqlContext SqlService { get; set; }

		IMatToaster Toaster { get; set; }

		public Session InfoSession { get; set; }

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

		/// <see cref="IDetailSession.CanArchiver"/>
		public bool CanArchiver { get; set; }

		public SessionView Session { get; set; }

		#endregion

		#region Protected Methods

		public DetailSessionViewModel(SqlContext sqlContext, IMatToaster toaster, IBlazorDownloadFileService downloadFileService)
		{
			SqlService = sqlContext;
			Toaster = toaster;
			BlazorDownloadFileService = downloadFileService;
		}

		public async Task LoadData()
		{
			// Chargement pour savoir s'il y a un fichier d'émargement.
			Session = await SqlService.GetSessionAsync(Id);
			CanArchiver = CanArchiveSession(Session.DateDebutSession, Session.NombreDeJour, DateTime.Now);
			IsArchiver = Session.IsArchive;

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
		public async void HandleFileSelected(IFileListEntry[] files)
		{
			try
			{
				if (files.Count() >= 1)
				{
					var fileMat = files.FirstOrDefault();
					FileNameEmergement = Path.GetFileName(fileMat.Name);

					var stream = await fileMat.ReadAllAsync();
					stream.Seek(0, SeekOrigin.Begin);

					await SqlService.AddEmargementFile(Id, stream.ToArray(), FileNameEmergement);
				}
			}
			catch (Exception e)
			{
				FileNameEmergement = string.Empty;
				Toaster.Add("Erreur sur la sauvegarde du fichier d'émargement.", MatToastType.Danger);
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
		public void EditRow(PersonnelInscritView currentPersonnel)
		{
			PersonnelViewGrid.EditRow(currentPersonnel);
		}

		public async void SaveRow(PersonnelInscritView currentPersonnel)
		{
			await SqlService.UpdateValidationUser(currentPersonnel.IsSessionValidate, Id, currentPersonnel.IdPersonnel);
			await PersonnelViewGrid.UpdateRow(currentPersonnel);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="currentFormation"></param>
		public void CancelEdit(PersonnelInscritView currentPersonnel)
		{
			PersonnelViewGrid.CancelEditRow(currentPersonnel);
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
							if (idFormationValide.Contains(formation.IdFormation))
							{
								formation.IsValidate = true;
							}
						}

						// Si ne contient pas 1 seul FALSE, valider la compétence.
						if (!formationsIsValidate.Any(x => x.IsValidate == false))
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
		}

		#endregion

		#region Private methods

		private bool CanArchiveSession(DateTime dateDebutSession, double nombreDeJour, DateTime now)
		{
			return DateTime.Compare(dateDebutSession.AddDays(nombreDeJour), now) > 0;
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
