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
        }

        public int TemplateID { get; set; }
        public List<Template> Templates { get; set; }
    }
}