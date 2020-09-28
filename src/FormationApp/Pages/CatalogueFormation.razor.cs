using AccessData;
using AccessData.Models;
using BlazorDownloadFile;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
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

		public CatalogueFormation()
		{
			AllFormations = new List<CatalogueFormations>();
		}

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


		public CatalogueFormations SelectedFormation { get; set; }
		public bool DisplayDetail { get; set; }

		#endregion

		#region Private methods


		#endregion

	}

}
