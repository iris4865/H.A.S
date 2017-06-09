using System;
using System.Windows;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Media;

namespace WpfServer
{
    class StateMenuItem
    {
        string menuVisualTitle;
        string bindingPropertyName;
        INotifyPropertyChanged bindingPropertyOwner;

        public StateMenuItem(string menuVisualTitle, string bindingPropertyName, INotifyPropertyChanged propertyChecker)
        {
            this.menuVisualTitle = menuVisualTitle;

            try
            {
                if (Name == null)
                    throw new ArgumentNullException();

                MemberInfo[] searchedProperties = propertyChecker.GetType().GetMember(bindingPropertyName);

                switch (searchedProperties.Length)
                {
                    case 0:
                        throw new NonExistPropertyException(bindingPropertyName, propertyChecker);
                    case 1:
                        break;
                    default:
                        throw new InvalidPropertyNameException(bindingPropertyName);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Exception occured - " + e.Message + "\n"
                    + "This exception has thrown from " + e.Source + ".");
            }

            this.bindingPropertyName = bindingPropertyName;
            this.bindingPropertyOwner = propertyChecker;
        }

        public string Name
        {
            set { menuVisualTitle = value; }
            get { return menuVisualTitle; }
        }

        public string Property
        {
            set { this.bindingPropertyName = value; }
            get { return bindingPropertyName; }
        }

        public INotifyPropertyChanged PropertyOwner
        {
            set { this.bindingPropertyOwner = value; }
            get { return bindingPropertyOwner; }
        }

        [Serializable]
        class NonExistPropertyException : Exception
        {
            public NonExistPropertyException(string propertyName, INotifyPropertyChanged propertyOwner)
                : base("The " + propertyName + " is not exist in " + propertyOwner.GetType().ToString() + "."){ }

            public NonExistPropertyException(string propertyName, INotifyPropertyChanged propertyOwner, Exception innerException)
                : base("The " + propertyName + " is not exist in " + propertyOwner.GetType().ToString() + "."
                      , innerException) { }
        }

        [Serializable]
        class InvalidPropertyNameException : Exception
        {
            const string alertStatement = "If you used something like query statement, don't do that.\n"
                                        + "Only use property name clearly.";
            
            public InvalidPropertyNameException(string inputtedPropertyName)
                : base(inputtedPropertyName + " is not valid property name.\n"
                      + alertStatement) { }

            public InvalidPropertyNameException(string inputtedPropertyName, Exception innerException)
                : base(inputtedPropertyName + " is not valid property name.\n"
                      + alertStatement, innerException) { }
        }
    }
}
