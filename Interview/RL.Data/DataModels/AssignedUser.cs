using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RL.Data.DataModels
{
    public class AssignedUser
    {
        [Key]
        public int AssignmentId { get; set; }

        public int PlanId { get; set; } 

        public int ProcedureId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; } 

        public DateTime AssignedDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public virtual PlanProcedure PlanProcedure { get; set; } 
        public virtual User User { get; set; } 
    }
}
