using AccessData;
using AccessData.Models;
using MatBlazor;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FormationApp.ViewModel
{
	public class GestionFormationViewModel : IGestionFormation
	{
		#region Properties

		public SqlContext SqlService { get; set; }

		public RadzenGrid<CatalogueFormations> FormationViewGrid { get; set; }

		public List<CatalogueFormations> AllFormations { get; set; }

		public bool LineEnCoursModif { get; set; }

		public CatalogueFormations CurrentLine { get; set; }

		#region Pour nouvelle formation

		public FormationModel FormationModel { get; set; } = new FormationModel();

		public List<TypeFormation> AllTypeFormations { get; set; }

		public bool DialogIsOpenNewFormation { get; set; } = false;

		#endregion

		#endregion

		#region Constructeur

		public GestionFormationViewModel(SqlContext sqlContext)
		{
			SqlService = sqlContext;
			AllTypeFormations = new List<TypeFormation>();
			LoadAllFormations().GetAwaiter().GetResult();
		}

		#endregion

		#region Internal methods

		/// <summary>
		/// Charge toutes les salles
		/// </summary>
		/// <returns></returns>
		public async Task LoadAllFormations()
		{
			AllTypeFormations = await SqlService.GetAllTypeFormations();
			AllFormations = await SqlService.GetAllFormationsAsync();
		}
		
		#endregion

		#region Event sur DataGrid

		/// <summary>
		/// Lors du click sur le bouton Edit
		/// </summary>
		/// <param name="currentFormation"></param>
		public void EditRow(CatalogueFormations currentFormation)
		{
			LineEnCoursModif = true;
			CurrentLine = currentFormation;
			FormationViewGrid.EditRow(currentFormation);
		}

		public void SaveRow(CatalogueFormations currentFormation)
		{
			FormationViewGrid.UpdateRow(currentFormation);
		}

		/// <summary>
		/// Sauvegarde en BDD des modifications
		/// </summary>
		/// <param name="currentFormation"></param>
		public async Task OnUpdateRow(CatalogueFormations currentFormation)
		{
			// Indicateur que le fichier à été mis à jour.
			if (CurrentLine.ContenuFormationN != null && CurrentLine.ContenuFormationN.Any())
			{
				await SqlService.UpdateFormationAsync(currentFormation);
			}
			else
			{
				await SqlService.UpdateFormatioWithoutFilenAsync(currentFormation);
			}

			currentFormation.TypeFormation = AllTypeFormations.FirstOrDefault(x => x.IdTypeFormation == currentFormation.TypeFormationId).TitreTypeFormation;

			LineEnCoursModif = false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="currentFormation"></param>
		public async Task CancelEdit(CatalogueFormations currentFormation)
		{
			FormationViewGrid.CancelEditRow(currentFormation);

			// récupération de la valeur en BDD
			CatalogueFormations backup = await SqlService.GetFormationAsync(currentFormation.IdFormation);

			AllFormations.Remove(currentFormation);
			AllFormations.Add(backup);

			LineEnCoursModif = false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="currentFormation"></param>
		public async Task DeleteRow(CatalogueFormations currentFormation)
		{
			await SqlService.DeleteFormation(currentFormation);
			AllFormations.Remove(currentFormation);

			await FormationViewGrid.Reload();
		}

		/// <summary>
		/// Recoit les fichiers qui sont Uploader par l'utilisateur
		/// </summary>
		/// <param name="files">Liste des fichiers</param>
		/// <returns></returns>
		public async Task UploadFilePourMiseAJour(IMatFileUploadEntry[] files)
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

		

		public void OpenDialogNewFormation()
		{
			DialogIsOpenNewFormation = true;
		}

		/// <summary>
		/// Méthode levé quand le model est validé.
		/// </summary>
		public async Task HandleValidSubmit()
		{
			CatalogueFormations nouvelleFormation = FormationModel.ToFormation();
			nouvelleFormation.TypeFormation = AllTypeFormations.FirstOrDefault(x => x.IdTypeFormation == nouvelleFormation.TypeFormationId).TitreTypeFormation;

			await SqlService.InsertFormation(nouvelleFormation);

			AllFormations.Add(nouvelleFormation);
			await FormationViewGrid.Reload();

			// Remise à zéro de l'objet.
			FormationModel = new FormationModel();

			DialogIsOpenNewFormation = false;
		}

		/// <summary>
		/// Ajout d'une nouvelle salle en BDD.
		/// </summary>
		public void AnnulationClickNewFormation()
		{
			// Remise à zéro de l'objet.
			FormationModel = new FormationModel();
			DialogIsOpenNewFormation = false;
		}

		/// <summary>
		/// Recoit les fichiers qui sont Uploader par l'utilisateur
		/// </summary>
		/// <param name="files">Liste des fichiers</param>
		/// <returns></returns>
		public async Task UploadFiles(IMatFileUploadEntry[] files)
		{
			if (files.Count() >= 1)
			{
				IMatFileUploadEntry fileMat = files.FirstOrDefault();
				FormationModel.FileName = Path.GetFileName(fileMat.Name);

				using (var stream = new MemoryStream())
				{
					await fileMat.WriteToStreamAsync(stream);
					stream.Seek(0, SeekOrigin.Begin);
					FormationModel.Contenu = stream.ToArray();
				}
			}
		}

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
		[StringLength(40, ErrorMessage = "Le nom du fichier est trop long, 40 caractères avec l'extension .xxx")]
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
