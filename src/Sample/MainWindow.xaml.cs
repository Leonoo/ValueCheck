using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sample
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ValueCheck.ValueCheck<double> m_SValue1;
        private ValueCheck.ValueCheck<double> m_SValue2;
        private ValueCheck.ValueCheck<double> m_SValue3;

        public ValueCheck.ValueCheck<double> SValue1
        {
            get
            {
                return m_SValue1;
            }
        }

        public ValueCheck.ValueCheck<double> SValue2
        {
            get
            {
                return m_SValue2;
            }
        }

        public ValueCheck.ValueCheck<double> SValue3
        {
            get
            {
                return m_SValue3;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            m_SValue1 = new ValueCheck.ValueCheck<double>(
                x =>
                {
                    return x.Value < SValue2.Value;
                },
                () =>
                {
                    SValue2.Check();
                    SValue3.Check();
                });

            m_SValue2 = new ValueCheck.ValueCheck<double>(
                x =>
                {
                    return x.Value > SValue1.Value && x.Value <= SValue3.Value;
                },
                () =>
                {
                    SValue1.Check();
                    SValue3.Check();
                });

            m_SValue3 = new ValueCheck.ValueCheck<double>(
                x =>
                {
                    return x.Value >= SValue2.Value;
                },
                () =>
                {
                    SValue1.Check();
                    SValue2.Check();
                });

            DataContext = this;
        }
    }
}
