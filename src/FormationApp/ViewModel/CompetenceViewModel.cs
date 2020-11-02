using AccessData;
using AccessData.Models;
using AccessData.Views;
using BlazorDownloadFile;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace FormationApp.ViewModel
{
	public class CompetenceViewModel : ICompetence
	{
		#region Properties

		SqlContext SqlService { get; set; }

		IBlazorDownloadFileService BlazorDownloadFileService { get; set; }

		public CompetenceDetailView CompetenceView { get; set; }

		public bool DisplayDetail { get; set; }

		public CatalogueFormations SelectedFormation { get; set; }

		#endregion

		#region Constructeur

		public CompetenceViewModel(SqlContext sqlContext, IBlazorDownloadFileService blazorDownloadFileService)
		{
			SqlService = sqlContext;
			BlazorDownloadFileService = blazorDownloadFileService;
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Charge les informations sur la compétence.
		/// </summary>
		/// <returns></returns>
		public async Task LoadCompetence(int idCompetence)
		{
			CompetenceView = await SqlService.GetCompetenceView(idCompetence);
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
		public void CloseDetail(MouseEventArgs e)
		{
			DisplayDetail = false;
		}

		#endregion
	}
}
