using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace ValueCheck
{
    public class ValueCheck<T> : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public delegate void ValueChangedDelegate(T value);
        public event ValueChangedDelegate ValueChanged;

        private bool m_OK;

        public bool OK
        {
            get { return m_OK; }
            set
            {
                m_OK = value;
                OnPropertyChanged("OK");
            }
        }

        private bool m_Error;

        public bool Error
        {
            get { return m_Error; }
            private set
            {
                if (m_Error != value)
                {
                    m_Error = value;
                    OK = !value;
                    OnPropertyChanged("Error");
                }
            }
        }


        private T m_Value = default(T);

        public T Value
        {
            get { return m_Value; }
            set
            {
                if (m_Value == null || !m_Value.Equals(value))
                {
                    m_Value = value;
                    Check();
                    Check2();
                    OnPropertyChanged("Value");
                    if (ValueChanged != null)
                    {
                        ValueChanged(value);
                    }
                }
            }
        }

        private Func<ValueCheck<T>, bool> m_Check;

        private Action m_AfterCheck;

        private Func<string> m_DataErrorInfoString;

        public ValueCheck(Func<ValueCheck<T>, bool> check, Action afterCheck)
        {
            m_Check = check;
            m_AfterCheck = afterCheck;
        }

        public ValueCheck(Func<ValueCheck<T>, bool> check, Action afterCheck, Func<string> dataErrorInfoString)
            : this(check, afterCheck)
        {
            m_DataErrorInfoString = dataErrorInfoString;
        }

        public void Check()
        {
            if (m_Check != null && Value != null)
            {
                bool error = !m_Check(this);

                if (error != Error)
                {
                    Error = error;
                    OnErrorsChanged("Value");
                }
            }
         }

        private void Check2()
        {
            if (m_AfterCheck != null)
            {
                m_AfterCheck();
            }
        }

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion // INotifyPropertyChanged Members

        #region INotifyDataErrorInfo Members
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        protected virtual void OnErrorsChanged(string propertyName)
        {
            EventHandler<DataErrorsChangedEventArgs> handler = this.ErrorsChanged;
            if (handler != null)
            {
                var e = new DataErrorsChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            var errorList = new List<string>();

            if (Error && propertyName == "Value" && m_DataErrorInfoString != null)
            {
                errorList.Add(m_DataErrorInfoString());
            }

            return errorList;
        }

        public bool HasErrors
        {
            get 
            { 
                return Error; 
            }
        }
        #endregion
    }
}
