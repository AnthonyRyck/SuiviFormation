using AccessData;
using AccessData.Models;
using BlazorDownloadFile;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FormationApp.Pages
{
	public partial class CatalogueFormation : ComponentBase
	{

		#region Properties

		[Inject]
		public SqlContext SqlService { get; set; }

		[Inject] 
		IBlazorDownloadFileService BlazorDownloadFileService { get; set; }

		public List<CatalogueFormations> AllFormations { get; set; }

		#endregion

		#region Constructeur



		#endregion

		#region Héritage

		protected override async Task OnInitializedAsync()
		{
			await LoadAllFormations();
		}

		#endregion

		#region Internal methods

		/// <summary>
		/// Charge toutes les salles
		/// </summary>
		/// <returns></returns>
		internal async Task LoadAllFormations()
		{
			AllFormations = await SqlService.GetAllFormationsEncoreValideAsync();
			StateHasChanged();
		}

		#endregion

		#region Events


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

		#endregion

		#region Private methods


		#endregion

	}

}
