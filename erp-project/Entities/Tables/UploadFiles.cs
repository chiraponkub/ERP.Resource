﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace erp_project.Entities.Tables
{
    [Table("UploadFiles", Schema = "Upload")]
    public partial class UploadFiles
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Column("imagename")]
        public string Imagename { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
    }
}