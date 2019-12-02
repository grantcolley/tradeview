//-----------------------------------------------------------------------
// <copyright file="XamlFilterBox.cs" company="Development In Progress Ltd">
//     Copyright © Development In Progress Ltd 2013. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace DevelopmentInProgress.TradeView.Wpf.Controls.FilterBox
{
    /// <summary>
    /// A filter control. Bind a list to it and filter by the text typed into the text box.
    /// </summary>
    public class XamlFilterBox : Control
    {
        private static readonly DependencyProperty FilterTextProperty;
        private static readonly DependencyProperty FilterFieldNameProperty;
        private static readonly DependencyProperty ItemsSourceProperty;

        private int counter;
        private Delegate getter;

        /// <summary>
        /// Uses System.Reactive.Subjects.
        /// </summary>
        public Subject<bool> FilterTextSubject = new Subject<bool>();

        static XamlFilterBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XamlFilterBox), new FrameworkPropertyMetadata(typeof(XamlFilterBox)));

            FilterTextProperty = DependencyProperty.Register("FilterText", typeof(string), typeof(XamlFilterBox),
                new PropertyMetadata("", (o, e) => (o as XamlFilterBox).FilterTextSubject.OnNext(true)));

            FilterFieldNameProperty = DependencyProperty.Register("FilterFieldName", typeof(string), typeof(XamlFilterBox));

            ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(XamlFilterBox));
        }

        public XamlFilterBox()
        {
            // https://www.codeproject.com/Articles/1268558/A-WPF-ListView-Custom-Control-with-Search-Filter-T

            FilterTextSubject.Throttle(TimeSpan.FromMilliseconds(500))
                .ObserveOnDispatcher()
                .Subscribe(HandleFilterThrottle);
        }

        /// <summary>
        /// Gets or sets the filter text.
        /// </summary>
        public string FilterText
        {
            get { return GetValue(FilterTextProperty)?.ToString(); }
            set { SetValue(FilterTextProperty, value); }
        }

        /// <summary>
        /// Gets or sets the filter field name of the target object.
        /// </summary>
        public string FilterFieldName
        {
            get { return GetValue(FilterFieldNameProperty)?.ToString(); }
            set { SetValue(FilterFieldNameProperty, value); }
        }

        /// <summary>
        /// Gets or sets the items source.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private void HandleFilterThrottle(bool b)
        {
            System.ComponentModel.ICollectionView collectionView 
                = System.Windows.Data.CollectionViewSource.GetDefaultView(ItemsSource);

            if (collectionView == null)
            {
                return;
            }

            collectionView.Filter = (item) => FilterPredicate(item, FilterText);
        }

        private bool FilterPredicate(object item, string text)
        {
            if (getter == null)
            {
                var t = item.GetType();
                CreateTypeHelpers(t, FilterFieldName);
            }

            if (string.IsNullOrEmpty(text))
            {
                return true;
            }

            var val = getter.DynamicInvoke(item);
            if (val != null
                && val.ToString().ToLower().Contains(text.ToLower()))
            {
                return true;
            }

            return false;
        }

        private void CreateTypeHelpers(Type t, string getterName)
        {
            var propertyInfos = t.GetProperties();
            var getterPropertyInfo = propertyInfos.First(p => p.Name.Equals(getterName));
            getter = GetValue(t, getterPropertyInfo);
        }

        private Delegate GetValue(Type t, PropertyInfo propertyInfo)
        {
            var getAccessor = propertyInfo.GetGetMethod();
            var methodName = "GetValue_" + propertyInfo.Name + "_" + GetNextCounterValue();
            var dynMethod = new DynamicMethod(methodName, t, new Type[] { typeof(object) }, typeof(XamlFilterBox).Module);

            ILGenerator il = dynMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.EmitCall(OpCodes.Callvirt, getAccessor, null);
            if (propertyInfo.PropertyType.IsValueType)
            {
                il.Emit(OpCodes.Box, propertyInfo.PropertyType);
            }

            il.Emit(OpCodes.Ret);

            var dataType = new Type[] { t, typeof(object) };
            var genericBase = typeof(Func<,>);
            var combinedType = genericBase.MakeGenericType(dataType);

            return dynMethod.CreateDelegate(combinedType);
        }

        private int GetNextCounterValue()
        {
            return Interlocked.Increment(ref counter);
        }
    }
}
