using AccessData.Models;
using BlazorInputFile;
using Microsoft.AspNetCore.Components.Web;
using Radzen.Blazor;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FormationApp.ViewModel
{
	public interface IDetailSession
	{
		string FileNameEmergement { get; set; }
		int Id { get; set; }
		Session InfoSession { get; set; }
		
		/// <summary>
		/// Session archivé.
		/// </summary>
		bool IsArchiver { get; set; }

		/// <summary>
		/// Indicateur pour savoir s'il est possible d'archiver la session.
		/// </summary>
		bool CanArchiver { get; set; }

		List<PersonnelInscritView> PersonnelsInscrit { get; set; }
		RadzenGrid<PersonnelInscritView> PersonnelViewGrid { get; set; }
		SessionView Session { get; set; }

		Task LoadData();

		void EditRow(PersonnelInscritView currentPersonnel);

		void SaveRow(PersonnelInscritView currentPersonnel);
		void CancelEdit(PersonnelInscritView currentPersonnel);
		void ClickArchiver(MouseEventArgs args);
		void DownloadOnClick(MouseEventArgs e);
		void HandleFileSelected(IFileListEntry[] files);
	}
}