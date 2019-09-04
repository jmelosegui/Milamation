using Caliburn.Micro;
using HarvestClient.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Milamation.Models
{
    public class ProjectModel : INotifyPropertyChanged
    {
        private bool isSelected;

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

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                NotifyPropertyChanged(nameof(IsSelected));
            }
        }

        public string DisplayName
        {
            get
            {
                return $"[{Code}] {Name}";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
