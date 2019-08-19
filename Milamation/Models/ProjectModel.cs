using HarvestClient.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Milamation.Models
{
    public class ProjectModel
    {
        public ProjectModel()
        {

        }

        public ProjectModel(Project project)
        {
            this.Code = project.Code;
            this.Id = project.Id;
            this.Name = project.Name;
        }
        
        public int Id { get; set; }

        
        public string Name { get; set; }
        
        public string Code { get; set; }

        public bool IsSelected { get; set; }

        public string DisplayName
        {
            get
            {
                return $"[{Code}] {Name}";
            }
        }

        public static implicit operator ProjectModel(Project d)
        {
            return new ProjectModel(d);
        }
    }
}
