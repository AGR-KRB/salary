using salary.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace salary.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {

        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand WorkersViewCommand { get; set; }
        public RelayCommand SessonViewCommand { get; set; }
        public RelayCommand ReportViewCommand { get; set; }
        public RelayCommand ImportViewCommand { get; set; }
        public HomeViewModel HomeVm { get; set; }
        
       
        public WorkersViewModel WorkersVm { get; set; }
        public SeasonViewModel SeasonVm { get; set; }
        public ReportViewModel ReportVm { get; set; }
        public ImportViewModel ImportVm { get; set; }
        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set { _currentView = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel() 
        { 
            HomeVm = new HomeViewModel();
            WorkersVm = new WorkersViewModel();
            SeasonVm = new SeasonViewModel();
            ReportVm = new ReportViewModel(); 
            ImportVm = new ImportViewModel();
            CurrentView = new HomeViewModel();
            HomeViewCommand = new RelayCommand(o =>
            {
                CurrentView = new HomeViewModel();
            });
            WorkersViewCommand = new RelayCommand(o =>
            {
                CurrentView = new WorkersViewModel();
            });
            SessonViewCommand = new RelayCommand(o =>
            {
                CurrentView = new SeasonViewModel();
            });
            ReportViewCommand = new RelayCommand(o => 
            { CurrentView = new ReportViewModel(); 
            });
            ImportViewCommand = new RelayCommand(o =>
            {
                CurrentView = new ImportViewModel();
            });
        }
    }
}
