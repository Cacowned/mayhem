using System.Collections.Generic;
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
        /// Initializes a new instance of the BindingCollection. 
        /// </summary> 
        public BindingCollection()
        {
            InstallSynchronizationContext();
        }

        /// <summary> 
        /// Initializes a new instance of the BindingCollection
        /// class that contains elements copied from the specified list. 
        /// </summary> 
        /// <param name="list">The list from which the elements are copied.</param> 
        /// <exception cref="System.ArgumentNullException">The list parameter cannot be null.</exception> 
        public BindingCollection(IEnumerable<T> list)
            : base(list)
        {
            InstallSynchronizationContext();
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (creationThread == Thread.CurrentThread)
            {
                // We are running in the same thread, so just directly fire the collection changed notification. 
                OnCollectionChangedInternal(e);
            }
            else if (creationSyncContext.GetType() == typeof(SynchronizationContext))
            {
                // We are running in free threaded context, also directly fire the collection changed notification. 
                OnCollectionChangedInternal(e);
            }
            else
            {
                // We are running in WindowsFormsSynchronizationContext or DispatcherSynchronizationContext, 
                // so we should marshal the collection changed notification back to the creation thread. 
                // Note that we should use the blocking SynchronizationContext.Send operation to marshal the call, 
                // because SynchronizationContext.Post will fail under heavy usage scenario. 
                creationSyncContext.Send(delegate
                {
                    OnCollectionChangedInternal(e);
                }, null);
            }
        }

        private void InstallSynchronizationContext()
        {
            creationSyncContext = SynchronizationContext.Current;
            creationThread = Thread.CurrentThread;
        }

        internal void OnCollectionChangedInternal(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
        }
    }
}
