using System.Collections.Generic;
using Caliburn.Micro;

namespace AI.Projects.UI.Views
{
    public class ShellViewModel : Conductor<object>.Collection.OneActive
    {
        private bool _projectOne;
        private bool _projectTwo;

        public ShellViewModel()
        {
            ProjectOne = true;
        }

        public bool ProjectOne
        {
            get => _projectOne;
            set
            {
                if (_projectOne == value) return;

                _projectOne = value;
                if (value)
                    ActivateItem(new ProjectOneViewModel());
            }
        }

        public bool ProjectTwo
        {
            get => _projectTwo;
            set
            {
                if (_projectTwo == value) return;

                _projectTwo = value;
                if (value)
                    ActivateItem(new ProjectTwoViewModel());
            }
        }

    }
}
