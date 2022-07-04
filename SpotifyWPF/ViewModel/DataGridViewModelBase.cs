using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Nito.AsyncEx;
using SpotifyAPI.Web;
using SpotifyWPF.View.Extension;

namespace SpotifyWPF.ViewModel
{
    public abstract class DataGridViewModelBase<T> : ViewModelBase, IDataGridViewModel
    {
        private readonly AsyncLock _mutex = new AsyncLock();

        /// <summary>
        ///     True if we're completed an initial load (MaybeLoadUntilScrollable)
        /// </summary>
        private volatile bool _loadedInitially;

        /// <summary>
        ///     If we're currently loading the DataGrid
        /// </summary>
        private volatile bool _loading;

        protected DataGridViewModelBase()
        {
            UpdateVisibilityCommand = new RelayCommand<bool>(async isVisible =>
            {
                // If we're visible try to load if we can
                if (isVisible) await MaybeLoadUntilScrollable();

                // Else stop loading if we're during the initial load
                else if (!_loadedInitially) _loadedInitially = true;
            });

            UpdateScrollCommand = new RelayCommand<ScrollInfo>(async scrollInfo =>
            {
                // If we haven't loaded the first set yet and we can scroll, then stop loading
                if (!_loadedInitially && scrollInfo.IsScrollable) _loadedInitially = true;
                
                // Otherwise load the next page
                else if (_loadedInitially && scrollInfo.ScrollPercentage >= 0.85) await FetchAndLoadPageAsync();
            });
        }

        /// <summary>
        ///     The initial query used for this DataGrid
        /// </summary>
        protected string Query { get; private set; }

        /// <summary>
        ///     The items in the DataGrid
        /// </summary>
        public ObservableCollection<T> Items { get; } = new ObservableCollection<T>();

        /// <summary>
        ///     Command to execute when our visibility is updated
        /// </summary>
        public RelayCommand<bool> UpdateVisibilityCommand { get; }

        /// <summary>
        ///     Command to execute when our ScrollInfo is updated
        /// </summary>
        public RelayCommand<ScrollInfo> UpdateScrollCommand { get; }

        /// <summary>
        ///     The total items in the results set (includes not fetched/displayed)
        /// </summary>
        public int? Total { get; private set; }

        public bool Loading
        {
            get => _loading;

            set
            {
                _loading = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        ///    Try to do an initial load if we need to (until the scrollbar show up).
        ///    If the scrollbar is shown, this method exits.  It can also only run once from the time
        ///    the viewmodel is initialized (if a query is present)
        /// </summary>
        /// <returns></returns>
        public async Task MaybeLoadUntilScrollable()
        {
            if (string.IsNullOrWhiteSpace(Query)) return;

            await Task.Run(async () =>
            {
                while (!_loadedInitially)
                {
                    if (Items.Count >= Total) break;

                    await FetchAndLoadPageAsync();
                }
            });

            _loadedInitially = true;
        }

        /// <summary>
        ///     Reinitialize with a starting page and query to be able to retrieve more.
        ///     Locked because it could be called multiple times from visibility commands
        /// </summary>
        /// <param name="query"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        public async Task InitializeAsync(string query, Paging<T, SearchResponse> paging)
        {
            using (await _mutex.LockAsync())
            {
                Loading = false;
                _loadedInitially = false;
                Items.Clear();
                Query = query;
                Total = 0;

                await LoadPageAsync(paging);
            }
        }

        /// <summary>
        ///     Loads a page into the DataGrid and notifies those interested that the items changed
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        private async Task LoadPageAsync(Paging<T, SearchResponse> paging)
        {
            // ReSharper disable once PossibleNullReferenceException
            await Application.Current.Dispatcher?.BeginInvoke((Action) (() =>
            {
                foreach (var item in paging.Items) Items.Add(item);

                Total = paging.Total;

                PageLoaded?.Invoke(this, EventArgs.Empty);
            }));
        }

        /// <summary>
        ///     Fetches the next page from a subclass and loads it into the DataGrid
        ///     Locked because it can be invoked from multiple UI commands
        /// </summary>
        /// <returns></returns>
        private async Task FetchAndLoadPageAsync()
        {
            using (await _mutex.LockAsync())
            {
                Loading = true;

                if (Items.Count >= Total) return;

                var page = await FetchPageInternalAsync();
                await LoadPageAsync(page);

                Loading = false;
            }
        }

        /// <summary>
        ///     Subclass specific implementation to retrieve the next page
        /// </summary>
        /// <returns></returns>
        private protected abstract Task<Paging<T, SearchResponse>> FetchPageInternalAsync();

        /// <summary>
        ///     When a new page has been loaded
        /// </summary>
        public event EventHandler PageLoaded;
    }

    public interface IDataGridViewModel
    {
        bool Loading { get; }
        Task MaybeLoadUntilScrollable();
    }
}