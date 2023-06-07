using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.util;
using HRMS_WEB.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRMS_WEB.ViewModels
{
    public class ImageUploadViewModel
    {
        public ImageUploadViewModel()
        {
            Templates = new List<Template>();
        }

        public int TemplateId { get; set; }
        public List<Template> Templates { get; set; }
    }
}