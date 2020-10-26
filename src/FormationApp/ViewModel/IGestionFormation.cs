using AccessData.Models;
using MatBlazor;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormationApp.ViewModel
{
	public interface IGestionFormation
	{
		#region Properties

		
		public RadzenGrid<CatalogueFormations> FormationViewGrid { get; set; }

		public List<CatalogueFormations> AllFormations { get; set; }

		#region Pour nouvelle formation

		public FormationModel FormationModel { get; set; }

		public List<TypeFormation> AllTypeFormations { get; set; }

		public bool LineEnCoursModif { get; set; }

		public CatalogueFormations CurrentLine { get; set; }

		public bool DialogIsOpenNewFormation { get; set; }

		#endregion

		#endregion


		#region Methods

		/// <summary>
		/// Charge toutes les salles
		/// </summary>
		/// <returns></returns>
		Task LoadAllFormations();

		#endregion

		#region Event sur DataGrid

		/// <summary>
		/// Lors du click sur le bouton Edit
		/// </summary>
		/// <param name="currentFormation"></param>
		void EditRow(CatalogueFormations currentFormation);

		void SaveRow(CatalogueFormations currentFormation);

		/// <summary>
		/// Sauvegarde en BDD des modifications
		/// </summary>
		/// <param name="currentFormation"></param>
		Task OnUpdateRow(CatalogueFormations currentFormation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="currentFormation"></param>
		Task CancelEdit(CatalogueFormations currentFormation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="currentFormation"></param>
		Task DeleteRow(CatalogueFormations currentFormation);

		/// <summary>
		/// Recoit les fichiers qui sont Uploader par l'utilisateur
		/// </summary>
		/// <param name="files">Liste des fichiers</param>
		/// <returns></returns>
		Task UploadFilePourMiseAJour(IMatFileUploadEntry[] files);

		#endregion

		#region MatDialog Ajouter une formation

		void OpenDialogNewFormation();

		/// <summary>
		/// Méthode levé quand le model est validé.
		/// </summary>
		Task HandleValidSubmit();

		/// <summary>
		/// Ajout d'une nouvelle salle en BDD.
		/// </summary>
		void AnnulationClickNewFormation();

		/// <summary>
		/// Recoit les fichiers qui sont Uploader par l'utilisateur
		/// </summary>
		/// <param name="files">Liste des fichiers</param>
		/// <returns></returns>
		Task UploadFiles(IMatFileUploadEntry[] files);

		#endregion
	}
}
