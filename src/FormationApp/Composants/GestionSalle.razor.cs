using AccessData;
using AccessData.Models;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormationApp.Composants
{
	public partial class GestionSalle : ComponentBase
	{
		#region Properties

		[Inject]
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


		#endregion

		#region Héritage

		protected override async Task OnInitializedAsync()
		{
			await LoadAllSalles();
		}

		#endregion

		#region Internal Methods

		/// <summary>
		/// Charge toutes les salles
		/// </summary>
		/// <returns></returns>
		internal async Task LoadAllSalles()
		{
			AllSalles = await SqlService.GetAllSalleAsync();
			StateHasChanged();
		}

		#endregion

		#region Event sur DataGrid

		internal void EditRow(Salle currentSalle)
		{
			SallesViewGrid.EditRow(currentSalle);
		}

		internal void SaveRow(Salle currentSalle)
		{
			SallesViewGrid.UpdateRow(currentSalle);
		}

		/// <summary>
		/// Sauvegarde en BDD des modifications
		/// </summary>
		/// <param name="currentSalle"></param>
		internal async void OnUpdateRow(Salle currentSalle)
		{
			await SqlService.UpdateSalleAsync(currentSalle);
			StateHasChanged();
		}

		internal async void CancelEdit(Salle currentSalle)
		{
			SallesViewGrid.CancelEditRow(currentSalle);

			// récupération de la valeur en BDD
			Salle backup = await SqlService.GetSalle(currentSalle.IdSalle);

			AllSalles.Remove(currentSalle);
			AllSalles.Add(backup);
		}

		internal async void DeleteRow(Salle currentSalle)
		{
			await SqlService.DeleteSalle(currentSalle);
			AllSalles.Remove(currentSalle);

			SallesViewGrid.Reload();
		}

		#endregion

		#region MatDialog Suppression Salle

		//public bool DialogIsOpen = false;
		////public bool OkDelete = false;
		//public string NomSalleToDelete = string.Empty;

		//private Salle _salleToDelete = null;

		//internal void OpenDialog()
		//{
		//	DialogIsOpen = true;
		//}

		//internal void OkClick()
		//{
		//	DialogIsOpen = false;

		//	if(_salleToDelete != null)
		//	{
		//		SqlService.DeleteSalle(_salleToDelete).Wait();
		//	}

		//	NomSalleToDelete = string.Empty;
		//	_salleToDelete = null;

		//	//OkDelete = false;
		//	SallesViewGrid.Reload();
		//}

		#endregion

		#region MatDialog Ajouter salle

		public bool DialogIsOpenNewSalle = false;

		internal void OpenDialogNewSalle()
		{
			DialogIsOpenNewSalle = true;
		}

		/// <summary>
		/// Ajout d'une nouvelle salle en BDD.
		/// </summary>
		internal async void OkClickNewSalle()
		{
			Salle nouvelleSalle = new Salle()
			{
				NomSalle = NomSalleNew,
				Description = DescriptionNew,
				NombreDePlace = NbrePlaceNew
			};

			await SqlService.InsertSalle(nouvelleSalle);

			AllSalles.Add(nouvelleSalle);

			SallesViewGrid.Reload();

			DialogIsOpenNewSalle = false;
			StateHasChanged();
		}

		#endregion
	}
}
