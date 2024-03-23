using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolApp.Models.DataModels
{
	[Table("FeeStructure")]
	public class FeeStructure
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int FeeStructureId { get; set; }
		public int FeeTypeId { get; set; }
		public int StandardId { get; set; }
		public bool? Monthly { get; set; }
		public bool? Yearly { get; set; }
		public decimal FeeAmount { get; set; }

		public FeeType? FeeType { get; set; }
		public Standard? Standard { get; set; }


		//public int? FeePaymentId { get; set; }  // This property represents the foreign key
		//public FeePayment? FeePayment { get; set; }
	}
}
