using Dapper;
using System;

namespace SVC_CustomerManagement_Domain.DBModel
{
    public class Entity
    {
        [Key]
        public int PKID { get; set; }
    }
}