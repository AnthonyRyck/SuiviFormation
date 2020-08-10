
namespace AccessData.Models
{
	public class Salle
	{
		public int IdSalle { get; set; }


		public string NomSalle
		{
			get { return _nomSalle; }
			set { _nomSalle = value.Replace("'", "’"); }
		}
		private string _nomSalle;



		public int NombreDePlace { get; set; }

		

		public string Description
		{
			get { return _description; }
			set { _description = value.Replace("'", "’"); }
		}
		private string _description;

	}
}
