using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
namespace TestS3cr3txApp.Models
{
    
    public class S3cr3tx
    {
        public S3cr3tx()
        {
        }
        [Required]
        public string AuthCode { get; set; } 
        [Required]
        public string Token { get; set; } 
        [Required]
        public string EoD { get; set; } 
        [Required]
        public string Email { get; set; }
        [Required]
        public string Input { get; set; } 
        [Required]
        public string Default { get; set; } 
        
        public string Output { get; set; }
    }
}
