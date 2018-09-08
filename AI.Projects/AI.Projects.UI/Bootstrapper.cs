using System.Windows;
using AI.Projects.UI.Views;
using Caliburn.Micro;

namespace AI.Projects.UI
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);

            DisplayRootViewFor<ShellViewModel>();
        }
    }
}
