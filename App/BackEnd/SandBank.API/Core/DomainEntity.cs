using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace Core
{
    public abstract class DomainEntity<TKey>
    {
        //the id should be a data type appropriate to the required performance characteristics
        //the global uniqueness concern should be left to the PrivateUniqueId
        //this way we don't have to trade-off performance for uniqueness
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public TKey Id { get; set; }
        
        /*[Editable(false)]
        [Required]
        public DateTime CreatedOn { get; set; }

        //use optimistic locking
        [Timestamp] 
        public byte[] RowVersion { get; set; }

        //this provides uniqueness should we ever need it, i.e exporting data from sharded databases with duplicated ids to an OLAP system
        //it's never queried on, should never change and should never be indexed
        [Editable(false)]
        [Required]
        public Guid PrivateUniqueId { get; set; }*/
    }
}