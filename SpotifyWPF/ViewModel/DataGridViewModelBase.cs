using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SpotifyAPI.Web.Models;

namespace SpotifyWPF.ViewModel
{
    /// <summary>
    ///     Marker interface to be able to add to collections
    /// </summary>
    public interface IDataGridViewModel
    {
        Task LoadUntilScrollable();

        bool Loading { get; }
    }

    public abstract class DataGridViewModelBase<T> : ViewModelBase, IDataGridViewModel
    {
        /// <summary>
        ///     If we're in the middle of a long running load operation, allow it to be canceled
        /// </summary>
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        ///     If we're currently loading the DataGrid
        /// </summary>
        private bool _loading;

        /// <summary>
        /// True if we're completed an initial load (LoadUntilScrollable)
        /// </summary>
        private bool _loadedInitially;

        protected DataGridViewModelBase()
        {
            LoadMoreCommand = new RelayCommand(async () =>
            {
                if (_loading) return;

                Loading = true;
                await FetchAndLoadPageAsync();
                await Application.Current.Dispatcher.BeginInvoke((Action) (() => { Loading = false; }));
            });

            StopLoadingCommand = new RelayCommand(StopLoading);
            StartLoadingCommand = new RelayCommand(async () => await LoadUntilScrollable());
        }

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
        ///     The initial query used for this DataGrid
        /// </summary>
        protected string Query { get; private set; }

        /// <summary>
        ///     The items in the DataGrid
        /// </summary>
        public ObservableCollection<T> Items { get; } = new ObservableCollection<T>();

        /// <summary>
        /// Command that can be executed to start loading until the scrollbar is visible
        /// </summary>
        public RelayCommand StartLoadingCommand { get; }

        /// <summary>
        ///     Command that can be executed to stop loading
        /// </summary>
        public RelayCommand StopLoadingCommand { get; }

        /// <summary>
        ///     Command that can be executed to load another page
        /// </summary>
        public RelayCommand LoadMoreCommand { get; }

        /// <summary>
        ///     The total items in the results set (includes not fetched/displayed)
        /// </summary>
        public int Total { get; private set; }

        /// <summary>
        ///     Start loading the DataGrid until the scrollbars show up (if there are enough results)
        ///     StopLoading will be triggered by the display of the scroll bar
        /// </summary>
        /// <returns></returns>
        public async Task LoadUntilScrollable()
        {
            if (_loading) return;

            Loading = true;

            if (_loadedInitially || Items.Count >= Total || string.IsNullOrWhiteSpace(Query))
            {
                Loading = false;

                return;
            }

            using (_cancellationTokenSource = new CancellationTokenSource())
            {
                var token = _cancellationTokenSource.Token;

                await Task.Run(async () =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        if (Items.Count >= Total) break;

                        await FetchAndLoadPageAsync();
                    }
                }, token);
            }

            await Application.Current.Dispatcher.BeginInvoke((Action) (() =>
            {
                Loading = false;
            }));

            _loadedInitially = true;
        }

        /// <summary>
        ///     Reinitialize with a starting page and query to be able to retrieve more
        /// </summary>
        /// <param name="query"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        public async Task InitializeAsync(string query, Paging<T> paging)
        {
            StopLoading();

            Items.Clear();
            Query = query;
            Total = 0;
            Loading = false;
            _loadedInitially = false;

            await LoadPageAsync(paging);
        }

        /// <summary>
        ///     Loads a page into the DataGrid and notifies those interested that the items changed
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        private async Task LoadPageAsync(Paging<T> paging)
        {
            await Application.Current.Dispatcher.BeginInvoke((Action) (() =>
            {
                foreach (var item in paging.Items) Items.Add(item);

                Total = paging.Total;

                PageLoaded?.Invoke(this, EventArgs.Empty);
            }));
        }

        /// <summary>
        ///     Fetches the next page from a subclass and loads it into the DataGrid
        /// </summary>
        /// <returns></returns>
        private async Task FetchAndLoadPageAsync()
        {
            if (Items.Count >= Total) return;

            var page = await FetchPageInternalAsync();
            await LoadPageAsync(page);
        }

        /// <summary>
        ///     This can be called by the user or when the grid goes out of view.  We stop
        ///     loading when the grid goes out of view because we use the scrollbar visibility
        ///     to determine if we can stop loading or not.  This is so the user has a scrollbar to scroll
        ///     down, which triggers loading of next pages
        /// </summary>
        private void StopLoading()
        {
            if (!_loading) return;

            Loading = false;

            try
            {
                _cancellationTokenSource?.Cancel();
            }
            catch (ObjectDisposedException)
            {
                Debug.WriteLine($"{GetType()} -> StopLoading(): CancellationTokenSource disposed");
            }
        }

        /// <summary>
        ///     Subclass specific implementation to retrieve the next page
        /// </summary>
        /// <returns></returns>
        private protected abstract Task<Paging<T>> FetchPageInternalAsync();

        /// <summary>
        ///     When a new page has been loaded
        /// </summary>
        public event EventHandler PageLoaded;
    }
}