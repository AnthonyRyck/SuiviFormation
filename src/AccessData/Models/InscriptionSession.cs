

namespace AccessData.Models
{
	public class InscriptionSession
	{
		public int IdSession { get; set; }

		public string IdPersonnel { get; set; }

		public bool IsSessionValidate { get; set; }

		public int? Note { get; set; }

		public string Commentaire { get; set; }
	}
}
