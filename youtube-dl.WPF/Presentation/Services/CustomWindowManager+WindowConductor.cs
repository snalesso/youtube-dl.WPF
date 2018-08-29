using Caliburn.Micro;
using System;
using System.ComponentModel;
using System.Windows;

namespace youtube_dl.WPF.Presentation.Services
{
    /// <summary>
    /// A service that manages windows.
    /// </summary>
    public partial class CustomWindowManager : IWindowManager
    {
        private class WindowConductor
        {
            bool deactivatingFromView;
            bool deactivateFromViewModel;
            bool actuallyClosing;
            readonly Window view;
            readonly object model;

            public WindowConductor(object model, Window view)
            {
                this.model = model;
                this.view = view;

                var activatable = model as IActivate;
                activatable?.Activate();

                if (model is IDeactivate deactivatable)
                {
                    view.Closed += this.Closed;
                    deactivatable.Deactivated += this.Deactivated;
                }

                if (model is IGuardClose guard)
                {
                    view.Closing += this.Closing;
                }
            }

            private void Closed(object sender, EventArgs e)
            {
                this.view.Closed -= this.Closed;
                this.view.Closing -= this.Closing;

                if (this.deactivateFromViewModel)
                    return;

                var deactivatable = (IDeactivate)this.model;

                this.deactivatingFromView = true;
                deactivatable.Deactivate(true);
                this.deactivatingFromView = false;
            }

            private void Deactivated(object sender, DeactivationEventArgs e)
            {
                if (!e.WasClosed)
                    return;

                ((IDeactivate)this.model).Deactivated -= this.Deactivated;

                if (this.deactivatingFromView)
                    return;

                this.deactivateFromViewModel = true;
                this.actuallyClosing = true;
                this.view.Close();
                this.actuallyClosing = false;
                this.deactivateFromViewModel = false;
            }

            private void Closing(object sender, CancelEventArgs e)
            {
                if (e.Cancel)
                    return;

                var guard = (IGuardClose)this.model;

                if (this.actuallyClosing)
                {
                    this.actuallyClosing = false;
                    return;
                }

                bool runningAsync = false;
                bool shouldEnd = false;

                guard.CanClose(canClose =>
                {
                    Execute.OnUIThread(() =>
                    {
                        if (runningAsync && canClose)
                        {
                            this.actuallyClosing = true;
                            this.view.Close();
                        }
                        else
                            e.Cancel = !canClose;

                        shouldEnd = true;
                    });
                });

                if (shouldEnd)
                    return;

                runningAsync = e.Cancel = true;
            }
        }
    }
}