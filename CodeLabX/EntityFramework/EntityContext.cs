using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeLabX.EntityFramework
{
    public interface IEntityContext
    {
        long Id { get; set; }
        DateTimeOffset CreatedDate { get; set; }
        DateTimeOffset ModifiedDate { get; set; }
    }

    public class EntityContext : IEntityContext
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long Id { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset ModifiedDate { get; set; }
    }
}
