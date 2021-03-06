﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace ValueCheck
{
    public class ValueCheck<T> : INotifyPropertyChanged, IDataErrorInfo
    {
        public delegate void ValueChangedDelegate(T value);

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
                Error = !m_Check(this);
            }
            OnPropertyChanged("Value");
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

        public event ValueChangedDelegate ValueChanged;

        string IDataErrorInfo.Error
        {
            get { 
                throw new NotImplementedException(); 
            }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                string result = string.Empty;
                if (Error && columnName == "Value" && m_DataErrorInfoString != null)
                {
                    result = m_DataErrorInfoString();
                }

                return result;
            }
        }
    }
}
