using AccessData;
using AccessData.Models;
using BlazorDownloadFile;
using FormationApp.Data;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FormationApp.Pages
{
	public partial class DetailSession : ComponentBase
	{
		#region Properties

		[Inject]
		protected CurrentUserService UserService { get; set; }

		[Inject]
		IBlazorDownloadFileService BlazorDownloadFileService { get; set; }

		[Inject]
		public SqlContext SqlService { get; set; }

		[Inject]
		protected IMatToaster Toaster { get; set; }

		public Session InfoSession { get; set; }

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

		#endregion

		#region Protected Methods

		protected async override Task OnInitializedAsync()
		{
			// Chargement pour savoir s'il y a un fichier d'émargement.
			InfoSession = await SqlService.GetFileNameEmargementAsync(UserService.SessionView.IdSession);
			PersonnelsInscrit = await SqlService.GetPersonnelsInscritSession(UserService.SessionView.IdSession);

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
		public async Task FileEmargement(IMatFileUploadEntry[] files)
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

						await SqlService.AddEmargementFile(UserService.SessionView.IdSession, stream.ToArray(), FileNameEmergement);
					}
				}
			}
            catch (Exception e)
            {
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
				byte[] fileTemp = await SqlService.GetEmargementFileAsync(UserService.SessionView.IdSession);
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
			SqlService.UpdateValidationUser(currentPersonnel.IsSessionValidate, UserService.SessionView.IdSession, currentPersonnel.IdPersonnel);
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
				await SqlService.ArchiverSession(UserService.SessionView.IdSession);
				IsArchiver = true;
			}
			catch (Exception)
			{
				Toaster.Add("Erreur sur l'archivage de la session.", MatToastType.Danger);
			}

			StateHasChanged();
		}

		#endregion
	}
}
