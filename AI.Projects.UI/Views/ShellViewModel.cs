using Caliburn.Micro;

namespace AI.Projects.UI.Views
{
    public class ShellViewModel : Conductor<object>.Collection.OneActive
    {
        // Fields
        private bool _projectOne;
        private bool _projectTwo;
        private bool _projectThree;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ShellViewModel()
        {
            // Initialize the app on the first tab
            ProjectOne = true;
        }

        /// <summary>
        /// A property that tells the UI if the first project is selected
        /// </summary>
        public bool ProjectOne
        {
            get => _projectOne;
            set
            {
                if (_projectOne == value) return;

                _projectOne = value;

                // If set to true, display the view for project one 
                if (value)
                    ActivateItem(new ProjectOneViewModel());
            }
        }

        /// <summary>
        /// A property that tells the UI if the second project is selected
        /// </summary>
        public bool ProjectTwo
        {
            get => _projectTwo;
            set
            {
                if (_projectTwo == value) return;

                _projectTwo = value;

                // If set to true, display the view for project two
                if (value)
                    ActivateItem(new ProjectTwoViewModel());
            }
        }

        public bool ProjectThree
        {
            get => _projectThree;
            set
            {
                if (_projectThree == value) return;

                _projectThree = value;

                // If set to true, display the view for project three
                if (value)
                    ActivateItem(new ProjectThreeViewModel());
            }
        }
    }
}
