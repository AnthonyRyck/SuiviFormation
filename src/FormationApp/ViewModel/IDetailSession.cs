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
		bool IsArchiver { get; set; }
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