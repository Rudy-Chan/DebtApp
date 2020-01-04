using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Wpf_Audit
{
    class MainViewModel : ViewModel
    {
        private Page_OperationLog page_operation;

        private ICommand _firstPageCommand;
        public ICommand FirstPageCommand
        {
            get { return _firstPageCommand; }
            set { _firstPageCommand = value; }
        }


        private ICommand _previousPageCommand;
        public ICommand PreviousPageCommand
        {
            get { return _previousPageCommand; }
            set { _previousPageCommand = value; }
        }


        private ICommand _nextPageCommand;
        public ICommand NextPageCommand
        {
            get { return _nextPageCommand; }
            set { _nextPageCommand = value; }
        }


        private ICommand _lastPageCommand;
        public ICommand LastPageCommand
        {
            get { return _lastPageCommand; }
            set { _lastPageCommand = value; }
        }

        private int _pageSize;

        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                if (_pageSize != value)
                {
                    _pageSize = value;
                    OnPropertyChanged("PageSize");
                }
            }
        }

        private int _currentPage;
        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    OnPropertyChanged("CurrentPage");
                }
            }
        }

        private int _totalPage;
        public int TotalPage
        {
            get { return _totalPage; }
            set
            {
                if (_totalPage != value)
                {
                    _totalPage = value;
                    OnPropertyChanged("TotalPage");
                }
            }
        }

        //已审核列表数据源
        private ObservableCollection<Dg_AuditItem> _fakeSoruce_Audit;
        public ObservableCollection<Dg_AuditItem> FakeSource_Audit
        {
            get { return _fakeSoruce_Audit; }
            set
            {
                if (_fakeSoruce_Audit != value)
                {
                    _fakeSoruce_Audit = value;
                    OnPropertyChanged("FakeSource");
                }
            }
        }

        private ObservableCollection<Dg_NotAuditItem> _fakeSoruce_NotAudit;
        public ObservableCollection<Dg_NotAuditItem> FakeSource_NotAudit
        {
            get { return _fakeSoruce_NotAudit; }
            set
            {
                if (_fakeSoruce_NotAudit != value)
                {
                    _fakeSoruce_NotAudit = value;
                    OnPropertyChanged("FakeSource");
                }
            }
        }

        private ObservableCollection<Dg_ChangedItem> _fakeSoruce_Changed;
        public ObservableCollection<Dg_ChangedItem> FakeSource_Changed
        {
            get { return _fakeSoruce_Changed; }
            set
            {
                if (_fakeSoruce_Changed != value)
                {
                    _fakeSoruce_Changed = value;
                    OnPropertyChanged("FakeSource");
                }
            }
        }

        private ObservableCollection<Json_Operation> _fakeSoruce_OperationLog;
        public ObservableCollection<Json_Operation> FakeSource_OperationLog
        {
            get { return _fakeSoruce_OperationLog; }
            set
            {
                if (_fakeSoruce_OperationLog != value)
                {
                    _fakeSoruce_OperationLog = value;
                    OnPropertyChanged("FakeSource");
                }
            }
        }


        private List<Dg_AuditItem> _source_audit;
        private List<Dg_NotAuditItem> _source_notaudit;
        private List<Dg_ChangedItem> _source_changed;
        private List<Json_Operation> _source_operationlog;

        public MainViewModel(List<Dg_AuditItem> _source)
        {
            _source_audit = _source;
            _currentPage = _source.Count <= 0 ? 0 : 1;
            _pageSize = 30;

            int rest_page = (_source.Count % _pageSize) != 0 ? 1 : 0;
            _totalPage = _source.Count / _pageSize + rest_page;
            _fakeSoruce_Audit = new ObservableCollection<Dg_AuditItem>();
            List<Dg_AuditItem> result = _source.Take(_pageSize).ToList();

            _fakeSoruce_Audit.Clear();
            _fakeSoruce_Audit.AddRange(result);
            _firstPageCommand = new DelegateCommand(FirstPageAction_Audit);
            _previousPageCommand = new DelegateCommand(PreviousPageAction_Audit);
            _nextPageCommand = new DelegateCommand(NextPageAction_Audit);
            _lastPageCommand = new DelegateCommand(LastPageAction_Audit);
        }


        public MainViewModel(List<Dg_NotAuditItem> _source)
        {
            _source_notaudit = _source;
            _currentPage = _source.Count <= 0 ? 0 : 1;
            _pageSize = 30;

            int rest_page = (_source.Count % _pageSize) != 0 ? 1 : 0;
            _totalPage = _source.Count / _pageSize + rest_page;
            _fakeSoruce_NotAudit = new ObservableCollection<Dg_NotAuditItem>();
            List<Dg_NotAuditItem> result = _source.Take(_pageSize).ToList();

            _fakeSoruce_NotAudit.Clear();
            _fakeSoruce_NotAudit.AddRange(result);
            _firstPageCommand = new DelegateCommand(FirstPageAction_NotAudit);
            _previousPageCommand = new DelegateCommand(PreviousPageAction_NotAudit);
            _nextPageCommand = new DelegateCommand(NextPageAction_NotAudit);
            _lastPageCommand = new DelegateCommand(LastPageAction_NotAudit);
        }

        public MainViewModel(List<Dg_ChangedItem> _source)
        {
            _source_changed = _source;
            _currentPage = _source.Count <= 0 ? 0 : 1;
            _pageSize = 30;

            int rest_page = (_source.Count % _pageSize) != 0 ? 1 : 0;
            _totalPage = _source.Count / _pageSize + rest_page;
            _fakeSoruce_Changed = new ObservableCollection<Dg_ChangedItem>();
            List<Dg_ChangedItem> result = _source.Take(_pageSize).ToList();

            _fakeSoruce_Changed.Clear();
            _fakeSoruce_Changed.AddRange(result);
            _firstPageCommand = new DelegateCommand(FirstPageAction_Changed);
            _previousPageCommand = new DelegateCommand(PreviousPageAction_Changed);
            _nextPageCommand = new DelegateCommand(NextPageAction_Changed);
            _lastPageCommand = new DelegateCommand(LastPageAction_Changed);
        }

        public MainViewModel(List<Json_Operation> _source,int currentPage, int totalPage, Page_OperationLog page_operation)
        {
            this.page_operation = page_operation;
            _source_operationlog = _source;
            _currentPage = _source.Count <= 0 ? 0 : currentPage;
            _pageSize = 30;

            _totalPage = totalPage;
            _fakeSoruce_OperationLog = new ObservableCollection<Json_Operation>();
            List<Json_Operation> result = _source.Take(_pageSize).ToList();

            _fakeSoruce_OperationLog.Clear();
            _fakeSoruce_OperationLog.AddRange(result);
            _firstPageCommand = new DelegateCommand(FirstPageAction_OperationLog);
            _previousPageCommand = new DelegateCommand(PreviousPageAction_OperationLog);
            _nextPageCommand = new DelegateCommand(NextPageAction_OperationLog);
            _lastPageCommand = new DelegateCommand(LastPageAction_OperationLog);
        }

        private void FirstPageAction_Audit()
        {
            CurrentPage = _source_audit.Count <= 0 ? 0 : 1;
            var result = _source_audit.Take(_pageSize).ToList();
            _fakeSoruce_Audit.Clear();
            _fakeSoruce_Audit.AddRange(result);
        }

        private void FirstPageAction_NotAudit()
        {
            CurrentPage = _source_notaudit.Count <= 0 ? 0 : 1;
            var result = _source_notaudit.Take(_pageSize).ToList();
            _fakeSoruce_NotAudit.Clear();
            _fakeSoruce_NotAudit.AddRange(result);
        }

        private void FirstPageAction_Changed()
        {
            CurrentPage = _source_changed.Count <= 0 ? 0 : 1;
            var result = _source_changed.Take(_pageSize).ToList();
            _fakeSoruce_Changed.Clear();
            _fakeSoruce_Changed.AddRange(result);
        }

        private void FirstPageAction_OperationLog()
        {
            if(CurrentPage == 1)
            {
                return;
            }
            page_operation.proBar.Visibility = System.Windows.Visibility.Visible;
            Thread thread = new Thread(page_operation.GetUserOperationLog);
            thread.IsBackground = true;//设置为后台线程
            thread.Start("1");
        }


        private void PreviousPageAction_Audit()
        {
            if (CurrentPage <= 1)
            {
                return;
            }

            List<Dg_AuditItem> result = new List<Dg_AuditItem>();

            if (CurrentPage == 2)
            {
                result = _source_audit.Take(_pageSize).ToList();
            }
            else
            {
                result = _source_audit.Skip((CurrentPage - 2) * _pageSize).Take(_pageSize).ToList();
            }

            _fakeSoruce_Audit.Clear();
            _fakeSoruce_Audit.AddRange(result);
            CurrentPage--;
        }

        private void PreviousPageAction_NotAudit()
        {
            if (CurrentPage <= 1)
            {
                return;
            }

            List<Dg_NotAuditItem> result = new List<Dg_NotAuditItem>();

            if (CurrentPage == 2)
            {
                result = _source_notaudit.Take(_pageSize).ToList();
            }
            else
            {
                result = _source_notaudit.Skip((CurrentPage - 2) * _pageSize).Take(_pageSize).ToList();
            }

            _fakeSoruce_NotAudit.Clear();
            _fakeSoruce_NotAudit.AddRange(result);
            CurrentPage--;
        }

        private void PreviousPageAction_Changed()
        {
            if (CurrentPage <= 1)
            {
                return;
            }

            List<Dg_ChangedItem> result = new List<Dg_ChangedItem>();

            if (CurrentPage == 2)
            {
                result = _source_changed.Take(_pageSize).ToList();
            }
            else
            {
                result = _source_changed.Skip((CurrentPage - 2) * _pageSize).Take(_pageSize).ToList();
            }

            _fakeSoruce_Changed.Clear();
            _fakeSoruce_Changed.AddRange(result);
            CurrentPage--;
        }

        private void PreviousPageAction_OperationLog()
        {
            if (CurrentPage <= 1)
            {
                return;
            }
            CurrentPage--;
            page_operation.proBar.Visibility = System.Windows.Visibility.Visible;
            Thread thread = new Thread(page_operation.GetUserOperationLog);
            thread.IsBackground = true;//设置为后台线程
            thread.Start(CurrentPage.ToString());
        }


        private void NextPageAction_Audit()
        {
            if (CurrentPage >= _totalPage)
            {
                return;
            }

            List<Dg_AuditItem> result = new List<Dg_AuditItem>();
            result = _source_audit.Skip(CurrentPage * _pageSize).Take(_pageSize).ToList();

            _fakeSoruce_Audit.Clear();
            _fakeSoruce_Audit.AddRange(result);
            CurrentPage++;
        }

        private void NextPageAction_NotAudit()
        {
            if (CurrentPage >= _totalPage)
            {
                return;
            }

            List<Dg_NotAuditItem> result = new List<Dg_NotAuditItem>();
            result = _source_notaudit.Skip(CurrentPage * _pageSize).Take(_pageSize).ToList();

            _fakeSoruce_NotAudit.Clear();
            _fakeSoruce_NotAudit.AddRange(result);
            CurrentPage++;
        }

        private void NextPageAction_Changed()
        {
            if (CurrentPage >= _totalPage)
            {
                return;
            }

            List<Dg_ChangedItem> result = new List<Dg_ChangedItem>();
            result = _source_changed.Skip(CurrentPage * _pageSize).Take(_pageSize).ToList();

            _fakeSoruce_Changed.Clear();
            _fakeSoruce_Changed.AddRange(result);
            CurrentPage++;
        }

        private void NextPageAction_OperationLog()
        {
            if (CurrentPage >= _totalPage)
            {
                return;
            }
            CurrentPage++;
            page_operation.proBar.Visibility = System.Windows.Visibility.Visible;
            Thread thread = new Thread(page_operation.GetUserOperationLog);
            thread.IsBackground = true;//设置为后台线程
            thread.Start(CurrentPage.ToString());
        }

        private void LastPageAction_Audit()
        {
            CurrentPage = TotalPage;

            int skipCount = (_totalPage - 1) * _pageSize;
            int takeCount = _source_audit.Count - skipCount;
            var result = _source_audit.Skip(skipCount).Take(takeCount).ToList();

            _fakeSoruce_Audit.Clear();
            _fakeSoruce_Audit.AddRange(result);
        }

        private void LastPageAction_NotAudit()
        {
            CurrentPage = TotalPage;

            int skipCount = (_totalPage - 1) * _pageSize;
            int takeCount = _source_notaudit.Count - skipCount;
            var result = _source_notaudit.Skip(skipCount).Take(takeCount).ToList();

            _fakeSoruce_NotAudit.Clear();
            _fakeSoruce_NotAudit.AddRange(result);
        }

        private void LastPageAction_Changed()
        {
            CurrentPage = TotalPage;

            int skipCount = (_totalPage - 1) * _pageSize;
            int takeCount = _source_changed.Count - skipCount;
            var result = _source_changed.Skip(skipCount).Take(takeCount).ToList();

            _fakeSoruce_Changed.Clear();
            _fakeSoruce_Changed.AddRange(result);
        }

        private void LastPageAction_OperationLog()
        {
            if (CurrentPage == TotalPage)
            {
                return;
            }
            CurrentPage = TotalPage;
            page_operation.proBar.Visibility = System.Windows.Visibility.Visible;
            Thread thread = new Thread(page_operation.GetUserOperationLog);
            thread.IsBackground = true;//设置为后台线程
            thread.Start(CurrentPage.ToString());
        }

    }
}
