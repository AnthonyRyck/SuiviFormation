using AccessData;
using AccessData.Models;
using AccessData.Views;
using BlazorDownloadFile;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormationApp.Pages
{
	public partial class CompetencePage
	{
		#region Properties

		[Parameter]
		public int IdCompetence { get; set; }

		[Inject]
		public SqlContext SqlService { get; set; }

		[Inject]
		IBlazorDownloadFileService BlazorDownloadFileService { get; set; }


		public CompetenceDetailView CompetenceView { get; set; }

		public bool DisplayDetail { get; set; }

		public CatalogueFormations SelectedFormation { get; set; }

		#endregion

		#region Héritage

		protected async override Task OnInitializedAsync()
		{
			await LoadCompetence();
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Charge les informations sur la compétence.
		/// </summary>
		/// <returns></returns>
		private async Task LoadCompetence()
		{
			CompetenceView = await SqlService.GetCompetenceView(IdCompetence);
		}


		/// <summary>
		/// Event sur un click pour DL un fichier.
		/// </summary>
		/// <param name="e"></param>
		/// <param name="id">ID de la formation.</param>
		public async void DownloadOnClick(MouseEventArgs e, int id, string fileName)
		{
			byte[] fileTemp = await SqlService.GetFormationFileAsync(id);
			await BlazorDownloadFileService.DownloadFile(fileName, fileTemp, "application/octet-stream");
		}


		public void ClickOnFormation(CatalogueFormations formation)
		{
			SelectedFormation = formation;
			DisplayDetail = true;
		}

		/// <summary>
		/// Event sur un click pour DL un fichier.
		/// </summary>
		/// <param name="e"></param>
		/// <param name="id">ID de la formation.</param>
		public async void CloseDetail(MouseEventArgs e)
		{
			DisplayDetail = false;
		}

		#endregion
	}
}
