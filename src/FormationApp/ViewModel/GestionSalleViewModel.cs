using AccessData;
using AccessData.Models;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormationApp.ViewModel
{
	public class GestionSalleViewModel : IGestionSalleViewModel
	{
		#region Properties

		
		public SqlContext SqlService { get; set; }

		public RadzenGrid<Salle> SallesViewGrid { get; set; }

		public List<Salle> AllSalles { get; set; }


		// ****** pour l'ajout d'une salle

		/// <summary>
		/// Nom de la nouvelle salle
		/// </summary>
		public string NomSalleNew { get; set; }

		/// <summary>
		/// Description de la nouvelle salle
		/// </summary>
		public string DescriptionNew { get; set; }

		/// <summary>
		/// Nombre de place dans la nouvelle salle
		/// </summary>	
		public int NbrePlaceNew { get; set; }
		public bool DialogIsOpenNewSalle { get ; set; }

		#endregion

		public GestionSalleViewModel(SqlContext sqlContext)
		{
			SqlService = sqlContext;

			LoadAllSalles().GetAwaiter().GetResult();
		}


		public async Task LoadAllSalles()
		{
			AllSalles = await SqlService.GetAllSalleAsync();
		}


		#region Event sur DataGrid

		public void EditRow(Salle currentSalle)
		{
			SallesViewGrid.EditRow(currentSalle);
		}

		public void SaveRow(Salle currentSalle)
		{
			SallesViewGrid.UpdateRow(currentSalle);
		}

		/// <summary>
		/// Sauvegarde en BDD des modifications
		/// </summary>
		/// <param name="currentSalle"></param>
		public async void OnUpdateRow(Salle currentSalle)
		{
			await SqlService.UpdateSalleAsync(currentSalle);
		}

		public async void CancelEdit(Salle currentSalle)
		{
			SallesViewGrid.CancelEditRow(currentSalle);

			// récupération de la valeur en BDD
			Salle backup = await SqlService.GetSalle(currentSalle.IdSalle);

			AllSalles.Remove(currentSalle);
			AllSalles.Add(backup);
		}

		public async void DeleteRow(Salle currentSalle)
		{
			await SqlService.DeleteSalle(currentSalle);
			AllSalles.Remove(currentSalle);

			await SallesViewGrid.Reload();
		}

		#endregion

		#region MatDialog Ajouter salle

		public void OpenDialogNewSalle()
		{
			DialogIsOpenNewSalle = true;
		}

		/// <summary>
		/// Ajout d'une nouvelle salle en BDD.
		/// </summary>
		public async Task OkClickNewSalle()
		{
			Salle nouvelleSalle = new Salle()
			{
				NomSalle = NomSalleNew,
				Description = DescriptionNew,
				NombreDePlace = NbrePlaceNew
			};

			await SqlService.InsertSalle(nouvelleSalle);

			AllSalles.Add(nouvelleSalle);

			await SallesViewGrid.Reload();

			DialogIsOpenNewSalle = false;
		}




		#endregion

	}
}
