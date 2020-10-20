using AccessData;
using AccessData.Models;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Radzen;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FormationApp.Composants
{
	public partial class GestionFormation : ComponentBase
	{

		#region Properties

		[Inject]
		public SqlContext SqlService { get; set; }

		public RadzenGrid<CatalogueFormations> FormationViewGrid { get; set; }

		public List<CatalogueFormations> AllFormations { get; set; }

		#region Pour nouvelle formation

		protected FormationModel formationModel = new FormationModel();

		public List<TypeFormation> AllTypeFormations { get; set; }

		#endregion

		#endregion

		#region Héritage

		protected override async Task OnInitializedAsync()
		{
			await LoadAllFormations();
		}

		#endregion

		#region Constructeur

		public GestionFormation()
		{
			AllTypeFormations = new List<TypeFormation>();
		}

		#endregion

		#region Internal methods

		/// <summary>
		/// Charge toutes les salles
		/// </summary>
		/// <returns></returns>
		internal async Task LoadAllFormations()
		{
			AllTypeFormations = await SqlService.GetAllTypeFormations();
			AllFormations = await SqlService.GetAllFormationsAsync();
			StateHasChanged();
		}

		protected bool lineEnCoursModif;

		//internal string AddCurrentLineToUpdate(CatalogueFormations currentLine)
		//{
		//	ElementsModifies.Add(currentLine);
		//	return string.Empty;
		//}

		#endregion

		#region Event sur DataGrid

		/// <summary>
		/// Lors du click sur le bouton Edit
		/// </summary>
		/// <param name="currentFormation"></param>
		internal void EditRow(CatalogueFormations currentFormation)
		{
			lineEnCoursModif = true;

			CurrentLine = currentFormation;

			FormationViewGrid.EditRow(currentFormation);
		}

		internal void SaveRow(CatalogueFormations currentFormation)
		{
			FormationViewGrid.UpdateRow(currentFormation);
		}

		/// <summary>
		/// Sauvegarde en BDD des modifications
		/// </summary>
		/// <param name="currentFormation"></param>
		internal async void OnUpdateRow(CatalogueFormations currentFormation)
		{
			// Indicateur que le fichier à été mis à jour.
			if(CurrentLine.ContenuFormationN != null && CurrentLine.ContenuFormationN.Any())
			{
				await SqlService.UpdateFormationAsync(currentFormation);
			}
			else
			{
				await SqlService.UpdateFormatioWithoutFilenAsync(currentFormation);
			}
						
			lineEnCoursModif = false;
			StateHasChanged();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="currentFormation"></param>
		internal async void CancelEdit(CatalogueFormations currentFormation)
		{
			FormationViewGrid.CancelEditRow(currentFormation);
			
			// récupération de la valeur en BDD
			CatalogueFormations backup = await SqlService.GetFormationAsync(currentFormation.IdFormation);

			AllFormations.Remove(currentFormation);
			AllFormations.Add(backup);

			lineEnCoursModif = false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="currentFormation"></param>
		internal async void DeleteRow(CatalogueFormations currentFormation)
		{
			await SqlService.DeleteFormation(currentFormation);
			AllFormations.Remove(currentFormation);

			await FormationViewGrid.Reload();
		}

		public CatalogueFormations CurrentLine { get; set; }


		/// <summary>
		/// Recoit les fichiers qui sont Uploader par l'utilisateur
		/// </summary>
		/// <param name="files">Liste des fichiers</param>
		/// <returns></returns>
		public async void UploadFilePourMiseAJour(IMatFileUploadEntry[] files)
		{
			if (files.Count() >= 1)
			{
				IMatFileUploadEntry tempFile = files.FirstOrDefault();
				CurrentLine.NomDuFichier = Path.GetFileName(tempFile.Name);

				using (var stream = new MemoryStream())
				{
					await tempFile.WriteToStreamAsync(stream);
					stream.Seek(0, SeekOrigin.Begin);
					CurrentLine.ContenuFormationN = stream.ToArray();
				}
			}
		}

		#endregion

		#region MatDialog Ajouter une formation

		public bool DialogIsOpenNewFormation = false;

		internal void OpenDialogNewFormation()
		{
			DialogIsOpenNewFormation = true;
		}

		/// <summary>
		/// Méthode levé quand le model est validé.
		/// </summary>
		protected async void HandleValidSubmit()
		{
			CatalogueFormations nouvelleFormation = formationModel.ToFormation();
			nouvelleFormation.TypeFormation = AllTypeFormations.FirstOrDefault(x => x.IdTypeFormation == nouvelleFormation.TypeFormationId).TitreTypeFormation;

			await SqlService.InsertFormation(nouvelleFormation);

			AllFormations.Add(nouvelleFormation);
			await FormationViewGrid.Reload();

			// Remise à zéro de l'objet.
			formationModel = new FormationModel();

			DialogIsOpenNewFormation = false;
			StateHasChanged();
		}

		/// <summary>
		/// Ajout d'une nouvelle salle en BDD.
		/// </summary>
		internal void AnnulationClickNewFormation()
		{
			// Remise à zéro de l'objet.
			formationModel = new FormationModel();
			DialogIsOpenNewFormation = false;
		}

		/// <summary>
		/// Recoit les fichiers qui sont Uploader par l'utilisateur
		/// </summary>
		/// <param name="files">Liste des fichiers</param>
		/// <returns></returns>
		public async void UploadFiles(IMatFileUploadEntry[] files)
		{
			if (files.Count() >= 1)
			{
				IMatFileUploadEntry fileMat = files.FirstOrDefault();
				formationModel.FileName = Path.GetFileName(fileMat.Name);

				using (var stream = new MemoryStream())
				{
					await fileMat.WriteToStreamAsync(stream);
					stream.Seek(0, SeekOrigin.Begin);
					formationModel.Contenu = stream.ToArray();
				}
			}
		}

		#endregion


		#region Private methods



		#endregion

	}

	public class FormationModel
	{
		[Required(ErrorMessage = "La formation doit avoir un titre.")]
		[StringLength(32, ErrorMessage = "Le titre est trop long, 32 caractères max")]
		public string Titre { get; set; }

		[Required(ErrorMessage = "Manque une description.")]
		public string Description { get; set; }

		public byte[] Contenu { get; set; }

		[Required(ErrorMessage = "Il faut le contenu de la formation")]
		public string FileName { get; set; }

		public bool EstInterne { get; set; }

		public double Duree { get; set; }

		[Required(ErrorMessage = "Il faut choisir un type de formation")]
		public string IdTypeFormation { get; set; }

		public CatalogueFormations ToFormation()
		{
			return new CatalogueFormations()
			{
				Titre = this.Titre,
				Description = this.Description,
				DateDeFin = null,
				NomDuFichier = this.FileName,
				ContenuFormationN = this.Contenu,
				EstInterne = this.EstInterne,
				Duree = this.Duree,
				TypeFormationId = Convert.ToInt32(IdTypeFormation)
			};
		}

		public FormationModel()
		{
			EstInterne = false;
		}
	}

}
