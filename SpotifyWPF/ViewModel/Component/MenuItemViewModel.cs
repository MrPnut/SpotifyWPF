using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace SpotifyWPF.ViewModel.Component
{
    public class MenuItemViewModel : ViewModelBase
    {
        private string _header;
        private bool _isChecked;

        public MenuItemViewModel(string header) : this(header, null) { }

        public MenuItemViewModel(string header, ICommand command)
        {
            _header = header;
            Command = command ?? new RelayCommand(() => {});
        }

        public string Header
        {
            get => _header;

            set
            {
                _header = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<MenuItemViewModel> MenuItems { get; set; }

        public bool IsChecked
        {
            get => _isChecked;

            set
            {
                _isChecked = value;
                RaisePropertyChanged();
            }
        }

        public ICommand Command { get; }
    }
}