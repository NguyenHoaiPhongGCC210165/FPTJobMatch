using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FPTJOB.Models
{
	public class Job
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Industry { get; set; }
		public string Location { get; set; }
		public string Description { get; set; }
		public string Requirement { get; set; }
		public DateTime Deadline { get; set; }

		[ForeignKey("CategoryId")]
		[DisplayName("Category")]
		public int CategoryId { get; set; }
		public virtual Category? Category { get; set; }

		[InverseProperty("Job")]
		public virtual ICollection<Profile_Job>? Profile_Jobs { get; set;}
	}
}
