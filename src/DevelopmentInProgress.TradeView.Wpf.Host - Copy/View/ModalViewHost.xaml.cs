//-----------------------------------------------------------------------
// <copyright file="ModalViewHost.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using System.Windows;

namespace DevelopmentInProgress.TradeView.Wpf.Host.View
{
    /// <summary>
    /// Interaction logic for ModalViewHost.xaml
    /// </summary>
    public partial class ModalViewHost : Window
    {
        public ModalViewHost(ModalViewBase modalViewBase)
        {
            InitializeComponent();

            MainContent.Content = modalViewBase;
            DataContext = modalViewBase.DataContext;
        }
    }
}
