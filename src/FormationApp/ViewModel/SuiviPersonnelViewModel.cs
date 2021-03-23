using AccessData;
using AccessData.Models;
using BlazorDownloadFile;
using FormationApp.Codes.ViewModel;
using MatBlazor;
using Microsoft.AspNetCore.Identity;
using OfficeOpenXml;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FormationApp.ViewModel
{
	public class SuiviPersonnelViewModel : ISuiviPersonnelViewModel
	{
		#region Properties

		private List<string> nomColonne = new List<string>() { "Titre de formation", "Date", "Durée", "Nom du formateur", "Formation externe" };

		public IEnumerable<UtilisateurView> UsersList { get; set; }

		public RadzenGrid<UtilisateurView> UsersViewGrid { get; set; }


		public IEnumerable<SessionInscritUserView> FormationUser { get; set; }


		public IEnumerable<CompetenceUserView> CompetenceUser { get; set; }


		public UtilisateurView UserSelected { get; set; }

		#endregion

		private IMatToaster Toaster;
		private SqlContext SqlService;
		private UserManager<IdentityUser> UserManager;
		private IBlazorDownloadFileService DownloadFileService;

		public SuiviPersonnelViewModel(IMatToaster toaster, SqlContext sqlContext, UserManager<IdentityUser> userManager, IBlazorDownloadFileService blazorDownloadFileService)
		{
			Toaster = toaster;
			SqlService = sqlContext;
			UserManager = userManager;

			DownloadFileService = blazorDownloadFileService;
		}

		#region Public Methods

		public async Task LoadAllUsers()
		{
			IEnumerable<Personnel> allPersonnels = await SqlService.GetAllPersonnel();

			List<UtilisateurView> userView = new List<UtilisateurView>();
			foreach (var personnel in allPersonnels)
			{
				var userIdentity = await UserManager.FindByIdAsync(personnel.IdPersonnel);
				UtilisateurView view = new UtilisateurView(personnel, userIdentity, new List<string>());
				userView.Add(view);
			}

			UsersList = userView;
		}


		public async Task LoadFormationsCompetences(string idPersonnel)
		{
			UserSelected = UsersList.FirstOrDefault(x => x.Personnel.IdPersonnel == idPersonnel);

			CompetenceUser = await SqlService.GetCompetencesUser(idPersonnel);
			FormationUser = await SqlService.GetInscriptionSessionUserFinishAsync(idPersonnel);
		}



		public async Task ExportAllToExcel()
		{
			try
			{
				string fileName = "Suivi_Formations-ToutLePersonnel-" + DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx";

				using (var memStream = new MemoryStream())
				{
					using (var package = new ExcelPackage(memStream))
					{

						foreach (var personnel in UsersList)
						{
							string idPersonnel = personnel.Personnel.IdPersonnel;
							List<SessionInscritUserView> formations = await SqlService.GetInscriptionSessionUserFinishAsync(idPersonnel);

							// Création d'une feuille Excel
							ExcelWorksheet sheet = package.Workbook.Worksheets.Add(personnel.Personnel.Nom);
							CreateColonnes(sheet);
							AddValues(sheet, formations);
						}

						byte[] fileTemp = package.GetAsByteArray();
						await DownloadFileService.DownloadFile(fileName, fileTemp, "application/octet-stream");
					}
				}
			}
			catch (Exception ex)
			{
				// Message d'erreur.
				Toaster.Add("Erreur sur l'export du fichier Excel", MatToastType.Danger);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public async Task ExportPersonnelToExcel(string idPersonnel)
		{
			try
			{
				using (var memStream = new MemoryStream())
				{
					using (var package = new ExcelPackage(memStream))
					{
						var personnel = UsersList.FirstOrDefault(x => x.Personnel.IdPersonnel == idPersonnel);
						List<SessionInscritUserView> formations = await SqlService.GetInscriptionSessionUserFinishAsync(idPersonnel);

						string fileName = "Suivi_Formations-" + personnel.Personnel.Nom + "-" + DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx";

						// Création d'une feuille Excel
						ExcelWorksheet sheet = package.Workbook.Worksheets.Add(personnel.Personnel.Nom);
						CreateColonnes(sheet);
						AddValues(sheet, formations);

						byte[] fileTemp = package.GetAsByteArray();
						await DownloadFileService.DownloadFile(fileName, fileTemp, "application/octet-stream");
					}
				}
			}
			catch (Exception)
			{
				// Message d'erreur.
				Toaster.Add("Erreur sur l'export du fichier Excel", MatToastType.Danger);
			}
		}

		#endregion

		#region Private Methods


		private void CreateColonnes(ExcelWorksheet sheet)
		{
			int i = 0;

			foreach (var formation in nomColonne)
			{
				i++;
				string nomIndexColonne = GetColonneLetter(i) + 1;

				sheet.Cells[nomIndexColonne].Value = formation;
				sheet.Cells[nomIndexColonne].Style.Font.Bold = true;

				AddBorder(sheet, nomIndexColonne);
			}
		}

		private void AddBorder(ExcelWorksheet sheet, string position)
		{
			// Ajout sur les 4 côtés une bordure.
			sheet.Cells[position].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
			sheet.Cells[position].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
			sheet.Cells[position].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
			sheet.Cells[position].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
		}

		/// <summary>
		/// Ajoute les valeurs.
		/// </summary>
		/// <param name="sheet"></param>
		/// <param name="formations"></param>
		private void AddValues(ExcelWorksheet sheet, List<SessionInscritUserView> formations)
		{
			int numLigne = 2;

			foreach (var formation in formations)
			{
				for (int numColonne = 1; numColonne < formations.Count; numColonne++)
				{
					string lettreColonne = GetColonneLetter(numColonne);

					sheet.Cells["A" + numLigne].Value = formation.TitreFormation;
					AddBorder(sheet, "A" + numLigne);

					sheet.Cells["B" + numLigne].Value = formation.DateDeLaFormation;
					sheet.Cells["B" + numLigne].Style.Numberformat.Format = "dd/mm/yyyy";
					AddBorder(sheet, "B" + numLigne);

					sheet.Cells["C" + numLigne].Value = Convert.ToDecimal(formation.NombreJourFormation); ;
					AddBorder(sheet, "C" + numLigne);

					sheet.Cells["D" + numLigne].Value = formation.NomFormateur;
					AddBorder(sheet, "D" + numLigne);

					sheet.Cells["E" + numLigne].Value = formation.IsExterne;
					AddBorder(sheet, "E" + numLigne);
				}

				// Passe à la nouvelle ligne.
				numLigne++;
			}
		}


		/// <summary>
		/// Donne la colonne Excel en fonction de l'index.
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		private string GetColonneLetter(int i)
		{
			switch (i)
			{
				case 1:
					return "A";
				case 2:
					return "B";
				case 3:
					return "C";
				case 4:
					return "D";
				case 5:
					return "E";
				case 6:
					return "F";
				case 7:
					return "G";
				case 8:
					return "H";
				case 9:
					return "I";
				case 10:
					return "J";
				case 11:
					return "K";
				case 12:
					return "L";
				case 13:
					return "M";
				case 14:
					return "N";
				case 15:
					return "O";
				case 16:
					return "P";
				case 17:
					return "Q";
				case 18:
					return "R";
				case 19:
					return "S";
				case 20:
					return "T";
				case 21:
					return "U";
				case 22:
					return "V";
				case 23:
					return "W";
				case 24:
					return "X";
				case 25:
					return "Y";
				case 26:
					return "Z";
				default:
					return "AA";
			}
		}

		#endregion

	}
}
