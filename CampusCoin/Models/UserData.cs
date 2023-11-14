﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CampusCoin.Models;

public partial class UserData
{
    [Key]
    [Column("ExpenseID")]
    public int ExpenseId { get; set; }

    [Column("UserID")]
    public int UserId { get; set; }

    [Required]
    [StringLength(50)]
    public string Category { get; set; }

    [Required]
    [StringLength(50)]
    public string Amount { get; set; }

    [Required]
    [Column("Date Entered")]
    [StringLength(50)]
    public string DateEntered { get; set; }

    [StringLength(50)]
    public string Description { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserData")]
    public virtual Users User { get; set; }
}