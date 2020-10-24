using AccessData;
using AccessData.Models;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormationApp.ViewModel
{
	public interface IGestionSalleViewModel
	{
		#region Properties

		SqlContext SqlService { get; set; }

		RadzenGrid<Salle> SallesViewGrid { get; set; }

		List<Salle> AllSalles { get; set; }


		// ****** pour l'ajout d'une salle

		/// <summary>
		/// Nom de la nouvelle salle
		/// </summary>
		string NomSalleNew { get; set; }

		/// <summary>
		/// Description de la nouvelle salle
		/// </summary>
		string DescriptionNew { get; set; }

		/// <summary>
		/// Nombre de place dans la nouvelle salle
		/// </summary>	
		int NbrePlaceNew { get; set; }

		#endregion

		Task LoadAllSalles();

		#region Event sur DataGrid

		void EditRow(Salle currentSalle);

		void SaveRow(Salle currentSalle);

		/// <summary>
		/// Sauvegarde en BDD des modifications
		/// </summary>
		/// <param name="currentSalle"></param>
		void OnUpdateRow(Salle currentSalle);

		void CancelEdit(Salle currentSalle);

		void DeleteRow(Salle currentSalle);

		#endregion

		#region MatDialog Ajouter salle

		bool DialogIsOpenNewSalle { get; set; }

		void OpenDialogNewSalle();

		/// <summary>
		/// Ajout d'une nouvelle salle en BDD.
		/// </summary>
		Task OkClickNewSalle();
		

		#endregion
	}
}
