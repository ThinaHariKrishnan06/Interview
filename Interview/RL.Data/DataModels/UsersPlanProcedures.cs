using RL.Data.DataModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RL.Data.DataModels
{
    public class UsersPlanProcedures : IChangeTrackable
    {
        public int UserId { get; set; }
        public int PlanId { get; set; }
        public int ProcedureId { get; set; }

        [JsonIgnore]
        public virtual Procedure Procedure { get; set; }
        
        [JsonIgnore]
        public virtual Plan Plan { get; set; }
        
        [JsonIgnore]
        public virtual User User { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool Deleted { get; set; }
    }
}
