using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;

namespace MayhemCore
{
    public class BindingCollection<T> : ObservableCollection<T>
    {
        private SynchronizationContext creationSyncContext;
        private Thread creationThread;

        /// <summary> 
        /// Initializes a new instance of the<see cref="BindingCollection<T>">BindingCollection</see>. 
        /// </summary> 
        public BindingCollection()
            : base()
        {
            InstallSynchronizationContext();
        }

        /// <summary> 
        /// Initializes a new instance of the<see cref="BindingCollection<T>">BindingCollection</see> 
        /// class that contains elements copied from the specified list. 
        /// </summary> 
        /// <param name="list">The list from which the elements are copied.</param> 
        /// <exception cref="System.ArgumentNullException">The list parameter cannot be null.</exception> 
        public BindingCollection(Collection<T> list)
            : base(list)
        {
            InstallSynchronizationContext();
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.creationThread == Thread.CurrentThread)
            {
                // We are running in the same thread, so just directly fire the collection changed notification. 
                this.OnCollectionChangedInternal(e);
            }
            else if (this.creationSyncContext.GetType() == typeof(SynchronizationContext))
            {
                // We are running in free threaded context, also directly fire the collection changed notification. 
                this.OnCollectionChangedInternal(e);
            }
            else
            {
                // We are running in WindowsFormsSynchronizationContext or DispatcherSynchronizationContext, 
                // so we should marshal the collection changed notification back to the creation thread. 
                // Note that we should use the blocking SynchronizationContext.Send operation to marshal the call, 
                // because SynchronizationContext.Post will fail under heavy usage scenario. 
                this.creationSyncContext.Send(new SendOrPostCallback(delegate
                {
                    this.OnCollectionChangedInternal(e);
                }), null);
            }
        }

        private void InstallSynchronizationContext()
        {
            this.creationSyncContext = SynchronizationContext.Current;
            this.creationThread = Thread.CurrentThread;
        }

        internal void OnCollectionChangedInternal(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
        }
    }
}
