using AccessData;
using AccessData.Models;
using BlazorDownloadFile;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormationApp.ViewModel
{
	public class CatalogueFormationViewModel : ICatalogueFormation
	{
		#region Properties

		public SqlContext SqlService { get; set; }

		IBlazorDownloadFileService BlazorDownloadFileService { get; set; }

		public List<CatalogueFormations> AllFormations { get; set; }

		public CatalogueFormations SelectedFormation { get; set; }

		public bool DisplayDetail { get; set; }

		#endregion

		#region Constructeur

		public CatalogueFormationViewModel(SqlContext sqlContext, IBlazorDownloadFileService blazorDownloadFileService)
		{
			SqlService = sqlContext;
			BlazorDownloadFileService = blazorDownloadFileService;

			LoadAllFormations().GetAwaiter().GetResult();
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
		public void CloseDetail(MouseEventArgs e)
		{
			DisplayDetail = false;
		}

		#endregion

	}
}
