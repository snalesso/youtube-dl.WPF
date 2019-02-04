using Caliburn.Micro;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caliburn.Micro.ReactiveUI
{
    /// <summary>
    /// A base implementation of <see cref = "IScreen" />.
    /// </summary>
    public class ReactiveScreen : ReactiveViewAware, Caliburn.Micro.IScreen, IChild
    {
        static readonly ILog Log = LogManager.GetLog(typeof(ReactiveScreen));

        bool isActive;
        bool isInitialized;
        object parent;
        string displayName;

        /// <summary>
        /// Creates an instance of <see cref="ReactiveScreen"/>.
        /// </summary>
        public ReactiveScreen()
        {
            this.displayName = this.GetType().FullName;
        }

        /// <summary>
        /// Gets or Sets the Parent <see cref = "IConductor" />.
        /// </summary>
        public virtual object Parent
        {
            get => this.parent;
            set => this.RaiseAndSetIfChanged(ref this.parent, value);
        }

        /// <summary>
        /// Gets or Sets the Display Name.
        /// </summary>
        public virtual string DisplayName
        {
            get => this.displayName;
            set => this.RaiseAndSetIfChanged(ref this.displayName, value);
        }

        /// <summary>
        /// Indicates whether or not this instance is currently active.
        /// Virtualized in order to help with document oriented view models.
        /// </summary>
        public virtual bool IsActive
        {
            get => this.isActive;
            private set => this.RaiseAndSetIfChanged(ref this.isActive, value);
        }

        /// <summary>
        /// Indicates whether or not this instance is currently initialized.
        /// Virtualized in order to help with document oriented view models.
        /// </summary>
        public virtual bool IsInitialized
        {
            get => this.isInitialized;
            private set => this.RaiseAndSetIfChanged(ref this.isInitialized, value);
        }

        /// <summary>
        /// Raised after activation occurs.
        /// </summary>
        public virtual event EventHandler<ActivationEventArgs> Activated = delegate { };

        /// <summary>
        /// Raised before deactivation.
        /// </summary>
        public virtual event EventHandler<DeactivationEventArgs> AttemptingDeactivation = delegate { };

        /// <summary>
        /// Raised after deactivation.
        /// </summary>
        public virtual event EventHandler<DeactivationEventArgs> Deactivated = delegate { };

        void IActivate.Activate()
        {
            if (this.IsActive)
            {
                return;
            }

            var initialized = false;

            if (!this.IsInitialized)
            {
                this.IsInitialized = initialized = true;
                this.OnInitialize();
            }

            this.IsActive = true;
            Log.Info("Activating {0}.", this);
            this.OnActivate();

            Activated?.Invoke(this, new ActivationEventArgs
            {
                WasInitialized = initialized
            });
        }

        /// <summary>
        /// Called when initializing.
        /// </summary>
        protected virtual void OnInitialize() { }

        /// <summary>
        /// Called when activating.
        /// </summary>
        protected virtual void OnActivate() { }

        void IDeactivate.Deactivate(bool close)
        {
            if (this.IsActive || (this.IsInitialized && close))
            {
                AttemptingDeactivation?.Invoke(this, new DeactivationEventArgs
                {
                    WasClosed = close
                });

                this.IsActive = false;
                Log.Info("Deactivating {0}.", this);
                this.OnDeactivate(close);

                Deactivated?.Invoke(this, new DeactivationEventArgs
                {
                    WasClosed = close
                });

                if (close)
                {
                    this.Views.Clear();
                    Log.Info("Closed {0}.", this);
                }
            }
        }

        /// <summary>
        /// Called when deactivating.
        /// </summary>
        /// <param name="close">Inidicates whether this instance will be closed.</param>
        protected virtual void OnDeactivate(bool close) { }

        /// <summary>
        /// Called to check whether or not this instance can close.
        /// </summary>
        /// <param name="callback">The implementor calls this action with the result of the close check.</param>
        public virtual void CanClose(Action<bool> callback)
        {
            callback(true);
        }

        /// <summary>
        /// Tries to close this instance by asking its Parent to initiate shutdown or by asking its corresponding view to close.
        /// Also provides an opportunity to pass a dialog result to it's corresponding view.
        /// </summary>
        /// <param name="dialogResult">The dialog result.</param>
        public virtual void TryClose(bool? dialogResult = null)
        {
            PlatformProvider.Current.GetViewCloseAction(this, this.Views.Values, dialogResult).OnUIThread();
        }
    }
}